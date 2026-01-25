using Microsoft.EntityFrameworkCore;
using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Interfaces;
using MedicSoft.Telemedicine.Infrastructure.Persistence;

namespace MedicSoft.Telemedicine.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for identity verification
/// </summary>
public class IdentityVerificationRepository : IIdentityVerificationRepository
{
    private readonly TelemedicineDbContext _context;

    public IdentityVerificationRepository(TelemedicineDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IdentityVerification?> GetByIdAsync(Guid id, string tenantId)
    {
        return await _context.IdentityVerifications
            .FirstOrDefaultAsync(v => v.Id == id && v.TenantId == tenantId);
    }

    public async Task<IdentityVerification?> GetLatestByUserIdAsync(Guid userId, string userType, string tenantId)
    {
        return await _context.IdentityVerifications
            .Where(v => v.UserId == userId && v.UserType == userType && v.TenantId == tenantId)
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<IdentityVerification>> GetByUserIdAsync(Guid userId, string userType, string tenantId)
    {
        return await _context.IdentityVerifications
            .Where(v => v.UserId == userId && v.UserType == userType && v.TenantId == tenantId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<IdentityVerification>> GetBySessionIdAsync(Guid sessionId, string tenantId)
    {
        return await _context.IdentityVerifications
            .Where(v => v.TelemedicineSessionId == sessionId && v.TenantId == tenantId)
            .ToListAsync();
    }

    public async Task<IEnumerable<IdentityVerification>> GetPendingVerificationsAsync(string tenantId, int skip = 0, int take = 50)
    {
        return await _context.IdentityVerifications
            .Where(v => v.Status == VerificationStatus.Pending && v.TenantId == tenantId)
            .OrderBy(v => v.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<IdentityVerification>> GetExpiredVerificationsAsync(string tenantId, int skip = 0, int take = 50)
    {
        var now = DateTime.UtcNow;
        return await _context.IdentityVerifications
            .Where(v => v.Status == VerificationStatus.Verified && v.ValidUntil <= now && v.TenantId == tenantId)
            .OrderBy(v => v.ValidUntil)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<bool> HasValidVerificationAsync(Guid userId, string userType, string tenantId)
    {
        var now = DateTime.UtcNow;
        return await _context.IdentityVerifications
            .AnyAsync(v => 
                v.UserId == userId && 
                v.UserType == userType && 
                v.TenantId == tenantId &&
                v.Status == VerificationStatus.Verified &&
                v.ValidUntil > now);
    }

    public async Task AddAsync(IdentityVerification verification)
    {
        await _context.IdentityVerifications.AddAsync(verification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(IdentityVerification verification)
    {
        _context.IdentityVerifications.Update(verification);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, string tenantId)
    {
        var verification = await GetByIdAsync(id, tenantId);
        if (verification != null)
        {
            _context.IdentityVerifications.Remove(verification);
            await _context.SaveChangesAsync();
        }
    }
}
