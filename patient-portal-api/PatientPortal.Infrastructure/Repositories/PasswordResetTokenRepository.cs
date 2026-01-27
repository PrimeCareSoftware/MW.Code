using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for password reset tokens
/// </summary>
public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly PatientPortalDbContext _context;

    public PasswordResetTokenRepository(PatientPortalDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken> CreateAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
    {
        return await _context.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task UpdateAsync(PasswordResetToken token)
    {
        _context.PasswordResetTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasValidTokenAsync(Guid patientUserId)
    {
        return await _context.PasswordResetTokens
            .AnyAsync(t => t.PatientUserId == patientUserId && 
                          !t.IsUsed && 
                          t.ExpiresAt > DateTime.UtcNow);
    }

    public async Task RevokeAllActiveTokensAsync(Guid patientUserId)
    {
        var activeTokens = await _context.PasswordResetTokens
            .Where(t => t.PatientUserId == patientUserId && 
                       !t.IsUsed && 
                       t.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.IsUsed = true;
            token.UsedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }
}
