using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostelMaintenanceAPI.Models
{
    [Table("Users", Schema = "allan")] 
    public class User
    {
        [Key]
        [Column("studentID")]
        public string StudentID { get; set; } = null!;

        [Column("username")]
        public string Username { get; set; } = null!;

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("phone")]
        public string Phone { get; set; } = null!;

        [Column("gender")]
        public string Gender { get; set; } = null!;

        public string Role { get; set; }
    }
}