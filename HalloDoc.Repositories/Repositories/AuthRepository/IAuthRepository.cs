using HalloDoc.Entities.Data.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.AuthRepository
{
    public interface IAuthRepository
    {
        Users? GetUserByUsernameOrEmail(string usernameOrEmail, Func<IQueryable<Users>, IQueryable<Users>>? include = null);
        Task<Users?> GetUserByEmailAsync(string email);
        Task<bool> UpdateUserPasswordAsync(int userId, string newPasswordHash);
        Task<int> CreatePatientAccountAsync(Users user, Patient patient);
        Task<Patient?> GetPatientByUserIdAsync(int userId);
        Task<Patient> CreatePatientProfileAsync(Patient patient);
    }
} 