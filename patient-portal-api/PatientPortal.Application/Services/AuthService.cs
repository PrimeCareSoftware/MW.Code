using PatientPortal.Application.DTOs.Auth;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Interfaces;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PatientPortal.Application.Services;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly IPatientUserRepository _patientUserRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IEmailVerificationTokenRepository _emailVerificationTokenRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IPatientUserRepository patientUserRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IEmailVerificationTokenRepository emailVerificationTokenRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        ITokenService tokenService,
        INotificationService notificationService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _patientUserRepository = patientUserRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _emailVerificationTokenRepository = emailVerificationTokenRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _tokenService = tokenService;
        _notificationService = notificationService;
        _configuration = configuration;
        _logger = logger;
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

        // Generate and send email verification token
        await SendVerificationEmailAsync(patientUser);

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

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var verificationToken = await _emailVerificationTokenRepository.GetByTokenAsync(token);
        
        if (verificationToken == null || !verificationToken.IsValid)
        {
            return false;
        }

        var user = await _patientUserRepository.GetByIdAsync(verificationToken.PatientUserId);
        if (user == null)
        {
            return false;
        }

        // Mark email as confirmed
        user.EmailConfirmed = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _patientUserRepository.UpdateAsync(user);

        // Mark token as used
        verificationToken.IsUsed = true;
        verificationToken.UsedAt = DateTime.UtcNow;
        await _emailVerificationTokenRepository.UpdateAsync(verificationToken);

        _logger.LogInformation("Email verified for user {PatientUserId}", user.Id);
        return true;
    }

    public async Task ResendVerificationEmailAsync(string email)
    {
        var user = await _patientUserRepository.GetByEmailAsync(email.ToLower());
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado");
        }

        if (user.EmailConfirmed)
        {
            throw new InvalidOperationException("E-mail já verificado");
        }

        await SendVerificationEmailAsync(user);
    }

    public async Task RequestPasswordResetAsync(string email, string ipAddress)
    {
        var user = await _patientUserRepository.GetByEmailAsync(email.ToLower());
        if (user == null)
        {
            // Don't reveal that the user doesn't exist for security reasons
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
            return;
        }

        // Create password reset token
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = user.Id,
            Token = GenerateSecureToken(),
            ExpiresAt = DateTime.UtcNow.AddHours(1), // Token valid for 1 hour
            CreatedAt = DateTime.UtcNow,
            IsUsed = false,
            CreatedByIp = ipAddress
        };

        await _passwordResetTokenRepository.CreateAsync(resetToken);

        // Send password reset email
        var portalBaseUrl = _configuration["PortalBaseUrl"] ?? "https://portal.primecare.com";
        var resetLink = $"{portalBaseUrl}/reset-password?token={resetToken.Token}";
        
        var emailBody = EmailTemplateHelper.GeneratePasswordResetEmail(user.FullName, resetLink, portalBaseUrl);
        
        try
        {
            await _notificationService.SendEmailAsync(user.Email, "Recuperação de Senha - Portal do Paciente", emailBody);
            _logger.LogInformation("Password reset email sent to {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
            throw new InvalidOperationException("Falha ao enviar e-mail de recuperação de senha");
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var resetToken = await _passwordResetTokenRepository.GetByTokenAsync(token);
        
        if (resetToken == null || !resetToken.IsValid)
        {
            return false;
        }

        var user = await _patientUserRepository.GetByIdAsync(resetToken.PatientUserId);
        if (user == null)
        {
            return false;
        }

        // Update password
        user.PasswordHash = HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _patientUserRepository.UpdateAsync(user);

        // Mark token as used
        resetToken.IsUsed = true;
        resetToken.UsedAt = DateTime.UtcNow;
        await _passwordResetTokenRepository.UpdateAsync(resetToken);

        // Revoke all active password reset tokens for this user
        await _passwordResetTokenRepository.RevokeAllActiveTokensAsync(user.Id);

        _logger.LogInformation("Password reset successfully for user {PatientUserId}", user.Id);
        return true;
    }

    private async Task SendVerificationEmailAsync(PatientUser user)
    {
        // Create email verification token
        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = user.Id,
            Token = GenerateSecureToken(),
            ExpiresAt = DateTime.UtcNow.AddHours(24), // Token valid for 24 hours
            CreatedAt = DateTime.UtcNow,
            IsUsed = false
        };

        await _emailVerificationTokenRepository.CreateAsync(verificationToken);

        // Send verification email
        var portalBaseUrl = _configuration["PortalBaseUrl"] ?? "https://portal.primecare.com";
        var verificationLink = $"{portalBaseUrl}/verify-email?token={verificationToken.Token}";
        
        var emailBody = EmailTemplateHelper.GenerateEmailVerificationEmail(user.FullName, verificationLink, portalBaseUrl);
        
        try
        {
            await _notificationService.SendEmailAsync(user.Email, "Confirme seu E-mail - Portal do Paciente", emailBody);
            _logger.LogInformation("Verification email sent to {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email to {Email}", user.Email);
            // Don't throw here - allow registration to succeed even if email fails
        }
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
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
