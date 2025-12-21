using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for in-app notifications (different from SMS/WhatsApp/Email notifications)
    /// </summary>
    public class NotificationDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AppointmentCompletedNotificationDto
    {
        public string AppointmentId { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime CompletedAt { get; set; }
        public string? NextPatientId { get; set; }
        public string? NextPatientName { get; set; }
    }
}
