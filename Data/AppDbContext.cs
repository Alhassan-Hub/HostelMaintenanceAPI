using System.Collections.Generic;
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
    }
}