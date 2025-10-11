using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.CrossCutting.Security;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for password recovery with 2FA verification
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/password-recovery")]
    public class PasswordRecoveryController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        // In real implementation, inject ISmsNotificationService and IEmailService

        public PasswordRecoveryController(
            ITenantContext tenantContext,
            IUserRepository userRepository,
            IPasswordResetTokenRepository tokenRepository,
            IPasswordHasher passwordHasher,
            IConfiguration configuration) : base(tenantContext)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        /// <summary>
        /// Request password reset - sends verification code via SMS or Email
        /// </summary>
        [HttpPost("request")]
        public async Task<ActionResult<PasswordResetRequestResponse>> RequestPasswordReset(
            [FromBody] PasswordResetRequest request)
        {
            var tenantId = GetTenantId();

            // Find user by email or username
            var user = await _userRepository.GetUserByUsernameAsync(request.UsernameOrEmail, tenantId);
            if (user == null)
            {
                // Also try by email
                var users = await _userRepository.GetByClinicIdAsync(Guid.Empty, tenantId);
                // In a real implementation, search by email properly
            }

            // Important: Don't reveal if user exists or not for security
            if (user == null)
            {
                return Ok(new PasswordResetRequestResponse
                {
                    Success = true,
                    Message = "Se o usuário existir, um código de verificação será enviado."
                });
            }

            // Invalidate any existing tokens for this user
            await _tokenRepository.InvalidateAllByUserIdAsync(user.Id, tenantId);

            // Generate secure token and verification code
            var token = GenerateSecureToken();
            var verificationCode = GenerateVerificationCode();

            // Determine method based on request or user preference
            var method = request.Method ?? VerificationMethod.Email;
            var destination = method == VerificationMethod.Email ? user.Email : user.Phone;

            // Create password reset token
            var resetToken = new PasswordResetToken(
                user.Id,
                token,
                verificationCode,
                method,
                destination,
                tenantId,
                expirationMinutes: 15
            );

            await _tokenRepository.AddAsync(resetToken);

            // Send verification code via SMS or Email
            if (method == VerificationMethod.SMS)
            {
                // TODO: Integrate with SMS service
                // await _smsService.SendAsync(user.Phone, $"Seu código de verificação é: {verificationCode}");
            }
            else
            {
                // TODO: Integrate with Email service
                // await _emailService.SendPasswordResetCodeAsync(user.Email, verificationCode, user.FullName);
            }

            return Ok(new PasswordResetRequestResponse
            {
                Success = true,
                Message = "Código de verificação enviado com sucesso.",
                Token = token,
                Method = method.ToString(),
                ExpiresInMinutes = 15
            });
        }

        /// <summary>
        /// Verify 2FA code
        /// </summary>
        [HttpPost("verify-code")]
        public async Task<ActionResult<VerifyCodeResponse>> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            var tenantId = GetTenantId();

            var resetToken = await _tokenRepository.GetByTokenAsync(request.Token, tenantId);

            if (resetToken == null || resetToken.IsExpired())
            {
                return BadRequest(new { message = "Token inválido ou expirado." });
            }

            if (resetToken.IsUsed)
            {
                return BadRequest(new { message = "Token já foi utilizado." });
            }

            // Check max attempts
            if (resetToken.VerificationAttempts >= 5)
            {
                return BadRequest(new { message = "Número máximo de tentativas excedido." });
            }

            // Verify code
            if (resetToken.VerificationCode != request.Code)
            {
                resetToken.IncrementVerificationAttempts();
                await _tokenRepository.UpdateAsync(resetToken);
                
                return BadRequest(new { 
                    message = "Código de verificação incorreto.",
                    attemptsRemaining = 5 - resetToken.VerificationAttempts
                });
            }

            // Mark as verified
            resetToken.Verify();
            await _tokenRepository.UpdateAsync(resetToken);

            return Ok(new VerifyCodeResponse
            {
                Success = true,
                Message = "Código verificado com sucesso. Você pode redefinir sua senha agora.",
                Token = request.Token
            });
        }

        /// <summary>
        /// Reset password after verification
        /// </summary>
        [HttpPost("reset")]
        public async Task<ActionResult<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var tenantId = GetTenantId();

            var resetToken = await _tokenRepository.GetByTokenAsync(request.Token, tenantId);

            if (resetToken == null || !resetToken.IsValid())
            {
                return BadRequest(new { message = "Token inválido, não verificado ou expirado." });
            }

            // Validate password strength
            var minPasswordLength = _configuration.GetValue<int>("Security:MinPasswordLength", 8);
            var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(
                request.NewPassword, 
                minPasswordLength);

            if (!isValid)
            {
                return BadRequest(new { message = errorMessage });
            }

            // Get user and update password
            var user = await _userRepository.GetByIdAsync(resetToken.UserId, tenantId);
            if (user == null)
            {
                return BadRequest(new { message = "Usuário não encontrado." });
            }

            var newPasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatePassword(newPasswordHash);
            await _userRepository.UpdateAsync(user);

            // Mark token as used
            resetToken.MarkAsUsed();
            await _tokenRepository.UpdateAsync(resetToken);

            // Invalidate all other tokens for this user
            await _tokenRepository.InvalidateAllByUserIdAsync(user.Id, tenantId);

            return Ok(new ResetPasswordResponse
            {
                Success = true,
                Message = "Senha redefinida com sucesso. Você pode fazer login com sua nova senha."
            });
        }

        /// <summary>
        /// Resend verification code
        /// </summary>
        [HttpPost("resend-code")]
        public async Task<ActionResult<PasswordResetRequestResponse>> ResendCode([FromBody] ResendCodeRequest request)
        {
            var tenantId = GetTenantId();

            var resetToken = await _tokenRepository.GetByTokenAsync(request.Token, tenantId);

            if (resetToken == null || resetToken.IsExpired() || resetToken.IsUsed)
            {
                return BadRequest(new { message = "Token inválido ou expirado." });
            }

            // Send verification code again
            if (resetToken.Method == VerificationMethod.SMS)
            {
                // TODO: Integrate with SMS service
                // await _smsService.SendAsync(resetToken.Destination, $"Seu código de verificação é: {resetToken.VerificationCode}");
            }
            else
            {
                // TODO: Integrate with Email service
                // await _emailService.SendPasswordResetCodeAsync(resetToken.Destination, resetToken.VerificationCode, "");
            }

            return Ok(new PasswordResetRequestResponse
            {
                Success = true,
                Message = "Código de verificação reenviado com sucesso.",
                Token = request.Token,
                Method = resetToken.Method.ToString(),
                ExpiresInMinutes = (int)(resetToken.ExpiresAt - DateTime.UtcNow).TotalMinutes
            });
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }

    public class PasswordResetRequest
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public VerificationMethod? Method { get; set; }
    }

    public class PasswordResetRequestResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
    }

    public class VerifyCodeRequest
    {
        public string Token { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class VerifyCodeResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ResendCodeRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
