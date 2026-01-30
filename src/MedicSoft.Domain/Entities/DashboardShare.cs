using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Dashboard sharing entity for Category 4.1
    /// Allows dashboards to be shared with specific users or roles
    /// </summary>
    [Table("DashboardShares")]
    public class DashboardShare : BaseEntity
    {
        [Required]
        public Guid DashboardId { get; set; }

        [ForeignKey(nameof(DashboardId))]
        public virtual CustomDashboard Dashboard { get; set; }

        /// <summary>
        /// User ID to share dashboard with (null if shared with role)
        /// </summary>
        [MaxLength(450)]
        public string SharedWithUserId { get; set; }

        /// <summary>
        /// Role name to share dashboard with (null if shared with specific user)
        /// </summary>
        [MaxLength(100)]
        public string SharedWithRole { get; set; }

        /// <summary>
        /// Permission level: View, Edit
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string PermissionLevel { get; set; }

        /// <summary>
        /// User who shared the dashboard
        /// </summary>
        [Required]
        [MaxLength(450)]
        public string SharedBy { get; set; }

        /// <summary>
        /// Date when the share expires (null for permanent)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }
    }
}
