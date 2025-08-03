using HalloDoc.Entities.Data.Context;
using HalloDoc.Entities.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Repositories.Repositories.PhysicianRepository
{
    public class PhysicianRepository : IPhysicianRepository
    {
        private readonly ApplicationDbContext _db;

        public PhysicianRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Physician>> GetActivePhysiciansAsync()
        {
            return await _db.Physicians
                .Include(p => p.User) // Include User to get Email and PhoneNumber
                .Where(p => !p.IsDeleted) // Only non-deleted physicians
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToListAsync();
        }

        public async Task<Physician?> GetByIdAsync(int physicianId)
        {
            return await _db.Physicians
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PhysicianId == physicianId && !p.IsDeleted);
        }

        public async Task<Physician?> GetByUserIdAsync(int userId)
        {
            return await _db.Physicians
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
        }

        public async Task<bool> UpdateAsync(Physician physician)
        {
            try
            {
                _db.Physicians.Update(physician);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateAsync(Physician physician)
        {
            try
            {
                await _db.Physicians.AddAsync(physician);
                await _db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int physicianId)
        {
            try
            {
                var physician = await _db.Physicians.FindAsync(physicianId);
                if (physician != null)
                {
                    physician.IsDeleted = true; // Soft delete
                    await _db.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
} 