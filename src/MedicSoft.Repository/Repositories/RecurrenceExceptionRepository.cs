using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class RecurrenceExceptionRepository : BaseRepository<RecurrenceException>, IRecurrenceExceptionRepository
    {
        public RecurrenceExceptionRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RecurrenceException>> GetByRecurringSeriesIdAsync(Guid recurringSeriesId, string tenantId)
        {
            return await _dbSet
                .Where(e => e.RecurringSeriesId == recurringSeriesId && 
                           e.TenantId == tenantId)
                .OrderBy(e => e.OriginalDate)
                .ToListAsync();
        }

        public async Task<RecurrenceException?> GetByOriginalDateAsync(Guid recurringSeriesId, DateTime originalDate, string tenantId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.RecurringSeriesId == recurringSeriesId && 
                                        e.OriginalDate.Date == originalDate.Date &&
                                        e.TenantId == tenantId);
        }

        public async Task<bool> ExistsForDateAsync(Guid recurringSeriesId, DateTime date, string tenantId)
        {
            return await _dbSet
                .AnyAsync(e => e.RecurringSeriesId == recurringSeriesId && 
                              e.OriginalDate.Date == date.Date &&
                              e.TenantId == tenantId);
        }

        public async Task<IEnumerable<RecurrenceException>> GetByRecurringPatternIdAsync(Guid recurringPatternId, string tenantId)
        {
            return await _dbSet
                .Where(e => e.RecurringPatternId == recurringPatternId && 
                           e.TenantId == tenantId)
                .OrderBy(e => e.OriginalDate)
                .ToListAsync();
        }
    }
}
