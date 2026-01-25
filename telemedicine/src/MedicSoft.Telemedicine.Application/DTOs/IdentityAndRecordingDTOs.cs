namespace MedicSoft.Telemedicine.Application.DTOs;

/// <summary>
/// Request to create identity verification
/// </summary>
public class CreateIdentityVerificationRequest
{
    public Guid UserId { get; set; }
    public string UserType { get; set; } = string.Empty; // "Provider" or "Patient"
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string? CrmNumber { get; set; }
    public string? CrmState { get; set; }
    public Guid? TelemedicineSessionId { get; set; }
    
    // File uploads will be handled separately via multipart/form-data
    // This DTO is for JSON metadata
}

/// <summary>
/// Response with identity verification details
/// </summary>
public class IdentityVerificationResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string? CrmNumber { get; set; }
    public string? CrmState { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime VerifiedAt { get; set; }
    public Guid? VerifiedByUserId { get; set; }
    public string? VerificationNotes { get; set; }
    public DateTime ValidUntil { get; set; }
    public bool IsValid { get; set; }
    public bool HasExpired { get; set; }
}

/// <summary>
/// Request to approve or reject identity verification
/// </summary>
public class VerifyIdentityRequest
{
    public bool Approved { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request to create recording
/// </summary>
public class CreateRecordingRequest
{
    public Guid SessionId { get; set; }
    public Guid ConsentId { get; set; }
    public string RecordingPath { get; set; } = string.Empty;
    public string FileFormat { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; } = true;
    public string? EncryptionKeyId { get; set; }
}

/// <summary>
/// Response with recording details
/// </summary>
public class RecordingResponse
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime RecordingStartedAt { get; set; }
    public DateTime? RecordingCompletedAt { get; set; }
    public int DurationSeconds { get; set; }
    public long FileSizeBytes { get; set; }
    public string FormattedFileSize { get; set; } = string.Empty;
    public string FormattedDuration { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; }
    public DateTime RetentionUntil { get; set; }
    public bool IsDeleted { get; set; }
}

/// <summary>
/// Request to complete recording
/// </summary>
public class CompleteRecordingRequest
{
    public long FileSizeBytes { get; set; }
    public int DurationSeconds { get; set; }
}

/// <summary>
/// Request to delete recording
/// </summary>
public class DeleteRecordingRequest
{
    public string Reason { get; set; } = string.Empty;
}
