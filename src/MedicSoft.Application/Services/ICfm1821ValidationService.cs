using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for validating CFM 1.821 compliance in medical records
    /// </summary>
    public interface ICfm1821ValidationService
    {
        /// <summary>
        /// Validates if a medical record meets all CFM 1.821 requirements
        /// </summary>
        /// <param name="medicalRecordId">The medical record ID</param>
        /// <param name="tenantId">The tenant ID</param>
        /// <returns>Validation result with detailed information</returns>
        Task<Cfm1821ValidationResult> ValidateMedicalRecordCompleteness(Guid medicalRecordId, string tenantId);
        
        /// <summary>
        /// Checks if a medical record is ready to be closed according to CFM 1.821
        /// </summary>
        /// <param name="medicalRecordId">The medical record ID</param>
        /// <param name="tenantId">The tenant ID</param>
        /// <returns>True if the medical record is ready for closure</returns>
        Task<bool> IsMedicalRecordReadyForClosure(Guid medicalRecordId, string tenantId);
    }
}
