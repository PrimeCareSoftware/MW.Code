using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class UserClinicLinkRepository : BaseRepository<UserClinicLink>, IUserClinicLinkRepository
    {
        public UserClinicLinkRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserClinicLink>> GetByUserIdAsync(Guid userId, string tenantId)
        {
            return await _dbSet
                .Include(ucl => ucl.Clinic)
                .Where(ucl => ucl.UserId == userId && ucl.TenantId == tenantId)
                .OrderByDescending(ucl => ucl.IsPreferredClinic)
                .ThenBy(ucl => ucl.Clinic!.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserClinicLink>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(ucl => ucl.User)
                .Where(ucl => ucl.ClinicId == clinicId && ucl.TenantId == tenantId)
                .OrderBy(ucl => ucl.User!.FullName)
                .ToListAsync();
        }

        public async Task<UserClinicLink?> GetByUserAndClinicAsync(Guid userId, Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(ucl => ucl.Clinic)
                .FirstOrDefaultAsync(ucl => 
                    ucl.UserId == userId && 
                    ucl.ClinicId == clinicId && 
                    ucl.TenantId == tenantId);
        }

        public async Task<IEnumerable<Clinic>> GetUserClinicsAsync(Guid userId, string tenantId)
        {
            return await _dbSet
                .Include(ucl => ucl.Clinic)
                .Where(ucl => ucl.UserId == userId && ucl.TenantId == tenantId && ucl.IsActive)
                .Select(ucl => ucl.Clinic!)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> UserHasAccessToClinicAsync(Guid userId, Guid clinicId, string tenantId)
        {
            return await _dbSet
                .AnyAsync(ucl => 
                    ucl.UserId == userId && 
                    ucl.ClinicId == clinicId && 
                    ucl.TenantId == tenantId && 
                    ucl.IsActive);
        }
    }
}
