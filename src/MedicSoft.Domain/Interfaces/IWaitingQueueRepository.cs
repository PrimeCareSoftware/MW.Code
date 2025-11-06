using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IWaitingQueueRepository : IRepository<WaitingQueueEntry>
    {
        Task<IEnumerable<WaitingQueueEntry>> GetActiveEntriesByClinicAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<WaitingQueueEntry>> GetByStatusAsync(Guid clinicId, QueueStatus status, string tenantId);
        Task<WaitingQueueEntry?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<int> SaveChangesAsync();
    }
}
