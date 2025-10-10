using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a configurable notification routine for automated sending of SMS, Email, or WhatsApp messages.
    /// Can be configured at clinic level or system level (for admins).
    /// </summary>
    public class NotificationRoutine : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public NotificationChannel Channel { get; private set; }
        public NotificationType Type { get; private set; }
        public string MessageTemplate { get; private set; }
        public RoutineScheduleType ScheduleType { get; private set; }
        public string ScheduleConfiguration { get; private set; } // JSON configuration for schedule
        public RoutineScope Scope { get; private set; }
        public bool IsActive { get; private set; }
        public int MaxRetries { get; private set; }
        public string? RecipientFilter { get; private set; } // JSON filter criteria
        public DateTime? LastExecutedAt { get; private set; }
        public DateTime? NextExecutionAt { get; private set; }

        private NotificationRoutine()
        {
            // EF Constructor
            Name = null!;
            Description = null!;
            MessageTemplate = null!;
            ScheduleConfiguration = null!;
        }

        public NotificationRoutine(
            string name,
            string description,
            NotificationChannel channel,
            NotificationType type,
            string messageTemplate,
            RoutineScheduleType scheduleType,
            string scheduleConfiguration,
            RoutineScope scope,
            string tenantId,
            int maxRetries = 3,
            string? recipientFilter = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(messageTemplate))
                throw new ArgumentException("Message template cannot be empty", nameof(messageTemplate));

            if (string.IsNullOrWhiteSpace(scheduleConfiguration))
                throw new ArgumentException("Schedule configuration cannot be empty", nameof(scheduleConfiguration));

            if (maxRetries < 0 || maxRetries > 10)
                throw new ArgumentException("Max retries must be between 0 and 10", nameof(maxRetries));

            // For system-wide routines, require admin scope
            if (scope == RoutineScope.System && tenantId != "system-admin")
            {
                // Validation can be enhanced based on user role
            }

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Channel = channel;
            Type = type;
            MessageTemplate = messageTemplate.Trim();
            ScheduleType = scheduleType;
            ScheduleConfiguration = scheduleConfiguration.Trim();
            Scope = scope;
            MaxRetries = maxRetries;
            RecipientFilter = recipientFilter?.Trim();
            IsActive = true;
        }

        public void Update(
            string name,
            string description,
            NotificationChannel channel,
            NotificationType type,
            string messageTemplate,
            RoutineScheduleType scheduleType,
            string scheduleConfiguration,
            int maxRetries,
            string? recipientFilter = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(messageTemplate))
                throw new ArgumentException("Message template cannot be empty", nameof(messageTemplate));

            if (string.IsNullOrWhiteSpace(scheduleConfiguration))
                throw new ArgumentException("Schedule configuration cannot be empty", nameof(scheduleConfiguration));

            if (maxRetries < 0 || maxRetries > 10)
                throw new ArgumentException("Max retries must be between 0 and 10", nameof(maxRetries));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Channel = channel;
            Type = type;
            MessageTemplate = messageTemplate.Trim();
            ScheduleType = scheduleType;
            ScheduleConfiguration = scheduleConfiguration.Trim();
            MaxRetries = maxRetries;
            RecipientFilter = recipientFilter?.Trim();
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public void MarkAsExecuted(DateTime? nextExecution = null)
        {
            LastExecutedAt = DateTime.UtcNow;
            NextExecutionAt = nextExecution;
            UpdateTimestamp();
        }

        public void SetNextExecution(DateTime nextExecution)
        {
            if (nextExecution <= DateTime.UtcNow)
                throw new ArgumentException("Next execution must be in the future", nameof(nextExecution));

            NextExecutionAt = nextExecution;
            UpdateTimestamp();
        }

        public bool ShouldExecute()
        {
            return IsActive && 
                   (NextExecutionAt == null || NextExecutionAt <= DateTime.UtcNow);
        }
    }

    /// <summary>
    /// Defines the schedule type for routine execution
    /// </summary>
    public enum RoutineScheduleType
    {
        Daily,          // Execute daily at specific time
        Weekly,         // Execute on specific days of the week
        Monthly,        // Execute on specific day of the month
        Custom,         // Custom cron-like expression
        BeforeAppointment, // Execute X hours/days before appointment
        AfterAppointment   // Execute X hours/days after appointment
    }

    /// <summary>
    /// Defines the scope of the notification routine
    /// </summary>
    public enum RoutineScope
    {
        Clinic,  // Applies to a specific clinic (tenant)
        System   // System-wide, managed by admin
    }
}
