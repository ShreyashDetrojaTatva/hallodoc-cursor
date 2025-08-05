using Microsoft.AspNetCore.Mvc;
using HalloDoc.Services.Services;
using HalloDoc.Repositories.DTOs;
using System;

namespace HalloDoc.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet("admin/{requestId}")]
        public async Task<IActionResult> GetRequestDocumentsForAdmin(int requestId)
        {
            try
            {
                var documents = await _documentService.GetRequestDocumentsForAdminAsync(requestId);
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

        [HttpGet("physician/{requestId}")]
        public async Task<IActionResult> GetRequestDocumentsForPhysician(int requestId)
        {
            try
            {
                var documents = await _documentService.GetRequestDocumentsForPhysicianAsync(requestId);
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

        [HttpGet("download/{documentId}")]
        public async Task<IActionResult> DownloadDocument(int documentId, [FromQuery] bool isAdmin = false)
        {
            try
            {
                var (fileContents, contentType, fileName) = await _documentService.GetDocumentFileAsync(documentId, isAdmin);
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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] int requestId, [FromForm] IFormFile file)
        {
            try
            {
                var success = await _documentService.UploadDocumentAsync(requestId, file);
                if (success)
                {
                    return Ok(new { message = "Document uploaded successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Failed to upload document." });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while uploading the document." });
            }
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(int documentId, [FromQuery] bool isAdmin = false)
        {
            try
            {
                var success = await _documentService.DeleteDocumentAsync(documentId, isAdmin);
                if (success)
                {
                    return Ok(new { message = "Document deleted successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Failed to delete document or access denied." });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the document." });
            }
        }

        [HttpPost("email")]
        public async Task<IActionResult> EmailDocuments([FromBody] EmailDocumentsDto emailDto, [FromQuery] bool isAdmin = false)
        {
            try
            {
                var success = await _documentService.EmailDocumentsAsync(emailDto, isAdmin);
                if (success)
                {
                    return Ok(new { message = "Documents sent successfully." });
                }
                else
                {
                    return BadRequest(new { message = "Failed to send documents." });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while sending documents." });
            }
        }
    }
} 