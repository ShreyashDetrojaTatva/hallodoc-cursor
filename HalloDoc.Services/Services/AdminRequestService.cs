using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Repositories.RequestRepository;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Services.Services
{
    public class AdminRequestService : IAdminRequestService
    {
        private readonly IRequestRepository _requestRepository;

        public AdminRequestService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<RequestDetailsDto?> GetRequestDetailsAsync(int requestId)
        {
            var request = await _requestRepository.GetByIdAsync(requestId);
            if (request == null || !request.RequestClients.Any())
            {
                return null;
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
                var physicians = await _requestRepository.GetPhysiciansAsync();
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
    }
} 