using System;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.ValueObjects
{
    public class AuditFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? UserId { get; set; }
        public string? TenantId { get; set; }
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public AuditAction? Action { get; set; }
        public OperationResult? Result { get; set; }
        public AuditSeverity? Severity { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
