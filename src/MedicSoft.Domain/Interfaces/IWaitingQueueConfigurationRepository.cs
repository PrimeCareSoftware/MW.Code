using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IWaitingQueueConfigurationRepository : IRepository<WaitingQueueConfiguration>
    {
        Task<WaitingQueueConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<int> SaveChangesAsync();
    }
}
