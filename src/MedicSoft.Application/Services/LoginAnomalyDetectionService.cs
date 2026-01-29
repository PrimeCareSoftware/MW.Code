using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using UAParser;

namespace MedicSoft.Application.Services
{
    public class LoginAnomalyDetectionService : ILoginAnomalyDetectionService
    {
        private readonly IUserSessionRepository _sessionRepository;
        private readonly IAuditService _auditService;
        private readonly INotificationService _notificationService;

        public LoginAnomalyDetectionService(
            IUserSessionRepository sessionRepository,
            IAuditService auditService,
            INotificationService notificationService)
        {
            _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }

        public async Task<bool> IsLoginSuspicious(string userId, LoginAttemptDto attempt, string tenantId)
        {
            if (string.IsNullOrEmpty(userId) || attempt == null)
                return false;

            // Get recent successful login sessions
            var recentSessions = await _sessionRepository.GetRecentSessionsByUserIdAsync(userId, tenantId, 10);

            if (!recentSessions.Any())
                return false; // First login, not suspicious

            var flags = new List<string>();

            // 1. Check for new IP address
            var knownIps = recentSessions.Select(s => s.IpAddress).Distinct().ToList();
            if (!knownIps.Contains(attempt.IpAddress))
            {
                flags.Add("new_ip");
            }

            // 2. Check for new location (country)
            if (!string.IsNullOrEmpty(attempt.Country))
            {
                var knownCountries = recentSessions
                    .Where(s => !string.IsNullOrEmpty(s.Country))
                    .Select(s => s.Country)
                    .Distinct()
                    .ToList();

                if (knownCountries.Any() && !knownCountries.Contains(attempt.Country))
                {
                    flags.Add("new_country");
                }
            }

            // 3. Check for new device/user agent
            var knownDevices = recentSessions.Select(s => s.UserAgent).Distinct().ToList();
            if (!knownDevices.Any(ua => AreSimilarUserAgents(ua, attempt.UserAgent)))
            {
                flags.Add("new_device");
            }

            // 4. Check for impossible travel (location change too fast)
            var lastSession = recentSessions.FirstOrDefault();
            if (lastSession != null && !string.IsNullOrEmpty(attempt.Country) && !string.IsNullOrEmpty(lastSession.Country))
            {
                var hoursSinceLastLogin = (DateTime.UtcNow - lastSession.StartedAt).TotalHours;
                if (hoursSinceLastLogin < 1 && lastSession.Country != attempt.Country)
                {
                    flags.Add("impossible_travel");
                }
            }

            // Consider suspicious if 2 or more flags
            var isSuspicious = flags.Count >= 2;

            if (isSuspicious)
            {
                // Log the suspicious login attempt
                await _auditService.LogAsync(new DTOs.CreateAuditLogDto(
                    UserId: userId,
                    UserName: "Unknown", // Will be filled by context
                    UserEmail: "Unknown",
                    Action: Domain.Enums.AuditAction.LOGIN_FAILED,
                    ActionDescription: "Tentativa de login suspeita detectada",
                    EntityType: "User",
                    EntityId: userId,
                    EntityDisplayName: null,
                    IpAddress: attempt.IpAddress,
                    UserAgent: attempt.UserAgent,
                    RequestPath: "/api/auth/login",
                    HttpMethod: "POST",
                    Result: Domain.Enums.OperationResult.WARNING,
                    DataCategory: Domain.Enums.DataCategory.PERSONAL,
                    Purpose: Domain.ValueObjects.LgpdPurpose.LEGAL_OBLIGATION,
                    Severity: Domain.Enums.AuditSeverity.WARNING,
                    TenantId: tenantId,
                    FailureReason: $"Flags detectados: {string.Join(", ", flags)}"
                ));

                // Send notification to user (if notification service is available)
                try
                {
                    await _notificationService.CreateAsync(new DTOs.CreateNotificationDto
                    {
                        UserId = userId,
                        Type = "warning",
                        Title = "Login Suspeito Detectado",
                        Message = $"Detectamos uma tentativa de login de um novo dispositivo/localização. Se não foi você, altere sua senha imediatamente.",
                        ActionUrl = "/security/activity",
                        TenantId = tenantId
                    });
                }
                catch
                {
                    // Notification service might not be available, continue anyway
                }
            }

            return isSuspicious;
        }

        public async Task RecordLoginAttempt(string userId, LoginAttemptDto attempt, bool success, string tenantId)
        {
            if (!success)
            {
                // For failed attempts, we just log them via audit service
                await _auditService.LogAsync(new DTOs.CreateAuditLogDto(
                    UserId: userId,
                    UserName: "Unknown",
                    UserEmail: "Unknown",
                    Action: Domain.Enums.AuditAction.LOGIN_FAILED,
                    ActionDescription: "Tentativa de login falhou",
                    EntityType: "User",
                    EntityId: userId,
                    EntityDisplayName: null,
                    IpAddress: attempt.IpAddress,
                    UserAgent: attempt.UserAgent,
                    RequestPath: "/api/auth/login",
                    HttpMethod: "POST",
                    Result: Domain.Enums.OperationResult.FAILED,
                    DataCategory: Domain.Enums.DataCategory.PERSONAL,
                    Purpose: Domain.ValueObjects.LgpdPurpose.LEGAL_OBLIGATION,
                    Severity: Domain.Enums.AuditSeverity.WARNING,
                    TenantId: tenantId
                ));
            }
            // Successful login sessions are recorded by the authentication service
        }

        private bool AreSimilarUserAgents(string ua1, string ua2)
        {
            if (string.IsNullOrEmpty(ua1) || string.IsNullOrEmpty(ua2))
                return false;

            try
            {
                var parser = Parser.GetDefault();
                var client1 = parser.Parse(ua1);
                var client2 = parser.Parse(ua2);

                // Compare browser family and OS family (ignoring versions)
                return client1.UA.Family == client2.UA.Family &&
                       client1.OS.Family == client2.OS.Family;
            }
            catch
            {
                // If parsing fails, do simple string comparison
                return ua1.Equals(ua2, StringComparison.OrdinalIgnoreCase);
            }
        }

        private async Task<string> GetCountryFromIp(string ipAddress)
        {
            // TODO: Implement IP geolocation using MaxMind, IP2Location, or similar service
            // For now, return a placeholder
            await Task.CompletedTask;
            return "BR";
        }
    }
}
