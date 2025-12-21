using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class TicketHistory : BaseEntity
    {
        public Guid TicketId { get; private set; }
        public TicketStatus OldStatus { get; private set; }
        public TicketStatus NewStatus { get; private set; }
        public Guid ChangedById { get; private set; }
        public string ChangedByName { get; private set; }
        public string? Comment { get; private set; }
        public DateTime ChangedAt { get; private set; }

        // Navigation property
        public Ticket Ticket { get; private set; } = null!;

        private TicketHistory()
        {
            // EF Core constructor
            ChangedByName = null!;
        }

        public TicketHistory(
            Guid ticketId,
            TicketStatus oldStatus,
            TicketStatus newStatus,
            Guid changedById,
            string changedByName,
            string? comment,
            string tenantId) : base(tenantId)
        {
            TicketId = ticketId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedById = changedById;
            ChangedByName = changedByName ?? throw new ArgumentNullException(nameof(changedByName));
            Comment = comment;
            ChangedAt = DateTime.UtcNow;
        }
    }
}
