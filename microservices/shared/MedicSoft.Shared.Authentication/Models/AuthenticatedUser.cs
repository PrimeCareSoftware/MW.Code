namespace MedicSoft.Shared.Authentication.Models;

/// <summary>
/// Represents the authenticated user context extracted from JWT token
/// </summary>
public class AuthenticatedUser
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public Guid? ClinicId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string? SessionId { get; set; }
    public bool IsSystemOwner { get; set; }
}
