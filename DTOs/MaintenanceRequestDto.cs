namespace HostelMaintenanceAPI.DTOs
{
    public class MaintenanceRequestDto
    {
        public int Id { get; set; }
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string RoomNumber { get; set; }
        public string IssueDescription { get; set; }
        public DateTime RequestDate { get; set; }
        public string? Status { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string? ImageUrl { get; set; }  // ? Add Image URL
    }
}