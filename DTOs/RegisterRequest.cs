namespace HostelMaintenanceAPI.Models.DTOs
{
    public class RegisterRequest
    {
        public string StudentID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // "Student" or "Admin"

        // Add these:
       
        public string Gender { get; set; }
        public string Department { get; set; }
        public string Year { get; set; }
        public string Phone { get; set; }
    }
}