using Microsoft.AspNetCore.Mvc;
using HalloDoc.Services.Services;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.DTOs.Pagination;
using HalloDoc.Common.Constants;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequestsByState(
            [FromQuery] DashboardRequestStatus state,
            [FromQuery] string? searchString = null,
            [FromQuery] int? requestType = null,
            [FromQuery] int? regionId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortColumn = null,
            [FromQuery] string? sortDirection = "asc")
        {
            try
            {
                var filters = new DashboardFiltersDto
                {
                    SearchTerm = searchString,
                    RequestType = requestType,
                    RegionId = regionId,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var request = new PaginationRequestDto<DashboardFiltersDto>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection,
                    SearchString = searchString,
                    Filters = filters
                };

                var result = await _adminDashboardService.GetRequestsByStateAsync(state, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("state-counts")]
        public async Task<IActionResult> GetStateCounts()
        {
            try
            {
                var stateCounts = await _adminDashboardService.GetStateCountsAsync();
                return Ok(stateCounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportRequests(
            [FromQuery] DashboardRequestStatus state,
            [FromQuery] string? searchString = null,
            [FromQuery] int? requestType = null,
            [FromQuery] int? regionId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortColumn = null,
            [FromQuery] string? sortDirection = "asc")
        {
            try
            {
                var filters = new DashboardFiltersDto
                {
                    SearchTerm = searchString,
                    RequestType = requestType,
                    RegionId = regionId,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var request = new PaginationRequestDto<DashboardFiltersDto>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection,
                    SearchString = searchString,
                    Filters = filters
                };

                var csvData = await _adminDashboardService.ExportRequestsAsync(state, request);
                return File(csvData, "text/csv", $"{state.ToString().ToLower()}-requests.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("export-all")]
        public async Task<IActionResult> ExportAllRequests(
            [FromQuery] string? searchString = null,
            [FromQuery] int? requestType = null,
            [FromQuery] int? regionId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortColumn = null,
            [FromQuery] string? sortDirection = "asc")
        {
            try
            {
                var filters = new DashboardFiltersDto
                {
                    SearchTerm = searchString,
                    RequestType = requestType,
                    RegionId = regionId,
                    FromDate = fromDate,
                    ToDate = toDate
                };

                var request = new PaginationRequestDto<DashboardFiltersDto>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection,
                    SearchString = searchString,
                    Filters = filters
                };

                var csvData = await _adminDashboardService.ExportAllRequestsAsync(request);
                return File(csvData, "text/csv", "all-requests.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }
} 