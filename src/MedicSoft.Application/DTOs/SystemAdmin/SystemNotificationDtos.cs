using System;

namespace MedicSoft.Application.DTOs.SystemAdmin
{
    public class SystemNotificationDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // critical, warning, info, success
        public string Category { get; set; } = string.Empty; // subscription, customer, system, ticket
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public string? ActionLabel { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? Data { get; set; }
    }

    public class CreateSystemNotificationDto
    {
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public string? ActionLabel { get; set; }
        public string? Data { get; set; }
    }

    public class NotificationRuleDto
    {
        public int Id { get; set; }
        public string Trigger { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public string? Conditions { get; set; }
        public string? Actions { get; set; }
    }
}
