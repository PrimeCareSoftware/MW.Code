using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for email verification tokens
/// </summary>
public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly PatientPortalDbContext _context;

    public EmailVerificationTokenRepository(PatientPortalDbContext context)
    {
        _context = context;
    }

    public async Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token)
    {
        _context.EmailVerificationTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }

    public async Task<EmailVerificationToken?> GetByTokenAsync(string token)
    {
        return await _context.EmailVerificationTokens
            .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task<EmailVerificationToken?> GetLatestByPatientUserIdAsync(Guid patientUserId)
    {
        return await _context.EmailVerificationTokens
            .Where(t => t.PatientUserId == patientUserId)
            .OrderByDescending(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(EmailVerificationToken token)
    {
        _context.EmailVerificationTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> HasValidTokenAsync(Guid patientUserId)
    {
        return await _context.EmailVerificationTokens
            .AnyAsync(t => t.PatientUserId == patientUserId && 
                          !t.IsUsed && 
                          t.ExpiresAt > DateTime.UtcNow);
    }
}
