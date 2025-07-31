using System.ComponentModel.DataAnnotations;
using HalloDoc.Common.Constants;

namespace HalloDoc.Repositories.DTOs
{
    public class DashboardFiltersDto
    {
        public string? SearchTerm { get; set; }
        public string? RequestType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class DashboardStateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }

    public class RequestDataDto
    {
        public int Id { get; set; }
        public string PatientFullName { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string RequestorName { get; set; } = string.Empty;
        public string? PhysicianName { get; set; }
        public string? DateOfService { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? RequestStatus { get; set; }
        public int RequestType { get; set; }
        public string RequestedDate { get; set; } = string.Empty;
    }

    public class DashboardResponseDto
    {
        public List<RequestDataDto> Requests { get; set; } = new();
        public int TotalCount { get; set; }
        public List<DashboardStateDto> StateCounts { get; set; } = new();
    }
} 