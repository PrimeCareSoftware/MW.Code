using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class HealthInsuranceOperatorRepository : BaseRepository<HealthInsuranceOperator>, IHealthInsuranceOperatorRepository
    {
        public HealthInsuranceOperatorRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<HealthInsuranceOperator?> GetByRegisterNumberAsync(string registerNumber, string tenantId)
        {
            return await _dbSet
                .Where(o => o.RegisterNumber == registerNumber && o.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<HealthInsuranceOperator?> GetByDocumentAsync(string document, string tenantId)
        {
            return await _dbSet
                .Where(o => o.Document == document && o.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<HealthInsuranceOperator>> SearchByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .Where(o => (o.TradeName.Contains(name) || o.CompanyName.Contains(name)) && o.TenantId == tenantId)
                .OrderBy(o => o.TradeName)
                .ToListAsync();
        }

        public async Task<IEnumerable<HealthInsuranceOperator>> GetActiveOperatorsAsync(string tenantId)
        {
            return await _dbSet
                .Where(o => o.IsActive && o.TenantId == tenantId)
                .OrderBy(o => o.TradeName)
                .ToListAsync();
        }

        public async Task<bool> IsRegisterNumberUniqueAsync(string registerNumber, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(o => o.RegisterNumber == registerNumber && o.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(o => o.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}
