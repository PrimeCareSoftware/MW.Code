using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.Application.Services.Reports;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.SystemAdmin
{
    public interface ISmartActionService
    {
        Task<string> ImpersonateClinicAsync(Guid clinicId, Guid adminUserId);
        Task GrantCreditAsync(Guid clinicId, int days, string reason, Guid adminUserId);
        Task ApplyDiscountAsync(Guid clinicId, decimal percentage, int months, Guid adminUserId);
        Task SuspendTemporarilyAsync(Guid clinicId, DateTime? reactivationDate, string reason, Guid adminUserId);
        Task<byte[]> ExportClinicDataAsync(Guid clinicId, Guid adminUserId);
        Task MigratePlanAsync(Guid clinicId, Guid newPlanId, bool proRata, Guid adminUserId);
        Task SendCustomEmailAsync(Guid clinicId, string subject, string body, Guid adminUserId);
    }

    public class SmartActionService : ISmartActionService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<SmartActionService> _logger;
        private readonly IEmailService _emailService;
        private readonly IAuditService _auditService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SmartActionService(
            MedicSoftDbContext context,
            ILogger<SmartActionService> logger,
            IEmailService emailService,
            IAuditService auditService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _auditService = auditService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> ImpersonateClinicAsync(Guid clinicId, Guid adminUserId)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == clinicId);
            
            var admin = await _context.Users.FindAsync(adminUserId);

            if (clinic == null)
                throw new Exception("Clinic not found");
            
            if (admin == null)
                throw new Exception("Admin user not found");

            // Criar token de impersonation (simplified - in real scenario use JWT service)
            var token = GenerateImpersonationToken(clinic, admin);

            // Registrar no audit log
            await _auditService.LogAsync(new CreateAuditLogDto(
                UserId: adminUserId.ToString(),
                UserName: admin.FullName,
                UserEmail: admin.Email,
                Action: Domain.Enums.AuditAction.SYSTEM_ADMIN_ACTION,
                ActionDescription: $"Admin {admin.FullName} impersonated clinic {clinic.Name}",
                EntityType: "Clinic",
                EntityId: clinicId.ToString(),
                EntityDisplayName: clinic.Name,
                IpAddress: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                UserAgent: _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown",
                RequestPath: _httpContextAccessor.HttpContext?.Request.Path.ToString() ?? "/",
                HttpMethod: _httpContextAccessor.HttpContext?.Request.Method ?? "POST",
                Result: Domain.Enums.OperationResult.SUCCESS,
                DataCategory: Domain.Enums.DataCategory.OPERATIONAL,
                Purpose: Domain.ValueObjects.LgpdPurpose.SYSTEM_ADMINISTRATION,
                Severity: Domain.Enums.AuditSeverity.HIGH,
                TenantId: clinic.TenantId
            ));

            _logger.LogWarning($"Admin {adminUserId} impersonating clinic {clinicId}");

            return token;
        }

        public async Task GrantCreditAsync(Guid clinicId, int days, string reason, Guid adminUserId)
        {
            var clinic = await _context.Clinics
                .FirstOrDefaultAsync(c => c.Id == clinicId);

            if (clinic == null)
                throw new Exception("Clinic not found");
            
            var subscription = await _context.ClinicSubscriptions
                .Where(s => s.ClinicId == clinicId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription == null)
                throw new Exception("Clinic subscription not found");

            // Estender assinatura
            if (subscription.EndDate.HasValue)
            {
                subscription.EndDate = subscription.EndDate.Value.AddDays(days);
            }

            // Criar crédito
            var credit = new SubscriptionCredit
            {
                SubscriptionId = subscription.Id,
                Days = days,
                Reason = reason,
                GrantedAt = DateTime.UtcNow,
                GrantedByUserId = adminUserId
            };

            _context.SubscriptionCredits.Add(credit);
            await _context.SaveChangesAsync();

            // Notificar cliente
            await _emailService.SendEmailAsync(
                to: clinic.Email,
                subject: "Você ganhou dias grátis!",
                body: $"Olá {clinic.Name},\n\n" +
                      $"Concedemos {days} dias grátis em sua assinatura.\n" +
                      $"Motivo: {reason}\n\n" +
                      $"Sua nova data de vencimento: {subscription.EndDate:dd/MM/yyyy}\n\n" +
                      $"Aproveite!\n\n" +
                      $"Equipe PrimeCare"
            );

            var admin = await _context.Users.FindAsync(adminUserId);
            // Audit log
            await _auditService.LogAsync(new CreateAuditLogDto(
                UserId: adminUserId.ToString(),
                UserName: admin?.FullName ?? "System",
                UserEmail: admin?.Email ?? "system@system",
                Action: Domain.Enums.AuditAction.SYSTEM_ADMIN_ACTION,
                ActionDescription: $"Granted {days} days. Reason: {reason}",
                EntityType: "Clinic",
                EntityId: clinicId.ToString(),
                EntityDisplayName: clinic.Name,
                IpAddress: _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                UserAgent: _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown",
                RequestPath: _httpContextAccessor.HttpContext?.Request.Path.ToString() ?? "/",
                HttpMethod: _httpContextAccessor.HttpContext?.Request.Method ?? "POST",
                Result: Domain.Enums.OperationResult.SUCCESS,
                DataCategory: Domain.Enums.DataCategory.OPERATIONAL,
                Purpose: Domain.ValueObjects.LgpdPurpose.SYSTEM_ADMINISTRATION,
                Severity: Domain.Enums.AuditSeverity.MEDIUM,
                TenantId: clinic.TenantId
            ));

            _logger.LogInformation($"Admin {adminUserId} granted {days} days credit to clinic {clinicId}");
        }

        public async Task ApplyDiscountAsync(int clinicId, decimal percentage, int months, int adminUserId)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Subscription)
                .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(c => c.Id == clinicId);

            if (clinic?.Subscription == null)
                throw new Exception("Clinic or subscription not found");

            // Note: Discount functionality would require additional fields in ClinicSubscription
            // For now, we log the intent and send notification
            var discountCode = $"ADMIN-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

            clinic.Subscription.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Notificar cliente
            await _emailService.SendEmailAsync(
                to: clinic.Email,
                subject: "Desconto especial aplicado!",
                body: $"Olá {clinic.Name},\n\n" +
                      $"Aplicamos um desconto de {percentage}% em sua assinatura por {months} meses.\n" +
                      $"Código: {discountCode}\n\n" +
                      $"Aproveite!\n\n" +
                      $"Equipe PrimeCare"
            );

            // Audit log
            await _auditService.LogAsync(new AuditLogDto
            {
                Action = "ApplyDiscount",
                EntityType = "Clinic",
                EntityId = clinicId.ToString(),
                UserId = adminUserId,
                Details = $"Applied {percentage}% discount for {months} months. Code: {discountCode}",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            });

            _logger.LogInformation($"Admin {adminUserId} applied {percentage}% discount to clinic {clinicId}");
        }

        public async Task SuspendTemporarilyAsync(int clinicId, DateTime? reactivationDate, string reason, int adminUserId)
        {
            var clinic = await _context.Clinics.FindAsync(clinicId);

            if (clinic == null)
                throw new Exception("Clinic not found");

            // Note: Suspension fields would require additional fields in Clinic entity
            // For now, we use IsActive and log the details
            clinic.IsActive = false;
            clinic.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Notificar cliente
            var reactivationInfo = reactivationDate.HasValue 
                ? $"Reativação programada para: {reactivationDate:dd/MM/yyyy}" 
                : "Entre em contato conosco para reativação";

            await _emailService.SendEmailAsync(
                to: clinic.Email,
                subject: "Sua conta foi temporariamente suspensa",
                body: $"Olá {clinic.Name},\n\n" +
                      $"Sua conta foi temporariamente suspensa.\n" +
                      $"Motivo: {reason}\n\n" +
                      $"{reactivationInfo}\n\n" +
                      $"Se tiver dúvidas, entre em contato com nosso suporte.\n\n" +
                      $"Equipe PrimeCare"
            );

            // Audit log
            await _auditService.LogAsync(new AuditLogDto
            {
                Action = "SuspendClinic",
                EntityType = "Clinic",
                EntityId = clinicId.ToString(),
                UserId = adminUserId,
                Details = $"Suspended clinic. Reason: {reason}. Reactivation: {reactivationDate?.ToString("yyyy-MM-dd") ?? "Manual"}",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            });

            _logger.LogWarning($"Admin {adminUserId} suspended clinic {clinicId}");
        }

        public async Task<byte[]> ExportClinicDataAsync(int clinicId, int adminUserId)
        {
            // LGPD compliance - direito aos dados
            var clinic = await _context.Clinics
                .Include(c => c.UserClinicLinks)
                    .ThenInclude(ucl => ucl.User)
                .Include(c => c.PatientClinicLinks)
                    .ThenInclude(pcl => pcl.Patient)
                .Include(c => c.Subscription)
                    .ThenInclude(s => s.Plan)
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == clinicId);

            if (clinic == null)
                throw new Exception("Clinic not found");

            var appointmentCount = await _context.Appointments
                .Where(a => a.ClinicId == clinicId)
                .CountAsync();

            var data = new
            {
                clinic = new
                {
                    clinic.Name,
                    clinic.Email,
                    clinic.Phone,
                    clinic.Address,
                    clinic.City,
                    clinic.State,
                    clinic.ZipCode,
                    clinic.CreatedAt
                },
                users = clinic.UserClinicLinks.Select(ucl => new
                {
                    ucl.User.Name,
                    ucl.User.Email,
                    ucl.User.Role,
                    ucl.User.CreatedAt
                }),
                patientsCount = clinic.PatientClinicLinks.Count,
                appointmentsCount = appointmentCount,
                subscription = clinic.Subscription == null ? null : new
                {
                    Plan = clinic.Subscription.Plan?.Name,
                    clinic.Subscription.Status,
                    clinic.Subscription.Mrr,
                    clinic.Subscription.CreatedAt,
                    clinic.Subscription.ExpiresAt
                },
                exportedAt = DateTime.UtcNow,
                exportedBy = $"Admin User ID: {adminUserId}"
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // Audit log
            await _auditService.LogAsync(new AuditLogDto
            {
                Action = "ExportData",
                EntityType = "Clinic",
                EntityId = clinicId.ToString(),
                UserId = adminUserId,
                Details = "Exported all clinic data (LGPD compliance)",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            });

            _logger.LogInformation($"Admin {adminUserId} exported data for clinic {clinicId}");

            return Encoding.UTF8.GetBytes(json);
        }

        public async Task MigratePlanAsync(int clinicId, int newPlanId, bool proRata, int adminUserId)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Subscription)
                .ThenInclude(s => s.Plan)
                .FirstOrDefaultAsync(c => c.Id == clinicId);

            var newPlan = await _context.SubscriptionPlans.FindAsync(newPlanId);

            if (clinic?.Subscription == null)
                throw new Exception("Clinic or subscription not found");

            if (newPlan == null)
                throw new Exception("New plan not found");

            var oldPlan = clinic.Subscription.Plan;
            var oldMrr = clinic.Subscription.Mrr;

            // Update subscription
            clinic.Subscription.PlanId = newPlanId;
            clinic.Subscription.Mrr = newPlan.MonthlyPrice;
            clinic.Subscription.UpdatedAt = DateTime.UtcNow;

            if (proRata && oldPlan != null)
            {
                // Calculate pro-rata credit/charge
                var daysRemaining = (clinic.Subscription.ExpiresAt - DateTime.UtcNow).Days;
                var creditAmount = (oldMrr / 30) * daysRemaining;
                _logger.LogInformation($"Pro-rata credit calculated: {creditAmount:C} for {daysRemaining} days");
            }

            await _context.SaveChangesAsync();

            // Notificar cliente
            await _emailService.SendEmailAsync(
                to: clinic.Email,
                subject: "Seu plano foi atualizado!",
                body: $"Olá {clinic.Name},\n\n" +
                      $"Seu plano foi migrado de '{oldPlan?.Name}' para '{newPlan.Name}'.\n" +
                      $"Novo valor mensal: R$ {newPlan.MonthlyPrice:F2}\n\n" +
                      (proRata ? "Ajuste pro-rata será aplicado na próxima fatura.\n\n" : "") +
                      $"Aproveite os novos recursos!\n\n" +
                      $"Equipe PrimeCare"
            );

            // Audit log
            await _auditService.LogAsync(new AuditLogDto
            {
                Action = "MigratePlan",
                EntityType = "Clinic",
                EntityId = clinicId.ToString(),
                UserId = adminUserId,
                Details = $"Migrated from plan {oldPlan?.Name} to {newPlan.Name}. Pro-rata: {proRata}",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            });

            _logger.LogInformation($"Admin {adminUserId} migrated clinic {clinicId} to plan {newPlanId}");
        }

        public async Task SendCustomEmailAsync(int clinicId, string subject, string body, int adminUserId)
        {
            var clinic = await _context.Clinics.FindAsync(clinicId);

            if (clinic == null)
                throw new Exception("Clinic not found");

            await _emailService.SendEmailAsync(clinic.Email, subject, body);

            // Audit log
            await _auditService.LogAsync(new AuditLogDto
            {
                Action = "SendCustomEmail",
                EntityType = "Clinic",
                EntityId = clinicId.ToString(),
                UserId = adminUserId,
                Details = $"Sent custom email. Subject: {subject}",
                IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
            });

            _logger.LogInformation($"Admin {adminUserId} sent custom email to clinic {clinicId}");
        }

        private string GenerateImpersonationToken(Clinic clinic, User admin)
        {
            // Simplified token generation - in production use proper JWT service
            var payload = new
            {
                clinicId = clinic.Id,
                tenantId = clinic.TenantId,
                impersonated = true,
                impersonatorId = admin.Id,
                impersonatorName = admin.Name,
                expiresAt = DateTime.UtcNow.AddHours(2)
            };

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
        }
    }
}
