using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Interfaces
{
    public interface IPresenceService
    {
        Task SetUserOnlineAsync(Guid userId, string connectionId, string tenantId);
        Task SetUserOfflineAsync(Guid userId, string tenantId);
        Task UpdateUserStatusAsync(Guid userId, string status, string? statusMessage, string tenantId);
        Task<UserPresenceDto?> GetUserPresenceAsync(Guid userId, string tenantId);
        Task<List<UserPresenceDto>> GetAllUserPresencesAsync(string tenantId);
        Task<List<UserPresenceDto>> GetOnlineUsersAsync(string tenantId);
    }
}
