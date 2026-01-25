namespace MedicSoft.Telemedicine.Domain.Entities;

/// <summary>
/// Represents a recording of a telemedicine session
/// Optional feature per CFM 2.314/2022 with patient consent
/// Must be retained for 20+ years
/// </summary>
public class TelemedicineRecording
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; }
    public Guid SessionId { get; private set; }
    
    /// <summary>
    /// Path to recording file in secure storage
    /// </summary>
    public string RecordingPath { get; private set; }
    
    /// <summary>
    /// File format (mp4, webm, etc.)
    /// </summary>
    public string FileFormat { get; private set; }
    
    /// <summary>
    /// File size in bytes
    /// </summary>
    public long FileSizeBytes { get; private set; }
    
    /// <summary>
    /// Duration of recording in seconds
    /// </summary>
    public int DurationSeconds { get; private set; }
    
    /// <summary>
    /// When recording started
    /// </summary>
    public DateTime RecordingStartedAt { get; private set; }
    
    /// <summary>
    /// When recording completed
    /// </summary>
    public DateTime? RecordingCompletedAt { get; private set; }
    
    /// <summary>
    /// Status of the recording
    /// </summary>
    public RecordingStatus Status { get; private set; }
    
    /// <summary>
    /// Whether the recording is encrypted
    /// </summary>
    public bool IsEncrypted { get; private set; }
    
    /// <summary>
    /// Encryption key identifier (not the key itself)
    /// </summary>
    public string? EncryptionKeyId { get; private set; }
    
    /// <summary>
    /// Consent ID that authorized this recording
    /// </summary>
    public Guid ConsentId { get; private set; }
    
    /// <summary>
    /// Date when recording must be deleted (CFM requires 20+ years retention)
    /// </summary>
    public DateTime RetentionUntil { get; private set; }
    
    /// <summary>
    /// Whether recording has been deleted
    /// </summary>
    public bool IsDeleted { get; private set; }
    
    /// <summary>
    /// When recording was deleted
    /// </summary>
    public DateTime? DeletedAt { get; private set; }
    
    /// <summary>
    /// Reason for deletion
    /// </summary>
    public string? DeletionReason { get; private set; }
    
    /// <summary>
    /// User who requested deletion
    /// </summary>
    public Guid? DeletedByUserId { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // EF Core requires parameterless constructor
    private TelemedicineRecording()
    {
        TenantId = string.Empty;
        RecordingPath = string.Empty;
        FileFormat = string.Empty;
        Status = RecordingStatus.Pending;
        IsEncrypted = false;
        IsDeleted = false;
    }
    
    public TelemedicineRecording(
        string tenantId,
        Guid sessionId,
        string recordingPath,
        string fileFormat,
        Guid consentId,
        bool isEncrypted = true,
        string? encryptionKeyId = null,
        int retentionYears = 20)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required", nameof(tenantId));
            
        if (sessionId == Guid.Empty)
            throw new ArgumentException("SessionId is required", nameof(sessionId));
            
        if (string.IsNullOrWhiteSpace(recordingPath))
            throw new ArgumentException("RecordingPath is required", nameof(recordingPath));
            
        if (string.IsNullOrWhiteSpace(fileFormat))
            throw new ArgumentException("FileFormat is required", nameof(fileFormat));
            
        if (consentId == Guid.Empty)
            throw new ArgumentException("ConsentId is required - recording requires patient consent", nameof(consentId));
            
        if (isEncrypted && string.IsNullOrWhiteSpace(encryptionKeyId))
            throw new ArgumentException("EncryptionKeyId is required when recording is encrypted", nameof(encryptionKeyId));
        
        Id = Guid.NewGuid();
        TenantId = tenantId;
        SessionId = sessionId;
        RecordingPath = recordingPath;
        FileFormat = fileFormat;
        ConsentId = consentId;
        IsEncrypted = isEncrypted;
        EncryptionKeyId = encryptionKeyId;
        Status = RecordingStatus.Pending;
        RecordingStartedAt = DateTime.UtcNow;
        RetentionUntil = DateTime.UtcNow.AddYears(retentionYears);
        IsDeleted = false;
        CreatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks recording as in progress
    /// </summary>
    public void StartRecording()
    {
        if (Status != RecordingStatus.Pending)
            throw new InvalidOperationException($"Cannot start recording in {Status} status");
            
        Status = RecordingStatus.Recording;
        RecordingStartedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks recording as completed
    /// </summary>
    public void CompleteRecording(long fileSizeBytes, int durationSeconds)
    {
        if (Status != RecordingStatus.Recording && Status != RecordingStatus.Pending)
            throw new InvalidOperationException($"Cannot complete recording in {Status} status");
            
        if (fileSizeBytes <= 0)
            throw new ArgumentException("File size must be greater than 0", nameof(fileSizeBytes));
            
        if (durationSeconds <= 0)
            throw new ArgumentException("Duration must be greater than 0", nameof(durationSeconds));
            
        Status = RecordingStatus.Available;
        RecordingCompletedAt = DateTime.UtcNow;
        FileSizeBytes = fileSizeBytes;
        DurationSeconds = durationSeconds;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks recording as failed
    /// </summary>
    public void FailRecording(string reason)
    {
        if (Status == RecordingStatus.Available)
            throw new InvalidOperationException("Cannot fail a completed recording");
            
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason is required", nameof(reason));
            
        Status = RecordingStatus.Failed;
        DeletionReason = reason; // Use deletion reason to store failure reason
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Soft deletes the recording (per LGPD request)
    /// </summary>
    public void Delete(Guid deletedByUserId, string reason)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Recording is already deleted");
            
        if (deletedByUserId == Guid.Empty)
            throw new ArgumentException("DeletedByUserId is required", nameof(deletedByUserId));
            
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Deletion reason is required", nameof(reason));
            
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletionReason = reason;
        DeletedByUserId = deletedByUserId;
        Status = RecordingStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Checks if recording has reached retention period
    /// </summary>
    public bool ShouldBeRetained()
    {
        return DateTime.UtcNow < RetentionUntil;
    }
    
    /// <summary>
    /// Gets human-readable file size
    /// </summary>
    public string GetFormattedFileSize()
    {
        const long kb = 1024;
        const long mb = kb * 1024;
        const long gb = mb * 1024;
        
        if (FileSizeBytes >= gb)
            return $"{FileSizeBytes / (double)gb:F2} GB";
        if (FileSizeBytes >= mb)
            return $"{FileSizeBytes / (double)mb:F2} MB";
        if (FileSizeBytes >= kb)
            return $"{FileSizeBytes / (double)kb:F2} KB";
            
        return $"{FileSizeBytes} bytes";
    }
    
    /// <summary>
    /// Gets human-readable duration
    /// </summary>
    public string GetFormattedDuration()
    {
        var hours = DurationSeconds / 3600;
        var minutes = (DurationSeconds % 3600) / 60;
        var seconds = DurationSeconds % 60;
        
        if (hours > 0)
            return $"{hours}h {minutes}m {seconds}s";
        if (minutes > 0)
            return $"{minutes}m {seconds}s";
            
        return $"{seconds}s";
    }
}

/// <summary>
/// Status of a telemedicine recording
/// </summary>
public enum RecordingStatus
{
    Pending,    // Recording scheduled but not started
    Recording,  // Currently recording
    Processing, // Post-processing (encoding, encrypting, etc.)
    Available,  // Recording complete and available
    Failed,     // Recording failed
    Deleted     // Recording has been deleted
}
