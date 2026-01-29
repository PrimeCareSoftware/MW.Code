using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services
{
    public class GdprService : IGdprService
    {
        private readonly MedicSoftDbContext _context;
        private readonly IAuditService _auditService;

        public GdprService(MedicSoftDbContext context, IAuditService auditService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        }

        public async Task<byte[]> ExportClinicDataAsync(Guid clinicId, string tenantId)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Users)
                .Include(c => c.Patients)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == clinicId && c.TenantId == tenantId);

            if (clinic == null)
                throw new InvalidOperationException("Clinic not found");

            var data = new
            {
                clinic.Id,
                clinic.Name,
                clinic.Email,
                clinic.Phone,
                clinic.Address,
                clinic.Cnpj,
                clinic.CreatedAt,
                Users = clinic.Users.Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.FullName,
                    u.Phone,
                    u.Role
                }),
                Patients = clinic.Patients.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Cpf,
                    p.Phone,
                    p.Email,
                    p.CreatedAt
                }),
                ExportedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            return Encoding.UTF8.GetBytes(json);
        }

        public async Task AnonymizeClinicAsync(Guid clinicId, string tenantId, string userId)
        {
            var clinic = await _context.Clinics
                .Include(c => c.Users)
                .Include(c => c.Patients)
                .FirstOrDefaultAsync(c => c.Id == clinicId && c.TenantId == tenantId);

            if (clinic == null)
                throw new InvalidOperationException("Clinic not found");

            // Store old data for audit
            var oldData = new
            {
                clinic.Name,
                clinic.Email,
                clinic.Phone,
                clinic.Cnpj
            };

            // IMPORTANT: This operation anonymizes the clinic AND all associated users and patients.
            // This is a significant operation with cascading effects.
            // Consider the following before calling:
            // - Users who work at multiple clinics will be anonymized
            // - Patients who are treated at multiple clinics will be anonymized
            // - This operation cannot be undone
            // Ensure appropriate authorization and confirmation before proceeding.

            // Anonymize clinic data
            clinic.UpdateBasicInfo(
                $"Clinic-{Guid.NewGuid()}",
                $"anonymized-{Guid.NewGuid()}@example.com",
                "***",
                "***"
            );

            // Anonymize users associated with clinic
            foreach (var user in clinic.Users)
            {
                user.UpdateProfile(
                    $"anonymized-{Guid.NewGuid()}@example.com",
                    $"User-{Guid.NewGuid()}",
                    "***"
                );
            }

            // Anonymize patients
            foreach (var patient in clinic.Patients)
            {
                patient.UpdatePersonalInfo(
                    $"Patient-{Guid.NewGuid()}",
                    "***",
                    null,
                    "***",
                    $"anonymized-{Guid.NewGuid()}@example.com"
                );
            }

            await _context.SaveChangesAsync();

            // Log the anonymization
            await _auditService.LogAsync(new DTOs.CreateAuditLogDto(
                UserId: userId,
                UserName: "System",
                UserEmail: "system@primecare.com",
                Action: AuditAction.DELETE,
                ActionDescription: "Dados da clínica anonimizados (LGPD)",
                EntityType: "Clinic",
                EntityId: clinicId.ToString(),
                EntityDisplayName: oldData.Name,
                IpAddress: "system",
                UserAgent: "system",
                RequestPath: "/api/gdpr/anonymize",
                HttpMethod: "POST",
                Result: Domain.Enums.OperationResult.SUCCESS,
                DataCategory: DataCategory.PERSONAL,
                Purpose: Domain.ValueObjects.LgpdPurpose.LEGAL_OBLIGATION,
                Severity: AuditSeverity.CRITICAL,
                TenantId: tenantId,
                OldValues: JsonSerializer.Serialize(oldData),
                NewValues: "*** ANONYMIZED ***"
            ));
        }

        public async Task<byte[]> ExportUserDataAsync(string userId, string tenantId)
        {
            var userGuid = Guid.Parse(userId);
            var user = await _context.Users
                .Include(u => u.ClinicLinks)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userGuid && u.TenantId == tenantId);

            if (user == null)
                throw new InvalidOperationException("User not found");

            // Get user's audit logs
            var auditLogs = await _auditService.GetUserActivityAsync(userId, null, null, tenantId);

            var data = new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Phone,
                user.Role,
                user.CreatedAt,
                user.LastLoginAt,
                Clinics = user.ClinicLinks.Select(cl => new
                {
                    cl.ClinicId,
                    cl.IsActive,
                    cl.IsPreferredClinic
                }),
                RecentActivity = auditLogs.Take(100),
                ExportedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            return Encoding.UTF8.GetBytes(json);
        }

        public async Task AnonymizeUserDataAsync(string userId, string tenantId, string requestedByUserId)
        {
            var userGuid = Guid.Parse(userId);
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userGuid && u.TenantId == tenantId);

            if (user == null)
                throw new InvalidOperationException("User not found");

            // Store old data for audit
            var oldData = new
            {
                user.Username,
                user.Email,
                user.FullName,
                user.Phone
            };

            // Anonymize user data
            user.UpdateProfile(
                $"anonymized-{Guid.NewGuid()}@example.com",
                $"User-{Guid.NewGuid()}",
                "***"
            );

            user.Deactivate();

            await _context.SaveChangesAsync();

            // Log the anonymization
            await _auditService.LogAsync(new DTOs.CreateAuditLogDto(
                UserId: requestedByUserId,
                UserName: "System",
                UserEmail: "system@primecare.com",
                Action: AuditAction.DELETE,
                ActionDescription: "Dados do usuário anonimizados (LGPD)",
                EntityType: "User",
                EntityId: userId,
                EntityDisplayName: oldData.FullName,
                IpAddress: "system",
                UserAgent: "system",
                RequestPath: "/api/gdpr/anonymize-user",
                HttpMethod: "POST",
                Result: Domain.Enums.OperationResult.SUCCESS,
                DataCategory: DataCategory.PERSONAL,
                Purpose: Domain.ValueObjects.LgpdPurpose.LEGAL_OBLIGATION,
                Severity: AuditSeverity.CRITICAL,
                TenantId: tenantId,
                OldValues: JsonSerializer.Serialize(oldData),
                NewValues: "*** ANONYMIZED ***"
            ));
        }
    }
}
