using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Services
{
    public interface ISuspiciousActivityDetector
    {
        Task<List<SecurityAlert>> DetectSuspiciousActivityAsync(string tenantId);
        Task<List<SecurityAlert>> GetActiveAlertsAsync(string tenantId);
    }

    public class SuspiciousActivityDetector : ISuspiciousActivityDetector
    {
        private readonly IAuditRepository _auditRepository;
        private readonly ILogger<SuspiciousActivityDetector> _logger;
        private const int FailedLoginsWindow = 10; // minutes
        private const int FailedLoginsThreshold = 5;
        private const int BulkExportRecordsThreshold = 100;
        private const int BulkExportTimeWindow = 5; // minutes

        public SuspiciousActivityDetector(
            IAuditRepository auditRepository,
            ILogger<SuspiciousActivityDetector> logger)
        {
            _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<SecurityAlert>> DetectSuspiciousActivityAsync(string tenantId)
        {
            var alerts = new List<SecurityAlert>();
            var now = DateTime.UtcNow;
            var lookbackPeriod = now.AddHours(-24);

            try
            {
                var filter = new AuditFilter
                {
                    TenantId = tenantId,
                    StartDate = lookbackPeriod,
                    EndDate = now,
                    PageNumber = 1,
                    PageSize = 10000
                };

                var recentLogs = await _auditRepository.QueryAsync(filter);

                // 1. Multiple failed login attempts
                alerts.AddRange(DetectFailedLoginPatterns(recentLogs));

                // 2. Bulk data exports
                alerts.AddRange(DetectBulkExports(recentLogs));

                // 3. Access from unusual IP addresses
                alerts.AddRange(DetectUnusualIpAccess(recentLogs));

                // 4. Access outside normal working hours
                alerts.AddRange(DetectAfterHoursAccess(recentLogs));

                // 5. Multiple clinic switching
                alerts.AddRange(DetectExcessiveClinicSwitching(recentLogs));

                // 6. Unauthorized access attempts
                alerts.AddRange(DetectUnauthorizedAccessAttempts(recentLogs));

                // 7. Mass data modifications
                alerts.AddRange(DetectMassDataModifications(recentLogs));

                _logger.LogInformation($"Detected {alerts.Count} suspicious activities for tenant {tenantId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error detecting suspicious activity for tenant {tenantId}");
            }

            return alerts;
        }

        public async Task<List<SecurityAlert>> GetActiveAlertsAsync(string tenantId)
        {
            return await DetectSuspiciousActivityAsync(tenantId);
        }

        private List<SecurityAlert> DetectFailedLoginPatterns(List<AuditLog> logs)
        {
            var alerts = new List<SecurityAlert>();
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-FailedLoginsWindow);

            var failedLogins = logs
                .Where(l => l.Action == AuditAction.LOGIN_FAILED && l.Timestamp >= windowStart)
                .GroupBy(l => new { l.UserId, l.IpAddress })
                .Where(g => g.Count() >= FailedLoginsThreshold)
                .ToList();

            foreach (var group in failedLogins)
            {
                alerts.Add(new SecurityAlert(
                    AlertType: "FailedLoginAttempts",
                    Severity: SecurityAlertSeverity.High,
                    UserId: group.Key.UserId,
                    IpAddress: group.Key.IpAddress,
                    Description: $"{group.Count()} failed login attempts in {FailedLoginsWindow} minutes",
                    DetectedAt: now,
                    EventCount: group.Count()
                ));
            }

            return alerts;
        }

        private List<SecurityAlert> DetectBulkExports(List<AuditLog> logs)
        {
            var alerts = new List<SecurityAlert>();
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-BulkExportTimeWindow);

            var bulkExports = logs
                .Where(l => (l.Action == AuditAction.EXPORT || l.Action == AuditAction.DOWNLOAD) 
                            && l.Timestamp >= windowStart)
                .GroupBy(l => l.UserId)
                .Where(g => g.Count() >= BulkExportRecordsThreshold)
                .ToList();

            foreach (var group in bulkExports)
            {
                var user = logs.First(l => l.UserId == group.Key);
                alerts.Add(new SecurityAlert(
                    AlertType: "BulkDataExport",
                    Severity: SecurityAlertSeverity.Critical,
                    UserId: group.Key,
                    IpAddress: user.IpAddress,
                    Description: $"User exported {group.Count()} records in {BulkExportTimeWindow} minutes",
                    DetectedAt: now,
                    EventCount: group.Count()
                ));
            }

            return alerts;
        }

        private List<SecurityAlert> DetectUnusualIpAccess(List<AuditLog> logs)
        {
            var alerts = new List<SecurityAlert>();
            var now = DateTime.UtcNow;

            var userIpPatterns = logs
                .GroupBy(l => l.UserId)
                .Where(g => g.Select(l => l.IpAddress).Distinct().Count() > 5) // More than 5 different IPs in 24h
                .ToList();

            foreach (var group in userIpPatterns)
            {
                var distinctIps = group.Select(l => l.IpAddress).Distinct().Count();
                var user = group.First();
                
                alerts.Add(new SecurityAlert(
                    AlertType: "UnusualIpAccess",
                    Severity: SecurityAlertSeverity.Medium,
                    UserId: group.Key,
                    IpAddress: string.Join(", ", group.Select(l => l.IpAddress).Distinct().Take(3)),
                    Description: $"User accessed system from {distinctIps} different IP addresses in 24 hours",
                    DetectedAt: now,
                    EventCount: distinctIps
                ));
            }

            return alerts;
        }

        private List<SecurityAlert> DetectAfterHoursAccess(List<AuditLog> logs)
        {
            var alerts = new List<SecurityAlert>();
            var now = DateTime.UtcNow;

            // Define business hours: 6 AM to 10 PM
            var afterHoursAccess = logs
                .Where(l => l.Timestamp.Hour < 6 || l.Timestamp.Hour >= 22)
                .Where(l => l.Action != AuditAction.LOGIN && l.Action != AuditAction.LOGOUT)
                .GroupBy(l => l.UserId)
                .Where(g => g.Count() > 10) // More than 10 actions outside business hours
                .ToList();

            foreach (var group in afterHoursAccess)
            {
                var user = group.First();
                alerts.Add(new SecurityAlert(
                    AlertType: "AfterHoursAccess",
                    Severity: SecurityAlertSeverity.Low,
                    UserId: group.Key,
                    IpAddress: user.IpAddress,
                    Description: $"User performed {group.Count()} actions outside business hours (10 PM - 6 AM)",
                    DetectedAt: now,
                    EventCount: group.Count()
                ));
            }

            return alerts;
        }

        private List<SecurityAlert> DetectExcessiveClinicSwitching(List<AuditLog> logs)
        {
            var alerts = new List<SecurityAlert>();
            var now = DateTime.UtcNow;

            // This would require tracking clinic changes if available in logs
            // For now, placeholder implementation
            
            return alerts;
        }

        private List<SecurityAlert> DetectUnauthorizedAccessAttempts(List<AuditLog> logs)
        {
            var alerts = new List<SecurityAlert>();
            var now = DateTime.UtcNow;

            var unauthorizedAccess = logs
                .Where(l => l.Action == AuditAction.ACCESS_DENIED)
                .GroupBy(l => l.UserId)
                .Where(g => g.Count() > 3) // More than 3 denied access attempts
                .ToList();

            foreach (var group in unauthorizedAccess)
            {
                var user = group.First();
                alerts.Add(new SecurityAlert(
                    AlertType: "UnauthorizedAccessAttempts",
                    Severity: SecurityAlertSeverity.High,
                    UserId: group.Key,
                    IpAddress: user.IpAddress,
                    Description: $"User attempted {group.Count()} unauthorized accesses",
                    DetectedAt: now,
                    EventCount: group.Count()
                ));
            }

            return alerts;
        }

        private List<SecurityAlert> DetectMassDataModifications(List<AuditLog> logs)
        {
            var alerts = new List<SecurityAlert>();
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-5);

            var massModifications = logs
                .Where(l => (l.Action == AuditAction.UPDATE || l.Action == AuditAction.DELETE) 
                            && l.Timestamp >= windowStart)
                .GroupBy(l => l.UserId)
                .Where(g => g.Count() >= 50) // More than 50 modifications in 5 minutes
                .ToList();

            foreach (var group in massModifications)
            {
                var user = group.First();
                alerts.Add(new SecurityAlert(
                    AlertType: "MassDataModification",
                    Severity: SecurityAlertSeverity.Critical,
                    UserId: group.Key,
                    IpAddress: user.IpAddress,
                    Description: $"User performed {group.Count()} data modifications in 5 minutes",
                    DetectedAt: now,
                    EventCount: group.Count()
                ));
            }

            return alerts;
        }
    }

    public record SecurityAlert(
        string AlertType,
        SecurityAlertSeverity Severity,
        string UserId,
        string IpAddress,
        string Description,
        DateTime DetectedAt,
        int EventCount
    );

    public enum SecurityAlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
}
