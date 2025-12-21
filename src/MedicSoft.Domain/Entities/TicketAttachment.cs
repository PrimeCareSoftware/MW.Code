using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class TicketAttachment : BaseEntity
    {
        public Guid TicketId { get; private set; }
        public string FileName { get; private set; }
        public string FileUrl { get; private set; }
        public string ContentType { get; private set; }
        public long FileSize { get; private set; }
        public DateTime UploadedAt { get; private set; }

        // Navigation property
        public Ticket Ticket { get; private set; } = null!;

        private TicketAttachment()
        {
            // EF Core constructor
            FileName = null!;
            FileUrl = null!;
            ContentType = null!;
        }

        public TicketAttachment(
            Guid ticketId,
            string fileName,
            string fileUrl,
            string contentType,
            long fileSize,
            string tenantId) : base(tenantId)
        {
            TicketId = ticketId;
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            FileUrl = fileUrl ?? throw new ArgumentNullException(nameof(fileUrl));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            FileSize = fileSize;
            UploadedAt = DateTime.UtcNow;
        }
    }
}
