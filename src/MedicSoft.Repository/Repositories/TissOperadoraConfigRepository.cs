using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TissOperadoraConfigRepository : BaseRepository<TissOperadoraConfig>, ITissOperadoraConfigRepository
    {
        public TissOperadoraConfigRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<TissOperadoraConfig?> GetByOperatorIdAsync(Guid operatorId, string tenantId)
        {
            return await _dbSet
                .Include(c => c.Operator)
                .Where(c => c.OperatorId == operatorId && c.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TissOperadoraConfig>> GetActiveConfigsAsync(string tenantId)
        {
            return await _dbSet
                .Include(c => c.Operator)
                .Where(c => c.IsActive && c.TenantId == tenantId)
                .ToListAsync();
        }
    }
}
