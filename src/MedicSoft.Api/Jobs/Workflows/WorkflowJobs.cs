using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services.Workflows;
using MedicSoft.Domain.Events;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Jobs.Workflows
{
    public class WorkflowJobs
    {
        private readonly MedicSoftDbContext _context;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<WorkflowJobs> _logger;

        public WorkflowJobs(
            MedicSoftDbContext context,
            IEventPublisher eventPublisher,
            ILogger<WorkflowJobs> logger)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task CheckSubscriptionExpirationsAsync()
        {
            _logger.LogInformation("Checking for expired subscriptions");

            var now = DateTime.UtcNow;
            var oneHourAgo = now.AddHours(-1);

            var expiredSubscriptions = await _context.ClinicSubscriptions
                .Include(s => s.Clinic)
                .Where(s =>
                    s.Status == SubscriptionStatus.Active &&
                    s.NextPaymentDate <= now &&
                    s.NextPaymentDate > oneHourAgo)
                .ToListAsync();

            _logger.LogInformation("Found {Count} expired subscriptions", expiredSubscriptions.Count);

            foreach (var subscription in expiredSubscriptions)
            {
                try
                {
                    await _eventPublisher.PublishAsync(new SubscriptionExpiredEvent
                    {
                        ClinicId = subscription.ClinicId,
                        SubscriptionId = subscription.Id,
                        ExpiredAt = subscription.NextPaymentDate ?? now,
                        ClinicName = subscription.Clinic?.Name ?? "Unknown",
                        Email = subscription.Clinic?.Email ?? "unknown@example.com"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing event for clinic {ClinicId}", subscription.ClinicId);
                }
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task CheckTrialExpiringAsync()
        {
            _logger.LogInformation("Checking for expiring trials");

            var now = DateTime.UtcNow;
            var threeDaysFromNow = now.AddDays(3);
            var twoDaysFromNow = now.AddDays(2);

            var expiringTrials = await _context.ClinicSubscriptions
                .Include(s => s.Clinic)
                .Where(s =>
                    s.Status == SubscriptionStatus.Trial &&
                    s.TrialEndDate <= threeDaysFromNow &&
                    s.TrialEndDate > twoDaysFromNow)
                .ToListAsync();

            _logger.LogInformation("Found {Count} expiring trials", expiringTrials.Count);

            foreach (var trial in expiringTrials)
            {
                try
                {
                    var daysRemaining = trial.TrialEndDate.HasValue 
                        ? (trial.TrialEndDate.Value - now).Days 
                        : 0;

                    await _eventPublisher.PublishAsync(new TrialExpiringEvent
                    {
                        ClinicId = trial.ClinicId,
                        SubscriptionId = trial.Id,
                        DaysRemaining = daysRemaining,
                        TrialEndsAt = trial.TrialEndDate ?? now,
                        ClinicName = trial.Clinic?.Name ?? "Unknown",
                        Email = trial.Clinic?.Email ?? "unknown@example.com"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing event for clinic {ClinicId}", trial.ClinicId);
                }
            }
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task CheckInactiveClientsAsync()
        {
            _logger.LogInformation("Checking for inactive clients");

            var now = DateTime.UtcNow;
            var thirtyDaysAgo = now.AddDays(-30);
            var thirtyOneDaysAgo = now.AddDays(-31);

            var inactiveClinics = await _context.Clinics
                .Where(c =>
                    c.IsActive &&
                    c.LastActivityAt < thirtyDaysAgo &&
                    c.LastActivityAt >= thirtyOneDaysAgo)
                .ToListAsync();

            _logger.LogInformation($"Found {inactiveClinics.Count} inactive clinics");

            foreach (var clinic in inactiveClinics)
            {
                try
                {
                    var daysSinceLastActivity = (now - clinic.LastActivityAt).Days;

                    await _eventPublisher.PublishAsync(new InactivityDetectedEvent
                    {
                        ClinicId = clinic.Id,
                        DaysSinceLastActivity = daysSinceLastActivity,
                        LastActivityAt = clinic.LastActivityAt,
                        ClinicName = clinic.Name,
                        Email = clinic.Email
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error publishing event for clinic {clinic.Id}");
                }
            }
        }
    }
}
