namespace MedicSoft.Telemedicine.Domain.Entities;

/// <summary>
/// Represents patient consent for telemedicine consultation
/// Required by CFM Resolution 2.314/2022
/// </summary>
public class TelemedicineConsent
{
    public Guid Id { get; private set; }
    public string TenantId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid? AppointmentId { get; private set; }
    
    /// <summary>
    /// Date and time when consent was given
    /// </summary>
    public DateTime ConsentDate { get; private set; }
    
    /// <summary>
    /// Full text of the consent agreement
    /// </summary>
    public string ConsentText { get; private set; }
    
    /// <summary>
    /// IP address from which consent was given (for audit trail)
    /// </summary>
    public string IpAddress { get; private set; }
    
    /// <summary>
    /// User agent string (browser/device info)
    /// </summary>
    public string UserAgent { get; private set; }
    
    /// <summary>
    /// Whether patient accepts recording of the consultation
    /// </summary>
    public bool AcceptsRecording { get; private set; }
    
    /// <summary>
    /// Whether patient accepts data sharing for telemedicine
    /// </summary>
    public bool AcceptsDataSharing { get; private set; }
    
    /// <summary>
    /// Digital signature or confirmation code
    /// </summary>
    public string? DigitalSignature { get; private set; }
    
    /// <summary>
    /// Whether this consent is still valid
    /// </summary>
    public bool IsActive { get; private set; }
    
    /// <summary>
    /// Date when consent was revoked (if applicable)
    /// </summary>
    public DateTime? RevokedAt { get; private set; }
    
    /// <summary>
    /// Reason for revocation
    /// </summary>
    public string? RevocationReason { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // EF Core requires parameterless constructor
    private TelemedicineConsent() 
    {
        TenantId = string.Empty;
        ConsentText = string.Empty;
        IpAddress = string.Empty;
        UserAgent = string.Empty;
        AcceptsRecording = false;
        AcceptsDataSharing = false;
        IsActive = false;
    }
    
    public TelemedicineConsent(
        string tenantId,
        Guid patientId,
        string consentText,
        string ipAddress,
        string userAgent,
        bool acceptsRecording,
        bool acceptsDataSharing,
        Guid? appointmentId = null,
        string? digitalSignature = null)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required", nameof(tenantId));
            
        if (patientId == Guid.Empty)
            throw new ArgumentException("PatientId is required", nameof(patientId));
            
        if (string.IsNullOrWhiteSpace(consentText))
            throw new ArgumentException("ConsentText is required", nameof(consentText));
            
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IpAddress is required", nameof(ipAddress));
            
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("UserAgent is required", nameof(userAgent));
        
        Id = Guid.NewGuid();
        TenantId = tenantId;
        PatientId = patientId;
        AppointmentId = appointmentId;
        ConsentText = consentText;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        AcceptsRecording = acceptsRecording;
        AcceptsDataSharing = acceptsDataSharing;
        DigitalSignature = digitalSignature;
        ConsentDate = DateTime.UtcNow;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Revokes this consent
    /// </summary>
    public void Revoke(string reason)
    {
        if (!IsActive)
            throw new InvalidOperationException("Consent is already revoked");
            
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Revocation reason is required", nameof(reason));
        
        IsActive = false;
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Checks if consent is valid for a telemedicine session
    /// </summary>
    public bool IsValidForSession()
    {
        return IsActive && ConsentDate <= DateTime.UtcNow;
    }
}
