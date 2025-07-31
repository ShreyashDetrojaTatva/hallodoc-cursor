using Microsoft.AspNetCore.Mvc;
using HalloDoc.Services.Services;
using HalloDoc.Repositories.DTOs;
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
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? requestType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filters = new DashboardFiltersDto
                {
                    SearchTerm = searchTerm,
                    RequestType = requestType,
                    Page = page,
                    PageSize = pageSize
                };

                var result = await _adminDashboardService.GetRequestsByStateAsync(state, filters);
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
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? requestType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filters = new DashboardFiltersDto
                {
                    SearchTerm = searchTerm,
                    RequestType = requestType,
                    Page = page,
                    PageSize = pageSize
                };

                var csvData = await _adminDashboardService.ExportRequestsAsync(state, filters);
                return File(csvData, "text/csv", $"{state.ToString().ToLower()}-requests.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("export-all")]
        public async Task<IActionResult> ExportAllRequests(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? requestType = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var filters = new DashboardFiltersDto
                {
                    SearchTerm = searchTerm,
                    RequestType = requestType,
                    Page = page,
                    PageSize = pageSize
                };

                var csvData = await _adminDashboardService.ExportAllRequestsAsync(filters);
                return File(csvData, "text/csv", "all-requests.csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }
    }
} 