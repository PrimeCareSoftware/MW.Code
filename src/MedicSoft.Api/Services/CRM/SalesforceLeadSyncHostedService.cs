using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Api.Services.CRM
{
    /// <summary>
    /// Background service that automatically creates leads from abandoned registration funnels
    /// and syncs them to Salesforce
    /// </summary>
    public class SalesforceLeadSyncHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SalesforceLeadSyncHostedService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);

        public SalesforceLeadSyncHostedService(
            IServiceProvider serviceProvider,
            ILogger<SalesforceLeadSyncHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Salesforce Lead Sync Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAbandonedFunnelsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing abandoned funnels");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Salesforce Lead Sync Service stopped");
        }

        private async Task ProcessAbandonedFunnelsAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var funnelRepository = scope.ServiceProvider.GetRequiredService<IRepository<SalesFunnelMetric>>();
            var salesforceService = scope.ServiceProvider.GetRequiredService<ISalesforceLeadService>();

            try
            {
                // Find abandoned sessions (last activity > 24 hours ago, not converted)
                var abandonedSessions = await funnelRepository
                    .FindAsync(m => 
                        !m.IsConverted && 
                        m.Action == "abandoned" &&
                        m.CreatedAt < DateTime.UtcNow.AddHours(-24));

                var sessionIds = abandonedSessions
                    .Select(m => m.SessionId)
                    .Distinct()
                    .ToList();

                _logger.LogInformation("Found {Count} abandoned sessions to process", sessionIds.Count);

                foreach (var sessionId in sessionIds)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    try
                    {
                        // Create lead from funnel (if not already exists)
                        var lead = await salesforceService.CreateLeadFromFunnelAsync(sessionId);
                        
                        // Try to sync to Salesforce if not already synced
                        if (!lead.IsSyncedToSalesforce)
                        {
                            await salesforceService.SyncLeadToSalesforceAsync(lead.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to process abandoned session {SessionId}", sessionId);
                    }
                }

                _logger.LogInformation("Completed processing abandoned sessions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessAbandonedFunnelsAsync");
                throw;
            }
        }
    }
}
