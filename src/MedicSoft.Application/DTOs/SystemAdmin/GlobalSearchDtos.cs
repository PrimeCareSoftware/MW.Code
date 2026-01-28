using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.SystemAdmin
{
    public class GlobalSearchResultDto
    {
        public List<ClinicSearchResult> Clinics { get; set; } = new();
        public List<UserSearchResult> Users { get; set; } = new();
        public List<TicketSearchResult> Tickets { get; set; } = new();
        public List<PlanSearchResult> Plans { get; set; } = new();
        public List<AuditLogSearchResult> AuditLogs { get; set; } = new();
        public int TotalResults { get; set; }
        public double SearchDurationMs { get; set; }
    }

    public class ClinicSearchResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class UserSearchResult
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class TicketSearchResult
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PlanSearchResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public int ActiveSubscriptions { get; set; }
        public bool IsActive { get; set; }
    }

    public class AuditLogSearchResult
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
