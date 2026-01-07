using PatientPortal.Application.DTOs.Auth;

namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, string ipAddress);
    Task<LoginResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress);
    Task RevokeTokenAsync(string refreshToken, string ipAddress);
    Task<bool> ChangePasswordAsync(Guid patientUserId, string currentPassword, string newPassword);
}
