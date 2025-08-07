using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Repositories.AgreementRepository;
using HalloDoc.Repositories.Repositories.RequestRepository;
using HalloDoc.Common.Helpers;
using HalloDoc.Common.Constants;
using HalloDoc.Entities.Data.Entities;
using System.Threading.Tasks;
using System;

namespace HalloDoc.Services.Services.AgreementService
{
    public class AgreementService : IAgreementService
    {
        private readonly IAgreementRepository _agreementRepository;
        private readonly IRequestRepository _requestRepository;

        public AgreementService(
            IAgreementRepository agreementRepository,
            IRequestRepository requestRepository)
        {
            _agreementRepository = agreementRepository;
            _requestRepository = requestRepository;
        }

        public async Task<bool> SendAgreementAsync(SendAgreementDto sendAgreementDto)
        {
            try
            {
                // Check if request is eligible for agreement
                if (!await _agreementRepository.IsRequestEligibleForAgreementAsync(sendAgreementDto.RequestId))
                {
                    return false;
                }

                // Get request details
                var request = await _requestRepository.GetByIdAsync(sendAgreementDto.RequestId);
                if (request == null)
                {
                    return false;
                }

                // Generate agreement token
                var token = AgreementHelper.GenerateAgreementToken(sendAgreementDto.RequestId);
                var expiry = AgreementHelper.GetAgreementExpiry();

                // Create agreement token
                var agreementToken = new AgreementToken
                {
                    RequestId = sendAgreementDto.RequestId,
                    Token = token,
                    Expiry = expiry,
                    Used = false,
                    CreatedAt = DateTime.Now
                };

                await _agreementRepository.CreateAsync(agreementToken);

                // Generate agreement link
                var agreementLink = $"http://localhost:4300/agreement/{token}";

                // Send email
                var emailSubject = "HalloDoc - Agreement for Medical Request";
                var emailBody = GenerateAgreementEmailBody(request, agreementLink);

                await EmailHelper.SendMail(sendAgreementDto.PatientEmail, emailSubject, emailBody);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending agreement: {ex.Message}");
                return false;
            }
        }

        public async Task<AgreementDetailsDto?> GetAgreementDetailsAsync(string token)
        {
            try
            {
                var agreementToken = await _agreementRepository.GetByTokenAsync(token);
                if (agreementToken == null)
                {
                    return null;
                }

                Request request = agreementToken.Request;
                if (request == null)
                {
                    return null;
                }

                var patient = request.Patient;
                if (patient == null)
                {
                    return null;
                }

                var isExpired = agreementToken.Expiry < DateTime.Now;
                var isUsed = agreementToken.Used;
                var requestClient = request.RequestClients.FirstOrDefault();
                return new AgreementDetailsDto
                {
                    RequestId = request.RequestId,
                    PatientName = $"{patient.FirstName} {patient.LastName}",
                    PatientEmail = patient.User?.Email ?? "",
                    CreatedAt = request.CreatedAt,
                    RequestorName = request.RequestType != (int)RequestType.Patient ? $"{request.RequestorFirstName} {request.RequestorLastName}" : $"{requestClient?.FirstName} {requestClient?.LastName}",
                    RequestorEmail = (request.RequestType != (int)RequestType.Patient ? request.RequestorEmail : requestClient?.Email) ?? "",
                    RequestorPhone = (request.RequestType != (int)RequestType.Patient ? request.RequestorPhone : requestClient?.Phone) ?? "",
                    Symptoms = request.Symptoms,
                    IsExpired = isExpired,
                    IsUsed = isUsed
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ProcessAgreementResponseAsync(AgreementResponseDto agreementResponseDto)
        {
            try
            {
                // Validate token
                if (!await _agreementRepository.IsTokenValidAsync(agreementResponseDto.Token))
                {
                    return false;
                }

                var agreementToken = await _agreementRepository.GetByTokenAsync(agreementResponseDto.Token);
                if (agreementToken == null)
                {
                    return false;
                }

                // Mark token as used
                agreementToken.Used = true;
                agreementToken.UsedAt = DateTime.Now;
                await _agreementRepository.UpdateAsync(agreementToken);

                // Update request status based on response
                var request = await _requestRepository.GetByIdAsync(agreementToken.RequestId);
                if (request == null)
                {
                    return false;
                }

                if (agreementResponseDto.IsAccepted)
                {
                    // Patient accepted - move to MDEnRoute
                    request.RequestStatus = (int)RequestStatus.MDEnRoute;
                }
                else
                {
                    // Patient cancelled - move to CancelledByPatient
                    request.RequestStatus = (int)RequestStatus.CancelledByPatient;
                    // Note: Cancellation reason will be stored in notes table later
                }

                return await _requestRepository.UpdateAsync(request);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            return await _agreementRepository.IsTokenValidAsync(token);
        }

        private string GenerateAgreementEmailBody(Request request, string agreementLink)
        {
            var requestClient = request.RequestClients.FirstOrDefault();
            var patientName = requestClient != null ? $"{requestClient.FirstName} {requestClient.LastName}" : "Patient";
            var requestorName = request.RequestType != (int)RequestType.Patient ? $"{request.RequestorFirstName} {request.RequestorLastName}" : patientName;
            var requestorEmail = request.RequestType != (int)RequestType.Patient ? request.RequestorEmail : requestClient?.Email ?? "";
            var requestorPhone = request.RequestType != (int)RequestType.Patient ? request.RequestorPhone : requestClient?.Phone ?? "";
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <div style='text-align: center; margin-bottom: 30px;'>
                            <h1 style='color: #1976d2;'>HalloDoc</h1>
                        </div>
                        
                        <div style='background-color: #f5f5f5; padding: 20px; border-radius: 8px; margin-bottom: 20px;'>
                            <h2 style='color: #1976d2; margin-top: 0;'>Medical Request Agreement</h2>
                            
                            <p>Dear {patientName},</p>
                            
                            <p>You have a pending medical request that requires your agreement to proceed with care. Please review the details below:</p>
                            
                            <div style='background-color: white; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                                <p><strong>Request Details:</strong></p>
                                <p><strong>Requestor:</strong> {requestorName}</p>
                                <p><strong>Contact:</strong> {requestorEmail} | {requestorPhone}</p>
                                {(!string.IsNullOrEmpty(request.Symptoms) ? $"<p><strong>Symptoms:</strong> {request.Symptoms}</p>" : "")}
                                <p><strong>Created:</strong> {request.CreatedAt.ToString("MMM dd, yyyy")}</p>
                            </div>
                            
                            <p>To proceed with your medical care, please click the button below to review and sign the agreement:</p>
                            
                            <div style='text-align: center; margin: 30px 0;'>
                                <a href='{agreementLink}' 
                                   style='background-color: #1976d2; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; display: inline-block; font-weight: bold;'>
                                    Review Agreement
                                </a>
                            </div>
                            
                            <p style='font-size: 14px; color: #666;'>
                                <strong>Important:</strong> This agreement link will expire in 7 days. Please review and respond promptly.
                            </p>
                        </div>
                        
                        <div style='text-align: center; margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee;'>
                            <p style='color: #666; font-size: 12px;'>
                                If you have any questions, please contact our support team.
                            </p>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}