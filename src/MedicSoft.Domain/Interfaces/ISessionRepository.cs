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
        Task SaveChangesAsync();
    }

    public interface IOwnerSessionRepository
    {
        Task AddAsync(OwnerSession session);
        Task<OwnerSession?> GetBySessionIdAsync(Guid ownerId, string sessionId, string tenantId);
        Task<bool> IsSessionValidAsync(Guid ownerId, string sessionId, string tenantId);
        Task DeleteAsync(OwnerSession session);
        Task DeleteExpiredSessionsAsync();
        Task SaveChangesAsync();
    }
}
