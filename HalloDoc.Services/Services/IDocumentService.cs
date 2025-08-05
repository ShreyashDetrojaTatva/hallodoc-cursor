using HalloDoc.Repositories.DTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HalloDoc.Services.Services
{
    public interface IDocumentService
    {
        Task<List<DocumentDto>> GetRequestDocumentsForAdminAsync(int requestId);
        Task<List<DocumentDto>> GetRequestDocumentsForPhysicianAsync(int requestId);
        Task<(byte[] FileContents, string ContentType, string FileName)> GetDocumentFileAsync(int documentId, bool isAdmin);
        Task<bool> UploadDocumentAsync(int requestId, IFormFile file);
        Task<bool> DeleteDocumentAsync(int documentId, bool isAdmin);
        Task<bool> EmailDocumentsAsync(EmailDocumentsDto emailDto, bool isAdmin);
    }
} 