using HalloDoc.Entities.Data.Entities;

namespace HalloDoc.Repositories.Repositories.PhysicianRepository
{
    public interface IPhysicianRepository
    {
        Task<List<Physician>> GetActivePhysiciansAsync();
        Task<Physician?> GetByIdAsync(int physicianId);
        Task<Physician?> GetByUserIdAsync(int userId);
        Task<bool> UpdateAsync(Physician physician);
        Task<bool> CreateAsync(Physician physician);
        Task<bool> DeleteAsync(int physicianId);
    }
} 