using HalloDoc.Repositories.DTOs;
using HalloDoc.Services.Services.RequestService;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        public RequestController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromBody] RequestCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _requestService.CreateRequestAsync(dto);
            // Return only the RequestId and a success message to avoid object cycles
            return Ok(new { requestId = result.RequestId, message = "Request created successfully" });
        }
    }
} 