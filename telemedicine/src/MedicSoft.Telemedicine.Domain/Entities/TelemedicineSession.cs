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
    
    /// <summary>
    /// Connection quality during the session (CFM 2.314 requirement)
    /// </summary>
    public ConnectionQuality ConnectionQuality { get; private set; }
    
    /// <summary>
    /// Whether patient has given consent for this telemedicine session
    /// </summary>
    public bool PatientConsented { get; private set; }
    
    /// <summary>
    /// Date when patient gave consent
    /// </summary>
    public DateTime? ConsentDate { get; private set; }
    
    /// <summary>
    /// Reference to the consent record
    /// </summary>
    public Guid? ConsentId { get; private set; }
    
    /// <summary>
    /// IP address from which consent was given
    /// </summary>
    public string? ConsentIpAddress { get; private set; }
    
    /// <summary>
    /// Whether this is the first appointment between this patient and provider
    /// CFM 2.314 requires first appointments to be in-person with exceptions
    /// </summary>
    public bool IsFirstAppointment { get; private set; }
    
    /// <summary>
    /// Justification for telemedicine on first appointment (if applicable)
    /// </summary>
    public string? FirstAppointmentJustification { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // EF Core requires parameterless constructor
    private TelemedicineSession() 
    {
        TenantId = string.Empty;
        RoomId = string.Empty;
        RoomUrl = string.Empty;
        ConnectionQuality = ConnectionQuality.Unknown;
        PatientConsented = false;
        IsFirstAppointment = false;
    }
    
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
        ConnectionQuality = ConnectionQuality.Unknown;
        PatientConsented = false;
        IsFirstAppointment = false;
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
    
    /// <summary>
    /// Records patient consent for this session (CFM 2.314 requirement)
    /// </summary>
    public void RecordConsent(Guid consentId, string ipAddress)
    {
        if (consentId == Guid.Empty)
            throw new ArgumentException("ConsentId is required", nameof(consentId));
            
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address is required", nameof(ipAddress));
            
        if (PatientConsented)
            throw new InvalidOperationException("Consent already recorded for this session");
        
        PatientConsented = true;
        ConsentDate = DateTime.UtcNow;
        ConsentId = consentId;
        ConsentIpAddress = ipAddress;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Sets connection quality for this session (CFM 2.314 requirement)
    /// </summary>
    public void SetConnectionQuality(ConnectionQuality quality)
    {
        ConnectionQuality = quality;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks this as a first appointment and provides justification if needed
    /// </summary>
    public void MarkAsFirstAppointment(string? justification = null)
    {
        IsFirstAppointment = true;
        FirstAppointmentJustification = justification;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Validates if session can start based on CFM 2.314 requirements
    /// </summary>
    public bool CanStart()
    {
        // Must have patient consent
        if (!PatientConsented)
            return false;
            
        // If first appointment without justification, cannot proceed via telemedicine
        if (IsFirstAppointment && string.IsNullOrWhiteSpace(FirstAppointmentJustification))
            return false;
            
        return Status == SessionStatus.Scheduled;
    }
}
