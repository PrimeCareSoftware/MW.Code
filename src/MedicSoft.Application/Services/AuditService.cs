using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;

        public AuditService(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
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
    }
}
