using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ICashFlowEntryRepository
    {
        Task<CashFlowEntry?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<CashFlowEntry>> GetAllAsync(string tenantId);
        Task<IEnumerable<CashFlowEntry>> GetByTypeAsync(CashFlowType type, string tenantId);
        Task<IEnumerable<CashFlowEntry>> GetByCategoryAsync(CashFlowCategory category, string tenantId);
        Task<IEnumerable<CashFlowEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<decimal> GetTotalIncomeAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<decimal> GetTotalExpenseAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<decimal> GetBalanceAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<IEnumerable<CashFlowEntry>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<CashFlowEntry> AddAsync(CashFlowEntry entry);
        Task UpdateAsync(CashFlowEntry entry);
        Task DeleteAsync(Guid id, string tenantId);
    }
}
