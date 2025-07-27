using HalloDoc.Entities.Data.Entities;
using HalloDoc.Entities.Data.Context;
using HalloDoc.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Mappers;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace HalloDoc.Repositories.Repositories.AuthRepository
{
    public class AuthRepository : BaseRepository<Users>, IAuthRepository
    {
        public AuthRepository(ApplicationDbContext db) : base(db) { }

        public Users? GetUserByUsernameOrEmail(string usernameOrEmail, Func<IQueryable<Users>, IQueryable<Users>>? include = null)
        {
            IQueryable<Users> query = _db.Users.Where(u => (u.Username == usernameOrEmail || u.Email == usernameOrEmail) && !u.IsDeleted && u.IsActive);
            if (include != null)
            {
                query = include(query);
            }
            return query.FirstOrDefault();
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<bool> UpdateUserPasswordAsync(int userId, string newPasswordHash)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.PasswordHash = newPasswordHash;
            user.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> CreatePatientAccountAsync(Users user, Patient patient)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Add user
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                // Set user ID for patient
                patient.UserId = user.UserId;

                // Add patient
                await _db.Patients.AddAsync(patient);
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return user.UserId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Patient?> GetPatientByUserIdAsync(int userId)
        {
            return await _db.Patients.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
        }

        public async Task<Patient> CreatePatientProfileAsync(Patient patient)
        {
            await _db.Patients.AddAsync(patient);
            await _db.SaveChangesAsync();
            return patient;
        }
    }
} 