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
                .Include(r => r.RequestClients)
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
                        (r.RequestClients.Any() && r.RequestClients.First().FirstName != null && r.RequestClients.First().FirstName.ToLower().Contains(search)) ||
                        (r.RequestClients.Any() && r.RequestClients.First().LastName != null && r.RequestClients.First().LastName.ToLower().Contains(search)) ||
                        (r.Symptoms != null && r.Symptoms.ToLower().Contains(search))
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
                    FirstName = r.RequestClients.Any() ? r.RequestClients.First().FirstName ?? string.Empty : string.Empty,
                    LastName = r.RequestClients.Any() ? r.RequestClients.First().LastName ?? string.Empty : string.Empty,
                    Symptoms = r.Symptoms ?? string.Empty
                })
                .ToListAsync();
        }

        public async Task<Request?> GetByIdAsync(int requestId)
        {
            return await _db.Requests
                .Include(r => r.RequestClients)
                .Include(r => r.Physician)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        // Admin Dashboard Methods
        public async Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStatusIdsAsync(int[] statusIds, PaginationRequestDto<DashboardFiltersDto> request)
        {
            var query = _db.Requests
                .Include(r => r.RequestClients)
                .Include(r => r.Physician)
                .Where(r => statusIds.Contains(r.RequestStatus));

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.SearchString.ToLower();
                query = query.Where(r =>
                    (r.RequestClients.Any() && r.RequestClients.First().FirstName != null && r.RequestClients.First().FirstName.ToLower().Contains(search)) ||
                    (r.RequestClients.Any() && r.RequestClients.First().LastName != null && r.RequestClients.First().LastName.ToLower().Contains(search))
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
                PatientFullName = r.RequestClients.Any() ? $"{r.RequestClients.First().FirstName} {r.RequestClients.First().LastName}".Trim() : "Unknown",
                DateOfBirth = r.RequestClients.Any() && r.RequestClients.First().DOB.HasValue ? r.RequestClients.First().DOB.Value.ToString("MMM dd, yyyy") : "Unknown",
                RequestorName = r.RequestClients.Any() ? $"{r.RequestClients.First().FirstName} {r.RequestClients.First().LastName}".Trim() : "Unknown",
                PhysicianName = r.Physician != null ? $"{r.Physician.FirstName} {r.Physician.LastName}".Trim() : null,
                PhysicianId = r.PhysicianId,
                DateOfService = r.AcceptedDate?.ToString("MMM dd, yyyy") ?? null,
                Phone = r.RequestClients.Any() ? r.RequestClients.First().Phone ?? "Unknown" : "Unknown",
                Address = r.RequestClients.Any() ? $"{r.RequestClients.First().Street}, {r.RequestClients.First().City} {r.RequestClients.First().State} {r.RequestClients.First().ZipCode}".Trim() : "Unknown",
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
                .Include(r => r.RequestClients)
                .Include(r => r.Physician)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.SearchString.ToLower();
                query = query.Where(r =>
                    (r.RequestClients.Any() && r.RequestClients.First().FirstName != null && r.RequestClients.First().FirstName.ToLower().Contains(search)) ||
                    (r.RequestClients.Any() && r.RequestClients.First().LastName != null && r.RequestClients.First().LastName.ToLower().Contains(search))
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
                PatientFullName = r.RequestClients.Any() ? $"{r.RequestClients.First().FirstName} {r.RequestClients.First().LastName}".Trim() : "Unknown",
                DateOfBirth = r.RequestClients.Any() && r.RequestClients.First().DOB.HasValue ? r.RequestClients.First().DOB.Value.ToString("MMM dd, yyyy") : "Unknown",
                RequestorName = r.RequestClients.Any() ? $"{r.RequestClients.First().FirstName} {r.RequestClients.First().LastName}".Trim() : "Unknown",
                PhysicianName = r.Physician != null ? $"{r.Physician.FirstName} {r.Physician.LastName}".Trim() : null,
                PhysicianId = r.PhysicianId,
                DateOfService = r.AcceptedDate?.ToString("MMM dd, yyyy") ?? null,
                Phone = r.RequestClients.Any() ? r.RequestClients.First().Phone ?? "Unknown" : "Unknown",
                Address = r.RequestClients.Any() ? $"{r.RequestClients.First().Street}, {r.RequestClients.First().City} {r.RequestClients.First().State} {r.RequestClients.First().ZipCode}".Trim() : "Unknown",
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

        // Physician Dashboard Methods
        public async Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStatusIdsForPhysicianAsync(int[] statusIds, PaginationRequestDto<DashboardFiltersDto> request, int physicianId)
        {
            var query = _db.Requests
                .Include(r => r.RequestClients)
                .Include(r => r.Physician)
                .Where(r => statusIds.Contains(r.RequestStatus) && r.PhysicianId == physicianId);

            // Apply search filter
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                var search = request.SearchString.ToLower();
                query = query.Where(r =>
                    (r.RequestClients.Any() && r.RequestClients.First().FirstName != null && r.RequestClients.First().FirstName.ToLower().Contains(search)) ||
                    (r.RequestClients.Any() && r.RequestClients.First().LastName != null && r.RequestClients.First().LastName.ToLower().Contains(search)) ||
                    (r.Symptoms != null && r.Symptoms.ToLower().Contains(search))
                );
            }

            // Apply additional filters
            if (request.Filters != null)
            {
                if (!string.IsNullOrEmpty(request.Filters.SearchTerm))
                {
                    var search = request.Filters.SearchTerm.ToLower();
                    query = query.Where(r =>
                        (r.RequestClients.Any() && r.RequestClients.First().FirstName != null && r.RequestClients.First().FirstName.ToLower().Contains(search)) ||
                        (r.RequestClients.Any() && r.RequestClients.First().LastName != null && r.RequestClients.First().LastName.ToLower().Contains(search)) ||
                        (r.Symptoms != null && r.Symptoms.ToLower().Contains(search))
                    );
                }

                if (request.Filters.RequestType.HasValue)
                {
                    query = query.Where(r => r.RequestType == request.Filters.RequestType.Value);
                }

                if (request.Filters.FromDate.HasValue)
                {
                    query = query.Where(r => r.CreatedAt >= request.Filters.FromDate.Value);
                }

                if (request.Filters.ToDate.HasValue)
                {
                    query = query.Where(r => r.CreatedAt <= request.Filters.ToDate.Value);
                }
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = ApplySorting(query, request.SortColumn, request.SortDirection == "asc");
            }
            else
            {
                query = query.OrderByDescending(r => r.CreatedAt);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var results = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Map to DTOs
            var items = results.Select(r => new RequestDataDto
            {
                Id = r.RequestId,
                PatientFullName = r.RequestClients.Any() ? $"{r.RequestClients.First().FirstName} {r.RequestClients.First().LastName}".Trim() : "Unknown",
                DateOfBirth = r.RequestClients.Any() && r.RequestClients.First().DOB.HasValue ? r.RequestClients.First().DOB.Value.ToString("MMM dd, yyyy") : "Unknown",
                RequestorName = r.RequestClients.Any() ? $"{r.RequestClients.First().FirstName} {r.RequestClients.First().LastName}".Trim() : "Unknown",
                PhysicianName = r.Physician != null ? $"{r.Physician.FirstName} {r.Physician.LastName}".Trim() : null,
                PhysicianId = r.PhysicianId,
                DateOfService = r.AcceptedDate?.ToString("MMM dd, yyyy") ?? null,
                Phone = r.RequestClients.Any() ? r.RequestClients.First().Phone ?? "Unknown" : "Unknown",
                Address = r.RequestClients.Any() ? $"{r.RequestClients.First().Street}, {r.RequestClients.First().City} {r.RequestClients.First().State} {r.RequestClients.First().ZipCode}".Trim() : "Unknown",
                RequestStatus = r.RequestStatus.ToString(),
                RequestType = (int)r.RequestType,
                RequestedDate = r.CreatedAt.ToString("MMM dd, yyyy HH:mm")
            }).ToList();

            return new PaginationResponseDto<RequestDataDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public async Task<int> GetRequestCountByStatusIdsForPhysicianAsync(int[] statusIds, int physicianId)
        {
            return await _db.Requests
                .Where(r => statusIds.Contains(r.RequestStatus) && r.PhysicianId == physicianId)
                .CountAsync();
        }

        public async Task<bool> UpdateAsync(Request request)
        {
            try
            {
                _db.Requests.Update(request);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private IQueryable<Request> ApplySorting(IQueryable<Request> query, string sortColumn, bool ascending)
        {
            return sortColumn.ToLower() switch
            {
                "patientfullname" => ascending 
                    ? query.OrderBy(r => r.RequestClients.Any() ? r.RequestClients.First().FirstName + " " + r.RequestClients.First().LastName : "")
                    : query.OrderByDescending(r => r.RequestClients.Any() ? r.RequestClients.First().FirstName + " " + r.RequestClients.First().LastName : ""),
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