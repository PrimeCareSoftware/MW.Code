using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
    {
        private readonly MedicSoftDbContext _context;

        public EmailVerificationTokenRepository(MedicSoftDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<EmailVerificationToken?> GetByCodeAndUserIdAsync(string code, Guid userId, string tenantId)
        {
            return await _context.EmailVerificationTokens
                .Where(t => t.Code == code && t.UserId == userId && t.TenantId == tenantId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountRecentTokensAsync(Guid userId, string tenantId, TimeSpan timeWindow)
        {
            var cutoffTime = DateTime.UtcNow - timeWindow;
            return await _context.EmailVerificationTokens
                .Where(t => t.UserId == userId && 
                           t.TenantId == tenantId && 
                           t.CreatedAt >= cutoffTime)
                .CountAsync();
        }

        public async Task AddAsync(EmailVerificationToken token)
        {
            await _context.EmailVerificationTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailVerificationToken token)
        {
            _context.EmailVerificationTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpiredTokensAsync(string tenantId)
        {
            var expiredTokens = await _context.EmailVerificationTokens
                .Where(t => t.TenantId == tenantId && t.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            _context.EmailVerificationTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }
    }
}
