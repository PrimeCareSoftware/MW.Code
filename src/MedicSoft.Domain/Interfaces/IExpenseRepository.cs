using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<Expense>> GetByStatusAsync(ExpenseStatus status, string tenantId);
        Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category, string tenantId);
        Task<IEnumerable<Expense>> GetOverdueExpensesAsync(string tenantId);
    }
}
