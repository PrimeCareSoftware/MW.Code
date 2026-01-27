namespace PatientPortal.Domain.Entities;

/// <summary>
/// Represents a password reset token for password recovery
/// </summary>
public class PasswordResetToken
{
    public Guid Id { get; set; }
    public Guid PatientUserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public string? CreatedByIp { get; set; }
    
    /// <summary>
    /// Gets whether the token is valid (not expired and not used)
    /// </summary>
    public bool IsValid => !IsUsed && DateTime.UtcNow < ExpiresAt;
}
