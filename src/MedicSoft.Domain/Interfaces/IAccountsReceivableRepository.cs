using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAccountsReceivableRepository
    {
        Task<AccountsReceivable?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<AccountsReceivable>> GetAllAsync(string tenantId);
        Task<IEnumerable<AccountsReceivable>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<AccountsReceivable>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<AccountsReceivable>> GetByStatusAsync(ReceivableStatus status, string tenantId);
        Task<IEnumerable<AccountsReceivable>> GetOverdueAsync(string tenantId);
        Task<IEnumerable<AccountsReceivable>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<decimal> GetTotalOutstandingAsync(string tenantId);
        Task<decimal> GetTotalOverdueAsync(string tenantId);
        Task<AccountsReceivable?> GetByDocumentNumberAsync(string documentNumber, string tenantId);
        Task AddAsync(AccountsReceivable receivable);
        Task UpdateAsync(AccountsReceivable receivable);
        Task DeleteAsync(Guid id, string tenantId);
    }
}
