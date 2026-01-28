using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Application.Services.SystemAdmin;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Jobs.SystemAdmin
{
    /// <summary>
    /// Background jobs for system admin notifications
    /// </summary>
    public class NotificationJobs
    {
        private readonly MedicSoftDbContext _context;
        private readonly ISystemNotificationService _notificationService;
        private readonly ILogger<NotificationJobs> _logger;

        public NotificationJobs(
            MedicSoftDbContext context,
            ISystemNotificationService notificationService,
            ILogger<NotificationJobs> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        /// <summary>
        /// Check for expired subscriptions (runs hourly)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CheckSubscriptionExpirationsAsync()
        {
            try
            {
                _logger.LogInformation("Checking for expired subscriptions...");

                var now = DateTime.UtcNow;
                var expiredSubscriptions = await _context.ClinicSubscriptions
                    .IgnoreQueryFilters()
                    .Where(s => s.Status == "Active" && s.EndDate <= now)
                    .Include(s => s.Clinic)
                    .ToListAsync();

                foreach (var subscription in expiredSubscriptions)
                {
                    await _notificationService.CreateNotificationAsync(new CreateSystemNotificationDto
                    {
                        Type = "critical",
                        Category = "subscription",
                        Title = "Assinatura Vencida",
                        Message = $"A assinatura da clínica {subscription.Clinic?.Name} venceu.",
                        ActionUrl = $"/clinics/{subscription.ClinicId}",
                        ActionLabel = "Ver Clínica"
                    });
                }

                _logger.LogInformation($"Found {expiredSubscriptions.Count} expired subscriptions");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking subscription expirations");
                throw;
            }
        }

        /// <summary>
        /// Check for trials expiring soon (runs daily)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CheckTrialExpiringAsync()
        {
            try
            {
                _logger.LogInformation("Checking for expiring trials...");

                var now = DateTime.UtcNow;
                var threeDaysFromNow = now.AddDays(3);

                var expiringTrials = await _context.ClinicSubscriptions
                    .IgnoreQueryFilters()
                    .Where(s => s.Status == "Trial" && 
                               s.TrialEndsAt > now && 
                               s.TrialEndsAt <= threeDaysFromNow)
                    .Include(s => s.Clinic)
                    .ToListAsync();

                foreach (var trial in expiringTrials)
                {
                    var daysRemaining = (trial.TrialEndsAt!.Value - now).Days;
                    await _notificationService.CreateNotificationAsync(new CreateSystemNotificationDto
                    {
                        Type = "warning",
                        Category = "subscription",
                        Title = "Trial Expirando",
                        Message = $"O trial da clínica {trial.Clinic?.Name} expira em {daysRemaining} dia(s).",
                        ActionUrl = $"/clinics/{trial.ClinicId}",
                        ActionLabel = "Contatar Cliente"
                    });
                }

                _logger.LogInformation($"Found {expiringTrials.Count} expiring trials");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking expiring trials");
                throw;
            }
        }

        /// <summary>
        /// Check for inactive clinics (runs daily)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CheckInactiveClinicsAsync()
        {
            try
            {
                _logger.LogInformation("Checking for inactive clinics...");

                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

                var inactiveClinics = await _context.Clinics
                    .IgnoreQueryFilters()
                    .Where(c => c.IsActive && c.UpdatedAt < thirtyDaysAgo)
                    .ToListAsync();

                foreach (var clinic in inactiveClinics)
                {
                    var daysSinceActivity = (DateTime.UtcNow - clinic.UpdatedAt).Days;
                    await _notificationService.CreateNotificationAsync(new CreateSystemNotificationDto
                    {
                        Type = "warning",
                        Category = "customer",
                        Title = "Clínica Inativa",
                        Message = $"Clínica {clinic.Name} sem atividade há {daysSinceActivity} dias.",
                        ActionUrl = $"/clinics/{clinic.Id}",
                        ActionLabel = "Ver Clínica"
                    });
                }

                _logger.LogInformation($"Found {inactiveClinics.Count} inactive clinics");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking inactive clinics");
                throw;
            }
        }

        /// <summary>
        /// Check for high-priority tickets without response (runs every 6 hours)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CheckUnrespondedTicketsAsync()
        {
            try
            {
                _logger.LogInformation("Checking for unresponded high-priority tickets...");

                var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);

                var unrespondedTickets = await _context.Tickets
                    .IgnoreQueryFilters()
                    .Where(t => t.Status == Domain.Entities.TicketStatus.Open && 
                               t.Priority == Domain.Entities.TicketPriority.High &&
                               t.CreatedAt < twentyFourHoursAgo)
                    .Include(t => t.Clinic)
                    .ToListAsync();

                foreach (var ticket in unrespondedTickets)
                {
                    var hoursSinceCreation = (DateTime.UtcNow - ticket.CreatedAt).TotalHours;
                    await _notificationService.CreateNotificationAsync(new CreateSystemNotificationDto
                    {
                        Type = "warning",
                        Category = "ticket",
                        Title = "Ticket Sem Resposta",
                        Message = $"Ticket #{ticket.Id} ({ticket.Title}) sem resposta há {(int)hoursSinceCreation} horas.",
                        ActionUrl = $"/tickets/{ticket.Id}",
                        ActionLabel = "Responder Ticket"
                    });
                }

                _logger.LogInformation($"Found {unrespondedTickets.Count} unresponded tickets");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking unresponded tickets");
                throw;
            }
        }
    }
}
