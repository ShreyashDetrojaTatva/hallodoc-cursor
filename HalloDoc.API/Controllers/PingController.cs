using HalloDoc.Services.Services.PingService;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        private readonly IPingService _pingService;
        public PingController(IPingService pingService)
        {
            _pingService = pingService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _pingService.GetPingAsync();
            return Ok(result);
        }

        [HttpGet("first")]
        public async Task<IActionResult> GetFirst()
        {
            var message = await _pingService.GetFirstPingMessageAsync();
            return Ok(message);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            // For demo, get all pings from the repository via the service
            var pings = await _pingService.GetAllPings();
            return Ok(pings);
        }
    }
} 