namespace PatientPortal.Application.DTOs.Auth;

/// <summary>
/// DTO for login response with tokens
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public PatientUserDto User { get; set; } = null!;
}

public class PatientUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public List<string> Permissions { get; set; } = new();
    public List<string> VisibleFields { get; set; } = new();
}
