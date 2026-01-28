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
                    s.Status == "Active" &&
                    s.ExpiresAt <= now &&
                    s.ExpiresAt > oneHourAgo)
                .ToListAsync();

            _logger.LogInformation($"Found {expiredSubscriptions.Count} expired subscriptions");

            foreach (var subscription in expiredSubscriptions)
            {
                try
                {
                    await _eventPublisher.PublishAsync(new SubscriptionExpiredEvent
                    {
                        ClinicId = subscription.ClinicId,
                        SubscriptionId = subscription.Id,
                        ExpiredAt = subscription.ExpiresAt,
                        ClinicName = subscription.Clinic?.Name,
                        Email = subscription.Clinic?.Email
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error publishing event for clinic {subscription.ClinicId}");
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
                    s.Status == "Trial" &&
                    s.TrialEndsAt <= threeDaysFromNow &&
                    s.TrialEndsAt > twoDaysFromNow)
                .ToListAsync();

            _logger.LogInformation($"Found {expiringTrials.Count} expiring trials");

            foreach (var trial in expiringTrials)
            {
                try
                {
                    var daysRemaining = (trial.TrialEndsAt - now).Days;

                    await _eventPublisher.PublishAsync(new TrialExpiringEvent
                    {
                        ClinicId = trial.ClinicId,
                        SubscriptionId = trial.Id,
                        DaysRemaining = daysRemaining,
                        TrialEndsAt = trial.TrialEndsAt,
                        ClinicName = trial.Clinic?.Name,
                        Email = trial.Clinic?.Email
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error publishing event for clinic {trial.ClinicId}");
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
