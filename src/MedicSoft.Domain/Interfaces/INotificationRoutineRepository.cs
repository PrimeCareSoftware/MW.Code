using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for NotificationRoutine entity
    /// </summary>
    public interface INotificationRoutineRepository : IRepository<NotificationRoutine>
    {
        /// <summary>
        /// Get all routines for a specific tenant
        /// </summary>
        Task<IEnumerable<NotificationRoutine>> GetByTenantAsync(string tenantId);

        /// <summary>
        /// Get all active routines for a specific tenant
        /// </summary>
        Task<IEnumerable<NotificationRoutine>> GetActiveRoutinesByTenantAsync(string tenantId);

        /// <summary>
        /// Get routines that should be executed now
        /// </summary>
        Task<IEnumerable<NotificationRoutine>> GetRoutinesDueForExecutionAsync();

        /// <summary>
        /// Get routines by scope (Clinic or System)
        /// </summary>
        Task<IEnumerable<NotificationRoutine>> GetRoutinesByScopeAsync(RoutineScope scope, string tenantId);

        /// <summary>
        /// Get routine by notification type
        /// </summary>
        Task<IEnumerable<NotificationRoutine>> GetRoutinesByTypeAsync(NotificationType type, string tenantId);

        /// <summary>
        /// Get routine by channel
        /// </summary>
        Task<IEnumerable<NotificationRoutine>> GetRoutinesByChannelAsync(NotificationChannel channel, string tenantId);
    }
}
