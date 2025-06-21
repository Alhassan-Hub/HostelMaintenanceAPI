using HostelMaintenanceAPI.Data;
using HostelMaintenanceAPI.DTOs;
using HostelMaintenanceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HostelMaintenanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MaintenanceRequestsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // -------------------- ADMIN METHODS --------------------

        [Authorize(Roles = "Admin")]
        [Tags("Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceRequest>>> GetAllRequests()
        {
            return await _context.MaintenanceRequests.ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [Tags("Admin")]
        [HttpPatch("UpdateStatus/{id}")]
        public async Task<IActionResult> UpdateRequestStatus(int id, UpdateStatusDto dto)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            request.Status = dto.Status;
            request.LastUpdated = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Status updated successfully." });
        }

        [Authorize(Roles = "Admin")]
        [Tags("Admin")]
        [HttpDelete("admin-delete/{id}")]
        public async Task<IActionResult> AdminDeleteRequest(int id)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            _context.MaintenanceRequests.Remove(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Request deleted by admin." });
        }

        [Authorize(Roles = "Admin")]
        [Tags("Admin")]
        [HttpGet("FilterByYear/{year}")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequest>>> FilterByYear(string year)
        {
            var results = await _context.MaintenanceRequests
                .Join(_context.Students,
                    req => req.StudentID,
                    stu => stu.StudentID,
                    (req, stu) => new { Request = req, Student = stu })
                .Where(x => x.Student.Year == year)
                .Select(x => x.Request)
                .ToListAsync();

            return Ok(results);
        }

        [Authorize(Roles = "Admin")]
        [Tags("Admin")]
        [HttpGet("FilterByBlock/{block}")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequest>>> FilterByBlock(string block)
        {
            if (string.IsNullOrWhiteSpace(block))
                return BadRequest(new { message = "Block name is required." });

            block = block.Trim().ToLower();

            var results = await _context.MaintenanceRequests
                .Where(r => r.RoomNumber.ToLower().StartsWith($"block {block}"))
                .ToListAsync();

            return Ok(results);
        }

        // -------------------- STUDENT METHODS --------------------

        [Authorize(Roles = "Student")]
        [Tags("Student")]
        [HttpPost("upload")]
        public async Task<IActionResult> CreateRequestWithImage([FromForm] CreateMaintenanceRequestDto dto, IFormFile image)
        {
            var studentId = User.FindFirst("studentID")?.Value;
            if (string.IsNullOrEmpty(studentId))
                return BadRequest(new { message = "Student ID missing from token." });

            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            string imagePath = null;

            if (image != null && image.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePath = $"uploads/{fileName}";
            }

            var request = new MaintenanceRequest
            {
                StudentID = studentId,
                StudentName = student.Username,
                RoomNumber = dto.RoomNumber,
                IssueDescription = dto.IssueDescription,
                RequestDate = DateTime.Now,
                IsResolved = false,
                Status = "Pending",
                LastUpdated = DateTime.Now,
                ImageUrl = imagePath
            };

            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Request submitted successfully.", request });
        }

        [Authorize(Roles = "Student")]
        [Tags("Student")]
        [HttpGet("MyRequests/{studentId}")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetMyRequests(string studentId)
        {
            var requests = await _context.MaintenanceRequests
                .Where(r => r.StudentID == studentId)
                .Select(r => new MaintenanceRequestDto
                {
                    Id = r.Id,
                    StudentID = r.StudentID,
                    StudentName = r.StudentName,
                    RoomNumber = r.RoomNumber,
                    IssueDescription = r.IssueDescription,
                    RequestDate = r.RequestDate,
                    Status = r.Status,
                    IsResolved = r.IsResolved,
                    LastUpdated = r.LastUpdated,
                    ImageUrl = string.IsNullOrEmpty(r.ImageUrl)
                        ? null
                        : $"{Request.Scheme}://{Request.Host}/uploads/{r.ImageUrl}"
                })
                .ToListAsync();

            if (!requests.Any())
                return NotFound(new { message = "No requests found." });

            return Ok(requests);
        }

        [Authorize(Roles = "Student")]
        [Tags("Student")]
        [HttpDelete("student-delete/{id}")]
        public async Task<IActionResult> StudentDeleteRequest(int id)
        {
            var studentId = User.FindFirst("studentID")?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized();

            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            if (request.StudentID != studentId)
                return Forbid("You can only delete your own request.");

            _context.MaintenanceRequests.Remove(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Request deleted successfully." });
        }
    }
}