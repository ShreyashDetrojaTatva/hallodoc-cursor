using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using HalloDoc.Services.Helpers;
using HalloDoc.Repositories.Repositories.AuthRepository;
using HalloDoc.Entities.Data.Entities;
using HalloDoc.Common.Constants;

namespace HalloDoc.Services.Helpers
{
    public class WorkContext : IWorkContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthRepository _authRepository;
        public WorkContext(IHttpContextAccessor httpContextAccessor, IAuthRepository authRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _authRepository = authRepository;
        }

        public ContextUser? CurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwtToken = null;
            try
            {
                jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            }
            catch
            {
                return null;
            }
            if (jwtToken == null) return null;

            // Use correct claim names (case-sensitive)
            var userIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var accountTypeStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "AccountType")?.Value;
            var roleIdStr = jwtToken.Claims.FirstOrDefault(c => c.Type == "RoleId")?.Value;

            if (!int.TryParse(userIdStr, out var userId)) return null;
            int.TryParse(accountTypeStr, out var accountType);
            int.TryParse(roleIdStr, out var roleId);

            // Fetch user from DB (sync for interface compatibility)
            var user = _authRepository.GetUserWithPatientAsync(userId).GetAwaiter().GetResult();
            if (user == null) return null;

            string? firstName = null;
            string? lastName = null;
            // Use enum for better readability
            if (accountType == (int)AccountType.Admin && user.Admins?.Any() == true)
            {
                var admin = user.Admins.First();
                firstName = admin.FirstName;
                lastName = admin.LastName;
            }
            else if (accountType == (int)AccountType.Physician && user.Physicians?.Any() == true)
            {
                var physician = user.Physicians.First();
                firstName = physician.FirstName;
                lastName = physician.LastName;
            }
            else if (accountType == (int)AccountType.Patient && user.Patients?.Any() == true)
            {
                var patient = user.Patients.First();
                firstName = patient.FirstName;
                lastName = patient.LastName;
            }

            return new ContextUser
            {
                UserId = user.UserId,
                AccountType = user.AccountType,
                RoleId = roleId,
                Email = user.Email,
                Username = user.Username,
                FirstName = firstName,
                LastName = lastName
            };
        }
    }
} 