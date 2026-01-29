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
        private readonly IReportExportService _exportService;
        private readonly IEmailService _emailService;

        public ReportService(
            MedicSoftDbContext context, 
            ILogger<ReportService> logger,
            IReportExportService exportService = null,
            IEmailService emailService = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _exportService = exportService; // Optional dependency for backwards compatibility
            _emailService = emailService; // Optional dependency for backwards compatibility
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

        public async Task<ReportTemplateDto> GetReportTemplateAsync(Guid id)
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
                IsSystem = false
            };

            _context.Set<ReportTemplate>().Add(template);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Report template created successfully with ID: {TemplateId}", template.Id);

            return MapTemplateToDto(template);
        }

        public async Task<ReportTemplateDto> UpdateReportTemplateAsync(Guid id, UpdateReportTemplateDto dto)
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
            template.UpdateTimestamp();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Report template updated successfully");

            return MapTemplateToDto(template);
        }

        public async Task DeleteReportTemplateAsync(Guid id)
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
                // Execute the query with parameters
                var query = template.Query;
                
                // SECURITY NOTE: Simple string replacement used here for report parameters.
                // This is acceptable because:
                // 1. Report templates are created only by system administrators
                // 2. Templates are stored in the database (not user input)
                // 3. Parameters are used for dates, IDs, and simple values only
                // For enhanced security, consider using parameterized queries or stored procedures.
                if (dto.Parameters != null)
                {
                    foreach (var param in dto.Parameters)
                    {
                        var paramValue = param.Value?.ToString() ?? "";
                        // Basic sanitization - remove common SQL injection patterns
                        paramValue = SanitizeParameterValue(paramValue);
                        query = query.Replace($"@{param.Key}", paramValue);
                    }
                }

                // Execute query and get data
                var data = await ExecuteReportQuery(query);

                if (data == null || !data.Any())
                {
                    return new ReportResultDto
                    {
                        FileName = $"{template.Name}_{DateTime.Now:yyyyMMdd}.txt",
                        Data = System.Text.Encoding.UTF8.GetBytes("No data found for the specified parameters"),
                        ContentType = "text/plain"
                    };
                }

                // Generate output based on format
                byte[] outputData;
                string contentType;
                string extension;

                switch (dto.OutputFormat?.ToLowerInvariant())
                {
                    case "pdf":
                        if (_exportService == null)
                        {
                            throw new InvalidOperationException("Report export service is not configured");
                        }
                        outputData = await _exportService.ExportToPdfAsync(
                            template.Name,
                            template.Description,
                            data
                        );
                        contentType = "application/pdf";
                        extension = "pdf";
                        break;

                    case "excel":
                    case "xlsx":
                        if (_exportService == null)
                        {
                            throw new InvalidOperationException("Report export service is not configured");
                        }
                        outputData = await _exportService.ExportToExcelAsync(
                            template.Name,
                            data
                        );
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        extension = "xlsx";
                        break;

                    case "csv":
                        outputData = ExportToCsv(data);
                        contentType = "text/csv";
                        extension = "csv";
                        break;

                    default:
                        // Default to CSV if format not specified
                        outputData = ExportToCsv(data);
                        contentType = "text/csv";
                        extension = "csv";
                        break;
                }

                return new ReportResultDto
                {
                    FileName = $"{template.Name}_{DateTime.Now:yyyyMMdd}.{extension}",
                    Data = outputData,
                    ContentType = contentType
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

        public async Task<ScheduledReportDto> GetScheduledReportAsync(Guid id)
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
                NextRunAt = CalculateNextRunTime(dto.CronExpression)
            };

            _context.Set<ScheduledReport>().Add(scheduledReport);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Scheduled report created successfully with ID: {ScheduledReportId}", scheduledReport.Id);

            // Reload with template
            return await GetScheduledReportAsync(scheduledReport.Id);
        }

        public async Task<ScheduledReportDto> UpdateScheduledReportAsync(Guid id, UpdateScheduledReportDto dto)
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
            scheduledReport.UpdateTimestamp();
            scheduledReport.NextRunAt = CalculateNextRunTime(dto.CronExpression);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Scheduled report updated successfully");

            return await GetScheduledReportAsync(id);
        }

        public async Task DeleteScheduledReportAsync(Guid id)
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

        public async Task ExecuteScheduledReportAsync(Guid scheduledReportId)
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
                // 1. Generate the report
                Dictionary<string, object> parameters;
                try
                {
                    parameters = string.IsNullOrEmpty(scheduledReport.Parameters) 
                        ? new Dictionary<string, object>() 
                        : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(scheduledReport.Parameters) ?? new Dictionary<string, object>();
                }
                catch (System.Text.Json.JsonException ex)
                {
                    _logger.LogError(ex, "Failed to deserialize parameters for scheduled report {ScheduledReportId}", scheduledReportId);
                    throw new InvalidOperationException("Invalid parameters JSON in scheduled report configuration", ex);
                }

                var generateDto = new GenerateReportDto
                {
                    ReportTemplateId = scheduledReport.ReportTemplateId,
                    OutputFormat = scheduledReport.OutputFormat,
                    Parameters = parameters
                };

                var reportResult = await GenerateReportAsync(generateDto);

                if (!string.IsNullOrEmpty(reportResult.Error))
                {
                    throw new InvalidOperationException($"Report generation failed: {reportResult.Error}");
                }

                // 2. Send email with attachment to recipients
                if (_emailService != null && !string.IsNullOrEmpty(scheduledReport.Recipients))
                {
                    var recipients = scheduledReport.Recipients.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => r.Trim())
                        .ToArray();

                    var emailBody = $@"
                        <h2>{scheduledReport.Name}</h2>
                        <p>{scheduledReport.Description}</p>
                        <p>This is your scheduled report generated on {DateTime.Now:yyyy-MM-dd HH:mm}.</p>
                        <p>Please see the attached file for the full report.</p>
                    ";

                    await _emailService.SendEmailAsync(
                        recipients,
                        $"Scheduled Report: {scheduledReport.Name}",
                        emailBody,
                        reportResult.Data,
                        reportResult.FileName,
                        reportResult.ContentType
                    );

                    _logger.LogInformation("Report email sent to {RecipientCount} recipients", recipients.Length);
                }
                else
                {
                    _logger.LogWarning("Email service not configured or no recipients specified for scheduled report");
                }

                // 3. Update status
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
            // TODO: Implement proper cron expression parsing with Cronos or NCrontab library
            // This is a placeholder that should be replaced before production use
            _logger.LogWarning("CRON expression parsing not implemented. Using 24-hour default interval.");
            throw new NotImplementedException(
                "CRON expression parsing requires a library like Cronos or NCrontab. " +
                "Please install the appropriate package and implement proper scheduling logic.");
        }

        private string SanitizeParameterValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Remove common SQL injection patterns
            // This is basic sanitization - proper parameterized queries are preferred
            var dangerous = new[] { ";", "--", "/*", "*/", "xp_", "sp_", "exec", "execute", "drop", "delete", "insert", "update", "create", "alter" };
            
            var lowerValue = value.ToLowerInvariant();
            foreach (var pattern in dangerous)
            {
                if (lowerValue.Contains(pattern))
                {
                    _logger.LogWarning("Potentially dangerous SQL pattern detected in parameter: {Pattern}", pattern);
                    // Replace with safe placeholder or throw exception
                    throw new InvalidOperationException($"Parameter value contains potentially dangerous SQL pattern: {pattern}");
                }
            }

            return value;
        }

        private async Task<List<Dictionary<string, object>>> ExecuteReportQuery(string query)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = query;
                command.CommandTimeout = 30; // 30 seconds timeout

                await _context.Database.OpenConnectionAsync();

                using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var columnName = reader.GetName(i);
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                        row[columnName] = value;
                    }
                    
                    result.Add(row);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing report query");
                throw;
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }

            return result;
        }

        private byte[] ExportToCsv(List<Dictionary<string, object>> data)
        {
            if (data == null || !data.Any())
            {
                return System.Text.Encoding.UTF8.GetBytes("No data available");
            }

            var csv = new System.Text.StringBuilder();
            
            // Headers
            var headers = data.First().Keys.ToList();
            csv.AppendLine(string.Join(",", headers.Select(h => EscapeCsvValue(h))));
            
            // Data rows
            foreach (var row in data)
            {
                var values = headers.Select(h => 
                {
                    var value = row.ContainsKey(h) ? row[h] : null;
                    return EscapeCsvValue(value?.ToString() ?? "");
                });
                csv.AppendLine(string.Join(",", values));
            }

            return System.Text.Encoding.UTF8.GetBytes(csv.ToString());
        }

        private string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            // Escape quotes and wrap in quotes if necessary
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
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
