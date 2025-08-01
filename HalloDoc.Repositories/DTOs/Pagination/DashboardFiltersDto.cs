namespace HalloDoc.Repositories.DTOs.Pagination
{
    public class DashboardFiltersDto
    {
        public string? SearchTerm { get; set; }
        public int? RequestType { get; set; }
        public int? RegionId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
} 