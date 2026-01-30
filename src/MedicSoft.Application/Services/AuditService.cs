using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            IAuditRepository auditRepository,
            MedicSoftDbContext context,
            ILogger<AuditService> logger)
        {
            _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task LogAsync(CreateAuditLogDto dto)
        {
            var auditLog = new AuditLog(
                dto.UserId,
                dto.UserName,
                dto.UserEmail,
                dto.Action,
                dto.ActionDescription,
                dto.EntityType,
                dto.EntityId,
                dto.IpAddress,
                dto.UserAgent,
                dto.RequestPath,
                dto.HttpMethod,
                dto.Result,
                dto.DataCategory,
                dto.Purpose,
                dto.Severity,
                dto.TenantId
            );

            if (!string.IsNullOrEmpty(dto.EntityDisplayName))
                auditLog.SetEntityDisplayName(dto.EntityDisplayName);

            if (!string.IsNullOrEmpty(dto.OldValues))
                auditLog.SetOldValues(dto.OldValues);

            if (!string.IsNullOrEmpty(dto.NewValues))
                auditLog.SetNewValues(dto.NewValues);

            if (dto.ChangedFields != null && dto.ChangedFields.Any())
                auditLog.SetChangedFields(dto.ChangedFields);

            if (!string.IsNullOrEmpty(dto.FailureReason))
                auditLog.SetFailureReason(dto.FailureReason);

            if (dto.StatusCode.HasValue)
                auditLog.SetStatusCode(dto.StatusCode.Value);

            await _auditRepository.AddAsync(auditLog);
        }

        public async Task LogAuthenticationAsync(
            string userId,
            string userName,
            string userEmail,
            AuditAction action,
            bool success,
            string ipAddress,
            string userAgent,
            string tenantId,
            string? reason = null)
        {
            var dto = new CreateAuditLogDto(
                UserId: userId,
                UserName: userName,
                UserEmail: userEmail,
                Action: action,
                ActionDescription: GetActionDescription(action),
                EntityType: "User",
                EntityId: userId,
                EntityDisplayName: userName,
                IpAddress: ipAddress,
                UserAgent: userAgent,
                RequestPath: "/api/auth",
                HttpMethod: "POST",
                Result: success ? OperationResult.SUCCESS : OperationResult.FAILED,
                DataCategory: DataCategory.PERSONAL,
                Purpose: LgpdPurpose.LEGAL_OBLIGATION,
                Severity: success ? AuditSeverity.INFO : AuditSeverity.WARNING,
                TenantId: tenantId,
                FailureReason: reason
            );

            await LogAsync(dto);
        }

        public async Task LogDataAccessAsync(
            string userId,
            string userName,
            string userEmail,
            string entityType,
            string entityId,
            string entityDisplayName,
            string ipAddress,
            string userAgent,
            string requestPath,
            string httpMethod,
            string tenantId,
            DataCategory dataCategory = DataCategory.PERSONAL,
            LgpdPurpose purpose = LgpdPurpose.HEALTHCARE)
        {
            var dto = new CreateAuditLogDto(
                UserId: userId,
                UserName: userName,
                UserEmail: userEmail,
                Action: AuditAction.READ,
                ActionDescription: $"Acesso a {entityType}",
                EntityType: entityType,
                EntityId: entityId,
                EntityDisplayName: entityDisplayName,
                IpAddress: ipAddress,
                UserAgent: userAgent,
                RequestPath: requestPath,
                HttpMethod: httpMethod,
                Result: OperationResult.SUCCESS,
                DataCategory: dataCategory,
                Purpose: purpose,
                Severity: AuditSeverity.INFO,
                TenantId: tenantId
            );

            await LogAsync(dto);
        }

        public async Task LogDataModificationAsync(
            string userId,
            string userName,
            string userEmail,
            string entityType,
            string entityId,
            string entityDisplayName,
            object oldValues,
            object newValues,
            string ipAddress,
            string userAgent,
            string requestPath,
            string httpMethod,
            string tenantId,
            DataCategory dataCategory = DataCategory.PERSONAL,
            LgpdPurpose purpose = LgpdPurpose.HEALTHCARE)
        {
            var oldValuesJson = JsonSerializer.Serialize(oldValues);
            var newValuesJson = JsonSerializer.Serialize(newValues);
            var changedFields = GetChangedFields(oldValues, newValues);

            var dto = new CreateAuditLogDto(
                UserId: userId,
                UserName: userName,
                UserEmail: userEmail,
                Action: AuditAction.UPDATE,
                ActionDescription: $"Modificação de {entityType}",
                EntityType: entityType,
                EntityId: entityId,
                EntityDisplayName: entityDisplayName,
                IpAddress: ipAddress,
                UserAgent: userAgent,
                RequestPath: requestPath,
                HttpMethod: httpMethod,
                Result: OperationResult.SUCCESS,
                DataCategory: dataCategory,
                Purpose: purpose,
                Severity: AuditSeverity.INFO,
                TenantId: tenantId,
                OldValues: oldValuesJson,
                NewValues: newValuesJson,
                ChangedFields: changedFields
            );

            await LogAsync(dto);
        }

        public async Task<List<AuditLogDto>> GetUserActivityAsync(
            string userId,
            DateTime? startDate,
            DateTime? endDate,
            string tenantId)
        {
            var logs = await _auditRepository.GetByUserIdAsync(userId, tenantId, startDate, endDate);
            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> GetEntityHistoryAsync(
            string entityType,
            string entityId,
            string tenantId)
        {
            var logs = await _auditRepository.GetByEntityAsync(entityType, entityId, tenantId);
            return logs.Select(MapToDto).ToList();
        }

        public async Task<List<AuditLogDto>> GetSecurityEventsAsync(
            DateTime? startDate,
            DateTime? endDate,
            string tenantId)
        {
            var logs = await _auditRepository.GetSecurityEventsAsync(tenantId, startDate, endDate);
            return logs.Select(MapToDto).ToList();
        }

        public async Task<AuditReport> GenerateLgpdReportAsync(string userId, string tenantId)
        {
            var logs = await _auditRepository.GetByUserIdAsync(userId, tenantId);
            
            var totalAccesses = logs.Count(l => l.Action == AuditAction.READ);
            var dataModifications = logs.Count(l => l.Action == AuditAction.UPDATE || l.Action == AuditAction.CREATE);
            var dataExports = logs.Count(l => l.Action == AuditAction.EXPORT || l.Action == AuditAction.DOWNLOAD);
            var recentActivity = logs.OrderByDescending(l => l.Timestamp).Take(50).Select(MapToDto).ToList();

            var userName = logs.FirstOrDefault()?.UserName ?? "Unknown";

            return new AuditReport(
                userId,
                userName,
                DateTime.UtcNow,
                totalAccesses,
                dataModifications,
                dataExports,
                recentActivity
            );
        }

        public async Task<(List<AuditLogDto> Logs, int TotalCount)> QueryAsync(AuditFilter filter)
        {
            var logs = await _auditRepository.QueryAsync(filter);
            var totalCount = await _auditRepository.CountAsync(filter);
            
            return (logs.Select(MapToDto).ToList(), totalCount);
        }

        private static AuditLogDto MapToDto(AuditLog log)
        {
            return new AuditLogDto(
                log.Id,
                log.Timestamp,
                log.UserName,
                log.UserEmail,
                log.Action.ToString(),
                log.ActionDescription,
                log.EntityType,
                log.EntityId,
                log.EntityDisplayName,
                log.Result.ToString(),
                log.IpAddress,
                log.UserAgent,
                log.RequestPath,
                log.HttpMethod,
                log.OldValues,
                log.NewValues,
                log.ChangedFields,
                log.FailureReason,
                log.StatusCode,
                log.DataCategory.ToString(),
                log.Purpose.ToString(),
                log.Severity.ToString()
            );
        }

        private static string GetActionDescription(AuditAction action)
        {
            return action switch
            {
                AuditAction.LOGIN => "Login realizado",
                AuditAction.LOGOUT => "Logout realizado",
                AuditAction.LOGIN_FAILED => "Tentativa de login falhou",
                AuditAction.PASSWORD_CHANGED => "Senha alterada",
                AuditAction.PASSWORD_RESET_REQUESTED => "Recuperação de senha solicitada",
                AuditAction.MFA_ENABLED => "MFA habilitado",
                AuditAction.MFA_DISABLED => "MFA desabilitado",
                AuditAction.ACCESS_DENIED => "Acesso negado",
                AuditAction.PERMISSION_CHANGED => "Permissões alteradas",
                AuditAction.ROLE_CHANGED => "Papel alterado",
                AuditAction.CREATE => "Registro criado",
                AuditAction.READ => "Registro acessado",
                AuditAction.UPDATE => "Registro atualizado",
                AuditAction.DELETE => "Registro excluído",
                AuditAction.EXPORT => "Dados exportados",
                AuditAction.DOWNLOAD => "Arquivo baixado",
                AuditAction.PRINT => "Documento impresso",
                AuditAction.DATA_ACCESS_REQUEST => "Solicitação de acesso aos dados",
                AuditAction.DATA_DELETION_REQUEST => "Solicitação de exclusão de dados",
                AuditAction.DATA_PORTABILITY_REQUEST => "Solicitação de portabilidade de dados",
                AuditAction.DATA_CORRECTION_REQUEST => "Solicitação de correção de dados",
                _ => action.ToString()
            };
        }

        private static List<string> GetChangedFields(object oldValues, object newValues)
        {
            var changedFields = new List<string>();

            try
            {
                var oldJson = JsonSerializer.Serialize(oldValues);
                var newJson = JsonSerializer.Serialize(newValues);

                var oldDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(oldJson);
                var newDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(newJson);

                if (oldDict == null || newDict == null)
                    return changedFields;

                foreach (var key in newDict.Keys)
                {
                    if (!oldDict.ContainsKey(key) || !JsonElementEquals(oldDict[key], newDict[key]))
                    {
                        changedFields.Add(key);
                    }
                }
            }
            catch
            {
                // If comparison fails, return empty list
            }

            return changedFields;
        }

        private static bool JsonElementEquals(JsonElement a, JsonElement b)
        {
            return a.GetRawText() == b.GetRawText();
        }

        public async Task<string> ExportToCsvAsync(AuditFilter filter)
        {
            var logs = await _auditRepository.QueryAsync(filter);
            var csv = new StringBuilder();

            // CSV Header
            csv.AppendLine("Timestamp,UserId,UserName,UserEmail,Action,EntityType,EntityId,Result,IpAddress,Severity,RequestPath,HttpMethod");

            // CSV Rows
            foreach (var log in logs)
            {
                csv.AppendLine($"\"{log.Timestamp:yyyy-MM-dd HH:mm:ss}\",\"{EscapeCsv(log.UserId)}\",\"{EscapeCsv(log.UserName)}\",\"{EscapeCsv(log.UserEmail)}\",\"{log.Action}\",\"{EscapeCsv(log.EntityType)}\",\"{EscapeCsv(log.EntityId)}\",\"{log.Result}\",\"{EscapeCsv(log.IpAddress)}\",\"{log.Severity}\",\"{EscapeCsv(log.RequestPath)}\",\"{log.HttpMethod}\"");
            }

            return csv.ToString();
        }

        public async Task<string> ExportToJsonAsync(AuditFilter filter)
        {
            var logs = await _auditRepository.QueryAsync(filter);
            var dtos = logs.Select(MapToDto).ToList();
            
            return JsonSerializer.Serialize(dtos, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public async Task<string> ExportLgpdComplianceReportAsync(string userId, string tenantId)
        {
            var report = await GenerateLgpdReportAsync(userId, tenantId);
            
            var json = new
            {
                ReportType = "LGPD Compliance Report",
                GeneratedAt = report.GeneratedAt,
                UserId = report.UserId,
                UserName = report.UserName,
                Summary = new
                {
                    TotalAccesses = report.TotalAccesses,
                    DataModifications = report.DataModifications,
                    DataExports = report.DataExports
                },
                RecentActivity = report.RecentActivity,
                ComplianceStatement = "Este relatório atende aos requisitos da LGPD Art. 37 - Registro de Acesso a Dados Pessoais"
            };

            return JsonSerializer.Serialize(json, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        public async Task<int> ApplyRetentionPolicyAsync(string tenantId, int retentionDays = 2555)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);
            return await CleanupOldLogsAsync(tenantId, cutoffDate);
        }

        public async Task<int> CleanupOldLogsAsync(string tenantId, DateTime beforeDate)
        {
            try
            {
                var deletedCount = await _context.AuditLogs
                    .Where(a => a.TenantId == tenantId && a.Timestamp < beforeDate)
                    .ExecuteDeleteAsync();

                _logger.LogInformation($"Cleaned up {deletedCount} audit logs before {beforeDate:yyyy-MM-dd} for tenant {tenantId}");
                
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to cleanup audit logs for tenant {tenantId}");
                throw;
            }
        }

        public async Task<AuditStatistics> GetStatisticsAsync(string tenantId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.AuditLogs.Where(a => a.TenantId == tenantId);

            if (startDate.HasValue)
                query = query.Where(a => a.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Timestamp <= endDate.Value);

            var totalLogs = await query.CountAsync();
            var successfulOperations = await query.CountAsync(a => a.Result == OperationResult.SUCCESS);
            var failedOperations = await query.CountAsync(a => a.Result == OperationResult.FAILED);
            
            var securityEvents = await query.CountAsync(a => 
                a.Severity == AuditSeverity.WARNING || 
                a.Severity == AuditSeverity.ERROR || 
                a.Severity == AuditSeverity.CRITICAL);

            var uniqueUsers = await query.Select(a => a.UserId).Distinct().CountAsync();
            
            var actionBreakdown = await query
                .GroupBy(a => a.Action)
                .Select(g => new ActionCount { Action = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            var severityBreakdown = await query
                .GroupBy(a => a.Severity)
                .Select(g => new SeverityCount { Severity = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            return new AuditStatistics(
                TotalLogs: totalLogs,
                SuccessfulOperations: successfulOperations,
                FailedOperations: failedOperations,
                SecurityEvents: securityEvents,
                UniqueUsers: uniqueUsers,
                ActionBreakdown: actionBreakdown,
                SeverityBreakdown: severityBreakdown,
                StartDate: startDate ?? DateTime.UtcNow.AddDays(-30),
                EndDate: endDate ?? DateTime.UtcNow
            );
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            
            return value.Replace("\"", "\"\"");
        }
    }

    public record AuditStatistics(
        int TotalLogs,
        int SuccessfulOperations,
        int FailedOperations,
        int SecurityEvents,
        int UniqueUsers,
        List<ActionCount> ActionBreakdown,
        List<SeverityCount> SeverityBreakdown,
        DateTime StartDate,
        DateTime EndDate
    );

    public record ActionCount
    {
        public string Action { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public record SeverityCount
    {
        public string Severity { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
