using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.Reports;

namespace MedicSoft.Application.Services.Reports
{
    /// <summary>
    /// Interface for report management and generation service
    /// Phase 3: Analytics and BI
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// Get all report templates
        /// </summary>
        Task<List<ReportTemplateDto>> GetAllReportTemplatesAsync();

        /// <summary>
        /// Get report templates by category
        /// </summary>
        Task<List<ReportTemplateDto>> GetReportTemplatesByCategoryAsync(string category);

        /// <summary>
        /// Get specific report template by ID
        /// </summary>
        Task<ReportTemplateDto> GetReportTemplateAsync(Guid id);

        /// <summary>
        /// Create a new report template
        /// </summary>
        Task<ReportTemplateDto> CreateReportTemplateAsync(CreateReportTemplateDto dto);

        /// <summary>
        /// Update an existing report template
        /// </summary>
        Task<ReportTemplateDto> UpdateReportTemplateAsync(Guid id, UpdateReportTemplateDto dto);

        /// <summary>
        /// Delete a report template
        /// </summary>
        Task DeleteReportTemplateAsync(Guid id);

        /// <summary>
        /// Generate a report on-demand
        /// </summary>
        Task<ReportResultDto> GenerateReportAsync(GenerateReportDto dto);

        /// <summary>
        /// Get all scheduled reports
        /// </summary>
        Task<List<ScheduledReportDto>> GetAllScheduledReportsAsync();

        /// <summary>
        /// Get specific scheduled report by ID
        /// </summary>
        Task<ScheduledReportDto> GetScheduledReportAsync(Guid id);

        /// <summary>
        /// Create a new scheduled report
        /// </summary>
        Task<ScheduledReportDto> CreateScheduledReportAsync(CreateScheduledReportDto dto, string userId);

        /// <summary>
        /// Update an existing scheduled report
        /// </summary>
        Task<ScheduledReportDto> UpdateScheduledReportAsync(Guid id, UpdateScheduledReportDto dto);

        /// <summary>
        /// Delete a scheduled report
        /// </summary>
        Task DeleteScheduledReportAsync(Guid id);

        /// <summary>
        /// Execute a scheduled report (used by Hangfire job)
        /// </summary>
        Task ExecuteScheduledReportAsync(Guid scheduledReportId);
    }
}
