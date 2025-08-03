using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HalloDoc.Services.Services;
using HalloDoc.Repositories.DTOs;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminRequestController : ControllerBase
    {
        private readonly IAdminRequestService _adminRequestService;

        public AdminRequestController(IAdminRequestService adminRequestService)
        {
            _adminRequestService = adminRequestService;
        }

        [HttpGet("details/{requestId}")]
        public async Task<IActionResult> GetRequestDetails(int requestId)
        {
            var result = await _adminRequestService.GetRequestDetailsAsync(requestId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateRequest([FromBody] UpdateRequestDto updateRequest)
        {
            var result = await _adminRequestService.UpdateRequestAsync(updateRequest);
            if (!result)
            {
                return BadRequest("Failed to update request");
            }
            return Ok(new { message = "Request updated successfully" });
        }

        [HttpGet("physicians")]
        public async Task<IActionResult> GetPhysicians()
        {
            var physicians = await _adminRequestService.GetPhysiciansAsync();
            return Ok(physicians);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignRequest([FromBody] AssignRequestDto assignRequest)
        {
            var result = await _adminRequestService.AssignRequestAsync(assignRequest);
            if (!result)
            {
                return BadRequest("Failed to assign request");
            }
            return Ok(new { message = "Request assigned successfully" });
        }
    }
} 