using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HostelMaintenanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitAllanSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Students",
                newName: "Students",
                newSchema: "allan");

            migrationBuilder.RenameTable(
                name: "MaintenanceRequests",
                newName: "MaintenanceRequests",
                newSchema: "allan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Students",
                schema: "allan",
                newName: "Students");

            migrationBuilder.RenameTable(
                name: "MaintenanceRequests",
                schema: "allan",
                newName: "MaintenanceRequests");
        }
    }
}
