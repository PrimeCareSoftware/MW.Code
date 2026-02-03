namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Service interface for checking clinic settings and configurations
/// </summary>
public interface IClinicSettingsService
{
    /// <summary>
    /// Checks if online appointment scheduling is enabled for a clinic
    /// </summary>
    /// <param name="clinicId">Clinic ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>True if online scheduling is enabled</returns>
    Task<bool> IsOnlineSchedulingEnabledAsync(Guid clinicId, string tenantId);
}
