using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for two-factor authentication tokens
/// </summary>
public class TwoFactorTokenRepository : ITwoFactorTokenRepository
{
    private readonly PatientPortalDbContext _context;

    public TwoFactorTokenRepository(PatientPortalDbContext context)
    {
        _context = context;
    }

    public async Task<TwoFactorToken> CreateAsync(TwoFactorToken token)
    {
        _context.TwoFactorTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<TwoFactorToken?> GetByCodeAsync(string code, Guid patientUserId)
    {
        return await _context.TwoFactorTokens
            .FirstOrDefaultAsync(t => t.Code == code && t.PatientUserId == patientUserId);
    }

    public async Task<TwoFactorToken?> GetMostRecentValidTokenAsync(Guid patientUserId, string purpose)
    {
        return await _context.TwoFactorTokens
            .Where(t => t.PatientUserId == patientUserId && 
                       t.Purpose == purpose && 
                       !t.IsUsed && 
                       t.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(TwoFactorToken token)
    {
        _context.TwoFactorTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpiredTokensAsync()
    {
        var expiredTokens = await _context.TwoFactorTokens
            .Where(t => t.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        _context.TwoFactorTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountRecentTokensAsync(Guid patientUserId, TimeSpan timeWindow)
    {
        var cutoffTime = DateTime.UtcNow.Subtract(timeWindow);
        return await _context.TwoFactorTokens
            .CountAsync(t => t.PatientUserId == patientUserId && t.CreatedAt >= cutoffTime);
    }
}
