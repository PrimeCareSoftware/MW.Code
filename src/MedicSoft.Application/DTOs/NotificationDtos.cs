using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "AppointmentId is required")]
        public string AppointmentId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "DoctorName is required")]
        [StringLength(200, ErrorMessage = "DoctorName cannot exceed 200 characters")]
        public string DoctorName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "PatientName is required")]
        [StringLength(200, ErrorMessage = "PatientName cannot exceed 200 characters")]
        public string PatientName { get; set; } = string.Empty;
        
        public DateTime CompletedAt { get; set; }
        public string? NextPatientId { get; set; }
        
        [StringLength(200, ErrorMessage = "NextPatientName cannot exceed 200 characters")]
        public string? NextPatientName { get; set; }
    }
}
