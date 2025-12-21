using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum TicketStatus
    {
        Open = 0,
        InAnalysis = 1,
        InProgress = 2,
        Blocked = 3,
        Completed = 4,
        Cancelled = 5
    }

    public enum TicketType
    {
        BugReport = 0,
        FeatureRequest = 1,
        SystemAdjustment = 2,
        FinancialIssue = 3,
        TechnicalSupport = 4,
        UserSupport = 5,
        Other = 6
    }

    public enum TicketPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

    public class Ticket : BaseEntity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public TicketType Type { get; private set; }
        public TicketStatus Status { get; private set; }
        public TicketPriority Priority { get; private set; }
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public string UserEmail { get; private set; }
        public Guid? ClinicId { get; private set; }
        public string? ClinicName { get; private set; }
        public Guid? AssignedToId { get; private set; }
        public string? AssignedToName { get; private set; }
        public DateTime? LastStatusChangeAt { get; private set; }

        // Navigation properties
        private readonly List<TicketComment> _comments = new();
        public IReadOnlyCollection<TicketComment> Comments => _comments.AsReadOnly();

        private readonly List<TicketAttachment> _attachments = new();
        public IReadOnlyCollection<TicketAttachment> Attachments => _attachments.AsReadOnly();

        private readonly List<TicketHistory> _history = new();
        public IReadOnlyCollection<TicketHistory> History => _history.AsReadOnly();

        private Ticket()
        {
            // EF Core constructor
            Title = null!;
            Description = null!;
            UserName = null!;
            UserEmail = null!;
        }

        public Ticket(
            string title,
            string description,
            TicketType type,
            TicketPriority priority,
            Guid userId,
            string userName,
            string userEmail,
            Guid? clinicId,
            string? clinicName,
            string tenantId) : base(tenantId)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Type = type;
            Status = TicketStatus.Open;
            Priority = priority;
            UserId = userId;
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
            ClinicId = clinicId;
            ClinicName = clinicName;
            LastStatusChangeAt = DateTime.UtcNow;
        }

        public void Update(string? title, string? description, TicketType? type, TicketPriority? priority)
        {
            if (!string.IsNullOrEmpty(title))
                Title = title;

            if (!string.IsNullOrEmpty(description))
                Description = description;

            if (type.HasValue)
                Type = type.Value;

            if (priority.HasValue)
                Priority = priority.Value;

            UpdateTimestamp();
        }

        public void UpdateStatus(TicketStatus newStatus, Guid changedById, string changedByName, string? comment = null)
        {
            var oldStatus = Status;
            Status = newStatus;
            LastStatusChangeAt = DateTime.UtcNow;
            UpdateTimestamp();

            // Add history entry
            var history = new TicketHistory(Id, oldStatus, newStatus, changedById, changedByName, comment, TenantId);
            _history.Add(history);
        }

        public void AssignTo(Guid? assignedToId, string? assignedToName)
        {
            AssignedToId = assignedToId;
            AssignedToName = assignedToName;
            UpdateTimestamp();
        }

        public void AddComment(Guid authorId, string authorName, string comment, bool isInternal, bool isSystemOwner)
        {
            var ticketComment = new TicketComment(Id, comment, authorId, authorName, isInternal, isSystemOwner, TenantId);
            _comments.Add(ticketComment);
            UpdateTimestamp();
        }

        public void AddAttachment(string fileName, string fileUrl, string contentType, long fileSize)
        {
            var attachment = new TicketAttachment(Id, fileName, fileUrl, contentType, fileSize, TenantId);
            _attachments.Add(attachment);
            UpdateTimestamp();
        }
    }
}
