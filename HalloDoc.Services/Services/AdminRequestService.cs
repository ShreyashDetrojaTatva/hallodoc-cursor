using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Repositories.RequestRepository;
using HalloDoc.Repositories.Repositories.PhysicianRepository;
using HalloDoc.Services.Helpers;
using HalloDoc.Services.Services.AgreementService;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Services.Services
{
    public class AdminRequestService : IAdminRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly IWorkContext _workContext;
        private readonly IAgreementService _agreementService;

        public AdminRequestService(
            IRequestRepository requestRepository,
            IPhysicianRepository physicianRepository,
            IWorkContext workContext,
            IAgreementService agreementService)
        {
            _requestRepository = requestRepository;
            _physicianRepository = physicianRepository;
            _workContext = workContext;
            _agreementService = agreementService;
        }

        public async Task<RequestDetailsDto?> GetRequestDetailsAsync(int requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || !request.RequestClients.Any())
            {
                return null;
            }

            // Check if current user is a physician and verify ownership
            var currentUser = _workContext.CurrentUser();
            if (currentUser != null)
            {
                var physician = await _physicianRepository.GetByUserIdAsync(currentUser.UserId);
                if (physician != null && request.PhysicianId != physician.PhysicianId)
                {
                    // Physician can only view their assigned requests
                    return null;
                }
            }

            var requestClient = request.RequestClients.FirstOrDefault();
            if (requestClient == null)
            {
                return null;
            }

            return new RequestDetailsDto
            {
                RequestId = request.RequestId,

                // Patient Information (from RequestClient)
                PatientFirstName = requestClient.FirstName ?? string.Empty,
                PatientLastName = requestClient.LastName ?? string.Empty,
                PatientDOB = requestClient.DOB,
                PatientPhone = requestClient.Phone ?? string.Empty,
                PatientEmail = requestClient.Email ?? string.Empty,
                PatientStreet = requestClient.Street ?? string.Empty,
                PatientCity = requestClient.City ?? string.Empty,
                PatientState = requestClient.State ?? string.Empty,
                PatientZipCode = requestClient.ZipCode ?? string.Empty,

                // Request Information
                RequestType = request.RequestType,
                RequestStatus = request.RequestStatus,
                Symptoms = request.Symptoms ?? string.Empty,
                CreatedAt = request.CreatedAt,

                // Requestor Information (from Request entity)
                RequestorFirstName = request.RequestorFirstName ?? string.Empty,
                RequestorLastName = request.RequestorLastName ?? string.Empty,
                RequestorPhone = request.RequestorPhone ?? string.Empty,
                RequestorEmail = request.RequestorEmail ?? string.Empty,
                RequestorRelation = request.RelationWithPatient ?? "Self"
            };
        }

        public async Task<bool> UpdateRequestAsync(UpdateRequestDto updateRequest)
        {
            try
            {
                var request = await _requestRepository.GetByIdAsync(updateRequest.RequestId);
                if (request == null || !request.RequestClients.Any())
                {
                    return false;
                }

                // Check if current user is a physician and verify ownership
                var currentUser = _workContext.CurrentUser();
                if (currentUser != null)
                {
                    var physician = await _physicianRepository.GetByUserIdAsync(currentUser.UserId);
                    if (physician != null && request.PhysicianId != physician.PhysicianId)
                    {
                        // Physician can only update their assigned requests
                        return false;
                    }
                }

                var requestClient = request.RequestClients.FirstOrDefault();
                if (requestClient == null)
                {
                    return false;
                }

                // Update Requestor information
                request.RequestorFirstName = updateRequest.RequestorFirstName;
                request.RequestorLastName = updateRequest.RequestorLastName;
                request.RequestorPhone = updateRequest.RequestorPhone;
                request.RequestorEmail = updateRequest.RequestorEmail;
                request.RelationWithPatient = updateRequest.RequestorRelation;

                // Update Request information
                request.Symptoms = updateRequest.Symptoms;

                // Update RequestClient properties
                requestClient.FirstName = updateRequest.PatientFirstName;
                requestClient.LastName = updateRequest.PatientLastName;
                requestClient.DOB = updateRequest.PatientDOB?.ToLocalTime(); // Ensure local time for timestamp without timezone
                requestClient.Phone = updateRequest.PatientPhone;
                requestClient.Email = updateRequest.PatientEmail;
                requestClient.Street = updateRequest.PatientStreet;
                requestClient.City = updateRequest.PatientCity;
                requestClient.State = updateRequest.PatientState;
                requestClient.ZipCode = updateRequest.PatientZipCode;

                // Save changes
                return await _requestRepository.UpdateAsync(request);
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PhysicianDto>> GetPhysiciansAsync()
        {
            try
            {
                var physicians = await _physicianRepository.GetActivePhysiciansAsync();
                return physicians.Select(p => new PhysicianDto
                {
                    PhysicianId = p.PhysicianId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.User?.Email ?? string.Empty,
                    Phone = p.User?.PhoneNumber ?? string.Empty
                }).ToList();
            }
            catch
            {
                return new List<PhysicianDto>();
            }
        }

        public async Task<bool> AssignRequestAsync(AssignRequestDto assignRequest)
        {
            try
            {
                // Only admins can assign requests
                var currentUser = _workContext.CurrentUser();
                if (currentUser == null)
                {
                    return false;
                }

                var physician = await _physicianRepository.GetByUserIdAsync(currentUser.UserId);
                if (physician != null)
                {
                    // Physicians cannot assign requests
                    return false;
                }

                var request = await _requestRepository.GetByIdAsync(assignRequest.RequestId);
                if (request == null)
                {
                    return false;
                }

                // Assign physician to request
                request.PhysicianId = assignRequest.PhysicianId;
                // Note: Request status remains unchanged as per business requirement

                return await _requestRepository.UpdateAsync(request);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelRequestAsync(CancelRequestDto cancelRequest)
        {
            try
            {
                // Only admins can cancel requests
                var currentUser = _workContext.CurrentUser();
                if (currentUser == null)
                {
                    return false;
                }

                var physician = await _physicianRepository.GetByUserIdAsync(currentUser.UserId);
                if (physician != null)
                {
                    // Physicians cannot cancel requests
                    return false;
                }

                var request = await _requestRepository.GetByIdAsync(cancelRequest.RequestId);
                if (request == null)
                {
                    return false;
                }

                // Cancel the request (change status to Cancelled)
                request.RequestStatus = (int)HalloDoc.Common.Constants.RequestStatus.Cancelled;
                // Note: CancellationReason will be stored in notes table later

                return await _requestRepository.UpdateAsync(request);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SendAgreementAsync(SendAgreementDto sendAgreementDto)
        {
            try
            {
                return await _agreementService.SendAgreementAsync(sendAgreementDto);
            }
            catch
            {
                return false;
            }
        }
    }
}