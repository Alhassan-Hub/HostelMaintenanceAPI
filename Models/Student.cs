using System.ComponentModel.DataAnnotations;

namespace HostelMaintenanceAPI.Models
{
    public class Student
    {
        [Key]
        public string StudentID { get; set; } = null;
        public string Username { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Year { get; set; } = null!;
        public string Department { get; set; } = null!;
    }
}