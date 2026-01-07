using PatientPortal.Application.DTOs.Auth;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Interfaces;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace PatientPortal.Application.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IPatientUserRepository _patientUserRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    public AuthService(
        IPatientUserRepository patientUserRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _patientUserRepository = patientUserRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress)
    {
        // Try to find user by email or CPF
        PatientUser? user = null;
        if (request.EmailOrCPF.Contains('@'))
        {
            user = await _patientUserRepository.GetByEmailAsync(request.EmailOrCPF);
        }
        else
        {
            // Remove non-digit characters from CPF
            var cpf = new string(request.EmailOrCPF.Where(char.IsDigit).ToArray());
            user = await _patientUserRepository.GetByCPFAsync(cpf);
        }

        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }

        // Check if account is locked
        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Conta bloqueada. Tente novamente mais tarde.");
        }

        // Verify password
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            // Increment failed access count
            user.AccessFailedCount++;
            
            // Lock account after 5 failed attempts
            if (user.AccessFailedCount >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }
            
            await _patientUserRepository.UpdateAsync(user);
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }

        // Reset failed access count on successful login
        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _patientUserRepository.UpdateAsync(user);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.FullName);
        var refreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            User = MapToUserDto(user)
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request, string ipAddress)
    {
        // Check if email already exists
        if (await _patientUserRepository.ExistsAsync(request.Email))
        {
            throw new InvalidOperationException("Email já cadastrado");
        }

        // Check if CPF already exists
        var cpf = new string(request.CPF.Where(char.IsDigit).ToArray());
        if (await _patientUserRepository.ExistsByCPFAsync(cpf))
        {
            throw new InvalidOperationException("CPF já cadastrado");
        }

        // Create patient user
        var patientUser = new PatientUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email.ToLower(),
            FullName = request.FullName,
            CPF = cpf,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            PasswordHash = HashPassword(request.Password),
            IsActive = true,
            EmailConfirmed = false,
            PhoneConfirmed = false,
            TwoFactorEnabled = false,
            AccessFailedCount = 0,
            CreatedAt = DateTime.UtcNow,
            PatientId = Guid.NewGuid(), // This should be linked to the main patient record
            ClinicId = Guid.NewGuid() // This should be obtained from context or request
        };

        await _patientUserRepository.CreateAsync(patientUser);

        // Generate tokens
        var accessToken = _tokenService.GenerateAccessToken(patientUser.Id, patientUser.Email, patientUser.FullName);
        var refreshToken = await CreateRefreshTokenAsync(patientUser.Id, ipAddress);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = refreshToken.ExpiresAt,
            User = MapToUserDto(patientUser)
        };
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(string refreshToken, string ipAddress)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (token == null || !token.IsActive)
        {
            throw new UnauthorizedAccessException("Token inválido ou expirado");
        }

        var user = await _patientUserRepository.GetByIdAsync(token.PatientUserId);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Usuário inválido");
        }

        // Revoke old token
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = "Replaced by new token";
        await _refreshTokenRepository.UpdateAsync(token);

        // Generate new tokens
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, user.FullName);
        var newRefreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress);
        
        // Set replacement reference
        token.ReplacedByToken = newRefreshToken.Token;
        await _refreshTokenRepository.UpdateAsync(token);

        return new LoginResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            ExpiresAt = newRefreshToken.ExpiresAt,
            User = MapToUserDto(user)
        };
    }

    public async Task RevokeTokenAsync(string refreshToken, string ipAddress)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (token == null || !token.IsActive)
        {
            throw new UnauthorizedAccessException("Token inválido");
        }

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReasonRevoked = "Revoked by user";
        await _refreshTokenRepository.UpdateAsync(token);
    }

    public async Task<bool> ChangePasswordAsync(Guid patientUserId, string currentPassword, string newPassword)
    {
        var user = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (user == null)
        {
            return false;
        }

        if (!VerifyPassword(currentPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _patientUserRepository.UpdateAsync(user);

        return true;
    }

    private async Task<RefreshToken> CreateRefreshTokenAsync(Guid patientUserId, string ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = patientUserId,
            Token = _tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        return await _refreshTokenRepository.CreateAsync(refreshToken);
    }

    private string HashPassword(string password)
    {
        // Generate a 128-bit salt
        byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);

        // Derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        // Combine salt and hash
        return $"{Convert.ToBase64String(salt)}:{hashed}";
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split(':');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            var hashToVerify = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hash == hashToVerify;
        }
        catch
        {
            return false;
        }
    }

    private PatientUserDto MapToUserDto(PatientUser user)
    {
        return new PatientUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            CPF = user.CPF,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            TwoFactorEnabled = user.TwoFactorEnabled
        };
    }
}
