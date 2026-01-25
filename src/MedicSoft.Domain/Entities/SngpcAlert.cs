using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a persistent SNGPC compliance alert that requires attention.
    /// Alerts are generated for deadline warnings, compliance violations, and anomalies.
    /// Provides audit trail, acknowledgement tracking, and resolution workflow.
    /// Complies with ANVISA RDC 27/2007 requirements for monitoring and compliance reporting.
    /// </summary>
    public class SngpcAlert : BaseEntity
    {
        public AlertType Type { get; private set; }
        public AlertSeverity Severity { get; private set; }
        
        // Alert Content
        public string Title { get; private set; }
        public string Description { get; private set; }
        
        // Related Entities
        public Guid? RelatedReportId { get; private set; }
        public Guid? RelatedRegistryId { get; private set; }
        public Guid? RelatedBalanceId { get; private set; }
        public string? RelatedMedication { get; private set; }
        
        // Additional data (JSON serialized)
        public string? AdditionalData { get; private set; }
        
        // Action Tracking
        public DateTime? AcknowledgedAt { get; private set; }
        public Guid? AcknowledgedByUserId { get; private set; }
        public string? AcknowledgmentNotes { get; private set; }
        
        public DateTime? ResolvedAt { get; private set; }
        public Guid? ResolvedByUserId { get; private set; }
        public string? Resolution { get; private set; }
        
        // Navigation Properties
        public SNGPCReport? RelatedReport { get; private set; }
        public ControlledMedicationRegistry? RelatedRegistry { get; private set; }
        public MonthlyControlledBalance? RelatedBalance { get; private set; }
        public User? AcknowledgedBy { get; private set; }
        public User? ResolvedBy { get; private set; }

        private SngpcAlert()
        {
            // EF Constructor
            Title = string.Empty;
            Description = string.Empty;
        }

        public SngpcAlert(
            string tenantId,
            AlertType type,
            AlertSeverity severity,
            string title,
            string description) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            Type = type;
            Severity = severity;
            Title = title;
            Description = description;
        }

        /// <summary>
        /// Factory method to create alert with related report
        /// </summary>
        public static SngpcAlert CreateReportAlert(
            string tenantId,
            AlertType type,
            AlertSeverity severity,
            string title,
            string description,
            Guid reportId)
        {
            var alert = new SngpcAlert(tenantId, type, severity, title, description);
            alert.RelatedReportId = reportId;
            return alert;
        }

        /// <summary>
        /// Factory method to create alert with related registry entry
        /// </summary>
        public static SngpcAlert CreateRegistryAlert(
            string tenantId,
            AlertType type,
            AlertSeverity severity,
            string title,
            string description,
            Guid registryId,
            string? medicationName = null)
        {
            var alert = new SngpcAlert(tenantId, type, severity, title, description);
            alert.RelatedRegistryId = registryId;
            alert.RelatedMedication = medicationName;
            return alert;
        }

        /// <summary>
        /// Factory method to create alert with related balance
        /// </summary>
        public static SngpcAlert CreateBalanceAlert(
            string tenantId,
            AlertType type,
            AlertSeverity severity,
            string title,
            string description,
            Guid balanceId,
            string? medicationName = null)
        {
            var alert = new SngpcAlert(tenantId, type, severity, title, description);
            alert.RelatedBalanceId = balanceId;
            alert.RelatedMedication = medicationName;
            return alert;
        }

        /// <summary>
        /// Sets additional JSON data for the alert
        /// </summary>
        public void SetAdditionalData(string jsonData)
        {
            AdditionalData = jsonData;
            UpdateTimestamp();
        }

        /// <summary>
        /// Marks the alert as acknowledged by a user
        /// </summary>
        public void Acknowledge(Guid userId, string? notes = null)
        {
            if (AcknowledgedAt.HasValue)
                throw new InvalidOperationException("Alert has already been acknowledged");

            AcknowledgedAt = DateTime.UtcNow;
            AcknowledgedByUserId = userId;
            AcknowledgmentNotes = notes;
            UpdateTimestamp();
        }

        /// <summary>
        /// Marks the alert as resolved by a user
        /// </summary>
        public void Resolve(Guid userId, string resolution)
        {
            if (string.IsNullOrWhiteSpace(resolution))
                throw new ArgumentException("Resolution description is required", nameof(resolution));

            if (ResolvedAt.HasValue)
                throw new InvalidOperationException("Alert has already been resolved");

            ResolvedAt = DateTime.UtcNow;
            ResolvedByUserId = userId;
            Resolution = resolution;
            UpdateTimestamp();
        }

        /// <summary>
        /// Reopens a resolved alert
        /// </summary>
        public void Reopen()
        {
            if (!ResolvedAt.HasValue)
                throw new InvalidOperationException("Cannot reopen an alert that is not resolved");

            ResolvedAt = null;
            ResolvedByUserId = null;
            Resolution = null;
            UpdateTimestamp();
        }

        /// <summary>
        /// Checks if the alert is active (not resolved)
        /// </summary>
        public bool IsActive() => !ResolvedAt.HasValue;

        /// <summary>
        /// Checks if the alert has been acknowledged
        /// </summary>
        public bool IsAcknowledged() => AcknowledgedAt.HasValue;

        /// <summary>
        /// Gets the age of the alert in days
        /// </summary>
        public int GetAgeInDays() => (DateTime.UtcNow - CreatedAt).Days;
    }
}
