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
    public class AccountsPayableRepository : BaseRepository<AccountsPayable>, IAccountsPayableRepository
    {
        public AccountsPayableRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AccountsPayable>> GetBySupplierIdAsync(Guid supplierId, string tenantId)
        {
            return await _dbSet
                .Where(p => p.SupplierId == supplierId && p.TenantId == tenantId)
                .Include(p => p.Payments)
                .OrderByDescending(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountsPayable>> GetByStatusAsync(PayableStatus status, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Status == status && p.TenantId == tenantId)
                .Include(p => p.Payments)
                .OrderBy(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountsPayable>> GetByCategoryAsync(ExpenseCategory category, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Category == category && p.TenantId == tenantId)
                .Include(p => p.Payments)
                .OrderByDescending(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountsPayable>> GetOverdueAsync(string tenantId)
        {
            var today = DateTime.Today;
            return await _dbSet
                .Where(p => p.TenantId == tenantId && 
                           p.DueDate < today && 
                           p.OutstandingAmount > 0 &&
                           p.Status != PayableStatus.Cancelled &&
                           p.Status != PayableStatus.Paid)
                .Include(p => p.Payments)
                .OrderBy(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountsPayable>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(p => p.TenantId == tenantId && 
                           p.DueDate >= startDate && 
                           p.DueDate <= endDate)
                .Include(p => p.Payments)
                .OrderBy(p => p.DueDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalOutstandingAsync(string tenantId)
        {
            return await _dbSet
                .Where(p => p.TenantId == tenantId && 
                           p.Status != PayableStatus.Cancelled &&
                           p.Status != PayableStatus.Paid)
                .SumAsync(p => p.OutstandingAmount);
        }

        public async Task<decimal> GetTotalOverdueAsync(string tenantId)
        {
            var today = DateTime.Today;
            return await _dbSet
                .Where(p => p.TenantId == tenantId && 
                           p.DueDate < today && 
                           p.OutstandingAmount > 0 &&
                           p.Status != PayableStatus.Cancelled &&
                           p.Status != PayableStatus.Paid)
                .SumAsync(p => p.OutstandingAmount);
        }

        public async Task<AccountsPayable?> GetByDocumentNumberAsync(string documentNumber, string tenantId)
        {
            return await _dbSet
                .Where(p => p.DocumentNumber == documentNumber && p.TenantId == tenantId)
                .Include(p => p.Payments)
                .FirstOrDefaultAsync();
        }

        public override async Task<AccountsPayable?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Id == id && p.TenantId == tenantId)
                .Include(p => p.Payments)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync();
        }
    }
}
