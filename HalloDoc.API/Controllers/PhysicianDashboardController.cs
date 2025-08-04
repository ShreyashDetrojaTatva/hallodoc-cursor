using Microsoft.AspNetCore.Mvc;
using HalloDoc.Services.Services;
using HalloDoc.Repositories.DTOs.Pagination;
using HalloDoc.Common.Constants;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhysicianDashboardController : ControllerBase
    {
        private readonly IPhysicianDashboardService _physicianDashboardService;

        public PhysicianDashboardController(IPhysicianDashboardService physicianDashboardService)
        {
            _physicianDashboardService = physicianDashboardService;
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests([FromQuery] DashboardRequestStatus state, [FromQuery] PaginationRequestDto<DashboardFiltersDto> request)
        {
            try
            {
                var result = await _physicianDashboardService.GetRequestsByStateAsync(state, request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("state-counts")]
        public async Task<IActionResult> GetStateCounts()
        {
            try
            {
                var result = await _physicianDashboardService.GetStateCountsAsync();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportRequests([FromQuery] DashboardRequestStatus state, [FromQuery] PaginationRequestDto<DashboardFiltersDto> request)
        {
            try
            {
                var csvData = await _physicianDashboardService.ExportRequestsAsync(state, request);
                return File(csvData, "text/csv", $"physician-requests-{state}-{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("export-all")]
        public async Task<IActionResult> ExportAllRequests([FromQuery] PaginationRequestDto<DashboardFiltersDto> request)
        {
            try
            {
                var csvData = await _physicianDashboardService.ExportAllRequestsAsync(request);
                return File(csvData, "text/csv", $"physician-all-requests-{DateTime.Now:yyyyMMdd}.csv");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 