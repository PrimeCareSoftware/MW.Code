using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.Dashboards
{
    /// <summary>
    /// DTO for custom dashboard display
    /// </summary>
    public class CustomDashboardDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Layout { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<DashboardWidgetDto> Widgets { get; set; } = new List<DashboardWidgetDto>();
    }

    /// <summary>
    /// DTO for creating a new dashboard
    /// </summary>
    public class CreateDashboardDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing dashboard
    /// </summary>
    public class UpdateDashboardDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Layout { get; set; }
        public bool IsDefault { get; set; }
        public bool IsPublic { get; set; }
    }
}
