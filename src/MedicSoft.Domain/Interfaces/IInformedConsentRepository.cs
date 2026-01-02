using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for InformedConsent entity (CFM 1.821 compliance)
    /// </summary>
    public interface IInformedConsentRepository : IRepository<InformedConsent>
    {
        /// <summary>
        /// Gets all informed consents for a specific medical record
        /// </summary>
        Task<IEnumerable<InformedConsent>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
        
        /// <summary>
        /// Gets all informed consents for a specific patient
        /// </summary>
        Task<IEnumerable<InformedConsent>> GetByPatientIdAsync(Guid patientId, string tenantId);
        
        /// <summary>
        /// Gets the active (accepted) consent for a medical record
        /// </summary>
        Task<InformedConsent?> GetActiveConsentByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
    }
}
