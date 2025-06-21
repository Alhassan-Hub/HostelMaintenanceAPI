using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMaintenanceAPI.Models
{
    [Table("Admin", Schema = "allan")]  
    public partial class Admin
    {
        public int AdminId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}