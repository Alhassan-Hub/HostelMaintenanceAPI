using System;
using System.ComponentModel.DataAnnotations;

namespace HostelMaintenanceAPI.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        [Required]
        public string IssueDescription { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public bool IsResolved { get; set; } = false;
    }
}