using HalloDoc.Entities.Data.Entities;
using HalloDoc.Entities.Data.Context;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.DTOs.Pagination;
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

        // Admin Dashboard Methods
        public async Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStatusIdsAsync(int[] statusIds, PaginationRequestDto<DashboardFiltersDto> request)
        {
            var query = _db.Requests
                .Include(r => r.RequestClient)
                .Include(r => r.Physician)
                .Where(r => statusIds.Contains(r.RequestStatus));

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.SearchString.ToLower();
                query = query.Where(r =>
                    (r.RequestClient != null && r.RequestClient.FirstName != null && r.RequestClient.FirstName.ToLower().Contains(search)) ||
                    (r.RequestClient != null && r.RequestClient.LastName != null && r.RequestClient.LastName.ToLower().Contains(search))
                );
            }

            // Apply request type filter
            if (request.Filters.RequestType.HasValue)
            {
                query = query.Where(r => (int)r.RequestType == request.Filters.RequestType.Value);
            }

            // Apply date range filters
            if (request.Filters.FromDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= request.Filters.FromDate.Value);
            }

            if (request.Filters.ToDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt <= request.Filters.ToDate.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = request.SortDirection?.ToLower() == "desc" 
                    ? ApplySorting(query, request.SortColumn, false)
                    : ApplySorting(query, request.SortColumn, true);
            }
            else
            {
                query = query.OrderByDescending(r => r.CreatedAt);
            }

            // Apply pagination
            var results = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var items = results.Select(r => new RequestDataDto
            {
                Id = r.RequestId,
                PatientFullName = r.RequestClient != null ? $"{r.RequestClient.FirstName} {r.RequestClient.LastName}".Trim() : "Unknown",
                DateOfBirth = r.RequestClient != null && r.RequestClient.DOB.HasValue ? r.RequestClient.DOB.Value.ToString("MMM dd, yyyy") : "Unknown",
                RequestorName = r.RequestClient != null ? $"{r.RequestClient.FirstName} {r.RequestClient.LastName}".Trim() : "Unknown",
                PhysicianName = r.Physician != null ? $"{r.Physician.FirstName} {r.Physician.LastName}".Trim() : null,
                DateOfService = r.AcceptedDate?.ToString("MMM dd, yyyy") ?? null,
                Phone = r.RequestClient != null ? r.RequestClient.Phone ?? "Unknown" : "Unknown",
                Address = r.RequestClient != null ? $"{r.RequestClient.Street}, {r.RequestClient.City} {r.RequestClient.State} {r.RequestClient.ZipCode}".Trim() : "Unknown",
                RequestStatus = r.RequestStatus.ToString(),
                RequestType = (int)r.RequestType,
                RequestedDate = r.CreatedAt.ToString("MMM dd, yyyy HH:mm")
            }).ToList();

            return new PaginationResponseDto<RequestDataDto>(
                items, 
                totalCount, 
                request.PageIndex, 
                request.PageSize, 
                request.SortDirection ?? "asc", 
                request.SortColumn ?? string.Empty
            );
        }

        public async Task<PaginationResponseDto<RequestDataDto>> GetAllRequestsAsync(PaginationRequestDto<DashboardFiltersDto> request)
        {
            var query = _db.Requests
                .Include(r => r.RequestClient)
                .Include(r => r.Physician)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.SearchString.ToLower();
                query = query.Where(r =>
                    (r.RequestClient != null && r.RequestClient.FirstName != null && r.RequestClient.FirstName.ToLower().Contains(search)) ||
                    (r.RequestClient != null && r.RequestClient.LastName != null && r.RequestClient.LastName.ToLower().Contains(search))
                );
            }

            // Apply request type filter
            if (request.Filters.RequestType.HasValue)
            {
                query = query.Where(r => (int)r.RequestType == request.Filters.RequestType.Value);
            }

            // Apply date range filters
            if (request.Filters.FromDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt >= request.Filters.FromDate.Value);
            }

            if (request.Filters.ToDate.HasValue)
            {
                query = query.Where(r => r.CreatedAt <= request.Filters.ToDate.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = request.SortDirection?.ToLower() == "desc" 
                    ? ApplySorting(query, request.SortColumn, false)
                    : ApplySorting(query, request.SortColumn, true);
            }
            else
            {
                query = query.OrderByDescending(r => r.CreatedAt);
            }

            // Apply pagination
            var results = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var items = results.Select(r => new RequestDataDto
            {
                Id = r.RequestId,
                PatientFullName = r.RequestClient != null ? $"{r.RequestClient.FirstName} {r.RequestClient.LastName}".Trim() : "Unknown",
                DateOfBirth = r.RequestClient != null && r.RequestClient.DOB.HasValue ? r.RequestClient.DOB.Value.ToString("MMM dd, yyyy") : "Unknown",
                RequestorName = r.RequestClient != null ? $"{r.RequestClient.FirstName} {r.RequestClient.LastName}".Trim() : "Unknown",
                PhysicianName = r.Physician != null ? $"{r.Physician.FirstName} {r.Physician.LastName}".Trim() : null,
                DateOfService = r.AcceptedDate?.ToString("MMM dd, yyyy") ?? null,
                Phone = r.RequestClient != null ? r.RequestClient.Phone ?? "Unknown" : "Unknown",
                Address = r.RequestClient != null ? $"{r.RequestClient.Street}, {r.RequestClient.City} {r.RequestClient.State} {r.RequestClient.ZipCode}".Trim() : "Unknown",
                RequestStatus = r.RequestStatus.ToString(),
                RequestType = (int)r.RequestType,
                RequestedDate = r.CreatedAt.ToString("MMM dd, yyyy HH:mm")
            }).ToList();

            return new PaginationResponseDto<RequestDataDto>(
                items, 
                totalCount, 
                request.PageIndex, 
                request.PageSize, 
                request.SortDirection ?? "asc", 
                request.SortColumn ?? string.Empty
            );
        }

        public async Task<int> GetRequestCountByStatusIdsAsync(int[] statusIds)
        {
            return await _db.Requests
                .Where(r => statusIds.Contains(r.RequestStatus))
                .CountAsync();
        }

        private IQueryable<Request> ApplySorting(IQueryable<Request> query, string sortColumn, bool ascending)
        {
            return sortColumn.ToLower() switch
            {
                "patientfullname" => ascending 
                    ? query.OrderBy(r => r.RequestClient != null ? r.RequestClient.FirstName + " " + r.RequestClient.LastName : "")
                    : query.OrderByDescending(r => r.RequestClient != null ? r.RequestClient.FirstName + " " + r.RequestClient.LastName : ""),
                "requesteddate" => ascending 
                    ? query.OrderBy(r => r.CreatedAt)
                    : query.OrderByDescending(r => r.CreatedAt),
                "requesttype" => ascending 
                    ? query.OrderBy(r => r.RequestType)
                    : query.OrderByDescending(r => r.RequestType),
                "requeststatus" => ascending 
                    ? query.OrderBy(r => r.RequestStatus)
                    : query.OrderByDescending(r => r.RequestStatus),
                _ => ascending 
                    ? query.OrderBy(r => r.CreatedAt)
                    : query.OrderByDescending(r => r.CreatedAt)
            };
        }
    }
} 