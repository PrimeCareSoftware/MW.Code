using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class TicketComment : BaseEntity
    {
        public Guid TicketId { get; private set; }
        public string Comment { get; private set; }
        public Guid AuthorId { get; private set; }
        public string AuthorName { get; private set; }
        public bool IsInternal { get; private set; }
        public bool IsSystemOwner { get; private set; }

        // Navigation property
        public Ticket Ticket { get; private set; } = null!;

        private TicketComment()
        {
            // EF Core constructor
            Comment = null!;
            AuthorName = null!;
        }

        public TicketComment(
            Guid ticketId,
            string comment,
            Guid authorId,
            string authorName,
            bool isInternal,
            bool isSystemOwner,
            string tenantId) : base(tenantId)
        {
            TicketId = ticketId;
            Comment = comment ?? throw new ArgumentNullException(nameof(comment));
            AuthorId = authorId;
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
            IsInternal = isInternal;
            IsSystemOwner = isSystemOwner;
        }
    }
}
