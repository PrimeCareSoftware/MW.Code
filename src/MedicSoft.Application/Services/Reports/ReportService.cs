using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.Reports;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<ReportService> _logger;

        public ReportService(MedicSoftDbContext context, ILogger<ReportService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ReportTemplateDto>> GetAllReportTemplatesAsync()
        {
            _logger.LogInformation("Retrieving all report templates");

            var templates = await _context.Set<ReportTemplate>()
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapTemplateToDto).ToList();
        }

        public async Task<List<ReportTemplateDto>> GetReportTemplatesByCategoryAsync(string category)
        {
            _logger.LogInformation("Retrieving report templates for category: {Category}", category);

            var templates = await _context.Set<ReportTemplate>()
                .Where(t => t.Category == category)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return templates.Select(MapTemplateToDto).ToList();
        }

        public async Task<ReportTemplateDto> GetReportTemplateAsync(int id)
        {
            _logger.LogInformation("Retrieving report template with ID: {TemplateId}", id);

            var template = await _context.Set<ReportTemplate>()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                throw new InvalidOperationException($"Report template with ID {id} not found");
            }

            return MapTemplateToDto(template);
        }

        public async Task<ReportTemplateDto> CreateReportTemplateAsync(CreateReportTemplateDto dto)
        {
            _logger.LogInformation("Creating new report template: {TemplateName}", dto.Name);

            var template = new ReportTemplate
            {
                Name = dto.Name,
                Description = dto.Description,
                Category = dto.Category,
                Configuration = dto.Configuration,
                Query = dto.Query,
                Icon = dto.Icon,
                SupportedFormats = dto.SupportedFormats,
                IsSystem = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Set<ReportTemplate>().Add(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Report template created successfully with ID: {TemplateId}", template.Id);

            return MapTemplateToDto(template);
        }

        public async Task<ReportTemplateDto> UpdateReportTemplateAsync(int id, UpdateReportTemplateDto dto)
        {
            _logger.LogInformation("Updating report template with ID: {TemplateId}", id);

            var template = await _context.Set<ReportTemplate>()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                throw new InvalidOperationException($"Report template with ID {id} not found");
            }

            if (template.IsSystem)
            {
                throw new InvalidOperationException("Cannot modify system report templates");
            }

            template.Name = dto.Name;
            template.Description = dto.Description;
            template.Category = dto.Category;
            template.Configuration = dto.Configuration;
            template.Query = dto.Query;
            template.Icon = dto.Icon;
            template.SupportedFormats = dto.SupportedFormats;
            template.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Report template updated successfully");

            return MapTemplateToDto(template);
        }

        public async Task DeleteReportTemplateAsync(int id)
        {
            _logger.LogInformation("Deleting report template with ID: {TemplateId}", id);

            var template = await _context.Set<ReportTemplate>()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (template == null)
            {
                throw new InvalidOperationException($"Report template with ID {id} not found");
            }

            if (template.IsSystem)
            {
                throw new InvalidOperationException("Cannot delete system report templates");
            }

            _context.Set<ReportTemplate>().Remove(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Report template deleted successfully");
        }

        public async Task<ReportResultDto> GenerateReportAsync(GenerateReportDto dto)
        {
            _logger.LogInformation("Generating report from template: {TemplateId}", dto.ReportTemplateId);

            var template = await _context.Set<ReportTemplate>()
                .FirstOrDefaultAsync(t => t.Id == dto.ReportTemplateId);

            if (template == null)
            {
                return new ReportResultDto
                {
                    Error = $"Report template with ID {dto.ReportTemplateId} not found"
                };
            }

            try
            {
                // TODO: Implement actual report generation logic
                // This would involve:
                // 1. Execute the query with parameters
                // 2. Format the data according to template configuration
                // 3. Generate PDF/Excel/CSV based on OutputFormat
                
                _logger.LogWarning("Report generation is not yet fully implemented");
                
                return new ReportResultDto
                {
                    Error = "Report generation functionality is not yet implemented. This is a placeholder for future implementation."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report from template: {TemplateId}", dto.ReportTemplateId);
                return new ReportResultDto
                {
                    Error = $"Error generating report: {ex.Message}"
                };
            }
        }

        public async Task<List<ScheduledReportDto>> GetAllScheduledReportsAsync()
        {
            _logger.LogInformation("Retrieving all scheduled reports");

            var scheduledReports = await _context.Set<ScheduledReport>()
                .Include(s => s.ReportTemplate)
                .OrderBy(s => s.Name)
                .ToListAsync();

            return scheduledReports.Select(MapScheduledReportToDto).ToList();
        }

        public async Task<ScheduledReportDto> GetScheduledReportAsync(int id)
        {
            _logger.LogInformation("Retrieving scheduled report with ID: {ScheduledReportId}", id);

            var scheduledReport = await _context.Set<ScheduledReport>()
                .Include(s => s.ReportTemplate)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scheduledReport == null)
            {
                throw new InvalidOperationException($"Scheduled report with ID {id} not found");
            }

            return MapScheduledReportToDto(scheduledReport);
        }

        public async Task<ScheduledReportDto> CreateScheduledReportAsync(CreateScheduledReportDto dto, string userId)
        {
            _logger.LogInformation("Creating new scheduled report: {ReportName}", dto.Name);

            var scheduledReport = new ScheduledReport
            {
                ReportTemplateId = dto.ReportTemplateId,
                Name = dto.Name,
                Description = dto.Description,
                CronExpression = dto.CronExpression,
                OutputFormat = dto.OutputFormat,
                Recipients = dto.Recipients,
                Parameters = dto.Parameters,
                IsActive = dto.IsActive,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                NextRunAt = CalculateNextRunTime(dto.CronExpression)
            };

            _context.Set<ScheduledReport>().Add(scheduledReport);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Scheduled report created successfully with ID: {ScheduledReportId}", scheduledReport.Id);

            // Reload with template
            return await GetScheduledReportAsync(scheduledReport.Id);
        }

        public async Task<ScheduledReportDto> UpdateScheduledReportAsync(int id, UpdateScheduledReportDto dto)
        {
            _logger.LogInformation("Updating scheduled report with ID: {ScheduledReportId}", id);

            var scheduledReport = await _context.Set<ScheduledReport>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scheduledReport == null)
            {
                throw new InvalidOperationException($"Scheduled report with ID {id} not found");
            }

            scheduledReport.Name = dto.Name;
            scheduledReport.Description = dto.Description;
            scheduledReport.CronExpression = dto.CronExpression;
            scheduledReport.OutputFormat = dto.OutputFormat;
            scheduledReport.Recipients = dto.Recipients;
            scheduledReport.Parameters = dto.Parameters;
            scheduledReport.IsActive = dto.IsActive;
            scheduledReport.UpdatedAt = DateTime.UtcNow;
            scheduledReport.NextRunAt = CalculateNextRunTime(dto.CronExpression);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Scheduled report updated successfully");

            return await GetScheduledReportAsync(id);
        }

        public async Task DeleteScheduledReportAsync(int id)
        {
            _logger.LogInformation("Deleting scheduled report with ID: {ScheduledReportId}", id);

            var scheduledReport = await _context.Set<ScheduledReport>()
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scheduledReport == null)
            {
                throw new InvalidOperationException($"Scheduled report with ID {id} not found");
            }

            _context.Set<ScheduledReport>().Remove(scheduledReport);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Scheduled report deleted successfully");
        }

        public async Task ExecuteScheduledReportAsync(int scheduledReportId)
        {
            _logger.LogInformation("Executing scheduled report: {ScheduledReportId}", scheduledReportId);

            var scheduledReport = await _context.Set<ScheduledReport>()
                .Include(s => s.ReportTemplate)
                .FirstOrDefaultAsync(s => s.Id == scheduledReportId);

            if (scheduledReport == null)
            {
                _logger.LogWarning("Scheduled report {ScheduledReportId} not found", scheduledReportId);
                return;
            }

            if (!scheduledReport.IsActive)
            {
                _logger.LogInformation("Scheduled report {ScheduledReportId} is not active, skipping", scheduledReportId);
                return;
            }

            try
            {
                // TODO: Implement scheduled report execution
                // This would involve:
                // 1. Generate the report using GenerateReportAsync
                // 2. Send email with attachment to recipients
                // 3. Update LastRunAt and NextRunAt
                
                scheduledReport.LastRunAt = DateTime.UtcNow;
                scheduledReport.NextRunAt = CalculateNextRunTime(scheduledReport.CronExpression);
                scheduledReport.LastRunStatus = "success";
                scheduledReport.LastRunError = null;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Scheduled report executed successfully: {ScheduledReportId}", scheduledReportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scheduled report: {ScheduledReportId}", scheduledReportId);
                
                scheduledReport.LastRunAt = DateTime.UtcNow;
                scheduledReport.NextRunAt = CalculateNextRunTime(scheduledReport.CronExpression);
                scheduledReport.LastRunStatus = "failed";
                scheduledReport.LastRunError = ex.Message;

                await _context.SaveChangesAsync();
            }
        }

        private DateTime? CalculateNextRunTime(string cronExpression)
        {
            // TODO: Implement proper cron expression parsing
            // For now, return a time 24 hours from now as a placeholder
            return DateTime.UtcNow.AddDays(1);
        }

        private ReportTemplateDto MapTemplateToDto(ReportTemplate template)
        {
            return new ReportTemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                Category = template.Category,
                Configuration = template.Configuration,
                Query = template.Query,
                IsSystem = template.IsSystem,
                Icon = template.Icon,
                SupportedFormats = template.SupportedFormats,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt
            };
        }

        private ScheduledReportDto MapScheduledReportToDto(ScheduledReport scheduledReport)
        {
            return new ScheduledReportDto
            {
                Id = scheduledReport.Id,
                ReportTemplateId = scheduledReport.ReportTemplateId,
                ReportTemplateName = scheduledReport.ReportTemplate?.Name,
                Name = scheduledReport.Name,
                Description = scheduledReport.Description,
                CronExpression = scheduledReport.CronExpression,
                OutputFormat = scheduledReport.OutputFormat,
                Recipients = scheduledReport.Recipients,
                Parameters = scheduledReport.Parameters,
                IsActive = scheduledReport.IsActive,
                CreatedBy = scheduledReport.CreatedBy,
                CreatedAt = scheduledReport.CreatedAt,
                UpdatedAt = scheduledReport.UpdatedAt,
                LastRunAt = scheduledReport.LastRunAt,
                NextRunAt = scheduledReport.NextRunAt,
                LastRunStatus = scheduledReport.LastRunStatus,
                LastRunError = scheduledReport.LastRunError
            };
        }
    }
}
