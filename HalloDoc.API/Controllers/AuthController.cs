using HalloDoc.Services.Services;
using HalloDoc.Services.ViewModels;
using HalloDoc.Services.Helpers;
using Microsoft.AspNetCore.Mvc;
using HalloDoc.Common.Constants;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            var user = _authService.ValidateUser(model);
            if (user == null)
                return Unauthorized(new { message = "Invalid username/email or password." });
            var token = _authService.GenerateJwtToken(user);
            return Ok(new { token, user });
        }
    }
} 