using HalloDoc.Entities.Data.Entities;
using HalloDoc.Entities.Data.Context;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.RequestRepository
{
    public class RequestRepository : BaseRepository<Request>, IRequestRepository
    {
        public RequestRepository(ApplicationDbContext db) : base(db) { }

        public async Task<Request> CreateRequestAsync(RequestCreateDto dto)
        {
            // Create Request entity using mapper
            var request = RequestMapper.ToRequestEntity(dto);
            
            // Add Request first
            await _db.Requests.AddAsync(request);
            await _db.SaveChangesAsync();

            // Create RequestClient entity using mapper and set the RequestId
            var requestClient = RequestMapper.ToRequestClientEntity(dto);
            requestClient.RequestId = request.RequestId;

            // Add RequestClient
            await _db.RequestClients.AddAsync(requestClient);
            await _db.SaveChangesAsync();

            return request;
        }

        public async Task<List<RequestListDto>> GetPatientRequestsAsync(int patientId, RequestFilterDto? filter = null)
        {
            var query = _db.Requests
                .Include(r => r.RequestClient)
                .Where(r => r.PatientId == patientId);

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Status))
                {
                    query = query.Where(r => r.RequestStatus.ToString() == filter.Status);
                }

                if (filter.StartDate.HasValue)
                {
                    query = query.Where(r => r.CreatedAt >= filter.StartDate.Value);
                }

                if (filter.EndDate.HasValue)
                {
                    query = query.Where(r => r.CreatedAt <= filter.EndDate.Value);
                }

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var search = filter.SearchTerm.ToLower();
                    query = query.Where(r =>
                        (r.RequestClient != null && r.RequestClient.FirstName != null && r.RequestClient.FirstName.ToLower().Contains(search)) ||
                        (r.RequestClient != null && r.RequestClient.LastName != null && r.RequestClient.LastName.ToLower().Contains(search)) ||
                        (r.RequestClient != null && r.RequestClient.Symptoms != null && r.RequestClient.Symptoms.ToLower().Contains(search))
                    );
                }
            }

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RequestListDto
                {
                    RequestId = r.RequestId,
                    RequestType = r.RequestType.ToString(),
                    Status = r.RequestStatus.ToString(),
                    CreatedAt = r.CreatedAt,
                    FirstName = r.RequestClient != null ? r.RequestClient.FirstName ?? string.Empty : string.Empty,
                    LastName = r.RequestClient != null ? r.RequestClient.LastName ?? string.Empty : string.Empty,
                    Symptoms = r.RequestClient != null ? r.RequestClient.Symptoms ?? string.Empty : string.Empty
                })
                .ToListAsync();
        }

        public async Task<Request?> GetByIdAsync(int requestId)
        {
            return await _db.Requests.FindAsync(requestId);
        }
    }
} 