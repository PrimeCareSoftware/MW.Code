namespace MedicSoft.Auth.Api.Models;

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? TenantId { get; set; }
}

/// <summary>
/// Response model for successful login
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid? ClinicId { get; set; }
    public bool IsSystemOwner { get; set; }
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Request model for token validation
/// </summary>
public class TokenValidationRequest
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Response model for token validation
/// </summary>
public class TokenValidationResponse
{
    public bool IsValid { get; set; }
    public string? Username { get; set; }
    public string? Role { get; set; }
    public string? TenantId { get; set; }
}

/// <summary>
/// Request model for session validation
/// </summary>
public class SessionValidationRequest
{
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Response model for session validation
/// </summary>
public class SessionValidationResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
}
