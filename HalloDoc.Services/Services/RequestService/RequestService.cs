using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Repositories.RequestRepository;
using System.Threading.Tasks;
using HalloDoc.Repositories.Repositories.DocumentRepository;
using HalloDoc.Repositories.Repositories.AuthRepository;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using HalloDoc.Common.Constants;
using HalloDoc.Common.Helpers;

namespace HalloDoc.Services.Services.RequestService
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IDocumentRepository _documentRepository;
        private readonly IAuthRepository _authRepository;

        public RequestService(
            IRequestRepository requestRepository, 
            IDocumentRepository documentRepository,
            IAuthRepository authRepository)
        {
            _requestRepository = requestRepository;
            _documentRepository = documentRepository;
            _authRepository = authRepository;
        }

        public async Task<Request> CreateRequestAsync(RequestCreateDto dto)
        {
            // Check if patient exists
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);
            
            if (existingUser == null)
            {
                // Check if email is already in use (double-check to prevent race conditions)
                var emailExists = await _authRepository.GetUserByEmailAsync(dto.Email);
                if (emailExists != null)
                {
                    throw new InvalidOperationException("Email is already in use.");
                }

                // Create new user account
                var newUser = new Users
                {
                    Email = dto.Email,
                    Username = dto.Email, // Use email as username
                    PhoneNumber = dto.Phone,
                    PasswordHash = "password", // Default password
                    AccountType = (int)AccountType.Patient,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                // Create patient profile
                var newPatient = new Patient
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DOB = dto.DOB,
                    Address = dto.Street,
                    City = dto.City,
                    ZipCode = dto.ZipCode,
                    Status = (int)Status.Active,
                    CreatedAt = DateTime.Now
                };

                try
                {
                    // Save user and patient
                    var userId = await _authRepository.CreatePatientAccountAsync(newUser, newPatient);

                    // Generate reset password link
                    var resetToken = JwtHelper.GenerateResetPasswordToken(userId, dto.Email);
                    var resetLink = $"http://localhost:4300/reset-password?token={resetToken}";

                    // Send email with reset link
                    await EmailHelper.SendMail(
                        dto.Email,
                        "Welcome to HalloDoc - Set Your Password",
                        $"Welcome to HalloDoc! Please click the following link to set your password: {resetLink}"
                    );

                    // Update DTO with new patient ID
                    dto.PatientId = newPatient.PatientId;
                }
                catch (Exception ex)
                {
                    // Log the error
                    throw new InvalidOperationException("Failed to create patient account.", ex);
                }
            }
            else
            {
                // Get existing patient ID
                var patient = await _authRepository.GetPatientByUserIdAsync(existingUser.UserId);
                if (patient != null)
                {
                    dto.PatientId = patient.PatientId;
                }
                else
                {
                    // Existing user but no patient profile - create one
                    var newPatient = new Patient
                    {
                        UserId = existingUser.UserId,
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        DOB = dto.DOB,
                        Address = dto.Street,
                        City = dto.City,
                        ZipCode = dto.ZipCode,
                        Status = (int)Status.Active,
                        CreatedAt = DateTime.Now
                    };

                    newPatient = await _authRepository.CreatePatientProfileAsync(newPatient);
                    dto.PatientId = newPatient.PatientId;
                }
            }

            // Create request
            var request = await _requestRepository.CreateRequestAsync(dto);

            // Handle file uploads
            if (dto.Files != null && dto.Files.Count > 0)
            {
                var savedPaths = await FileHelper.SaveRequestFilesAsync(request.RequestId, dto.Files);
                var documents = new List<Document>();
                for (int i = 0; i < savedPaths.Count; i++)
                {
                    var fileName = Path.GetFileName(savedPaths[i]);
                    documents.Add(new Document
                    {
                        RequestId = request.RequestId,
                        FileName = fileName,
                        FilePath = savedPaths[i],
                        UploadedAt = DateTime.Now,
                        UploadedBy = null // Set as needed
                    });
                }
                await _documentRepository.AddDocumentsAsync(documents);
            }

            return request;
        }

        public async Task<List<RequestListDto>> GetPatientRequestsAsync(int patientId, RequestFilterDto? filter = null)
        {
            return await _requestRepository.GetPatientRequestsAsync(patientId, filter);
        }

        public async Task<List<DocumentDto>> GetRequestDocumentsAsync(int requestId, int userId)
        {
            // Get the patient associated with the user
            var patient = await _authRepository.GetPatientByUserIdAsync(userId);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient profile not found.");
            }

            // Get the request to verify ownership
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || request.PatientId != patient.PatientId)
            {
                throw new InvalidOperationException("Request not found or access denied.");
            }

            return await _documentRepository.GetRequestDocumentsAsync(requestId);
        }

        public async Task<(byte[] FileContents, string ContentType, string FileName)> GetDocumentFileAsync(int documentId, int userId)
        {
            // Get the document
            var document = await _documentRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                throw new InvalidOperationException("Document not found.");
            }

            // Get the patient associated with the user
            var patient = await _authRepository.GetPatientByUserIdAsync(userId);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient profile not found.");
            }

            // Get the request to verify ownership
            var request = await _requestRepository.GetByIdAsync(document.RequestId);
            if (request == null || request.PatientId != patient.PatientId)
            {
                throw new InvalidOperationException("Access denied.");
            }

            // Read the file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), document.FilePath);
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException("File not found on server.");
            }

            var fileContents = await File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(document.FileName);

            return (fileContents, contentType, document.FileName);
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
} 