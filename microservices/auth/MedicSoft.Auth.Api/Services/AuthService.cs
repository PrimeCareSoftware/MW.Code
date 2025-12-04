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
        user.SessionId = sessionId;
        user.LastLoginAt = DateTime.UtcNow;

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
        owner.SessionId = sessionId;
        owner.LastLoginAt = DateTime.UtcNow;

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

        return user.SessionId == sessionId;
    }

    public async Task<bool> ValidateOwnerSessionAsync(Guid ownerId, string sessionId, string tenantId)
    {
        var owner = await _context.Owners
            .FirstOrDefaultAsync(o => o.Id == ownerId && o.TenantId == tenantId && o.IsActive);

        if (owner == null)
            return false;

        return owner.SessionId == sessionId;
    }
}
