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
    }
}
