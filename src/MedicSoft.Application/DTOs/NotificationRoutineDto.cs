using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for creating a new notification routine
    /// </summary>
    public class CreateNotificationRoutineDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty; // SMS, WhatsApp, Email, Push
        public string Type { get; set; } = string.Empty; // AppointmentReminder, etc.
        public string MessageTemplate { get; set; } = string.Empty;
        public string ScheduleType { get; set; } = string.Empty; // Daily, Weekly, etc.
        public string ScheduleConfiguration { get; set; } = string.Empty; // JSON config
        public string Scope { get; set; } = string.Empty; // Clinic or System
        public int MaxRetries { get; set; } = 3;
        public string? RecipientFilter { get; set; } // JSON filter
    }

    /// <summary>
    /// DTO for updating an existing notification routine
    /// </summary>
    public class UpdateNotificationRoutineDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string MessageTemplate { get; set; } = string.Empty;
        public string ScheduleType { get; set; } = string.Empty;
        public string ScheduleConfiguration { get; set; } = string.Empty;
        public int MaxRetries { get; set; } = 3;
        public string? RecipientFilter { get; set; }
    }

    /// <summary>
    /// DTO for notification routine response
    /// </summary>
    public class NotificationRoutineDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string MessageTemplate { get; set; } = string.Empty;
        public string ScheduleType { get; set; } = string.Empty;
        public string ScheduleConfiguration { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int MaxRetries { get; set; }
        public string? RecipientFilter { get; set; }
        public DateTime? LastExecutedAt { get; set; }
        public DateTime? NextExecutionAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string TenantId { get; set; } = string.Empty;
    }
}
