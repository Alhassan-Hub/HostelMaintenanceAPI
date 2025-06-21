using HostelMaintenanceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HostelMaintenanceAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("allan");
            base.OnModelCreating(modelBuilder);
        }
    
    }
}