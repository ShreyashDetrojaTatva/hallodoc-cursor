using Microsoft.AspNetCore.Mvc;
using HalloDoc.Services.Services.AgreementService;
using HalloDoc.Repositories.DTOs;
using System.Threading.Tasks;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgreementController : ControllerBase
    {
        private readonly IAgreementService _agreementService;

        public AgreementController(IAgreementService agreementService)
        {
            _agreementService = agreementService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendAgreement([FromBody] SendAgreementDto sendAgreementDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _agreementService.SendAgreementAsync(sendAgreementDto);
            if (!result)
            {
                return BadRequest(new { message = "Failed to send agreement. Request may not be eligible or patient email not found." });
            }

            return Ok(new { message = "Agreement sent successfully" });
        }

        [HttpGet("details/{token}")]
        public async Task<IActionResult> GetAgreementDetails(string token)
        {
            var agreementDetails = await _agreementService.GetAgreementDetailsAsync(token);
            if (agreementDetails == null)
            {
                return NotFound(new { message = "Agreement not found or invalid token" });
            }

            return Ok(agreementDetails);
        }

        [HttpPost("respond")]
        public async Task<IActionResult> ProcessAgreementResponse([FromBody] AgreementResponseDto agreementResponseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _agreementService.ProcessAgreementResponseAsync(agreementResponseDto);
            if (!result)
            {
                return BadRequest(new { message = "Failed to process agreement response. Token may be invalid or already used." });
            }

            var message = agreementResponseDto.IsAccepted
                ? "Agreement accepted successfully. Your medical care will proceed."
                : "Agreement cancelled successfully.";

            return Ok(new { message });
        }

        [HttpGet("validate/{token}")]
        public async Task<IActionResult> ValidateToken(string token)
        {
            var isValid = await _agreementService.IsTokenValidAsync(token);
            return Ok(new { isValid });
        }
    }
}