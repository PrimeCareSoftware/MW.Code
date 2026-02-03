using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class BlockedTimeSlotRepository : BaseRepository<BlockedTimeSlot>, IBlockedTimeSlotRepository
    {
        public BlockedTimeSlotRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BlockedTimeSlot>> GetByDateAsync(DateTime date, Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Clinic)
                .Include(b => b.Professional)
                .Where(b => b.Date.Date == date.Date && 
                           b.ClinicId == clinicId && 
                           b.TenantId == tenantId)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlockedTimeSlot>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Clinic)
                .Include(b => b.Professional)
                .Where(b => b.Date.Date >= startDate.Date && 
                           b.Date.Date <= endDate.Date &&
                           b.ClinicId == clinicId && 
                           b.TenantId == tenantId)
                .OrderBy(b => b.Date)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlockedTimeSlot>> GetByProfessionalAsync(Guid professionalId, DateTime date, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Clinic)
                .Include(b => b.Professional)
                .Where(b => b.ProfessionalId == professionalId && 
                           b.Date.Date == date.Date && 
                           b.TenantId == tenantId)
                .OrderBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlockedTimeSlot>> GetByClinicAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Clinic)
                .Include(b => b.Professional)
                .Where(b => b.ClinicId == clinicId && 
                           b.Date.Date >= startDate.Date && 
                           b.Date.Date <= endDate.Date && 
                           b.TenantId == tenantId)
                .OrderBy(b => b.Date)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task<bool> HasOverlappingBlockAsync(
            Guid clinicId, 
            DateTime date, 
            TimeSpan startTime, 
            TimeSpan endTime,
            Guid? professionalId, 
            string tenantId, 
            Guid? excludeBlockId = null)
        {
            var query = _dbSet
                .Where(b => b.ClinicId == clinicId && 
                           b.Date.Date == date.Date && 
                           b.TenantId == tenantId &&
                           b.StartTime < endTime && 
                           b.EndTime > startTime);

            // Only check blocks that apply to this professional (clinic-wide or professional-specific)
            if (professionalId.HasValue)
            {
                query = query.Where(b => !b.ProfessionalId.HasValue || b.ProfessionalId == professionalId);
            }
            else
            {
                // If no professional specified, only check clinic-wide blocks
                query = query.Where(b => !b.ProfessionalId.HasValue);
            }

            if (excludeBlockId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBlockId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<BlockedTimeSlot>> GetByRecurringPatternIdAsync(Guid recurringPatternId, string tenantId)
        {
            return await _dbSet
                .Include(b => b.Clinic)
                .Include(b => b.Professional)
                .Where(b => b.RecurringPatternId == recurringPatternId && 
                           b.TenantId == tenantId)
                .OrderBy(b => b.Date)
                .ThenBy(b => b.StartTime)
                .ToListAsync();
        }

        public async Task DeleteByRecurringPatternIdAsync(Guid recurringPatternId, string tenantId)
        {
            var blocksToDelete = await _dbSet
                .Where(b => b.RecurringPatternId == recurringPatternId && 
                           b.TenantId == tenantId)
                .ToListAsync();

            _dbSet.RemoveRange(blocksToDelete);
            // Note: SaveChanges will be called by the unit of work/calling code
        }
    }
}
