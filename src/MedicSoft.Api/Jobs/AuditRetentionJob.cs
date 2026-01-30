using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.Repository.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MedicSoft.Api.Jobs
{
    /// <summary>
    /// Background job para aplicar política de retenção de logs de auditoria
    /// Executa diariamente às 2:00 AM UTC
    /// LGPD: Mantém logs por 7 anos (2555 dias) conforme requisitos médicos
    /// </summary>
    public class AuditRetentionJob
    {
        private readonly IAuditService _auditService;
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<AuditRetentionJob> _logger;
        private const int DefaultRetentionDays = 2555; // 7 years

        public AuditRetentionJob(
            IAuditService auditService,
            MedicSoftDbContext context,
            ILogger<AuditRetentionJob> logger)
        {
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Configura o job recorrente no Hangfire
        /// </summary>
        public static void Schedule()
        {
            // Executa diariamente às 2:00 AM UTC
            RecurringJob.AddOrUpdate<AuditRetentionJob>(
                "audit-retention-policy",
                job => job.ExecuteAsync(),
                "0 2 * * *", // Cron: Every day at 2:00 AM
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 900 })]
        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Starting audit retention policy job");

            try
            {
                var tenants = await _context.Clinics
                    .Select(c => c.TenantId)
                    .Distinct()
                    .ToListAsync();

                var totalDeleted = 0;
                var successfulTenants = 0;
                var failedTenants = 0;

                foreach (var tenantId in tenants)
                {
                    try
                    {
                        _logger.LogInformation($"Applying retention policy for tenant {tenantId}");
                        
                        var deleted = await _auditService.ApplyRetentionPolicyAsync(tenantId, DefaultRetentionDays);
                        totalDeleted += deleted;
                        successfulTenants++;

                        if (deleted > 0)
                        {
                            _logger.LogInformation($"Deleted {deleted} audit logs for tenant {tenantId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to apply retention policy for tenant {tenantId}");
                        failedTenants++;
                    }
                }

                _logger.LogInformation(
                    $"Audit retention policy job completed. " +
                    $"Total deleted: {totalDeleted}, " +
                    $"Successful tenants: {successfulTenants}, " +
                    $"Failed tenants: {failedTenants}");

                // If there were failures, log a warning but don't fail the job
                if (failedTenants > 0)
                {
                    _logger.LogWarning($"Retention policy failed for {failedTenants} tenant(s)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in audit retention policy job");
                throw; // Re-throw to trigger Hangfire retry
            }
        }

        /// <summary>
        /// Execução manual do job para um tenant específico
        /// </summary>
        public async Task<int> ExecuteForTenantAsync(string tenantId, int? retentionDays = null)
        {
            try
            {
                var days = retentionDays ?? DefaultRetentionDays;
                _logger.LogInformation($"Manual execution of retention policy for tenant {tenantId} with {days} days retention");
                
                var deleted = await _auditService.ApplyRetentionPolicyAsync(tenantId, days);
                
                _logger.LogInformation($"Manual retention policy execution completed. Deleted {deleted} logs for tenant {tenantId}");
                
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to execute manual retention policy for tenant {tenantId}");
                throw;
            }
        }
    }
}
