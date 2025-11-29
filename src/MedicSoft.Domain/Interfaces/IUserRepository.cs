using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<object?> GetByUsernameAsync(string username);
        Task<User?> GetUserByUsernameAsync(string username, string tenantId);
        Task<User?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<User>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<int> GetUserCountByClinicIdAsync(Guid clinicId, string tenantId);
        Task AddAsync(User user);
        /// <summary>
        /// Adds a user to the context without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        Task AddWithoutSaveAsync(User user);
        /// <summary>
        /// Marks a user for deletion without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        Task DeleteWithoutSaveAsync(Guid id, string tenantId);
        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(User user);
    }
}
