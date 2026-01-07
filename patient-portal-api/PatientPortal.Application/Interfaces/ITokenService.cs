namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Token generator interface for JWT tokens
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(Guid patientUserId, string email, string fullName);
    string GenerateRefreshToken();
    Guid? ValidateAccessToken(string token);
}
