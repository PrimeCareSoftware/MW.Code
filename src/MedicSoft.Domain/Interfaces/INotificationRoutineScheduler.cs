using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Service interface for executing notification routines
    /// </summary>
    public interface INotificationRoutineScheduler
    {
        /// <summary>
        /// Execute a specific notification routine
        /// </summary>
        Task ExecuteRoutineAsync(NotificationRoutine routine);

        /// <summary>
        /// Execute all routines that are due for execution
        /// </summary>
        Task ExecuteDueRoutinesAsync();

        /// <summary>
        /// Calculate the next execution time based on the routine's schedule configuration
        /// </summary>
        DateTime? CalculateNextExecution(NotificationRoutine routine);

        /// <summary>
        /// Get recipients for a routine based on its filter configuration
        /// </summary>
        Task<IEnumerable<Guid>> GetRecipientsForRoutineAsync(NotificationRoutine routine);
    }
}
