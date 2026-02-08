using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Application.Configuration;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Middleware
{
    /// <summary>
    /// Middleware to enforce MFA for administrative roles
    /// </summary>
    public class MfaEnforcementMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MfaEnforcementMiddleware> _logger;
        private readonly MfaPolicySettings _settings;

        // Paths that are exempt from MFA enforcement
        private static readonly string[] ExemptPaths = new[]
        {
            "/api/auth/login",
            "/api/auth/owner-login",
            "/api/mfa/setup",
            "/api/mfa/verify",
            "/api/mfa/status",
            "/api/mfa/regenerate-backup-codes",
            "/api/password-recovery",
            "/swagger",
            "/health",
            "/api/public",
            "/hubs"
        };

        public MfaEnforcementMiddleware(
            RequestDelegate next,
            ILogger<MfaEnforcementMiddleware> logger,
            IOptions<MfaPolicySettings> settings)
        {
            _next = next;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IUserRepository userRepository,
            ITwoFactorAuthService twoFactorAuthService)
        {
            // Skip if enforcement is disabled
            if (!_settings.EnforcementEnabled)
            {
                await _next(context);
                return;
            }

            // Skip for exempt paths
            var path = context.Request.Path.Value ?? "";
            if (ExemptPaths.Any(exempt => path.StartsWith(exempt, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Skip if user is not authenticated
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                await _next(context);
                return;
            }

            // Get user info from claims
            var userIdClaim = context.User.FindFirst("userId")?.Value;
            var userRoleClaim = context.User.FindFirst("role")?.Value;
            var tenantIdClaim = context.User.FindFirst("tenantId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || 
                string.IsNullOrEmpty(userRoleClaim) || 
                string.IsNullOrEmpty(tenantIdClaim))
            {
                await _next(context);
                return;
            }

            // Check if role requires MFA
            if (!_settings.RequiredForRoles.Contains(userRoleClaim, StringComparer.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            try
            {
                // Get user from database
                var user = await userRepository.GetByIdAsync(Guid.Parse(userIdClaim), tenantIdClaim);
                if (user == null)
                {
                    _logger.LogWarning("User {UserId} not found during MFA enforcement", userIdClaim);
                    await _next(context);
                    return;
                }

                // Check if MFA is enabled
                var mfaEnabled = await twoFactorAuthService.IsTwoFactorEnabledAsync(userIdClaim, tenantIdClaim);

                if (!mfaEnabled)
                {
                    // Check if user is in grace period
                    if (user.IsInMfaGracePeriod)
                    {
                        _logger.LogInformation(
                            "User {UserId} ({Role}) in MFA grace period until {GracePeriodEnd}", 
                            userIdClaim, 
                            userRoleClaim, 
                            user.MfaGracePeriodEndsAt);
                        
                        // Add header to inform client about grace period
                        context.Response.Headers.Add("X-MFA-Required", "true");
                        context.Response.Headers.Add("X-MFA-Grace-Period-Ends", 
                            user.MfaGracePeriodEndsAt?.ToString("O") ?? "");
                        
                        await _next(context);
                        return;
                    }

                    // Grace period expired - block access
                    _logger.LogWarning(
                        "Blocking access for user {UserId} ({Role}) - MFA not configured and grace period expired",
                        userIdClaim,
                        userRoleClaim);

                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(@"{
                        ""error"": ""MFA_REQUIRED"",
                        ""message"": ""Multi-factor authentication is required for your role. Your grace period has expired. Please configure MFA to continue."",
                        ""setupUrl"": ""/api/mfa/setup"",
                        ""requiredByPolicy"": true
                    }");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during MFA enforcement for user {UserId}", userIdClaim);
                
                // SECURITY: Fail-secure approach - block access on critical errors
                // Log this as a security event for monitoring
                _logger.LogWarning(
                    "SECURITY ALERT: MFA enforcement check failed for admin user {UserId} ({Role}). Access blocked for security.",
                    userIdClaim,
                    userRoleClaim);
                
                context.Response.StatusCode = 503;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(@"{
                    ""error"": ""MFA_ENFORCEMENT_ERROR"",
                    ""message"": ""Unable to verify MFA status. Please try again or contact support."",
                    ""isSecurityError"": true
                }");
                return;
            }

            await _next(context);
        }
    }
}
