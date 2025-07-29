using System;

namespace HalloDoc.Repositories.DTOs
{
    public class RequestListDto
    {
        public int RequestId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Symptoms { get; set; } = string.Empty;
    }

    public class RequestFilterDto
    {
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SearchTerm { get; set; }
    }
} 