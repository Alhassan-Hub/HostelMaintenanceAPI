using Microsoft.EntityFrameworkCore;
using HostelMaintenanceAPI.Models;

namespace HostelMaintenanceAPI.Data
{
    public class HostelMaintenanceContext : DbContext
    {
        public HostelMaintenanceContext(DbContextOptions<HostelMaintenanceContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Request> Requests { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
    }
}