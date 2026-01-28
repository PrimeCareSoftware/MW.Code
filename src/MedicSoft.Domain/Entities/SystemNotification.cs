using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// System-level notifications for administrators (distinct from patient notifications)
    /// </summary>
    public class SystemNotification : BaseEntity
    {
        public string Type { get; set; } = string.Empty; // critical, warning, info, success
        public string Category { get; set; } = string.Empty; // subscription, customer, system, ticket
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public string? ActionLabel { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? Data { get; set; } // JSON with additional data

        public SystemNotification() : base(string.Empty)
        {
            // Default constructor for EF
        }

        public SystemNotification(string type, string category, string title, string message) : base(string.Empty)
        {
            Type = type;
            Category = category;
            Title = title;
            Message = message;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Rules for automatic notification creation
    /// </summary>
    public class NotificationRule : BaseEntity
    {
        public string Trigger { get; set; } = string.Empty; // subscription_expired, trial_expiring, etc
        public bool IsEnabled { get; set; }
        public string? Conditions { get; set; } // JSON
        public string? Actions { get; set; } // JSON: send notif, email, sms

        public NotificationRule() : base(string.Empty)
        {
            IsEnabled = true;
        }
    }
}
