using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class OwnerClinicLinkRepository : BaseRepository<OwnerClinicLink>, IOwnerClinicLinkRepository
    {
        public OwnerClinicLinkRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<OwnerClinicLink>> GetClinicsByOwnerIdAsync(Guid ownerId)
        {
            return await _dbSet
                .Where(l => l.OwnerId == ownerId && l.IsActive)
                .Include(l => l.Clinic)
                .OrderByDescending(l => l.IsPrimaryOwner)
                .ThenBy(l => l.LinkedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<OwnerClinicLink>> GetOwnersByClinicIdAsync(Guid clinicId)
        {
            return await _dbSet
                .Where(l => l.ClinicId == clinicId && l.IsActive)
                .Include(l => l.Owner)
                .OrderByDescending(l => l.IsPrimaryOwner)
                .ThenBy(l => l.LinkedDate)
                .ToListAsync();
        }

        public async Task<OwnerClinicLink?> GetPrimaryOwnerByClinicIdAsync(Guid clinicId)
        {
            return await _dbSet
                .Where(l => l.ClinicId == clinicId && l.IsActive && l.IsPrimaryOwner)
                .Include(l => l.Owner)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasAccessToClinicAsync(Guid ownerId, Guid clinicId)
        {
            return await _dbSet
                .AnyAsync(l => l.OwnerId == ownerId && l.ClinicId == clinicId && l.IsActive);
        }

        public async Task<OwnerClinicLink?> GetLinkAsync(Guid ownerId, Guid clinicId)
        {
            return await _dbSet
                .Where(l => l.OwnerId == ownerId && l.ClinicId == clinicId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> LinkExistsAsync(Guid ownerId, Guid clinicId)
        {
            return await _dbSet
                .AnyAsync(l => l.OwnerId == ownerId && l.ClinicId == clinicId);
        }

        public async Task<IEnumerable<OwnerClinicLink>> GetClinicsWithSubscriptionsAsync(Guid ownerId)
        {
            return await _dbSet
                .Where(l => l.OwnerId == ownerId && l.IsActive)
                .Include(l => l.Clinic)
                .OrderByDescending(l => l.IsPrimaryOwner)
                .ThenBy(l => l.LinkedDate)
                .ToListAsync();
        }
    }
}
