using System;

namespace MedicSoft.Application.DTOs.Dashboards
{
    /// <summary>
    /// DTO for dashboard sharing - Category 4.1
    /// </summary>
    public class DashboardShareDto
    {
        public Guid Id { get; set; }
        public Guid DashboardId { get; set; }
        public string SharedWithUserId { get; set; }
        public string SharedWithUserName { get; set; }
        public string SharedWithRole { get; set; }
        public string PermissionLevel { get; set; }
        public string SharedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a dashboard share
    /// </summary>
    public class CreateDashboardShareDto
    {
        public string SharedWithUserId { get; set; }
        public string SharedWithRole { get; set; }
        public string PermissionLevel { get; set; } = "View";
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// DTO for dashboard filter configuration - Category 4.1
    /// </summary>
    public class DashboardFilterDto
    {
        public string FieldName { get; set; }
        public string Operator { get; set; } // equals, contains, greaterThan, lessThan, between, in
        public object Value { get; set; }
        public string DataType { get; set; } // string, number, date, boolean
    }

    /// <summary>
    /// DTO for drill-down configuration - Category 4.1
    /// </summary>
    public class DrillDownConfigDto
    {
        public string TargetDashboardId { get; set; }
        public string TargetWidgetId { get; set; }
        public string FilterMapping { get; set; } // JSON mapping of parent filter to child filter
        public bool OpenInNewTab { get; set; }
    }
}
