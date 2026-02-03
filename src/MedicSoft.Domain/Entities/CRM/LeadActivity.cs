using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Represents an activity or interaction with a lead
    /// </summary>
    public class LeadActivity : BaseEntity
    {
        /// <summary>
        /// Lead this activity belongs to
        /// </summary>
        public Guid LeadId { get; private set; }

        /// <summary>
        /// Type of activity
        /// </summary>
        public ActivityType Type { get; private set; }

        /// <summary>
        /// Title/subject of the activity
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Description or notes about the activity
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// User who performed the activity
        /// </summary>
        public Guid? PerformedByUserId { get; private set; }

        /// <summary>
        /// Name of the user (cached for display)
        /// </summary>
        public string? PerformedByUserName { get; private set; }

        /// <summary>
        /// Date/time when activity occurred
        /// </summary>
        public DateTime ActivityDate { get; private set; }

        /// <summary>
        /// Duration in minutes (for calls, meetings)
        /// </summary>
        public int? DurationMinutes { get; private set; }

        /// <summary>
        /// Outcome or result of the activity
        /// </summary>
        public string? Outcome { get; private set; }

        /// <summary>
        /// Navigation property to Lead
        /// </summary>
        public virtual Lead Lead { get; private set; }

        private LeadActivity()
        {
            // EF Constructor
            Title = null!;
            Lead = null!;
        }

        public LeadActivity(
            Guid leadId,
            ActivityType type,
            string title,
            string? description = null,
            Guid? performedByUserId = null,
            string? performedByUserName = null,
            DateTime? activityDate = null,
            int? durationMinutes = null,
            string? outcome = null) : base(TenantConstants.SystemTenantId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            LeadId = leadId;
            Type = type;
            Title = title;
            Description = description;
            PerformedByUserId = performedByUserId;
            PerformedByUserName = performedByUserName;
            ActivityDate = activityDate ?? DateTime.UtcNow;
            DurationMinutes = durationMinutes;
            Outcome = outcome;
        }

        /// <summary>
        /// Update activity details
        /// </summary>
        public void Update(
            string? title = null,
            string? description = null,
            int? durationMinutes = null,
            string? outcome = null)
        {
            if (!string.IsNullOrWhiteSpace(title))
                Title = title;

            if (description != null)
                Description = description;

            if (durationMinutes.HasValue)
                DurationMinutes = durationMinutes;

            if (outcome != null)
                Outcome = outcome;

            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Types of activities that can be performed on a lead
    /// </summary>
    public enum ActivityType
    {
        PhoneCall = 0,
        Email = 1,
        Meeting = 2,
        Note = 3,
        StatusChange = 4,
        Assignment = 5,
        FollowUpScheduled = 6,
        Other = 99
    }
}
