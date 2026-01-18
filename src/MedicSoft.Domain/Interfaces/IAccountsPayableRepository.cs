using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAccountsPayableRepository
    {
        Task<AccountsPayable?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<AccountsPayable>> GetAllAsync(string tenantId);
        Task<IEnumerable<AccountsPayable>> GetBySupplierIdAsync(Guid supplierId, string tenantId);
        Task<IEnumerable<AccountsPayable>> GetByStatusAsync(PayableStatus status, string tenantId);
        Task<IEnumerable<AccountsPayable>> GetByCategoryAsync(ExpenseCategory category, string tenantId);
        Task<IEnumerable<AccountsPayable>> GetOverdueAsync(string tenantId);
        Task<IEnumerable<AccountsPayable>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<decimal> GetTotalOutstandingAsync(string tenantId);
        Task<decimal> GetTotalOverdueAsync(string tenantId);
        Task<AccountsPayable?> GetByDocumentNumberAsync(string documentNumber, string tenantId);
        Task AddAsync(AccountsPayable payable);
        Task UpdateAsync(AccountsPayable payable);
        Task DeleteAsync(Guid id, string tenantId);
    }
}
