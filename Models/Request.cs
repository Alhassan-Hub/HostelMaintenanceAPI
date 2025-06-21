using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMaintenanceAPI.Models
{
    public class Request
    {
        [Key]
        public int RequestID { get; set; }

        [Required]
        public string StudentID { get; set; } = null!;

        public string Description { get; set; } = "No description provided";

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        public DateTime? LastUpdated { get; set; }

        public string RoomNumber { get; set; } = "Unknown";

        [ForeignKey("StudentID")]
        public User Student { get; set; } = null!;
    }
}