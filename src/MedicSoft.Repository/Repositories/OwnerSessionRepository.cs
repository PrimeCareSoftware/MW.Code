using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class OwnerSessionRepository : IOwnerSessionRepository
    {
        private readonly MedicSoftDbContext _context;

        public OwnerSessionRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OwnerSession session)
        {
            await _context.OwnerSessions.AddAsync(session);
        }

        public async Task<OwnerSession?> GetBySessionIdAsync(Guid ownerId, string sessionId, string tenantId)
        {
            return await _context.OwnerSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.OwnerId == ownerId &&
                                        s.SessionId == sessionId &&
                                        s.TenantId == tenantId);
        }

        public async Task<bool> IsSessionValidAsync(Guid ownerId, string sessionId, string tenantId)
        {
            var session = await GetBySessionIdAsync(ownerId, sessionId, tenantId);
            return session != null && session.IsValid();
        }

        public async Task DeleteAsync(OwnerSession session)
        {
            _context.OwnerSessions.Remove(session);
        }

        public async Task DeleteExpiredSessionsAsync()
        {
            await _context.OwnerSessions
                .Where(s => s.ExpiresAt < DateTime.UtcNow)
                .ExecuteDeleteAsync();
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
