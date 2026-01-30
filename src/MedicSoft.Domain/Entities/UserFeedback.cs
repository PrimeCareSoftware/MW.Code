using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents user feedback collected through the in-app feedback widget.
    /// Part of Phase 2 validation for early adopter feedback collection.
    /// </summary>
    public class UserFeedback : BaseEntity
    {
        public string UserId { get; private set; }
        public FeedbackType Type { get; private set; }
        public FeedbackSeverity? Severity { get; private set; }
        public string Page { get; private set; }
        public string Description { get; private set; }
        public string? ScreenshotUrl { get; private set; }
        public string BrowserName { get; private set; }
        public string BrowserVersion { get; private set; }
        public string OperatingSystem { get; private set; }
        public string ScreenResolution { get; private set; }
        public FeedbackStatus Status { get; private set; }
        public string? ResolvedBy { get; private set; }
        public DateTime? ResolvedAt { get; private set; }
        public string? ResolutionNotes { get; private set; }

        // Navigation property
        public User? User { get; private set; }

        private UserFeedback()
        {
            // EF Constructor
            UserId = null!;
            Page = null!;
            Description = null!;
            BrowserName = null!;
            BrowserVersion = null!;
            OperatingSystem = null!;
            ScreenResolution = null!;
        }

        public UserFeedback(
            string userId,
            FeedbackType type,
            string page,
            string description,
            string browserName,
            string browserVersion,
            string operatingSystem,
            string screenResolution,
            string tenantId,
            FeedbackSeverity? severity = null,
            string? screenshotUrl = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(page))
                throw new ArgumentException("Page cannot be empty", nameof(page));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            UserId = userId;
            Type = type;
            Severity = severity;
            Page = page;
            Description = description;
            ScreenshotUrl = screenshotUrl;
            BrowserName = browserName;
            BrowserVersion = browserVersion;
            OperatingSystem = operatingSystem;
            ScreenResolution = screenResolution;
            Status = FeedbackStatus.New;
        }

        public void UpdateStatus(FeedbackStatus newStatus, string? resolvedBy = null, string? resolutionNotes = null)
        {
            Status = newStatus;
            
            if (newStatus == FeedbackStatus.Resolved || newStatus == FeedbackStatus.WontFix)
            {
                ResolvedBy = resolvedBy;
                ResolvedAt = DateTime.UtcNow;
                ResolutionNotes = resolutionNotes;
            }
            
            UpdateTimestamp();
        }
    }

    public enum FeedbackType
    {
        Bug = 1,
        FeatureRequest = 2,
        UxIssue = 3,
        Other = 4
    }

    public enum FeedbackSeverity
    {
        Critical = 1,
        High = 2,
        Medium = 3,
        Low = 4
    }

    public enum FeedbackStatus
    {
        New = 1,
        InProgress = 2,
        Resolved = 3,
        WontFix = 4
    }
}
