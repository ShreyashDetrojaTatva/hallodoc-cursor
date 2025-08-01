using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Repositories.DTOs.Pagination
{
    public class PaginationRequestDto<T> where T : class, new()
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageIndex must be greater than 0")]
        public int PageIndex { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        public string? SortColumn { get; set; }

        public string? SortDirection { get; set; } = "asc"; // asc, desc

        public string? SearchString { get; set; }

        public T Filters { get; set; } = new T();
    }
} 