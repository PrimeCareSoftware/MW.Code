using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MedicSoft.Auth.Api.Data;
using MedicSoft.Shared.Authentication.Models;
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
    private readonly SessionSettings _sessionSettings;

    public AuthService(
        AuthDbContext context, 
        ILogger<AuthService> logger,
        IOptions<SessionSettings> sessionSettings)
    {
        _context = context;
        _logger = logger;
        _sessionSettings = sessionSettings.Value;
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
        var expiresAt = now.AddHours(_sessionSettings.ExpiryHours);
        var session = new UserSessionEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionId = sessionId,
            TenantId = tenantId,
            CreatedAt = now,
            ExpiresAt = expiresAt,
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

        _logger.LogInformation("User login recorded: UserId={UserId}, SessionId={SessionId}, ExpiresAt={ExpiresAt}, ExpiryHours={ExpiryHours}", 
            userId, sessionId, expiresAt, _sessionSettings.ExpiryHours);
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
        var expiresAt = now.AddHours(_sessionSettings.ExpiryHours);
        var session = new OwnerSessionEntity
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            SessionId = sessionId,
            TenantId = tenantId,
            CreatedAt = now,
            ExpiresAt = expiresAt,
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

        _logger.LogInformation("Owner login recorded: OwnerId={OwnerId}, SessionId={SessionId}, ExpiresAt={ExpiresAt}, ExpiryHours={ExpiryHours}", 
            ownerId, sessionId, expiresAt, _sessionSettings.ExpiryHours);
        return sessionId;
    }

    public async Task<bool> ValidateUserSessionAsync(Guid userId, string sessionId, string tenantId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId && u.IsActive);

        if (user == null)
        {
            _logger.LogWarning("User not found or inactive during session validation: UserId={UserId}, TenantId={TenantId}", userId, tenantId);
            return false;
        }

        // Check if session exists in the UserSessions table and is not expired
        // Use AsNoTracking to avoid change tracking conflicts and get fresh data
        var now = DateTime.UtcNow;
        var session = await _context.UserSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && 
                                    s.SessionId == sessionId && 
                                    s.TenantId == tenantId &&
                                    s.ExpiresAt > now);

        if (session != null)
        {
            // Update session expiration efficiently using ExecuteUpdateAsync (sliding expiration)
            var newExpiresAt = now.AddHours(_sessionSettings.ExpiryHours);
            await _context.UserSessions
                .Where(s => s.Id == session.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.LastActivityAt, now)
                    .SetProperty(s => s.ExpiresAt, newExpiresAt));
            
            _logger.LogDebug("User session validated and extended: UserId={UserId}, SessionId={SessionId}, NewExpiresAt={ExpiresAt}", 
                userId, sessionId, newExpiresAt);
            return true;
        }

        // Check if there's a session record that has expired
        var expiredSession = await _context.UserSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && 
                                    s.SessionId == sessionId && 
                                    s.TenantId == tenantId);
        
        if (expiredSession != null)
        {
            _logger.LogWarning("User session has expired: UserId={UserId}, SessionId={SessionId}, ExpiredAt={ExpiredAt}, CurrentTime={CurrentTime}", 
                userId, sessionId, expiredSession.ExpiresAt, now);
            return false;
        }

        // Fallback to legacy SessionId field for backward compatibility
        // This allows sessions created before the migration to still work
        var legacySessionValid = user.SessionId == sessionId;
        if (legacySessionValid)
        {
            _logger.LogInformation("User session validated using legacy SessionId field: UserId={UserId}", userId);
        }
        else
        {
            _logger.LogWarning("User session not found: UserId={UserId}, SessionId={SessionId}", userId, sessionId);
        }
        
        return legacySessionValid;
    }

    public async Task<bool> ValidateOwnerSessionAsync(Guid ownerId, string sessionId, string tenantId)
    {
        var owner = await _context.Owners
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == ownerId && o.TenantId == tenantId && o.IsActive);

        if (owner == null)
        {
            _logger.LogWarning("Owner not found or inactive during session validation: OwnerId={OwnerId}, TenantId={TenantId}", ownerId, tenantId);
            return false;
        }

        // Check if session exists in the OwnerSessions table and is not expired
        // Use AsNoTracking to avoid change tracking conflicts and get fresh data
        var now = DateTime.UtcNow;
        var session = await _context.OwnerSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId && 
                                    s.SessionId == sessionId && 
                                    s.TenantId == tenantId &&
                                    s.ExpiresAt > now);

        if (session != null)
        {
            // Update session expiration efficiently using ExecuteUpdateAsync (sliding expiration)
            var newExpiresAt = now.AddHours(_sessionSettings.ExpiryHours);
            await _context.OwnerSessions
                .Where(s => s.Id == session.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(s => s.LastActivityAt, now)
                    .SetProperty(s => s.ExpiresAt, newExpiresAt));
            
            _logger.LogDebug("Owner session validated and extended: OwnerId={OwnerId}, SessionId={SessionId}, NewExpiresAt={ExpiresAt}", 
                ownerId, sessionId, newExpiresAt);
            return true;
        }

        // Check if there's a session record that has expired
        var expiredSession = await _context.OwnerSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.OwnerId == ownerId && 
                                    s.SessionId == sessionId && 
                                    s.TenantId == tenantId);
        
        if (expiredSession != null)
        {
            _logger.LogWarning("Owner session has expired: OwnerId={OwnerId}, SessionId={SessionId}, ExpiredAt={ExpiredAt}, CurrentTime={CurrentTime}", 
                ownerId, sessionId, expiredSession.ExpiresAt, now);
            return false;
        }

        // Fallback to legacy SessionId field for backward compatibility
        // This allows sessions created before the migration to still work
        var legacySessionValid = owner.SessionId == sessionId;
        if (legacySessionValid)
        {
            _logger.LogInformation("Owner session validated using legacy SessionId field: OwnerId={OwnerId}", ownerId);
        }
        else
        {
            _logger.LogWarning("Owner session not found: OwnerId={OwnerId}, SessionId={SessionId}", ownerId, sessionId);
        }
        
        return legacySessionValid;
    }
}
