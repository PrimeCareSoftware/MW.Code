using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly MedicSoftDbContext _context;

        public UserSessionRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSession session)
        {
            await _context.UserSessions.AddAsync(session);
        }

        public async Task<UserSession?> GetBySessionIdAsync(Guid userId, string sessionId, string tenantId)
        {
            return await _context.UserSessions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId &&
                                        s.SessionId == sessionId &&
                                        s.TenantId == tenantId);
        }

        public async Task<bool> IsSessionValidAsync(Guid userId, string sessionId, string tenantId)
        {
            var session = await GetBySessionIdAsync(userId, sessionId, tenantId);
            return session != null && session.IsValid();
        }

        public Task DeleteAsync(UserSession session)
        {
            _context.UserSessions.Remove(session);
            return Task.CompletedTask;
        }

        public async Task DeleteExpiredSessionsAsync()
        {
            await _context.UserSessions
                .Where(s => s.ExpiresAt < DateTime.UtcNow)
                .ExecuteDeleteAsync();
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
