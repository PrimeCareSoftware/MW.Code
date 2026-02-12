using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Interfaces;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<PresenceService> _logger;

        public PresenceService(MedicSoftDbContext context, ILogger<PresenceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SetUserOnlineAsync(Guid userId, string connectionId, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId is required", nameof(tenantId));

            try
            {
                var userExists = await _context.Users
                    .AnyAsync(u => u.Id == userId && u.TenantId == tenantId);

                if (!userExists)
                {
                    _logger.LogWarning("Skipping presence update: user {UserId} not found in tenant {TenantId}", userId, tenantId);
                    return;
                }

                var presence = await _context.UserPresences
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.TenantId == tenantId);

                if (presence == null)
                {
                    presence = new UserPresence(userId, tenantId);
                    _context.UserPresences.Add(presence);
                }

                presence.SetOnline(connectionId);
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) when (dbEx.InnerException?.Message?.Contains("FK_UserPresences_Users_UserId") == true)
            {
                _logger.LogWarning(dbEx, "Foreign key constraint violation when setting user {UserId} online in tenant {TenantId}. User may have been deleted.", userId, tenantId);
                // Silently ignore - user was likely deleted between check and save
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error setting user {UserId} online in tenant {TenantId}", userId, tenantId);
                throw;
            }

            _logger.LogInformation("User {UserId} is now online with connection {ConnectionId} in tenant {TenantId}", 
                userId, connectionId, tenantId);
        }

        public async Task SetUserOfflineAsync(Guid userId, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId is required", nameof(tenantId));

            try
            {
                var presence = await _context.UserPresences
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.TenantId == tenantId);

                if (presence != null)
                {
                    presence.SetOffline();
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("User {UserId} is now offline in tenant {TenantId}", userId, tenantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error setting user {UserId} offline in tenant {TenantId}", userId, tenantId);
                // Don't throw - offline status update is not critical
            }
        }

        public async Task UpdateUserStatusAsync(Guid userId, string status, string? statusMessage, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId is required", nameof(tenantId));

            var userExists = await _context.Users
                .AnyAsync(u => u.Id == userId && u.TenantId == tenantId);

            if (!userExists)
            {
                _logger.LogWarning("Skipping status update: user {UserId} not found in tenant {TenantId}", userId, tenantId);
                return;
            }

            var presence = await _context.UserPresences
                .FirstOrDefaultAsync(up => up.UserId == userId && up.TenantId == tenantId);

            if (presence == null)
            {
                presence = new UserPresence(userId, tenantId);
                _context.UserPresences.Add(presence);
            }

            if (Enum.TryParse<PresenceStatus>(status, out var presenceStatus))
            {
                presence.UpdateStatus(presenceStatus, statusMessage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} status updated to {Status} in tenant {TenantId}", 
                    userId, status, tenantId);
            }
            else
            {
                throw new ArgumentException($"Invalid status: {status}", nameof(status));
            }
        }

        public async Task<UserPresenceDto?> GetUserPresenceAsync(Guid userId, string tenantId)
        {
            var presence = await _context.UserPresences
                .Where(up => up.UserId == userId && up.TenantId == tenantId)
                .Include(up => up.User)
                .FirstOrDefaultAsync();

            if (presence == null)
                return null;

            return MapToDto(presence);
        }

        public async Task<List<UserPresenceDto>> GetAllUserPresencesAsync(string tenantId)
        {
            var users = await _context.Users
                .Where(u => u.TenantId == tenantId && u.IsActive)
                .ToListAsync();

            var presences = await _context.UserPresences
                .Where(up => up.TenantId == tenantId)
                .Include(up => up.User)
                .ToListAsync();

            return users.Select(user =>
            {
                var presence = presences.FirstOrDefault(p => p.UserId == user.Id);
                
                if (presence != null)
                {
                    return MapToDto(presence);
                }
                
                // Return offline status for users without presence record
                return new UserPresenceDto
                {
                    UserId = user.Id,
                    UserName = user.Username,
                    FullName = user.FullName,
                    Status = "Offline",
                    LastSeenAt = user.LastLoginAt ?? DateTime.UtcNow,
                    IsOnline = false,
                    StatusMessage = null
                };
            }).ToList();
        }

        public async Task<List<UserPresenceDto>> GetOnlineUsersAsync(string tenantId)
        {
            var onlinePresences = await _context.UserPresences
                .Where(up => up.TenantId == tenantId && up.IsOnline)
                .Include(up => up.User)
                .ToListAsync();

            return onlinePresences.Select(MapToDto).ToList();
        }

        private UserPresenceDto MapToDto(UserPresence presence)
        {
            return new UserPresenceDto
            {
                UserId = presence.UserId,
                UserName = presence.User?.Username ?? "Unknown",
                FullName = presence.User?.FullName ?? "Unknown",
                Status = presence.Status.ToString(),
                LastSeenAt = presence.LastSeenAt,
                IsOnline = presence.IsOnline,
                StatusMessage = presence.StatusMessage
            };
        }
    }
}
