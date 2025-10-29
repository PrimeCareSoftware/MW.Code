using MedicSoft.Telemedicine.Domain.Enums;
using MedicSoft.Telemedicine.Domain.ValueObjects;

namespace MedicSoft.Telemedicine.Domain.Entities;

/// <summary>
/// Represents a telemedicine video consultation session
/// Aggregate root for the telemedicine bounded context
/// </summary>
public class TelemedicineSession
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; }
    public Guid AppointmentId { get; private set; }
    public Guid ClinicId { get; private set; }
    
    /// <summary>
    /// External room ID from video service provider (e.g., Daily.co room name)
    /// </summary>
    public string RoomId { get; private set; }
    
    /// <summary>
    /// URL to access the video room
    /// </summary>
    public string RoomUrl { get; private set; }
    
    public SessionStatus Status { get; private set; }
    public SessionDuration Duration { get; private set; }
    
    public Guid ProviderId { get; private set; }
    public Guid PatientId { get; private set; }
    
    /// <summary>
    /// URL for recording if enabled
    /// </summary>
    public string? RecordingUrl { get; private set; }
    
    /// <summary>
    /// Notes taken during or after the session
    /// </summary>
    public string? SessionNotes { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // EF Core requires parameterless constructor
    private TelemedicineSession() { }
    
    public TelemedicineSession(
        string tenantId,
        Guid appointmentId,
        Guid clinicId,
        Guid providerId,
        Guid patientId,
        string roomId,
        string roomUrl)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required", nameof(tenantId));
            
        if (appointmentId == Guid.Empty)
            throw new ArgumentException("AppointmentId is required", nameof(appointmentId));
            
        if (clinicId == Guid.Empty)
            throw new ArgumentException("ClinicId is required", nameof(clinicId));
            
        if (providerId == Guid.Empty)
            throw new ArgumentException("ProviderId is required", nameof(providerId));
            
        if (patientId == Guid.Empty)
            throw new ArgumentException("PatientId is required", nameof(patientId));
            
        if (string.IsNullOrWhiteSpace(roomId))
            throw new ArgumentException("RoomId is required", nameof(roomId));
            
        if (string.IsNullOrWhiteSpace(roomUrl))
            throw new ArgumentException("RoomUrl is required", nameof(roomUrl));
        
        Id = Guid.NewGuid();
        TenantId = tenantId;
        AppointmentId = appointmentId;
        ClinicId = clinicId;
        ProviderId = providerId;
        PatientId = patientId;
        RoomId = roomId;
        RoomUrl = roomUrl;
        Status = SessionStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void StartSession()
    {
        if (Status != SessionStatus.Scheduled)
            throw new InvalidOperationException($"Cannot start session in {Status} status");
            
        Status = SessionStatus.InProgress;
        Duration = new SessionDuration(DateTime.UtcNow);
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void CompleteSession(string? notes = null)
    {
        if (Status != SessionStatus.InProgress)
            throw new InvalidOperationException($"Cannot complete session in {Status} status");
            
        Status = SessionStatus.Completed;
        Duration?.End();
        SessionNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void CancelSession(string? reason = null)
    {
        if (Status == SessionStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed session");
            
        Status = SessionStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            SessionNotes = $"Cancelled: {reason}";
        }
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void MarkAsFailed(string? errorMessage = null)
    {
        Status = SessionStatus.Failed;
        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            SessionNotes = $"Failed: {errorMessage}";
        }
        Duration?.End();
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetRecordingUrl(string recordingUrl)
    {
        if (string.IsNullOrWhiteSpace(recordingUrl))
            throw new ArgumentException("Recording URL cannot be empty", nameof(recordingUrl));
            
        RecordingUrl = recordingUrl;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AddNotes(string notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
            throw new ArgumentException("Notes cannot be empty", nameof(notes));
            
        SessionNotes = string.IsNullOrWhiteSpace(SessionNotes) 
            ? notes 
            : $"{SessionNotes}\n\n{notes}";
        UpdatedAt = DateTime.UtcNow;
    }
}
