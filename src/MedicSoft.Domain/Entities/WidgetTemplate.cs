using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Pre-built widget template for Phase 3 Analytics and BI
    /// Provides library of ready-to-use widgets for common metrics
    /// </summary>
    [Table("WidgetTemplates")]
    public class WidgetTemplate : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// Category: financial, customer, operational, clinical
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        /// <summary>
        /// Widget type: line, bar, pie, metric, table
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        /// <summary>
        /// Default JSON configuration for this template
        /// </summary>
        [Column(TypeName = "TEXT")]
        public string DefaultConfig { get; set; }

        /// <summary>
        /// Default SQL query or API endpoint
        /// </summary>
        [Column(TypeName = "TEXT")]
        public string DefaultQuery { get; set; }

        /// <summary>
        /// Indicates if this is a system-provided template (cannot be deleted)
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Icon name for UI display
        /// </summary>
        [MaxLength(50)]
        public string Icon { get; set; }
    }
}
