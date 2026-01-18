using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for Health Insurance Plans
    /// </summary>
    public class HealthInsurancePlanRepository : BaseRepository<HealthInsurancePlan>, IHealthInsurancePlanRepository
    {
        public HealthInsurancePlanRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<HealthInsurancePlan>> GetByOperatorIdAsync(Guid operatorId, string tenantId)
        {
            return await _context.HealthInsurancePlans
                .Include(p => p.Operator)
                .Where(p => p.TenantId == tenantId && p.OperatorId == operatorId)
                .ToListAsync();
        }

        public async Task<HealthInsurancePlan?> GetByPlanCodeAsync(string planCode, string tenantId)
        {
            return await _context.HealthInsurancePlans
                .Include(p => p.Operator)
                .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.PlanCode == planCode);
        }

        public async Task<IEnumerable<HealthInsurancePlan>> GetActiveAsync(string tenantId)
        {
            return await _context.HealthInsurancePlans
                .Include(p => p.Operator)
                .Where(p => p.TenantId == tenantId && p.IsActive)
                .ToListAsync();
        }
    }
}
