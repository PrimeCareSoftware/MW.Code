using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a sales funnel tracking metric for registration process
    /// Tracks customer journey through registration steps to identify drop-off points
    /// </summary>
    public class SalesFunnelMetric : BaseEntity
    {
        /// <summary>
        /// Session identifier to group metrics from the same registration attempt
        /// </summary>
        public string SessionId { get; private set; }

        /// <summary>
        /// Current registration step (1-6)
        /// Step 1: Clinic Info
        /// Step 2: Address
        /// Step 3: Owner Info
        /// Step 4: Login Credentials
        /// Step 5: Plan Selection
        /// Step 6: Confirmation
        /// </summary>
        public int Step { get; private set; }

        /// <summary>
        /// Name of the step for easier identification
        /// </summary>
        public string StepName { get; private set; }

        /// <summary>
        /// Action performed: 'entered', 'completed', 'abandoned', 'converted'
        /// </summary>
        public string Action { get; private set; }

        /// <summary>
        /// JSON representation of data captured at this step (sanitized, no passwords)
        /// </summary>
        public string? CapturedData { get; private set; }

        /// <summary>
        /// Selected plan ID if applicable
        /// </summary>
        public string? PlanId { get; private set; }

        /// <summary>
        /// IP address of the user (for analytics, respecting LGPD)
        /// </summary>
        public string? IpAddress { get; private set; }

        /// <summary>
        /// User agent string for device/browser analytics
        /// </summary>
        public string? UserAgent { get; private set; }

        /// <summary>
        /// Referrer URL to track traffic source
        /// </summary>
        public string? Referrer { get; private set; }

        /// <summary>
        /// If registration was completed, the clinic ID
        /// </summary>
        public Guid? ClinicId { get; private set; }

        /// <summary>
        /// If registration was completed, the owner ID
        /// </summary>
        public Guid? OwnerId { get; private set; }

        /// <summary>
        /// Whether this session resulted in a successful conversion
        /// </summary>
        public bool IsConverted { get; private set; }

        /// <summary>
        /// Duration in milliseconds spent on this step
        /// </summary>
        public long? DurationMs { get; private set; }

        /// <summary>
        /// Additional metadata (UTM parameters, A/B test variants, etc.)
        /// </summary>
        public string? Metadata { get; private set; }

        private SalesFunnelMetric()
        {
            // EF Constructor
            SessionId = null!;
            StepName = null!;
            Action = null!;
        }

        public SalesFunnelMetric(
            string sessionId,
            int step,
            string stepName,
            string action,
            string? capturedData = null,
            string? planId = null,
            string? ipAddress = null,
            string? userAgent = null,
            string? referrer = null,
            long? durationMs = null,
            string? metadata = null) : base(TenantConstants.SystemTenantId) // Sales metrics are system-wide, not tenant-specific
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));

            if (step < 1 || step > 6)
                throw new ArgumentException("Step must be between 1 and 6", nameof(step));

            if (string.IsNullOrWhiteSpace(stepName))
                throw new ArgumentException("Step name cannot be empty", nameof(stepName));

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action cannot be empty", nameof(action));

            SessionId = sessionId;
            Step = step;
            StepName = stepName;
            Action = action;
            CapturedData = capturedData;
            PlanId = planId;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            Referrer = referrer;
            DurationMs = durationMs;
            Metadata = metadata;
            IsConverted = false;
        }

        /// <summary>
        /// Mark this session as converted with clinic and owner IDs
        /// </summary>
        public void MarkAsConverted(Guid clinicId, Guid ownerId)
        {
            IsConverted = true;
            ClinicId = clinicId;
            OwnerId = ownerId;
            UpdateTimestamp();
        }

        /// <summary>
        /// Update the captured data for this metric
        /// </summary>
        public void UpdateCapturedData(string capturedData)
        {
            CapturedData = capturedData;
            UpdateTimestamp();
        }

        /// <summary>
        /// Get step name from step number
        /// </summary>
        public static string GetStepName(int step)
        {
            return step switch
            {
                1 => "Clinic Information",
                2 => "Address",
                3 => "Owner Information",
                4 => "Login Credentials",
                5 => "Plan Selection",
                6 => "Confirmation",
                _ => "Unknown"
            };
        }
    }
}
