using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Scheduled report entity for Phase 3 Analytics and BI
    /// Manages automated report generation and delivery
    /// </summary>
    [Table("ScheduledReports")]
    public class ScheduledReport : BaseEntity
    {
        [Required]
        public Guid ReportTemplateId { get; set; }

        [ForeignKey(nameof(ReportTemplateId))]
        public virtual ReportTemplate ReportTemplate { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Cron expression for scheduling (e.g., "0 0 * * *" for daily at midnight)
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CronExpression { get; set; }

        /// <summary>
        /// Output format: pdf, excel, csv
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string OutputFormat { get; set; }

        /// <summary>
        /// Comma-separated email addresses for report delivery
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Recipients { get; set; }

        /// <summary>
        /// JSON string with report parameters
        /// </summary>
        [Column(TypeName = "TEXT")]
        public string Parameters { get; set; }

        /// <summary>
        /// Indicates if the scheduled report is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// User ID of the schedule creator
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Last time the report was executed
        /// </summary>
        public DateTime? LastRunAt { get; set; }

        /// <summary>
        /// Next scheduled execution time
        /// </summary>
        public DateTime? NextRunAt { get; set; }

        /// <summary>
        /// Status of last execution: success, failed, running
        /// </summary>
        [MaxLength(50)]
        public string LastRunStatus { get; set; }

        /// <summary>
        /// Error message if last execution failed
        /// </summary>
        [MaxLength(2000)]
        public string LastRunError { get; set; }
    }
}
