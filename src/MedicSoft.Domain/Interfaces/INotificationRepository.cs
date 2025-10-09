using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<Notification>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<Notification>> GetPendingNotificationsAsync(string tenantId);
        Task<IEnumerable<Notification>> GetFailedNotificationsForRetryAsync(string tenantId);
    }
}
