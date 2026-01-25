namespace MedicSoft.Telemedicine.Domain.Entities;

/// <summary>
/// Represents identity verification for telemedicine participants
/// Required by CFM Resolution 2.314/2022 for bidirectional identification
/// </summary>
public class IdentityVerification
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; }
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// Type of user: "Provider" or "Patient"
    /// </summary>
    public string UserType { get; private set; }
    
    /// <summary>
    /// Type of identity document (RG, CPF, CNH, Passport, etc.)
    /// </summary>
    public string DocumentType { get; private set; }
    
    /// <summary>
    /// Document number
    /// </summary>
    public string DocumentNumber { get; private set; }
    
    /// <summary>
    /// Path to document photo in storage
    /// </summary>
    public string DocumentPhotoPath { get; private set; }
    
    /// <summary>
    /// Path to selfie photo in storage (optional but recommended)
    /// </summary>
    public string? SelfiePath { get; private set; }
    
    /// <summary>
    /// Path to CRM card photo (required for providers)
    /// </summary>
    public string? CrmCardPhotoPath { get; private set; }
    
    /// <summary>
    /// CRM number (required for providers)
    /// </summary>
    public string? CrmNumber { get; private set; }
    
    /// <summary>
    /// CRM state (required for providers)
    /// </summary>
    public string? CrmState { get; private set; }
    
    /// <summary>
    /// Current status of verification
    /// </summary>
    public VerificationStatus Status { get; private set; }
    
    /// <summary>
    /// Date when verification was performed
    /// </summary>
    public DateTime VerifiedAt { get; private set; }
    
    /// <summary>
    /// User who verified this identity (if manual verification)
    /// </summary>
    public Guid? VerifiedByUserId { get; private set; }
    
    /// <summary>
    /// Notes from verification process
    /// </summary>
    public string? VerificationNotes { get; private set; }
    
    /// <summary>
    /// Linked telemedicine session (if verification done during session)
    /// </summary>
    public Guid? TelemedicineSessionId { get; private set; }
    
    /// <summary>
    /// Date until which this verification is valid
    /// </summary>
    public DateTime ValidUntil { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // EF Core requires parameterless constructor
    private IdentityVerification()
    {
        TenantId = string.Empty;
        UserType = string.Empty;
        DocumentType = string.Empty;
        DocumentNumber = string.Empty;
        DocumentPhotoPath = string.Empty;
        Status = VerificationStatus.Pending;
    }
    
    public IdentityVerification(
        string tenantId,
        Guid userId,
        string userType,
        string documentType,
        string documentNumber,
        string documentPhotoPath,
        string? selfiePath = null,
        string? crmCardPhotoPath = null,
        string? crmNumber = null,
        string? crmState = null,
        Guid? telemedicineSessionId = null,
        int validityYears = 1)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required", nameof(tenantId));
            
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId is required", nameof(userId));
            
        if (string.IsNullOrWhiteSpace(userType))
            throw new ArgumentException("UserType is required", nameof(userType));
            
        if (userType != "Provider" && userType != "Patient")
            throw new ArgumentException("UserType must be 'Provider' or 'Patient'", nameof(userType));
            
        if (string.IsNullOrWhiteSpace(documentType))
            throw new ArgumentException("DocumentType is required", nameof(documentType));
            
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException("DocumentNumber is required", nameof(documentNumber));
            
        if (string.IsNullOrWhiteSpace(documentPhotoPath))
            throw new ArgumentException("DocumentPhotoPath is required", nameof(documentPhotoPath));
            
        // Validate provider requirements
        if (userType == "Provider")
        {
            if (string.IsNullOrWhiteSpace(crmCardPhotoPath))
                throw new ArgumentException("CRM card photo is required for providers", nameof(crmCardPhotoPath));
                
            if (string.IsNullOrWhiteSpace(crmNumber))
                throw new ArgumentException("CRM number is required for providers", nameof(crmNumber));
                
            if (string.IsNullOrWhiteSpace(crmState))
                throw new ArgumentException("CRM state is required for providers", nameof(crmState));
        }
        
        Id = Guid.NewGuid();
        TenantId = tenantId;
        UserId = userId;
        UserType = userType;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        DocumentPhotoPath = documentPhotoPath;
        SelfiePath = selfiePath;
        CrmCardPhotoPath = crmCardPhotoPath;
        CrmNumber = crmNumber;
        CrmState = crmState;
        TelemedicineSessionId = telemedicineSessionId;
        Status = VerificationStatus.Pending;
        VerifiedAt = DateTime.UtcNow;
        ValidUntil = DateTime.UtcNow.AddYears(validityYears);
        CreatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Approves this identity verification
    /// </summary>
    public void Approve(Guid? verifiedByUserId = null, string? notes = null)
    {
        if (Status != VerificationStatus.Pending)
            throw new InvalidOperationException($"Cannot approve verification in {Status} status");
            
        Status = VerificationStatus.Verified;
        VerifiedByUserId = verifiedByUserId;
        VerificationNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Rejects this identity verification
    /// </summary>
    public void Reject(string reason, Guid? verifiedByUserId = null)
    {
        if (Status != VerificationStatus.Pending)
            throw new InvalidOperationException($"Cannot reject verification in {Status} status");
            
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required", nameof(reason));
            
        Status = VerificationStatus.Rejected;
        VerifiedByUserId = verifiedByUserId;
        VerificationNotes = reason;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Marks this verification as expired
    /// </summary>
    public void Expire()
    {
        Status = VerificationStatus.Expired;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Checks if this verification is currently valid
    /// </summary>
    public bool IsValid()
    {
        return Status == VerificationStatus.Verified && 
               ValidUntil > DateTime.UtcNow;
    }
    
    /// <summary>
    /// Checks if verification has expired
    /// </summary>
    public bool HasExpired()
    {
        return ValidUntil <= DateTime.UtcNow;
    }
}

/// <summary>
/// Status of identity verification
/// </summary>
public enum VerificationStatus
{
    Pending,    // Awaiting verification
    Verified,   // Approved
    Rejected,   // Rejected
    Expired     // No longer valid
}
