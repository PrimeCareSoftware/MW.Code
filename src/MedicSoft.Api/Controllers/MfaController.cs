using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for Multi-Factor Authentication management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MfaController : BaseController
    {
        private readonly ITwoFactorAuthService _twoFactorAuthService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<MfaController> _logger;

        public MfaController(
            ITenantContext tenantContext,
            ITwoFactorAuthService twoFactorAuthService,
            IUserRepository userRepository,
            ILogger<MfaController> logger) : base(tenantContext)
        {
            _twoFactorAuthService = twoFactorAuthService;
            _userRepository = userRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get MFA status for current user
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<MfaStatusResponse>> GetStatus()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), GetTenantId());
                if (user == null)
                    return NotFound(new { message = "User not found" });

                var isEnabled = await _twoFactorAuthService.IsTwoFactorEnabledAsync(userId, GetTenantId());

                return Ok(new MfaStatusResponse
                {
                    IsEnabled = isEnabled,
                    RequiredByPolicy = user.MfaRequiredByPolicy,
                    IsInGracePeriod = user.IsInMfaGracePeriod,
                    GracePeriodEndsAt = user.MfaGracePeriodEndsAt,
                    MustSetupNow = user.MfaRequiredByPolicy && !isEnabled && user.MfaGracePeriodExpired
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting MFA status for user");
                return StatusCode(500, new { message = "Failed to retrieve MFA status" });
            }
        }

        /// <summary>
        /// Start MFA setup wizard - generates secret key and QR code
        /// </summary>
        [HttpPost("setup")]
        public async Task<ActionResult<MfaSetupResponse>> Setup()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), GetTenantId());
                if (user == null)
                    return NotFound(new { message = "User not found" });

                // Check if already enabled
                var isEnabled = await _twoFactorAuthService.IsTwoFactorEnabledAsync(userId, GetTenantId());
                if (isEnabled)
                    return BadRequest(new { message = "MFA is already enabled for this user" });

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var setupInfo = await _twoFactorAuthService.EnableTOTPAsync(
                    userId, 
                    user.Email, 
                    ipAddress, 
                    GetTenantId());

                // Note: Grace period is NOT cleared here - will be cleared after successful verification
                _logger.LogInformation("MFA setup initiated for user {UserId}", userId);

                return Ok(new MfaSetupResponse
                {
                    SecretKey = setupInfo.SecretKey,
                    QRCodeUrl = setupInfo.QRCodeUrl,
                    BackupCodes = setupInfo.BackupCodes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up MFA for user");
                return StatusCode(500, new { message = "Failed to setup MFA" });
            }
        }

        /// <summary>
        /// Verify MFA code during login or setup
        /// </summary>
        [HttpPost("verify")]
        public async Task<ActionResult<MfaVerifyResponse>> Verify([FromBody] MfaVerifyRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return BadRequest(new { message = "Verification code is required" });

                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                bool isValid;
                if (request.IsBackupCode)
                {
                    isValid = await _twoFactorAuthService.VerifyBackupCodeAsync(userId, request.Code, GetTenantId());
                }
                else
                {
                    isValid = await _twoFactorAuthService.VerifyTOTPAsync(userId, request.Code, GetTenantId());
                }

                if (!isValid)
                {
                    _logger.LogWarning("Invalid MFA code attempt for user {UserId}", userId);
                    return Unauthorized(new { message = "Invalid verification code" });
                }

                // Clear grace period after successful MFA verification
                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), GetTenantId());
                if (user != null && user.MfaGracePeriodEndsAt.HasValue)
                {
                    user.ClearMfaGracePeriod();
                    await _userRepository.UpdateAsync(user);
                    _logger.LogInformation("MFA grace period cleared for user {UserId} after successful verification", userId);
                }

                _logger.LogInformation("MFA verification successful for user {UserId}", userId);

                return Ok(new MfaVerifyResponse
                {
                    Success = true,
                    Message = "Verification successful"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying MFA code");
                return StatusCode(500, new { message = "Failed to verify MFA code" });
            }
        }

        /// <summary>
        /// Regenerate backup codes
        /// </summary>
        [HttpPost("regenerate-backup-codes")]
        public async Task<ActionResult<RegenerateBackupCodesResponse>> RegenerateBackupCodes()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var isEnabled = await _twoFactorAuthService.IsTwoFactorEnabledAsync(userId, GetTenantId());
                if (!isEnabled)
                    return BadRequest(new { message = "MFA is not enabled for this user" });

                var backupCodes = await _twoFactorAuthService.RegenerateBackupCodesAsync(userId, GetTenantId());

                _logger.LogInformation("Backup codes regenerated for user {UserId}", userId);

                return Ok(new RegenerateBackupCodesResponse
                {
                    BackupCodes = backupCodes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating backup codes");
                return StatusCode(500, new { message = "Failed to regenerate backup codes" });
            }
        }

        /// <summary>
        /// Disable MFA (requires verification)
        /// </summary>
        [HttpPost("disable")]
        public async Task<ActionResult> Disable([FromBody] MfaDisableRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Code))
                    return BadRequest(new { message = "Verification code is required" });

                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { message = "User not authenticated" });

                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), GetTenantId());
                if (user == null)
                    return NotFound(new { message = "User not found" });

                // Check if MFA is required by policy
                if (user.MfaRequiredByPolicy)
                    return BadRequest(new { message = "MFA is required by policy and cannot be disabled for your role" });

                // Verify the code first
                var isValid = await _twoFactorAuthService.VerifyTOTPAsync(userId, request.Code, GetTenantId());
                if (!isValid)
                {
                    _logger.LogWarning("Invalid MFA code during disable attempt for user {UserId}", userId);
                    return Unauthorized(new { message = "Invalid verification code" });
                }

                await _twoFactorAuthService.DisableTwoFactorAsync(userId, GetTenantId());

                _logger.LogInformation("MFA disabled for user {UserId}", userId);

                return Ok(new { message = "MFA disabled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling MFA");
                return StatusCode(500, new { message = "Failed to disable MFA" });
            }
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst("userId")?.Value;
        }
    }

    #region DTOs

    public class MfaStatusResponse
    {
        public bool IsEnabled { get; set; }
        public bool RequiredByPolicy { get; set; }
        public bool IsInGracePeriod { get; set; }
        public DateTime? GracePeriodEndsAt { get; set; }
        public bool MustSetupNow { get; set; }
    }

    public class MfaSetupResponse
    {
        public string SecretKey { get; set; } = null!;
        public string QRCodeUrl { get; set; } = null!;
        public List<string> BackupCodes { get; set; } = new();
    }

    public class MfaVerifyRequest
    {
        public string Code { get; set; } = null!;
        public bool IsBackupCode { get; set; }
    }

    public class MfaVerifyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }

    public class RegenerateBackupCodesResponse
    {
        public List<string> BackupCodes { get; set; } = new();
    }

    public class MfaDisableRequest
    {
        public string Code { get; set; } = null!;
    }

    #endregion
}
