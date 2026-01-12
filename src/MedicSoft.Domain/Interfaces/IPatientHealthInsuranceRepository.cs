using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPatientHealthInsuranceRepository : IRepository<PatientHealthInsurance>
    {
        Task<IEnumerable<PatientHealthInsurance>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<PatientHealthInsurance>> GetActiveByPatientIdAsync(Guid patientId, string tenantId);
        Task<PatientHealthInsurance?> GetByCardNumberAsync(string cardNumber, string tenantId);
        Task<IEnumerable<PatientHealthInsurance>> GetByPlanIdAsync(Guid planId, string tenantId);
        Task<bool> IsCardNumberUniqueAsync(string cardNumber, string tenantId, Guid? excludeId = null);
    }
}
