using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly MedicSoftDbContext _context;

        public PasswordResetTokenRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<PasswordResetToken?> GetByTokenAsync(string token, string tenantId)
        {
            return await _context.PasswordResetTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token && t.TenantId == tenantId);
        }

        public async Task<PasswordResetToken?> GetActiveByUserIdAsync(Guid userId, string tenantId)
        {
            return await _context.PasswordResetTokens
                .Include(t => t.User)
                .Where(t => t.UserId == userId && 
                           t.TenantId == tenantId && 
                           !t.IsUsed && 
                           t.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PasswordResetToken token)
        {
            _context.PasswordResetTokens.Update(token);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateAllByUserIdAsync(Guid userId, string tenantId)
        {
            var tokens = await _context.PasswordResetTokens
                .Where(t => t.UserId == userId && t.TenantId == tenantId && !t.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
            {
                if (!token.IsUsed)
                {
                    token.MarkAsUsed();
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
