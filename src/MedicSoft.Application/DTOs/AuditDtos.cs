using System;
using System.Collections.Generic;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs
{
    public record AuditLogDto(
        Guid Id,
        DateTime Timestamp,
        string UserName,
        string UserEmail,
        string Action,
        string ActionDescription,
        string EntityType,
        string EntityId,
        string? EntityDisplayName,
        string Result,
        string IpAddress,
        string UserAgent,
        string RequestPath,
        string HttpMethod,
        string? OldValues,
        string? NewValues,
        List<string>? ChangedFields,
        string? FailureReason,
        int? StatusCode,
        string DataCategory,
        string Purpose,
        string Severity
    );

    public record AuditReport(
        string UserId,
        string UserName,
        DateTime GeneratedAt,
        int TotalAccesses,
        int DataModifications,
        int DataExports,
        List<AuditLogDto> RecentActivity
    );

    public record CreateAuditLogDto(
        string UserId,
        string UserName,
        string UserEmail,
        AuditAction Action,
        string ActionDescription,
        string EntityType,
        string EntityId,
        string? EntityDisplayName,
        string IpAddress,
        string UserAgent,
        string RequestPath,
        string HttpMethod,
        OperationResult Result,
        DataCategory DataCategory,
        LgpdPurpose Purpose,
        AuditSeverity Severity,
        string TenantId,
        string? OldValues = null,
        string? NewValues = null,
        List<string>? ChangedFields = null,
        string? FailureReason = null,
        int? StatusCode = null
    );

    public record DataProcessingConsentDto(
        Guid Id,
        string UserId,
        DateTime ConsentDate,
        DateTime? RevokedDate,
        bool IsRevoked,
        string Purpose,
        string PurposeDescription,
        List<string> DataCategories,
        string ConsentText,
        string IpAddress,
        string UserAgent,
        string ConsentMethod
    );

    public record CreateDataProcessingConsentDto(
        string UserId,
        LgpdPurpose Purpose,
        string PurposeDescription,
        List<DataCategory> DataCategories,
        string ConsentText,
        string IpAddress,
        string UserAgent,
        string ConsentMethod,
        string TenantId
    );
}
