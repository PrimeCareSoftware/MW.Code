namespace PatientPortal.Application.Configuration;

/// <summary>
/// Configuration settings for appointment reminder service
/// </summary>
public class AppointmentReminderSettings
{
    public const string SectionName = "AppointmentReminder";

    /// <summary>
    /// Whether the reminder service is enabled
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Interval in minutes to check for appointments needing reminders
    /// </summary>
    public int CheckIntervalMinutes { get; set; } = 60;

    /// <summary>
    /// How many hours in advance to send reminders
    /// </summary>
    public int AdvanceNoticeHours { get; set; } = 24;
}
