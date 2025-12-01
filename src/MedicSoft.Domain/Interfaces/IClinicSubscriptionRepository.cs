using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IClinicSubscriptionRepository
    {
        Task<ClinicSubscription?> GetByIdAsync(Guid id, string tenantId);
        Task<ClinicSubscription?> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<ClinicSubscription>> GetAllAsync(string tenantId);
        Task<IEnumerable<ClinicSubscription>> GetOverdueSubscriptionsAsync();
        Task<IEnumerable<ClinicSubscription>> GetExpiringTrialsAsync(int daysBeforeExpiration);
        Task<IEnumerable<ClinicSubscription>> GetPendingDowngradesAsync();
        Task AddAsync(ClinicSubscription subscription);
        /// <summary>
        /// Adds a subscription to the context without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        Task AddWithoutSaveAsync(ClinicSubscription subscription);
        /// <summary>
        /// Marks a subscription for deletion without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        Task DeleteWithoutSaveAsync(Guid id, string tenantId);
        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(ClinicSubscription subscription);
    }
}
