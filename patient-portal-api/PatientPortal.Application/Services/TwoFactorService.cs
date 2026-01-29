using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PatientPortal.Application.Interfaces;
using PatientPortal.Application.Services;
using PatientPortal.Domain.Interfaces;

namespace PatientPortal.Application.Services;

/// <summary>
/// Service implementation for two-factor authentication
/// </summary>
public class TwoFactorService : ITwoFactorService
{
    private readonly IPatientUserRepository _patientUserRepository;
    private readonly ITwoFactorTokenRepository _twoFactorTokenRepository;
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TwoFactorService> _logger;

    // Constants for security
    private const int CodeLength = 6;
    private const int CodeValidityMinutes = 5;
    private const int MaxTokensPerHour = 3;
    private const int MaxVerificationAttempts = 5;

    public TwoFactorService(
        IPatientUserRepository patientUserRepository,
        ITwoFactorTokenRepository twoFactorTokenRepository,
        INotificationService notificationService,
        IConfiguration configuration,
        ILogger<TwoFactorService> logger)
    {
        _patientUserRepository = patientUserRepository;
        _twoFactorTokenRepository = twoFactorTokenRepository;
        _notificationService = notificationService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> EnableTwoFactorAsync(Guid patientUserId)
    {
        var user = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (user == null)
        {
            _logger.LogWarning("Attempted to enable 2FA for non-existent user {PatientUserId}", patientUserId);
            return false;
        }

        if (user.TwoFactorEnabled)
        {
            _logger.LogInformation("2FA already enabled for user {PatientUserId}", patientUserId);
            return true;
        }

        user.TwoFactorEnabled = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _patientUserRepository.UpdateAsync(user);

        _logger.LogInformation("2FA enabled successfully for user {PatientUserId}", patientUserId);
        
        // Send notification email about security change
        await SendSecurityNotificationAsync(user.Email, user.FullName, "habilitada");

        return true;
    }

    public async Task<bool> DisableTwoFactorAsync(Guid patientUserId)
    {
        var user = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (user == null)
        {
            _logger.LogWarning("Attempted to disable 2FA for non-existent user {PatientUserId}", patientUserId);
            return false;
        }

        if (!user.TwoFactorEnabled)
        {
            _logger.LogInformation("2FA already disabled for user {PatientUserId}", patientUserId);
            return true;
        }

        user.TwoFactorEnabled = false;
        user.UpdatedAt = DateTime.UtcNow;
        await _patientUserRepository.UpdateAsync(user);

        _logger.LogInformation("2FA disabled successfully for user {PatientUserId}", patientUserId);
        
        // Send notification email about security change
        await SendSecurityNotificationAsync(user.Email, user.FullName, "desabilitada");

        return true;
    }

    public async Task<string> GenerateAndSendCodeAsync(Guid patientUserId, string purpose, string ipAddress)
    {
        var user = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (user == null)
        {
            throw new InvalidOperationException("Usuário não encontrado");
        }

        // Rate limiting: check if user has exceeded token generation limit
        var recentTokensCount = await _twoFactorTokenRepository.CountRecentTokensAsync(
            patientUserId, 
            TimeSpan.FromHours(1)
        );

        if (recentTokensCount >= MaxTokensPerHour)
        {
            _logger.LogWarning("User {PatientUserId} exceeded 2FA token generation limit", patientUserId);
            throw new InvalidOperationException("Limite de códigos atingido. Tente novamente em 1 hora.");
        }

        // Generate a secure 6-digit code
        var code = GenerateSecureCode();

        // Create the token
        var token = new Domain.Entities.TwoFactorToken
        {
            Id = Guid.NewGuid(),
            PatientUserId = patientUserId,
            Code = code,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(CodeValidityMinutes),
            IsUsed = false,
            Purpose = purpose,
            IpAddress = ipAddress,
            VerificationAttempts = 0
        };

        await _twoFactorTokenRepository.CreateAsync(token);

        // Send the code via email
        await SendCodeByEmailAsync(user.Email, user.FullName, code);

        _logger.LogInformation("2FA code generated and sent to user {PatientUserId} for purpose {Purpose}", 
            patientUserId, purpose);

        // Return a temporary token (the token ID) that the client will use to verify
        return Convert.ToBase64String(token.Id.ToByteArray());
    }

    public async Task<bool> VerifyCodeAsync(Guid patientUserId, string code, string tempToken)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(tempToken))
        {
            return false;
        }

        // Decode the temporary token to get the token ID
        Guid tokenId;
        try
        {
            var tokenIdBytes = Convert.FromBase64String(tempToken);
            tokenId = new Guid(tokenIdBytes);
        }
        catch
        {
            _logger.LogWarning("Invalid temporary token format");
            return false;
        }

