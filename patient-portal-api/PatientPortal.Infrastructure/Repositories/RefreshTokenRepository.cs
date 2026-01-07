using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly PatientPortalDbContext _context;

    public RefreshTokenRepository(PatientPortalDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid patientUserId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.PatientUserId == patientUserId && rt.IsActive)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync();
    }

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(Guid patientUserId, string revokedByIp, string reason)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.PatientUserId == patientUserId && rt.IsActive)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = revokedByIp;
            token.ReasonRevoked = reason;
        }

        await _context.SaveChangesAsync();
    }
}
