using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ExpenseRepository : BaseRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Expense>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(e => e.ClinicId == clinicId && e.TenantId == tenantId)
                .OrderByDescending(e => e.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetByStatusAsync(ExpenseStatus status, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Status == status && e.TenantId == tenantId)
                .OrderByDescending(e => e.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Category == category && e.TenantId == tenantId)
                .OrderByDescending(e => e.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Expense>> GetOverdueExpensesAsync(string tenantId)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId && e.Status == ExpenseStatus.Pending && e.DueDate < DateTime.UtcNow)
                .OrderBy(e => e.DueDate)
                .ToListAsync();
        }
    }
}
