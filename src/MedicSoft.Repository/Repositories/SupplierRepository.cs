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
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Supplier>> GetActiveAsync(string tenantId)
        {
            return await _dbSet
                .Where(s => s.IsActive && s.TenantId == tenantId)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByDocumentNumberAsync(string documentNumber, string tenantId)
        {
            return await _dbSet
                .Where(s => s.DocumentNumber == documentNumber && s.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<Supplier?> GetByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .Where(s => s.Name == name && s.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }
    }
}
