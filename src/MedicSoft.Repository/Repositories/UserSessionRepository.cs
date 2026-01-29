using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task DeleteAllUserSessionsAsync(Guid userId, string tenantId)
        {
            await _context.UserSessions
                .Where(s => s.UserId == userId && s.TenantId == tenantId)
                .ExecuteDeleteAsync();
        }

        public async Task<int> GetActiveSessionCountAsync(Guid userId, string tenantId)
        {
            return await _context.UserSessions
                .CountAsync(s => s.UserId == userId &&
                                 s.TenantId == tenantId &&
                                 s.ExpiresAt > DateTime.UtcNow);
        }

        public async Task<List<UserSession>> GetRecentSessionsByUserIdAsync(Guid userId, string tenantId, int count)
        {
            // Include recently expired sessions (within last 30 days) for better anomaly detection
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            
            return await _context.UserSessions
                .Where(s => s.UserId == userId && 
                           s.TenantId == tenantId && 
                           s.StartedAt > thirtyDaysAgo)
                .OrderByDescending(s => s.StartedAt)
                .Take(count)
                .ToListAsync();
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
