using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Api.Hubs;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Interfaces;
using MedicSoft.Domain.Enums;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Jobs
{
    /// <summary>
    /// Background jobs para processar alertas automáticos do sistema
    /// </summary>
    public class AlertProcessingJob
    {
        private readonly MedicSoftDbContext _context;
        private readonly IAlertService _alertService;
        private readonly IHubContext<AlertHub> _alertHub;
        private readonly ILogger<AlertProcessingJob> _logger;

        public AlertProcessingJob(
            MedicSoftDbContext context,
            IAlertService alertService,
            IHubContext<AlertHub> alertHub,
            ILogger<AlertProcessingJob> logger)
        {
            _context = context;
            _alertService = alertService;
            _alertHub = alertHub;
            _logger = logger;
        }

        /// <summary>
        /// Marcar alertas expirados (executa a cada hora)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task MarkExpiredAlertsAsync()
        {
            try
            {
                _logger.LogInformation("Marcando alertas expirados...");

                // Obter todos os tenants únicos
                var tenants = await _context.Alerts
                    .Where(a => a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged)
                    .Where(a => a.ExpiresAt.HasValue && a.ExpiresAt.Value <= DateTime.UtcNow)
                    .Select(a => a.TenantId)
                    .Distinct()
                    .ToListAsync();

                int totalExpired = 0;
                foreach (var tenantId in tenants)
                {
                    await _alertService.MarkExpiredAlertsAsync(tenantId);
                    totalExpired++;
                }

                _logger.LogInformation($"Alertas expirados marcados em {totalExpired} tenants");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao marcar alertas expirados");
                throw;
            }
        }

        /// <summary>
        /// Limpar alertas antigos (executa diariamente)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CleanupOldAlertsAsync()
        {
            try
            {
                _logger.LogInformation("Limpando alertas antigos...");

                // Obter todos os tenants únicos
                var tenants = await _context.Alerts
                    .Select(a => a.TenantId)
                    .Distinct()
                    .ToListAsync();

                foreach (var tenantId in tenants)
                {
                    // Deletar alertas resolvidos/dispensados com mais de 90 dias
                    await _alertService.DeleteOldAlertsAsync(90, tenantId);
                }

                _logger.LogInformation($"Alertas antigos removidos de {tenants.Count} tenants");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao limpar alertas antigos");
                throw;
            }
        }

        /// <summary>
        /// Verificar consultas atrasadas e gerar alertas (executa a cada 15 minutos)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CheckOverdueAppointmentsAsync()
        {
            try
            {
                _logger.LogInformation("Verificando consultas atrasadas...");

                var now = DateTime.UtcNow;
                var today = now.Date;
                var tomorrow = today.AddDays(1);
                var threshold = now.AddMinutes(-15); // 15 minutos de tolerância

                var overdueAppointments = await _context.Appointments
                    .Where(a => a.Status == Domain.Entities.AppointmentStatus.Scheduled)
                    .Where(a => a.AppointmentDate >= today && a.AppointmentDate < tomorrow) // Date range for today
                    .Where(a => a.AppointmentDate < threshold)
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .ToListAsync();

                foreach (var appointment in overdueAppointments)
                {
                    // Verificar se já existe alerta para esse agendamento
                    var existingAlert = await _context.Alerts
                        .AnyAsync(a => a.Category == AlertCategory.AppointmentOverdue &&
                                     a.RelatedEntityType == "Appointment" &&
                                     a.RelatedEntityId == appointment.Id &&
                                     a.Status == AlertStatus.Active);

                    if (!existingAlert)
                    {
                        var alert = await _alertService.CreateAlertAsync(new CreateAlertDto
                        {
                            Category = AlertCategory.AppointmentOverdue,
                            Priority = AlertPriority.High,
                            Title = "Consulta Atrasada",
                            Message = $"Consulta com {appointment.Patient?.Name} está atrasada. Horário: {appointment.AppointmentDate:HH:mm}",
                            ActionUrl = $"/appointments/{appointment.Id}",
                            SuggestedAction = AlertAction.ViewDetails,
                            ActionLabel = "Ver Consulta",
                            RecipientType = AlertRecipientType.User,
                            UserId = appointment.DoctorId,
                            RelatedEntityType = "Appointment",
                            RelatedEntityId = appointment.Id,
                            ExpiresAt = now.AddHours(24)
                        }, appointment.TenantId);

                        // Enviar via SignalR
                        await _alertHub.Clients.User(appointment.DoctorId.ToString())
                            .SendAsync("ReceiveAlert", alert);
                    }
                }

                _logger.LogInformation($"Verificadas {overdueAppointments.Count} consultas atrasadas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar consultas atrasadas");
                throw;
            }
        }

        /// <summary>
        /// Verificar pagamentos vencidos e gerar alertas (executa diariamente)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CheckOverduePaymentsAsync()
        {
            try
            {
                _logger.LogInformation("Verificando pagamentos vencidos...");

                var now = DateTime.UtcNow.Date;

                var overdueReceivables = await _context.AccountsReceivable
                    .Where(ar => ar.Status == "Pending") // Status from domain entity
                    .Where(ar => ar.DueDate < now)
                    .Include(ar => ar.Patient)
                    .ToListAsync();

                foreach (var receivable in overdueReceivables)
                {
                    // Verificar se já existe alerta recente (últimos 7 dias)
                    var recentAlert = await _context.Alerts
                        .AnyAsync(a => a.Category == AlertCategory.PaymentOverdue &&
                                     a.RelatedEntityType == "AccountsReceivable" &&
                                     a.RelatedEntityId == receivable.Id &&
                                     a.CreatedAt >= now.AddDays(-7));

                    if (!recentAlert)
                    {
                        var daysOverdue = (now - receivable.DueDate).Days;
                        var priority = daysOverdue > 30 ? AlertPriority.Critical : 
                                     daysOverdue > 15 ? AlertPriority.High : AlertPriority.Normal;

                        var alert = await _alertService.CreateAlertAsync(new CreateAlertDto
                        {
                            Category = AlertCategory.PaymentOverdue,
                            Priority = priority,
                            Title = "Pagamento Vencido",
                            Message = $"Pagamento de {receivable.Patient?.Name ?? "Cliente"} venceu há {daysOverdue} dias. Valor: R$ {receivable.Amount:N2}",
                            ActionUrl = $"/financial/receivables/{receivable.Id}",
                            SuggestedAction = AlertAction.Contact,
                            ActionLabel = "Contatar Paciente",
                            RecipientType = AlertRecipientType.Clinic,
                            ClinicId = receivable.ClinicId,
                            RelatedEntityType = "AccountsReceivable",
                            RelatedEntityId = receivable.Id
                        }, receivable.TenantId);

                        // Enviar via SignalR para toda a clínica
                        if (receivable.ClinicId.HasValue)
                        {
                            await _alertHub.Clients.Group($"clinic_{receivable.ClinicId}")
                                .SendAsync("ReceiveAlert", alert);
                        }
                    }
                }

                _logger.LogInformation($"Verificados {overdueReceivables.Count} pagamentos vencidos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar pagamentos vencidos");
                throw;
            }
        }

        /// <summary>
        /// Verificar estoque baixo e gerar alertas (executa diariamente)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CheckLowStockAsync()
        {
            try
            {
                _logger.LogInformation("Verificando estoque baixo...");

                var lowStockMaterials = await _context.Materials
                    .Where(m => m.IsActive)
                    .Where(m => m.CurrentStock <= m.MinimumStock)
                    .ToListAsync();

                foreach (var material in lowStockMaterials)
                {
                    // Verificar se já existe alerta ativo
                    var existingAlert = await _context.Alerts
                        .AnyAsync(a => (a.Category == AlertCategory.LowStock || a.Category == AlertCategory.OutOfStock) &&
                                     a.RelatedEntityType == "Material" &&
                                     a.RelatedEntityId == material.Id &&
                                     a.Status == AlertStatus.Active);

                    if (!existingAlert)
                    {
                        var isOutOfStock = material.CurrentStock == 0;
                        var alert = await _alertService.CreateAlertAsync(new CreateAlertDto
                        {
                            Category = isOutOfStock ? AlertCategory.OutOfStock : AlertCategory.LowStock,
                            Priority = isOutOfStock ? AlertPriority.Critical : AlertPriority.High,
                            Title = isOutOfStock ? "Material sem Estoque" : "Estoque Baixo",
                            Message = $"{material.Name}: {material.CurrentStock} unidades (mínimo: {material.MinimumStock})",
                            ActionUrl = $"/inventory/materials/{material.Id}",
                            SuggestedAction = AlertAction.Restock,
                            ActionLabel = "Reabastecer",
                            RecipientType = AlertRecipientType.Clinic,
                            ClinicId = material.ClinicId,
                            RelatedEntityType = "Material",
                            RelatedEntityId = material.Id
                        }, material.TenantId);

                        // Enviar via SignalR
                        if (material.ClinicId.HasValue)
                        {
                            await _alertHub.Clients.Group($"clinic_{material.ClinicId}")
                                .SendAsync("ReceiveAlert", alert);
                        }
                    }
                }

                _logger.LogInformation($"Verificados {lowStockMaterials.Count} materiais com estoque baixo");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar estoque baixo");
                throw;
            }
        }

        /// <summary>
        /// Verificar assinaturas expirando e gerar alertas (executa diariamente)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task CheckExpiringSubscriptionsAsync()
        {
            try
            {
                _logger.LogInformation("Verificando assinaturas expirando...");

                var now = DateTime.UtcNow;
                var warningThreshold = now.AddDays(7); // 7 dias antes de expirar

                var expiringSubscriptions = await _context.ClinicSubscriptions
                    .Where(s => s.Status == Domain.Entities.SubscriptionStatus.Active)
                    .Where(s => s.EndDate.HasValue && s.EndDate.Value <= warningThreshold && s.EndDate.Value > now)
                    .Include(s => s.Clinic)
                    .ToListAsync();

                foreach (var subscription in expiringSubscriptions)
                {
                    // Verificar se já existe alerta ativo
                    var existingAlert = await _context.Alerts
                        .AnyAsync(a => a.Category == AlertCategory.SubscriptionExpiring &&
                                     a.RelatedEntityType == "ClinicSubscription" &&
                                     a.RelatedEntityId == subscription.Id &&
                                     a.Status == AlertStatus.Active);

                    if (!existingAlert)
                    {
                        var daysUntilExpiry = (subscription.EndDate.Value - now).Days;
                        var alert = await _alertService.CreateAlertAsync(new CreateAlertDto
                        {
                            Category = AlertCategory.SubscriptionExpiring,
                            Priority = daysUntilExpiry <= 3 ? AlertPriority.Critical : AlertPriority.High,
                            Title = "Assinatura Expirando",
                            Message = $"A assinatura da clínica {subscription.Clinic?.Name} expira em {daysUntilExpiry} dias",
                            ActionUrl = $"/clinics/{subscription.ClinicId}/subscription",
                            SuggestedAction = AlertAction.TakeAction,
                            ActionLabel = "Renovar Assinatura",
                            RecipientType = AlertRecipientType.Clinic,
                            ClinicId = subscription.ClinicId,
                            RelatedEntityType = "ClinicSubscription",
                            RelatedEntityId = subscription.Id,
                            ExpiresAt = subscription.EndDate.Value
                        }, subscription.TenantId);

                        // Enviar via SignalR
                        if (subscription.ClinicId.HasValue)
                        {
                            await _alertHub.Clients.Group($"clinic_{subscription.ClinicId}")
                                .SendAsync("ReceiveAlert", alert);
                        }
                    }
                }

                _logger.LogInformation($"Verificadas {expiringSubscriptions.Count} assinaturas expirando");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar assinaturas expirando");
                throw;
            }
        }
    }
}
