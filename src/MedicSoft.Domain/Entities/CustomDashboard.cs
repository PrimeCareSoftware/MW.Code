using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Custom dashboard entity for Phase 3 Analytics and BI
    /// Allows users to create personalized dashboards with drag-and-drop widgets
    /// </summary>
    [Table("CustomDashboards")]
    public class CustomDashboard : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// JSON string containing grid layout configuration
        /// Used by GridStack for positioning widgets
        /// </summary>
        [Column(TypeName = "TEXT")]
        public string Layout { get; set; }

        /// <summary>
        /// Indicates if this is the default dashboard for the user
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Indicates if dashboard is visible to other users
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// User ID of the dashboard creator
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string CreatedBy { get; set; }

        public new DateTime CreatedAt { get; set; }
        
        public new DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Collection of widgets associated with this dashboard
        /// </summary>
        public virtual ICollection<DashboardWidget> Widgets { get; set; } = new List<DashboardWidget>();
    }
}
