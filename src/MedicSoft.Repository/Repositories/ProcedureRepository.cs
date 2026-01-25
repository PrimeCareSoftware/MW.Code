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
            // Query procedures directly by joining with OwnerClinicLink for better performance
            var query = from procedure in _dbSet
                        join link in _context.OwnerClinicLinks
                            on procedure.TenantId equals link.ClinicId.ToString()
                        where link.OwnerId == ownerId && link.IsActive
                        select procedure;
            
            if (activeOnly)
            {
                query = query.Where(p => p.IsActive);
            }

            return await query
                .Distinct() // In case a procedure is returned multiple times
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
