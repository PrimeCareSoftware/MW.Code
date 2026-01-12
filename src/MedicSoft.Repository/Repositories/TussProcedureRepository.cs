using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TussProcedureRepository : BaseRepository<TussProcedure>, ITussProcedureRepository
    {
        public TussProcedureRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<TussProcedure?> GetByCodeAsync(string code, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Code == code && p.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TussProcedure>> SearchByDescriptionAsync(string description, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Description.Contains(description) && p.TenantId == tenantId)
                .OrderBy(p => p.Description)
                .ToListAsync();
        }

        public async Task<IEnumerable<TussProcedure>> GetByCategoryAsync(string category, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Category == category && p.TenantId == tenantId)
                .OrderBy(p => p.Description)
                .ToListAsync();
        }

        public async Task<IEnumerable<TussProcedure>> GetActiveAsync(string tenantId)
        {
            return await _dbSet
                .Where(p => p.IsActive && p.TenantId == tenantId)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Description)
                .ToListAsync();
        }

        public async Task<IEnumerable<TussProcedure>> GetRequiringAuthorizationAsync(string tenantId)
        {
            return await _dbSet
                .Where(p => p.RequiresAuthorization && p.IsActive && p.TenantId == tenantId)
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Description)
                .ToListAsync();
        }
    }
}
