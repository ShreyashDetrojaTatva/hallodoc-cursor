using Microsoft.AspNetCore.Mvc;
using HalloDoc.Services.Services;
using HalloDoc.Services.ViewModels;
using HalloDoc.Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using HalloDoc.Repositories.DTOs;

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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            var resetLink = await _authService.GenerateResetPasswordTokenAsync(model.Email);
            if (resetLink == null)
                return Ok(new { message = "If this email exists, a reset link has been sent." });
            // For now, return the reset link in the response (simulate email)
            return Ok(new { message = "If this email exists, a reset link has been sent.", resetLink });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!success)
                return BadRequest(new { message = "Invalid or expired reset token." });

            return Ok(new { message = "Password reset successfully." });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var profile = await _authService.GetProfileAsync();
                if (profile == null)
                {
                    return NotFound(new { message = "Profile not found." });
                }

                return Ok(profile);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while fetching profile." });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var success = await _authService.UpdateProfileAsync(dto);
                if (!success)
                {
                    return NotFound(new { message = "Profile not found." });
                }

                return Ok(new { message = "Profile updated successfully." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while updating profile." });
            }
        }

        public class ForgotPasswordRequest
        {
            public string Email { get; set; } = string.Empty;
        }
    }
} 