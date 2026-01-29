using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Report template entity for Phase 3 Analytics and BI
    /// Pre-built report templates for common business reports
    /// </summary>
    [Table("ReportTemplates")]
    public class ReportTemplate : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Category: financial, operational, clinical, compliance
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        /// <summary>
        /// JSON configuration for report parameters and layout
        /// </summary>
        [Column(TypeName = "TEXT")]
        public string Configuration { get; set; }

        /// <summary>
        /// SQL query or endpoint for data retrieval
        /// </summary>
        [Column(TypeName = "TEXT")]
        public string Query { get; set; }

        /// <summary>
        /// Indicates if this is a system-provided template (cannot be deleted)
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Icon name for UI display
        /// </summary>
        [MaxLength(50)]
        public string Icon { get; set; }

        /// <summary>
        /// Output format options: pdf, excel, csv
        /// </summary>
        [MaxLength(100)]
        public string SupportedFormats { get; set; }

        /// <summary>
        /// Collection of scheduled reports using this template
        /// </summary>
        public virtual ICollection<ScheduledReport> ScheduledReports { get; set; } = new List<ScheduledReport>();
    }
}
