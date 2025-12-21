using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(Guid userId, string tenantId)
        {
            return await _dbSet
                .Where(t => t.UserId == userId && t.TenantId == tenantId)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetClinicTicketsAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(t => t.ClinicId == clinicId && t.TenantId == tenantId)
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(
            TicketStatus? status,
            TicketType? type,
            Guid? clinicId,
            string? tenantId)
        {
            var query = _dbSet.AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (clinicId.HasValue)
                query = query.Where(t => t.ClinicId == clinicId.Value);

            if (!string.IsNullOrEmpty(tenantId))
                query = query.Where(t => t.TenantId == tenantId);

            return await query
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }

        public async Task<Ticket?> GetTicketWithDetailsAsync(Guid ticketId, string tenantId)
        {
            return await _dbSet
                .Include(t => t.Comments)
                .Include(t => t.Attachments)
                .Include(t => t.History)
                .Where(t => t.Id == ticketId && t.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<TicketComment> AddCommentAsync(TicketComment comment)
        {
            await _context.TicketComments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<TicketAttachment> AddAttachmentAsync(TicketAttachment attachment)
        {
            await _context.TicketAttachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<IEnumerable<TicketComment>> GetTicketCommentsAsync(Guid ticketId, string tenantId, bool includeInternal)
        {
            var query = _context.TicketComments
                .Where(c => c.TicketId == ticketId && c.TenantId == tenantId);

            if (!includeInternal)
                query = query.Where(c => !c.IsInternal);

            return await query
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TicketAttachment>> GetTicketAttachmentsAsync(Guid ticketId, string tenantId)
        {
            return await _context.TicketAttachments
                .Where(a => a.TicketId == ticketId && a.TenantId == tenantId)
                .OrderBy(a => a.UploadedAt)
                .ToListAsync();
        }

        public async Task<int> GetTicketCountAsync(
            TicketStatus? status,
            TicketType? type,
            Guid? clinicId,
            string? tenantId)
        {
            var query = _dbSet.AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (type.HasValue)
                query = query.Where(t => t.Type == type.Value);

            if (clinicId.HasValue)
                query = query.Where(t => t.ClinicId == clinicId.Value);

            if (!string.IsNullOrEmpty(tenantId))
                query = query.Where(t => t.TenantId == tenantId);

            return await query.CountAsync();
        }
    }
}
