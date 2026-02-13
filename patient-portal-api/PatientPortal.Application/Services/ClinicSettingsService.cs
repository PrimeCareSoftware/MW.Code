using Microsoft.Extensions.Logging;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Interfaces;

namespace PatientPortal.Application.Services;

/// <summary>
/// Service for checking clinic settings and configurations
/// </summary>
public class ClinicSettingsService : IClinicSettingsService
{
    private readonly IMainDatabaseContext _mainDatabase;
    private readonly ILogger<ClinicSettingsService> _logger;

    public ClinicSettingsService(
        IMainDatabaseContext mainDatabase,
        ILogger<ClinicSettingsService> logger)
    {
        _mainDatabase = mainDatabase;
        _logger = logger;
    }

    /// <summary>
    /// Checks if online appointment scheduling is enabled for a clinic
    /// </summary>
    /// <param name="clinicId">Clinic ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>True if online scheduling is enabled</returns>
    public async Task<bool> IsOnlineSchedulingEnabledAsync(Guid clinicId, string tenantId)
    {
        try
        {
            var query = @"
                SELECT ""EnableOnlineAppointmentScheduling""
                FROM ""Clinics""
                WHERE ""Id"" = {0} AND ""TenantId"" = {1}";

            var result = await _mainDatabase.ExecuteQueryAsync<ClinicSchedulingSetting>(
                query,
                clinicId,
                tenantId
            );

            var setting = result.FirstOrDefault();
            return setting?.EnableOnlineAppointmentScheduling ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking online scheduling setting for clinic {ClinicId}", clinicId);
            return false; // Default to disabled on error for security
        }
    }

    /// <summary>
    /// Gets the schedule settings for a clinic (opening/closing times and appointment duration)
    /// </summary>
    /// <param name="clinicId">Clinic ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>Clinic schedule settings</returns>
    public async Task<ClinicScheduleSettings?> GetScheduleSettingsAsync(Guid clinicId, string tenantId)
    {
        try
        {
            var query = @"
                SELECT ""OpeningTime"", ""ClosingTime"", ""AppointmentDurationMinutes""
                FROM ""Clinics""
                WHERE ""Id"" = {0} AND ""TenantId"" = {1}";

            var result = await _mainDatabase.ExecuteQueryAsync<ClinicScheduleSettingsDto>(
                query,
                clinicId,
                tenantId
            );

            var settings = result.FirstOrDefault();
            if (settings == null)
            {
                _logger.LogWarning("Schedule settings not found for clinic {ClinicId}", clinicId);
                return null;
            }

            return new ClinicScheduleSettings
            {
                OpeningTime = settings.OpeningTime,
                ClosingTime = settings.ClosingTime,
                AppointmentDurationMinutes = settings.AppointmentDurationMinutes
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching schedule settings for clinic {ClinicId}", clinicId);
            return null;
        }
    }

    // Helper class for query result
    private class ClinicSchedulingSetting
    {
        public bool EnableOnlineAppointmentScheduling { get; set; }
    }

    // Helper class for schedule settings query result
    private class ClinicScheduleSettingsDto
    {
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public int AppointmentDurationMinutes { get; set; }
    }
}
