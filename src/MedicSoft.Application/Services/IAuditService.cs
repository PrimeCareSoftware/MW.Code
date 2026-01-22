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
    }
}
