namespace MedicSoft.Appointments.Api.Models;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AppointmentCompletedNotificationDto
{
    public Guid AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
    public Guid? NextPatientId { get; set; }
    public string? NextPatientName { get; set; }
}
