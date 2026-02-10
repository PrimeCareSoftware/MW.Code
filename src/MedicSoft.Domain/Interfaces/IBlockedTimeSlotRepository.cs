using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IBlockedTimeSlotRepository : IRepository<BlockedTimeSlot>
    {
        Task<IEnumerable<BlockedTimeSlot>> GetByDateAsync(DateTime date, Guid clinicId, string tenantId);
        Task<IEnumerable<BlockedTimeSlot>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid clinicId, string tenantId);
        Task<IEnumerable<BlockedTimeSlot>> GetByProfessionalAsync(Guid professionalId, DateTime date, string tenantId);
        Task<IEnumerable<BlockedTimeSlot>> GetByClinicAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId);
        Task<bool> HasOverlappingBlockAsync(Guid clinicId, DateTime date, TimeSpan startTime, TimeSpan endTime, 
            Guid? professionalId, string tenantId, Guid? excludeBlockId = null);
        Task<IEnumerable<BlockedTimeSlot>> GetByRecurringPatternIdAsync(Guid recurringPatternId, string tenantId);
        
        // New methods for series-based operations
        Task<IEnumerable<BlockedTimeSlot>> GetByRecurringSeriesIdAsync(Guid recurringSeriesId, string tenantId);
        Task<IEnumerable<BlockedTimeSlot>> GetFutureOccurrencesAsync(Guid recurringSeriesId, DateTime fromDate, string tenantId);
        Task DeleteByRecurringSeriesIdAsync(Guid recurringSeriesId, string tenantId);
        Task DeleteFutureOccurrencesAsync(Guid recurringSeriesId, DateTime fromDate, string tenantId);
        
        // Deprecated - use DeleteByRecurringSeriesIdAsync instead
        [Obsolete("Use DeleteByRecurringSeriesIdAsync instead to avoid deleting multiple series")]
        Task DeleteByRecurringPatternIdAsync(Guid recurringPatternId, string tenantId);
    }
}
