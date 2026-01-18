using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing patient health insurance links
    /// </summary>
    public interface IPatientHealthInsuranceService
    {
        /// <summary>
        /// Links a patient to a health insurance plan
        /// </summary>
        Task<PatientHealthInsuranceDto> CreateAsync(CreatePatientHealthInsuranceDto dto, string tenantId);

        /// <summary>
        /// Updates patient health insurance information
        /// </summary>
        Task<PatientHealthInsuranceDto> UpdateAsync(Guid id, UpdatePatientHealthInsuranceDto dto, string tenantId);

        /// <summary>
        /// Gets all health insurance plans for a patient
        /// </summary>
        Task<IEnumerable<PatientHealthInsuranceDto>> GetByPatientIdAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Gets a specific patient health insurance by ID
        /// </summary>
        Task<PatientHealthInsuranceDto?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Gets patient health insurance by card number
        /// </summary>
        Task<PatientHealthInsuranceDto?> GetByCardNumberAsync(string cardNumber, string tenantId);

        /// <summary>
        /// Gets active health insurance plans for a patient
        /// </summary>
        Task<IEnumerable<PatientHealthInsuranceDto>> GetActiveByPatientIdAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Validates a health insurance card
        /// </summary>
        Task<CardValidationResultDto> ValidateCardAsync(string cardNumber, string tenantId);

        /// <summary>
        /// Activates a patient health insurance
        /// </summary>
        Task<PatientHealthInsuranceDto> ActivateAsync(Guid id, string tenantId);

        /// <summary>
        /// Deactivates a patient health insurance
        /// </summary>
        Task<PatientHealthInsuranceDto> DeactivateAsync(Guid id, string tenantId);

        /// <summary>
        /// Deletes a patient health insurance (soft delete by deactivating)
        /// </summary>
        Task<bool> DeleteAsync(Guid id, string tenantId);
    }
}
