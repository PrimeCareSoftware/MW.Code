using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ProcedureRepository : BaseRepository<Procedure>, IProcedureRepository
    {
        public ProcedureRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<Procedure?> GetByCodeAsync(string code, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Code == code && p.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Procedure>> GetByClinicAsync(string tenantId, bool activeOnly = true)
        {
            var query = _dbSet.Where(p => p.TenantId == tenantId);
            
            if (activeOnly)
            {
                query = query.Where(p => p.IsActive);
            }

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Procedure>> GetByCategoryAsync(ProcedureCategory category, string tenantId, bool activeOnly = true)
        {
            var query = _dbSet.Where(p => p.Category == category && p.TenantId == tenantId);
            
            if (activeOnly)
            {
                query = query.Where(p => p.IsActive);
            }

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> IsCodeUniqueAsync(string code, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(p => p.Code == code && p.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Procedure>> GetByOwnerAsync(Guid ownerId, bool activeOnly = true)
        {
            // Get all clinic IDs owned by this owner through OwnerClinicLink
            var clinicIds = await _context.OwnerClinicLinks
                .Where(ocl => ocl.OwnerId == ownerId && ocl.IsActive)
                .Select(ocl => ocl.ClinicId.ToString())
                .ToListAsync();

            // Get procedures from all owned clinics
            var query = _dbSet.Where(p => clinicIds.Contains(p.TenantId));
            
            if (activeOnly)
            {
                query = query.Where(p => p.IsActive);
            }

            return await query
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
