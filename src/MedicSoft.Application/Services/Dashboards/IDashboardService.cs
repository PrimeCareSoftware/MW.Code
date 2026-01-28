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
        Task<CustomDashboardDto> GetDashboardAsync(int id);

        /// <summary>
        /// Create a new dashboard
        /// </summary>
        Task<CustomDashboardDto> CreateDashboardAsync(CreateDashboardDto dto, string userId);

        /// <summary>
        /// Update an existing dashboard
        /// </summary>
        Task<CustomDashboardDto> UpdateDashboardAsync(int id, UpdateDashboardDto dto);

        /// <summary>
        /// Delete a dashboard
        /// </summary>
        Task DeleteDashboardAsync(int id);

        /// <summary>
        /// Add a widget to a dashboard
        /// </summary>
        Task<DashboardWidgetDto> AddWidgetAsync(int dashboardId, CreateWidgetDto dto);

        /// <summary>
        /// Update widget position on grid
        /// </summary>
        Task UpdateWidgetPositionAsync(int widgetId, WidgetPositionDto position);

        /// <summary>
        /// Delete a widget from dashboard
        /// </summary>
        Task DeleteWidgetAsync(int widgetId);

        /// <summary>
        /// Execute widget query and return data
        /// </summary>
        Task<WidgetDataDto> ExecuteWidgetQueryAsync(int widgetId);

        /// <summary>
        /// Export dashboard to specified format
        /// </summary>
        Task<byte[]> ExportDashboardAsync(int id, ExportFormat format);

        /// <summary>
        /// Get all widget templates
        /// </summary>
        Task<List<WidgetTemplateDto>> GetWidgetTemplatesAsync();

        /// <summary>
        /// Get widget templates by category
        /// </summary>
        Task<List<WidgetTemplateDto>> GetWidgetTemplatesByCategoryAsync(string category);
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
