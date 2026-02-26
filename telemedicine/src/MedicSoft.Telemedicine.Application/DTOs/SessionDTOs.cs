namespace MedicSoft.Telemedicine.Application.DTOs;

public class CreateSessionRequest
{
    public Guid AppointmentId { get; set; }
    public Guid ClinicId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid PatientId { get; set; }
}

public class SessionResponse
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid ClinicId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid PatientId { get; set; }
    public string RoomUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int? DurationMinutes { get; set; }
    public string? RecordingUrl { get; set; }
}

public class JoinSessionRequest
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // "provider" or "patient"
}

public class JoinSessionResponse
{
    public string Provider { get; set; } = "Twilio";
    public string RoomName { get; set; } = string.Empty;
    public string RoomUrl { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    // Frontend feature flags
    public bool WaitingRoomEnabled { get; set; } = true;
    public bool RecordingAvailable { get; set; } = true;
    public bool ScreenSharingAvailable { get; set; } = true;
    public bool ChatAvailable { get; set; } = true;
}

public class CompleteSessionRequest
{
    public Guid SessionId { get; set; }
    public string? Notes { get; set; }
}
