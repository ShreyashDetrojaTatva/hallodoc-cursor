namespace HalloDoc.Repositories.DTOs.Pagination
{
    public class PaginationResponseDto<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortDirection { get; set; } = "asc";
        public string SortColumn { get; set; } = string.Empty;
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public PaginationResponseDto()
        {
        }

        public PaginationResponseDto(List<T> items, int count, int pageIndex, int pageSize, string sortDirection, string sortColumn)
        {
            Items = items;
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            SortDirection = sortDirection;
            SortColumn = sortColumn;
        }
    }
} 