        // Get the token
        var token = await _twoFactorTokenRepository.GetByCodeAsync(code, patientUserId);

        if (token == null || token.Id != tokenId)
        {
            _logger.LogWarning("2FA token not found for user {PatientUserId}", patientUserId);
            return false;
        }

        // Increment verification attempts
        token.VerificationAttempts++;
        await _twoFactorTokenRepository.UpdateAsync(token);

        // Check if token is valid
        if (!token.IsValid)
        {
            _logger.LogWarning("Invalid 2FA token for user {PatientUserId}. Used: {IsUsed}, Expired: {Expired}, Attempts: {Attempts}", 
                patientUserId, token.IsUsed, DateTime.UtcNow >= token.ExpiresAt, token.VerificationAttempts);
            return false;
        }

        // Verify the code matches
        if (token.Code != code)
        {
            _logger.LogWarning("Incorrect 2FA code for user {PatientUserId}", patientUserId);
            return false;
        }

        // Mark token as used
        token.IsUsed = true;
        token.UsedAt = DateTime.UtcNow;
        await _twoFactorTokenRepository.UpdateAsync(token);

        _logger.LogInformation("2FA code verified successfully for user {PatientUserId}", patientUserId);
        return true;
    }

    public async Task<bool> IsTwoFactorEnabledAsync(Guid patientUserId)
    {
        var user = await _patientUserRepository.GetByIdAsync(patientUserId);
        return user?.TwoFactorEnabled ?? false;
    }

    public async Task<bool> ResendCodeAsync(string tempToken, string ipAddress)
    {
        // Decode the temporary token to get the token ID
        Guid tokenId;
        try
        {
            var tokenIdBytes = Convert.FromBase64String(tempToken);
            tokenId = new Guid(tokenIdBytes);
        }
        catch
        {
            _logger.LogWarning("Invalid temporary token format for resend");
            return false;
        }

        // We need to get the patient user ID from the original token
        // For now, we'll generate a new code and return the new temp token
        // This is a simplified implementation - in production, you might want to track the original request
        _logger.LogWarning("ResendCodeAsync called but requires refactoring to get patientUserId from tempToken");
        return false;
    }

    /// <summary>
    /// Generates a cryptographically secure 6-digit code
    /// </summary>
    private string GenerateSecureCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var number = BitConverter.ToUInt32(bytes, 0) % 1000000; // 0-999999
        return number.ToString("D6"); // Format as 6 digits with leading zeros
    }

    /// <summary>
    /// Sends the 2FA code to the user via email
    /// </summary>
    private async Task SendCodeByEmailAsync(string email, string fullName, string code)
    {
        var portalBaseUrl = _configuration["PortalBaseUrl"] ?? "https://portal.primecare.com";
        
        var emailBody = EmailTemplateHelper.GenerateTwoFactorCodeEmail(fullName, code, portalBaseUrl);
        
        try
        {
            await _notificationService.SendEmailAsync(
                email, 
                "Código de Verificação - Portal do Paciente", 
                emailBody
            );
            _logger.LogInformation("2FA code email sent to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send 2FA code email to {Email}", email);
            throw new InvalidOperationException("Falha ao enviar código de verificação por email");
        }
    }

    /// <summary>
    /// Sends a security notification when 2FA is enabled/disabled
    /// </summary>
    private async Task SendSecurityNotificationAsync(string email, string fullName, string action)
    {
        var portalBaseUrl = _configuration["PortalBaseUrl"] ?? "https://portal.primecare.com";
        
        var subject = "Alteração de Segurança - Autenticação de Dois Fatores";
        var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #007bff; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
        .alert {{ background-color: #fff3cd; border: 1px solid #ffc107; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Portal do Paciente</h2>
        </div>
        <div class='content'>
            <h3>Olá {fullName},</h3>
            <div class='alert'>
                <p><strong>A autenticação de dois fatores foi {action} para sua conta.</strong></p>
            </div>
            <p>Se você não realizou esta ação, entre em contato conosco imediatamente.</p>
            <p>Data: {DateTime.UtcNow.AddHours(-3):dd/MM/yyyy HH:mm} (Horário de Brasília)</p>
        </div>
        <div class='footer'>
            <p>© 2026 PrimeCare Software. Todos os direitos reservados.</p>
            <p><a href='{portalBaseUrl}'>Acessar Portal do Paciente</a></p>
        </div>
    </div>
</body>
</html>";

        try
        {
            await _notificationService.SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            // Don't throw - this is a notification, not critical
            _logger.LogError(ex, "Failed to send security notification email to {Email}", email);
        }
    }
}
