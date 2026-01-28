using Hangfire;
using MedicSoft.Application.Services.CRM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Api.Jobs.CRM
{
    /// <summary>
    /// Background job for processing pending webhook deliveries
    /// </summary>
    public class WebhookDeliveryJob
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<WebhookDeliveryJob> _logger;

        public WebhookDeliveryJob(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<WebhookDeliveryJob> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        /// <summary>
        /// Process pending and retry-eligible webhook deliveries
        /// Runs every minute to ensure timely delivery
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task ProcessDeliveriesAsync()
        {
            _logger.LogInformation("Starting webhook delivery processing job");

            using var scope = _serviceScopeFactory.CreateScope();
            var webhookService = scope.ServiceProvider.GetRequiredService<IWebhookService>();

            try
            {
                await webhookService.ProcessPendingDeliveriesAsync();
                _logger.LogInformation("Webhook delivery processing job completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook deliveries");
                throw;
            }
        }

        /// <summary>
        /// Configure recurring job schedule
        /// </summary>
        public static void ConfigureRecurringJob()
        {
            // Run every minute to process pending deliveries
            RecurringJob.AddOrUpdate<WebhookDeliveryJob>(
                "webhook-delivery-processor",
                job => job.ProcessDeliveriesAsync(),
                Cron.MinuteInterval(1));
        }
    }
}
