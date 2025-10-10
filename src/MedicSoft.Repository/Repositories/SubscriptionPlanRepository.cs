using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class SubscriptionPlanRepository : BaseRepository<SubscriptionPlan>, ISubscriptionPlanRepository
    {
        public SubscriptionPlanRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<List<SubscriptionPlan>> GetActiveInPlansAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .OrderBy(p => p.MonthlyPrice)
                .ToListAsync();
        }

        public async Task<SubscriptionPlan?> GetByTypeAsync(PlanType type, string tenantId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Type == type && p.TenantId == tenantId && p.IsActive);
        }

        public async Task<IEnumerable<SubscriptionPlan>> GetAllActiveAsync(string tenantId)
        {
            return await _dbSet
                .Where(p => p.IsActive && p.TenantId == tenantId)
                .OrderBy(p => p.MonthlyPrice)
                .ToListAsync();
        }
    }
}
