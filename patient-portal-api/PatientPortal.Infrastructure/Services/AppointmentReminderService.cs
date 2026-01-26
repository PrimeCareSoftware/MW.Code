using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatientPortal.Application.Configuration;
using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Application.Interfaces;
using PatientPortal.Application.Services;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Infrastructure.Services;

/// <summary>
/// Background service that periodically checks for upcoming appointments and sends reminders
/// </summary>
public class AppointmentReminderService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AppointmentReminderService> _logger;
    private readonly AppointmentReminderSettings _settings;
    private readonly PortalSettings _portalSettings;

    public AppointmentReminderService(
        IServiceProvider serviceProvider,
        ILogger<AppointmentReminderService> logger,
        IOptions<AppointmentReminderSettings> settings,
        IOptions<PortalSettings> portalSettings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = settings.Value;
        _portalSettings = portalSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.Enabled)
        {
            _logger.LogInformation("Appointment Reminder Service is disabled");
            return;
        }

        _logger.LogInformation("Appointment Reminder Service started. Check interval: {Interval} minutes, Advance notice: {Notice} hours",
            _settings.CheckIntervalMinutes, _settings.AdvanceNoticeHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SendRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending appointment reminders");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(_settings.CheckIntervalMinutes), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Appointment Reminder Service is stopping");
                break;
            }
        }
    }

    private async Task SendRemindersAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking for appointments needing reminders...");

        using var scope = _serviceProvider.CreateScope();
        var mainDatabase = scope.ServiceProvider.GetRequiredService<IMainDatabaseContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            // Calculate the target date/time range for reminders
            var now = DateTime.UtcNow;
            var targetStartTime = now.AddHours(_settings.AdvanceNoticeHours - 1); // 1 hour buffer before
            var targetEndTime = now.AddHours(_settings.AdvanceNoticeHours + 1);   // 1 hour buffer after

            _logger.LogDebug("Looking for appointments between {StartTime} and {EndTime}",
                targetStartTime, targetEndTime);

            // Query appointments that need reminders
            var query = @"
                SELECT 
                    a.""Id"" as AppointmentId,
                    a.""PatientId"",
                    p.""Name"" as PatientName,
                    p.""Email"" as PatientEmail,
                    p.""Phone"" as PatientPhone,
                    a.""ProfessionalId"" as DoctorId,
                    prof.""Name"" as DoctorName,
                    s.""Name"" as DoctorSpecialty,
                    c.""Name"" as ClinicName,
                    a.""ScheduledDate"" as AppointmentDate,
                    a.""ScheduledTime"" as AppointmentTime,
                    a.""Type"" as AppointmentType,
                    a.""Mode"" as AppointmentMode,
                    COALESCE(a.""ReminderSent"", false) as ReminderSent
                FROM ""Appointments"" a
                INNER JOIN ""Patients"" p ON a.""PatientId"" = p.""Id""
                INNER JOIN ""Professionals"" prof ON a.""ProfessionalId"" = prof.""Id""
                INNER JOIN ""Clinics"" c ON a.""ClinicId"" = c.""Id""
                LEFT JOIN ""Specialties"" s ON prof.""SpecialtyId"" = s.""Id""
                WHERE 
                    a.""Status"" IN ({0}, {1})
                    AND (a.""ScheduledDate"" + a.""ScheduledTime"") >= {2}
                    AND (a.""ScheduledDate"" + a.""ScheduledTime"") <= {3}
                    AND COALESCE(a.""ReminderSent"", false) = false
                    AND p.""Email"" IS NOT NULL
                    AND p.""Email"" != ''
                ORDER BY a.""ScheduledDate"", a.""ScheduledTime""";

            var appointments = await mainDatabase.ExecuteQueryAsync<AppointmentReminderQueryResult>(
                query,
                (int)AppointmentStatus.Scheduled,
                (int)AppointmentStatus.Confirmed,
                targetStartTime,
                targetEndTime
            );

            _logger.LogInformation("Found {Count} appointments needing reminders", appointments.Count);

            foreach (var appointment in appointments)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await SendReminderForAppointmentAsync(appointment, mainDatabase, notificationService, cancellationToken);
            }

            _logger.LogInformation("Finished sending reminders for {Count} appointments", appointments.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SendRemindersAsync");
            throw;
        }
    }

    private async Task SendReminderForAppointmentAsync(
        AppointmentReminderQueryResult appointment,
        IMainDatabaseContext mainDatabase,
        INotificationService notificationService,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Sending reminder for appointment {AppointmentId} to patient {PatientName} ({Email})",
                appointment.AppointmentId, appointment.PatientName, appointment.PatientEmail);

            // Map to DTO
            var reminderDto = new AppointmentReminderDto
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                PatientName = appointment.PatientName,
                PatientEmail = appointment.PatientEmail,
                PatientPhone = appointment.PatientPhone,
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.DoctorName,
                DoctorSpecialty = appointment.DoctorSpecialty ?? "Médico",
                ClinicName = appointment.ClinicName,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                AppointmentType = appointment.AppointmentType ?? "Consulta",
                IsTelehealth = appointment.AppointmentMode?.ToLower() == "telemedicine",
                TelehealthLink = null
            };

            // Generate email content
            var emailBody = EmailTemplateHelper.GenerateAppointmentReminderEmail(reminderDto, _portalSettings.BaseUrl);
            var subject = $"Lembrete: Consulta Médica Amanhã - {appointment.DoctorName}";

            // Send email
            await notificationService.SendEmailAsync(
                reminderDto.PatientEmail,
                subject,
                emailBody,
                cancellationToken
            );

            // Send SMS/WhatsApp if phone number is available
            if (!string.IsNullOrEmpty(reminderDto.PatientPhone))
            {
                var textMessage = EmailTemplateHelper.GenerateAppointmentReminderText(reminderDto, _portalSettings.BaseUrl);
                
                // Try WhatsApp first, then SMS as fallback
                try
                {
                    await notificationService.SendWhatsAppAsync(reminderDto.PatientPhone, textMessage, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "WhatsApp failed for {Phone}, trying SMS", reminderDto.PatientPhone);
                    await notificationService.SendSmsAsync(reminderDto.PatientPhone, textMessage, cancellationToken);
                }
            }

            // Mark reminder as sent
            await MarkReminderAsSentAsync(appointment.AppointmentId, mainDatabase);

            _logger.LogInformation("Successfully sent reminder for appointment {AppointmentId}", appointment.AppointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending reminder for appointment {AppointmentId}", appointment.AppointmentId);
            // Don't rethrow - continue with other appointments
        }
    }

    private async Task MarkReminderAsSentAsync(Guid appointmentId, IMainDatabaseContext mainDatabase)
    {
        try
        {
            var updateQuery = @"
                UPDATE ""Appointments""
                SET ""ReminderSent"" = true,
                    ""ReminderSentAt"" = {0},
                    ""UpdatedAt"" = {0}
                WHERE ""Id"" = {1}";

            await mainDatabase.ExecuteCommandAsync(updateQuery, DateTime.UtcNow, appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking reminder as sent for appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    // Helper class for query results
    private class AppointmentReminderQueryResult
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string? PatientPhone { get; set; }
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string? DoctorSpecialty { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string? AppointmentType { get; set; }
        public string? AppointmentMode { get; set; }
        public bool ReminderSent { get; set; }
    }
}
