using Microsoft.EntityFrameworkCore;
using MedicSoft.Auth.Api.Data;
using BCrypt.Net;

namespace MedicSoft.Auth.Api.Services;

public interface IAuthService
{
    Task<UserEntity?> AuthenticateUserAsync(string username, string password, string tenantId);
    Task<OwnerEntity?> AuthenticateOwnerAsync(string username, string password, string tenantId);
    Task<string> RecordUserLoginAsync(Guid userId, string tenantId);
    Task<string> RecordOwnerLoginAsync(Guid ownerId, string tenantId);
    Task<bool> ValidateUserSessionAsync(Guid userId, string sessionId, string tenantId);
    Task<bool> ValidateOwnerSessionAsync(Guid ownerId, string sessionId, string tenantId);
}

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AuthDbContext context, ILogger<AuthService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserEntity?> AuthenticateUserAsync(string username, string password, string tenantId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.TenantId == tenantId && u.IsActive);

        if (user == null)
        {
            _logger.LogWarning("User not found: {Username} in tenant: {TenantId}", username, tenantId);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            _logger.LogWarning("Invalid password for user: {Username}", username);
            return null;
        }

        return user;
    }

    public async Task<OwnerEntity?> AuthenticateOwnerAsync(string username, string password, string tenantId)
    {
        var owner = await _context.Owners
            .FirstOrDefaultAsync(o => o.Username == username && o.TenantId == tenantId && o.IsActive);

        if (owner == null)
        {
            _logger.LogWarning("Owner not found: {Username} in tenant: {TenantId}", username, tenantId);
            return null;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, owner.PasswordHash))
        {
            _logger.LogWarning("Invalid password for owner: {Username}", username);
            return null;
        }

        return owner;
    }

    public async Task<string> RecordUserLoginAsync(Guid userId, string tenantId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId);

        if (user == null)
            throw new InvalidOperationException("User not found");

        var sessionId = Guid.NewGuid().ToString();
        
        // Update legacy field for backward compatibility
        user.SessionId = sessionId;
        user.LastLoginAt = DateTime.UtcNow;

        // Create new session record to support multiple concurrent sessions
        var now = DateTime.UtcNow;
        var session = new UserSessionEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionId = sessionId,
            TenantId = tenantId,
            CreatedAt = now,
            ExpiresAt = now.AddHours(24), // Session expires after 24 hours of creation
            LastActivityAt = now
        };

        _context.UserSessions.Add(session);

        // Clean up expired sessions for this user
        var expiredSessions = await _context.UserSessions
            .Where(s => s.UserId == userId && s.ExpiresAt < now)
            .ToListAsync();
        
        if (expiredSessions.Any())
        {
            _context.UserSessions.RemoveRange(expiredSessions);
            _logger.LogInformation("Removed {Count} expired sessions for user {UserId}", expiredSessions.Count, userId);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User login recorded: {UserId}, Session: {SessionId}", userId, sessionId);
        return sessionId;
    }

    public async Task<string> RecordOwnerLoginAsync(Guid ownerId, string tenantId)
    {
        var owner = await _context.Owners
            .FirstOrDefaultAsync(o => o.Id == ownerId && o.TenantId == tenantId);

        if (owner == null)
            throw new InvalidOperationException("Owner not found");

        var sessionId = Guid.NewGuid().ToString();
        
        // Update legacy field for backward compatibility
        owner.SessionId = sessionId;
        owner.LastLoginAt = DateTime.UtcNow;

        // Create new session record to support multiple concurrent sessions
        var now = DateTime.UtcNow;
        var session = new OwnerSessionEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            SessionId = sessionId,
            TenantId = tenantId,
            CreatedAt = now,
            ExpiresAt = now.AddHours(24), // Session expires after 24 hours of creation
            LastActivityAt = now
        };

        _context.OwnerSessions.Add(session);

        // Clean up expired sessions for this owner
        var expiredSessions = await _context.OwnerSessions
            .Where(s => s.OwnerId == ownerId && s.ExpiresAt < now)
            .ToListAsync();
        
        if (expiredSessions.Any())
        {
            _context.OwnerSessions.RemoveRange(expiredSessions);
            _logger.LogInformation("Removed {Count} expired sessions for owner {OwnerId}", expiredSessions.Count, ownerId);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Owner login recorded: {OwnerId}, Session: {SessionId}", ownerId, sessionId);
        return sessionId;
    }

    public async Task<bool> ValidateUserSessionAsync(Guid userId, string sessionId, string tenantId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId && u.IsActive);

        if (user == null)
            return false;

        // Check if session exists in the UserSessions table and is not expired
        var now = DateTime.UtcNow;
        var session = await _context.UserSessions
            .FirstOrDefaultAsync(s => s.UserId == userId && 
                                    s.SessionId == sessionId && 
                                    s.TenantId == tenantId &&
                                    s.ExpiresAt > now);

        if (session != null)
        {
            // Update last activity time
            session.LastActivityAt = now;
            await _context.SaveChangesAsync();
            return true;
        }

        // Fallback to legacy SessionId field for backward compatibility
        // This allows sessions created before the migration to still work
        return user.SessionId == sessionId;
    }

    public async Task<bool> ValidateOwnerSessionAsync(Guid ownerId, string sessionId, string tenantId)
    {
        var owner = await _context.Owners
            .FirstOrDefaultAsync(o => o.Id == ownerId && o.TenantId == tenantId && o.IsActive);

        if (owner == null)
            return false;

        // Check if session exists in the OwnerSessions table and is not expired
        var now = DateTime.UtcNow;
        var session = await _context.OwnerSessions
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId && 
                                    s.SessionId == sessionId && 
                                    s.TenantId == tenantId &&
                                    s.ExpiresAt > now);

        if (session != null)
        {
            // Update last activity time
            session.LastActivityAt = now;
            await _context.SaveChangesAsync();
            return true;
        }

        // Fallback to legacy SessionId field for backward compatibility
        // This allows sessions created before the migration to still work
        return owner.SessionId == sessionId;
    }
}
