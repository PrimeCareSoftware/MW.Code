using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.Dashboards;

namespace MedicSoft.Application.Services.Dashboards
{
    /// <summary>
    /// Interface for dashboard management service
    /// Phase 3: Analytics and BI
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// Get all dashboards for the current user
        /// </summary>
        Task<List<CustomDashboardDto>> GetAllDashboardsAsync();

        /// <summary>
        /// Get specific dashboard by ID
        /// </summary>
        Task<CustomDashboardDto> GetDashboardAsync(Guid id);

        /// <summary>
        /// Create a new dashboard
        /// </summary>
        Task<CustomDashboardDto> CreateDashboardAsync(CreateDashboardDto dto, string userId);

        /// <summary>
        /// Update an existing dashboard
        /// </summary>
        Task<CustomDashboardDto> UpdateDashboardAsync(Guid id, UpdateDashboardDto dto);

        /// <summary>
        /// Delete a dashboard
        /// </summary>
        Task DeleteDashboardAsync(Guid id);

        /// <summary>
        /// Add a widget to a dashboard
        /// </summary>
        Task<DashboardWidgetDto> AddWidgetAsync(Guid dashboardId, CreateWidgetDto dto);

        /// <summary>
        /// Update widget position on grid
        /// </summary>
        Task UpdateWidgetPositionAsync(Guid widgetId, WidgetPositionDto position);

        /// <summary>
        /// Delete a widget from dashboard
        /// </summary>
        Task DeleteWidgetAsync(Guid widgetId);

        /// <summary>
        /// Execute widget query and return data
        /// </summary>
        Task<WidgetDataDto> ExecuteWidgetQueryAsync(Guid widgetId);

        /// <summary>
        /// Export dashboard to specified format
        /// </summary>
        Task<byte[]> ExportDashboardAsync(Guid id, ExportFormat format);

        /// <summary>
        /// Get all widget templates
        /// </summary>
        Task<List<WidgetTemplateDto>> GetWidgetTemplatesAsync();

        /// <summary>
        /// Get widget templates by category
        /// </summary>
        Task<List<WidgetTemplateDto>> GetWidgetTemplatesByCategoryAsync(string category);

        // Category 4.1: Dashboard Sharing Methods
        /// <summary>
        /// Share a dashboard with a user or role
        /// </summary>
        Task<DashboardShareDto> ShareDashboardAsync(Guid dashboardId, CreateDashboardShareDto dto, string sharedByUserId);

        /// <summary>
        /// Get all shares for a specific dashboard
        /// </summary>
        Task<List<DashboardShareDto>> GetDashboardSharesAsync(Guid dashboardId);

        /// <summary>
        /// Revoke dashboard share
        /// </summary>
        Task RevokeDashboardShareAsync(Guid shareId);

        /// <summary>
        /// Get dashboards shared with current user
        /// </summary>
        Task<List<CustomDashboardDto>> GetSharedDashboardsAsync(string userId, string userRole);

        /// <summary>
        /// Duplicate a dashboard (useful for creating templates)
        /// </summary>
        Task<CustomDashboardDto> DuplicateDashboardAsync(Guid dashboardId, string userId, string newName);
    }

    /// <summary>
    /// Export format enumeration
    /// </summary>
    public enum ExportFormat
    {
        Json,
        Pdf,
        Excel
    }
}
