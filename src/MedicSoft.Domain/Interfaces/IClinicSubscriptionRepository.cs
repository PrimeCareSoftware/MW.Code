using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IClinicSubscriptionRepository
    {
        Task<ClinicSubscription?> GetByIdAsync(Guid id, string tenantId);
        Task<ClinicSubscription?> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<ClinicSubscription>> GetOverdueSubscriptionsAsync();
        Task<IEnumerable<ClinicSubscription>> GetExpiringTrialsAsync(int daysBeforeExpiration);
        Task<IEnumerable<ClinicSubscription>> GetPendingDowngradesAsync();
        Task AddAsync(ClinicSubscription subscription);
        Task UpdateAsync(ClinicSubscription subscription);
    }
}
