using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IOwnerRepository
    {
        Task<Owner?> GetByUsernameAsync(string username, string tenantId);
        Task<Owner?> GetByIdAsync(Guid id, string tenantId);
        Task<Owner?> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<Owner?> GetByEmailConfirmationTokenAsync(string token, string tenantId);
        Task<IEnumerable<Owner>> GetAllAsync(string tenantId);
        Task AddAsync(Owner owner);
        /// <summary>
        /// Adds an owner to the context without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        Task AddWithoutSaveAsync(Owner owner);
        /// <summary>
        /// Marks an owner for deletion without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        Task DeleteWithoutSaveAsync(Guid id, string tenantId);
        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(Owner owner);
        Task<bool> ExistsByUsernameAsync(string username, string tenantId);
        Task<bool> ExistsByEmailAsync(string email, string tenantId);
    }
}
