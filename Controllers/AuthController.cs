using HostelMaintenanceAPI.Data;
using HostelMaintenanceAPI.Models;
using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HostelMaintenanceAPI.Models.DTOs;

namespace HostelMaintenanceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users
    .Where(u => u.Username == request.Username && u.Password == request.Password)
    .FirstOrDefault();

            if (user == null)
            {
                return Unauthorized("Incorrect username or password");
            }
            if (user.Password != request.Password)
            {
                return Unauthorized("Incorrect password");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
{
    new Claim(ClaimTypes.Name, request.Username),
    new Claim(ClaimTypes.Role, user.Role ?? "Student"),
    new Claim("studentID", user.StudentID) // This lets us retrieve it later
}),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
        [HttpPost("adminlogin")]
        public IActionResult AdminLogin([FromBody] LoginRequest request)
        {
            var admin = _context.Admins
                .Where(a => a.Username == request.Username && a.Password == request.Password)
                .FirstOrDefault();

            if (admin == null)
            {
                return Unauthorized("Incorrect admin username or password");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "Admin")
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            // ✅ Normalize and validate the role
            string role = request.Role?.Trim().ToLower();
            if (role != "student" && role != "admin")
            {
                return BadRequest("Role must be either 'Student' or 'Admin'.");
            }

            // ✅ Capitalize first letter: "student" → "Student", etc.
            role = char.ToUpper(role[0]) + role.Substring(1);

            // ✅ Create and save user
            var newUser = new User
            {
                StudentID = request.StudentID,
                Username = request.Username,
                Password = request.Password, // Hashing can be added later
                Gender = request.Gender,
                Phone = request.Phone,
                Role = role  // ✅ Save with proper casing
            };
            _context.Users.Add(newUser);

            // ✅ Create and save student profile
            var newStudent = new Student
            {
                StudentID = request.StudentID,
                Username = request.Username,
                Gender = request.Gender,
                Department = request.Department,
                Year = request.Year
            };
            _context.Students.Add(newStudent);

            await _context.SaveChangesAsync();

            return Ok("Registration successful.");
         
        }
        [Authorize]
        [HttpGet("secure")]
        public IActionResult GetSecureData()
        {
            return Ok("You are authorized! Welcome.");
        }
        
    }
}