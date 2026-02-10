using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IRecurrenceExceptionRepository : IRepository<RecurrenceException>
    {
        /// <summary>
        /// Gets all exceptions for a specific recurring series
        /// </summary>
        Task<IEnumerable<RecurrenceException>> GetByRecurringSeriesIdAsync(Guid recurringSeriesId, string tenantId);

        /// <summary>
        /// Gets an exception for a specific date in a recurring series
        /// </summary>
        Task<RecurrenceException?> GetByOriginalDateAsync(Guid recurringSeriesId, DateTime originalDate, string tenantId);

        /// <summary>
        /// Checks if an exception exists for a specific date in a recurring series
        /// </summary>
        Task<bool> ExistsForDateAsync(Guid recurringSeriesId, DateTime date, string tenantId);

        /// <summary>
        /// Gets all exceptions for a recurring pattern
        /// </summary>
        Task<IEnumerable<RecurrenceException>> GetByRecurringPatternIdAsync(Guid recurringPatternId, string tenantId);
    }
}
