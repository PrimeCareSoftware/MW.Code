using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Services
{
    public interface IAuditService
    {
        Task LogAsync(CreateAuditLogDto dto);
        
        Task LogAuthenticationAsync(
            string userId, 
            string userName, 
            string userEmail,
            AuditAction action, 
            bool success, 
            string ipAddress,
            string userAgent,
            string tenantId,
            string? reason = null);
        
        Task LogDataAccessAsync(
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
            LgpdPurpose purpose = LgpdPurpose.HEALTHCARE);
        
        Task LogDataModificationAsync(
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
            LgpdPurpose purpose = LgpdPurpose.HEALTHCARE);
        
        Task<List<AuditLogDto>> GetUserActivityAsync(
            string userId, 
            DateTime? startDate, 
            DateTime? endDate,
            string tenantId);
        
        Task<List<AuditLogDto>> GetEntityHistoryAsync(
            string entityType, 
            string entityId,
            string tenantId);
        
        Task<List<AuditLogDto>> GetSecurityEventsAsync(
            DateTime? startDate, 
            DateTime? endDate,
            string tenantId);
        
        Task<AuditReport> GenerateLgpdReportAsync(
            string userId,
            string tenantId);
        
        Task<(List<AuditLogDto> Logs, int TotalCount)> QueryAsync(AuditFilter filter);

        // Export functionality
        Task<string> ExportToCsvAsync(AuditFilter filter);
        Task<string> ExportToJsonAsync(AuditFilter filter);
        Task<string> ExportLgpdComplianceReportAsync(string userId, string tenantId);

        // Data retention
        Task<int> ApplyRetentionPolicyAsync(string tenantId, int retentionDays = 2555);
        Task<int> CleanupOldLogsAsync(string tenantId, DateTime beforeDate);

        // Statistics
        Task<AuditStatistics> GetStatisticsAsync(string tenantId, DateTime? startDate, DateTime? endDate);
    }
}
