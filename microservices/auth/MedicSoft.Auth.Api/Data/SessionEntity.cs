namespace MedicSoft.Auth.Api.Data;

/// <summary>
/// Represents an active user session
/// </summary>
public class UserSessionEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    
    // Navigation property
    public UserEntity? User { get; set; }
}

/// <summary>
/// Represents an active owner session
/// </summary>
public class OwnerSessionEntity
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public string? UserAgent { get; set; }
    public string? IpAddress { get; set; }
    
    // Navigation property
    public OwnerEntity? Owner { get; set; }
}
