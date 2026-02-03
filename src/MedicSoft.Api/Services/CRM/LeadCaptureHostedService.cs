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
    /// Background service that automatically captures abandoned registration leads
    /// </summary>
    public class LeadCaptureHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<LeadCaptureHostedService> _logger;
        private Timer? _timer;
        private const int CheckIntervalMinutes = 60; // Run every hour
        private const int AbandonmentThresholdHours = 24; // Consider abandoned after 24h

        public LeadCaptureHostedService(
            IServiceProvider serviceProvider,
            ILogger<LeadCaptureHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Lead Capture Hosted Service is starting");

            // Run immediately on startup, then every hour
            _timer = new Timer(
                DoWork,
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(CheckIntervalMinutes));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            try
            {
                _logger.LogInformation("Lead Capture Service: Starting lead capture check");

                using var scope = _serviceProvider.CreateScope();
                var funnelRepository = scope.ServiceProvider.GetRequiredService<IRepository<SalesFunnelMetric>>();
                var leadService = scope.ServiceProvider.GetRequiredService<ILeadManagementService>();

                // Find abandoned sessions (no conversion, older than 24h, reached at least step 2)
                var abandonmentThreshold = DateTime.UtcNow.AddHours(-AbandonmentThresholdHours);
                var abandonedSessions = await funnelRepository.FindAsync(m =>
                    m.CreatedAt < abandonmentThreshold &&
                    !m.Converted &&
                    m.Step >= 2); // At least reached step 2 (has some data)

                if (!abandonedSessions.Any())
                {
                    _logger.LogInformation("No abandoned sessions found");
                    return;
                }

                // Group by session ID
                var sessionGroups = abandonedSessions
                    .GroupBy(m => m.SessionId)
                    .ToList();

                _logger.LogInformation("Found {Count} abandoned sessions to process", sessionGroups.Count);

                int created = 0;
                int skipped = 0;

                foreach (var group in sessionGroups)
                {
                    try
                    {
                        var lead = await leadService.CreateLeadFromFunnelAsync(group.Key);
                        
                        if (lead != null)
                        {
                            created++;
                            _logger.LogInformation("Created lead {LeadId} from session {SessionId}", 
                                lead.Id, group.Key);
                        }
                        else
                        {
                            skipped++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating lead from session {SessionId}", group.Key);
                        skipped++;
                    }
                }

                _logger.LogInformation(
                    "Lead Capture Service: Completed. Created: {Created}, Skipped: {Skipped}",
                    created, skipped);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Lead Capture Service");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Lead Capture Hosted Service is stopping");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
