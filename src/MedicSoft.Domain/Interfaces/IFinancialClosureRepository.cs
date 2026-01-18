using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IFinancialClosureRepository
    {
        Task<FinancialClosure?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<FinancialClosure>> GetAllAsync(string tenantId);
        Task<FinancialClosure?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<FinancialClosure>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<FinancialClosure>> GetByStatusAsync(FinancialClosureStatus status, string tenantId);
        Task<IEnumerable<FinancialClosure>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<FinancialClosure?> GetByClosureNumberAsync(string closureNumber, string tenantId);
        Task<decimal> GetTotalOutstandingAsync(string tenantId);
        Task<FinancialClosure> AddAsync(FinancialClosure closure);
        Task UpdateAsync(FinancialClosure closure);
        Task DeleteAsync(Guid id, string tenantId);
    }
}
