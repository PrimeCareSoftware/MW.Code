using MedicSoft.Telemedicine.Domain.Entities;

namespace MedicSoft.Telemedicine.Domain.Interfaces;

/// <summary>
/// Repository for managing telemedicine consent records
/// Required for CFM 2.314/2022 compliance
/// </summary>
public interface ITelemedicineConsentRepository
{
    /// <summary>
    /// Adds a new consent record
    /// </summary>
    Task<TelemedicineConsent> AddAsync(TelemedicineConsent consent);
    
    /// <summary>
    /// Updates an existing consent record
    /// </summary>
    Task UpdateAsync(TelemedicineConsent consent);
    
    /// <summary>
    /// Gets consent by ID
    /// </summary>
    Task<TelemedicineConsent?> GetByIdAsync(Guid id, string tenantId);
    
    /// <summary>
    /// Gets all active consents for a patient
    /// </summary>
    Task<IEnumerable<TelemedicineConsent>> GetByPatientIdAsync(Guid patientId, string tenantId, bool activeOnly = true);
    
    /// <summary>
    /// Gets consent for a specific appointment
    /// </summary>
    Task<TelemedicineConsent?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
    
    /// <summary>
    /// Checks if patient has valid active consent
    /// </summary>
    Task<bool> HasValidConsentAsync(Guid patientId, string tenantId);
    
    /// <summary>
    /// Gets most recent consent for a patient
    /// </summary>
    Task<TelemedicineConsent?> GetMostRecentConsentAsync(Guid patientId, string tenantId);
}
