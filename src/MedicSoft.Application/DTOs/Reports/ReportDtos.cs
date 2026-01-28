using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.Reports
{
    /// <summary>
    /// DTO for report template display
    /// </summary>
    public class ReportTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Configuration { get; set; }
        public string Query { get; set; }
        public bool IsSystem { get; set; }
        public string Icon { get; set; }
        public string SupportedFormats { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a new report template
    /// </summary>
    public class CreateReportTemplateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Configuration { get; set; }
        public string Query { get; set; }
        public string Icon { get; set; }
        public string SupportedFormats { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing report template
    /// </summary>
    public class UpdateReportTemplateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Configuration { get; set; }
        public string Query { get; set; }
        public string Icon { get; set; }
        public string SupportedFormats { get; set; }
    }

    /// <summary>
    /// DTO for scheduled report display
    /// </summary>
    public class ScheduledReportDto
    {
        public int Id { get; set; }
        public int ReportTemplateId { get; set; }
        public string ReportTemplateName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CronExpression { get; set; }
        public string OutputFormat { get; set; }
        public string Recipients { get; set; }
        public string Parameters { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAt { get; set; }
        public string LastRunStatus { get; set; }
        public string LastRunError { get; set; }
    }

    /// <summary>
    /// DTO for creating a new scheduled report
    /// </summary>
    public class CreateScheduledReportDto
    {
        public int ReportTemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CronExpression { get; set; }
        public string OutputFormat { get; set; }
        public string Recipients { get; set; }
        public string Parameters { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for updating an existing scheduled report
    /// </summary>
    public class UpdateScheduledReportDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CronExpression { get; set; }
        public string OutputFormat { get; set; }
        public string Recipients { get; set; }
        public string Parameters { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for generating a report on-demand
    /// </summary>
    public class GenerateReportDto
    {
        public int ReportTemplateId { get; set; }
        public string OutputFormat { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// DTO for report generation result
    /// </summary>
    public class ReportResultDto
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string ContentType { get; set; }
        public string Error { get; set; }
    }
}
