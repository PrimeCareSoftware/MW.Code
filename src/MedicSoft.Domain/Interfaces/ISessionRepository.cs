using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IUserSessionRepository
    {
        Task AddAsync(UserSession session);
        Task<UserSession?> GetBySessionIdAsync(Guid userId, string sessionId, string tenantId);
        Task<bool> IsSessionValidAsync(Guid userId, string sessionId, string tenantId);
        Task DeleteAsync(UserSession session);
        Task DeleteExpiredSessionsAsync();
        Task DeleteAllUserSessionsAsync(Guid userId, string tenantId);
        Task<int> GetActiveSessionCountAsync(Guid userId, string tenantId);
        Task SaveChangesAsync();
    }

    public interface IOwnerSessionRepository
    {
        Task AddAsync(OwnerSession session);
        Task<OwnerSession?> GetBySessionIdAsync(Guid ownerId, string sessionId, string tenantId);
        Task<bool> IsSessionValidAsync(Guid ownerId, string sessionId, string tenantId);
        Task DeleteAsync(OwnerSession session);
        Task DeleteExpiredSessionsAsync();
        Task DeleteAllOwnerSessionsAsync(Guid ownerId, string tenantId);
        Task<int> GetActiveSessionCountAsync(Guid ownerId, string tenantId);
        Task SaveChangesAsync();
    }
}
