using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Dashboard widget entity for Phase 3 Analytics and BI
    /// Represents individual widgets on a custom dashboard
    /// </summary>
    [Table("DashboardWidgets")]
    public class DashboardWidget : BaseEntity
    {
        [Required]
        public Guid DashboardId { get; set; }

        [ForeignKey(nameof(DashboardId))]
        public virtual CustomDashboard Dashboard { get; set; }

        /// <summary>
        /// Widget type: line, bar, pie, metric, table, map, markdown
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// JSON configuration for widget-specific settings
        /// Contains chart options, colors, axes configuration, etc.
        /// </summary>
        [Column(TypeName = "TEXT")]
        public string Config { get; set; }

        /// <summary>
        /// SQL query or API endpoint for data retrieval
        /// If SQL query, must pass security validation (SELECT only)
        /// </summary>
        [Column(TypeName = "TEXT")]
        public string Query { get; set; }

        /// <summary>
        /// Refresh interval in seconds (0 = manual refresh only)
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// GridStack X position (column)
        /// </summary>
        public int GridX { get; set; }

        /// <summary>
        /// GridStack Y position (row)
        /// </summary>
        public int GridY { get; set; }

        /// <summary>
        /// GridStack width in grid columns
        /// </summary>
        public int GridWidth { get; set; }

        /// <summary>
        /// GridStack height in grid rows
        /// </summary>
        public int GridHeight { get; set; }
    }
}
