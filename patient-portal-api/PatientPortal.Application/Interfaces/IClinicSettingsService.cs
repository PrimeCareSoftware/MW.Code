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

    /// <summary>
    /// Gets the schedule settings for a clinic (opening/closing times and appointment duration)
    /// </summary>
    /// <param name="clinicId">Clinic ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Clinic schedule settings</returns>
    Task<ClinicScheduleSettings?> GetScheduleSettingsAsync(Guid clinicId, string tenantId);
}

/// <summary>
/// Represents clinic schedule settings
/// </summary>
public class ClinicScheduleSettings
{
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }
    public int AppointmentDurationMinutes { get; set; }
}
