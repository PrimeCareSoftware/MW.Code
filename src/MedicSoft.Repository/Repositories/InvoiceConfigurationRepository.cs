using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class InvoiceConfigurationRepository : BaseRepository<InvoiceConfiguration>, IInvoiceConfigurationRepository
    {
        public InvoiceConfigurationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<InvoiceConfiguration?> GetByTenantIdAsync(string tenantId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.TenantId == tenantId);
        }

        public async Task<InvoiceConfiguration?> GetByCnpjAsync(string cnpj)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        }

        public async Task<bool> ExistsForTenantAsync(string tenantId)
        {
            return await _dbSet
                .AnyAsync(c => c.TenantId == tenantId);
        }

        public async Task<int> GetNextInvoiceNumberAsync(string tenantId)
        {
            var config = await GetByTenantIdAsync(tenantId);
            if (config == null)
                throw new InvalidOperationException("Invoice configuration not found");

            var nextNumber = config.GetNextInvoiceNumber();
            await UpdateAsync(config);
            
            return nextNumber;
        }

        public async Task<int> GetNextRpsNumberAsync(string tenantId)
        {
            var config = await GetByTenantIdAsync(tenantId);
            if (config == null)
                throw new InvalidOperationException("Invoice configuration not found");

            var nextNumber = config.GetNextRpsNumber();
            await UpdateAsync(config);
            
            return nextNumber;
        }
    }
}
