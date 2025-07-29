using Microsoft.AspNetCore.Mvc;
using HalloDoc.Services.Services.RequestService;
using HalloDoc.Repositories.DTOs;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using HalloDoc.Repositories.Repositories.AuthRepository;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IAuthRepository _authRepository;

        public RequestController(IRequestService requestService, IAuthRepository authRepository)
        {
            _requestService = requestService;
            _authRepository = authRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequest([FromForm] RequestCreateDto dto)
        {
            try
            {
                var request = await _requestService.CreateRequestAsync(dto);
                return Ok(new { requestId = request.RequestId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("patient")]
        [Authorize]
        public async Task<IActionResult> GetPatientRequests([FromQuery] RequestFilterDto? filter)
        {
            try
            {
                var userIdClaim = User.FindFirst(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user token." });
                }

                var patient = await _authRepository.GetPatientByUserIdAsync(userId);
                if (patient == null)
                {
                    return NotFound(new { message = "Patient profile not found." });
                }

                var requests = await _requestService.GetPatientRequestsAsync(patient.PatientId, filter);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching requests." });
            }
        }

        [HttpGet("{requestId}/documents")]
        [Authorize]
        public async Task<IActionResult> GetRequestDocuments(int requestId)
        {
            try
            {
                var userIdClaim = User.FindFirst(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user token." });
                }

                var documents = await _requestService.GetRequestDocumentsAsync(requestId, userId);
                return Ok(documents);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while fetching documents." });
            }
        }

        [HttpGet("documents/{documentId}/download")]
        [Authorize]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            try
            {
                var userIdClaim = User.FindFirst(c => c.Type == "UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { message = "Invalid user token." });
                }

                var (fileContents, contentType, fileName) = await _requestService.GetDocumentFileAsync(documentId, userId);
                return File(fileContents, contentType, fileName);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while downloading the document." });
            }
        }
    }
} 