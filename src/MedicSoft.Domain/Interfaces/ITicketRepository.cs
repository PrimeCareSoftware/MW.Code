using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<IEnumerable<Ticket>> GetUserTicketsAsync(Guid userId, string tenantId);
        Task<IEnumerable<Ticket>> GetClinicTicketsAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<Ticket>> GetAllTicketsAsync(TicketStatus? status, TicketType? type, Guid? clinicId, string? tenantId);
        Task<Ticket?> GetTicketWithDetailsAsync(Guid ticketId, string tenantId);
        Task<TicketComment> AddCommentAsync(TicketComment comment);
        Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment);
        Task<IEnumerable<TicketComment>> GetTicketCommentsAsync(Guid ticketId, string tenantId, bool includeInternal);
        Task<IEnumerable<TicketAttachment>> GetTicketAttachmentsAsync(Guid ticketId, string tenantId);
        Task<int> GetTicketCountAsync(TicketStatus? status, TicketType? type, Guid? clinicId, string? tenantId);
    }
}
