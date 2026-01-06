using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class DigitalPrescriptionItemRepository : BaseRepository<DigitalPrescriptionItem>, IDigitalPrescriptionItemRepository
    {
        public DigitalPrescriptionItemRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<DigitalPrescriptionItem>> GetByPrescriptionIdAsync(Guid prescriptionId, string tenantId)
        {
            return await _dbSet
                .Include(dpi => dpi.Medication)
                .Where(dpi => dpi.DigitalPrescriptionId == prescriptionId && dpi.TenantId == tenantId)
                .OrderBy(dpi => dpi.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescriptionItem>> GetControlledItemsAsync(string tenantId)
        {
            return await _dbSet
                .Include(dpi => dpi.Medication)
                .Include(dpi => dpi.DigitalPrescription)
                .Where(dpi => dpi.IsControlledSubstance && dpi.TenantId == tenantId)
                .OrderByDescending(dpi => dpi.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescriptionItem>> GetByMedicationIdAsync(Guid medicationId, string tenantId)
        {
            return await _dbSet
                .Include(dpi => dpi.DigitalPrescription)
                .Where(dpi => dpi.MedicationId == medicationId && dpi.TenantId == tenantId)
                .OrderByDescending(dpi => dpi.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescriptionItem>> GetControlledSubstancesAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Include(dpi => dpi.Medication)
                .Include(dpi => dpi.DigitalPrescription)
                .Where(dpi => dpi.IsControlledSubstance && 
                            dpi.CreatedAt >= startDate && 
                            dpi.CreatedAt <= endDate && 
                            dpi.TenantId == tenantId)
                .OrderBy(dpi => dpi.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DigitalPrescriptionItem>> GetByControlledListAsync(ControlledSubstanceList controlledList, DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Include(dpi => dpi.Medication)
                .Include(dpi => dpi.DigitalPrescription)
                .Where(dpi => dpi.ControlledList == controlledList && 
                            dpi.CreatedAt >= startDate && 
                            dpi.CreatedAt <= endDate && 
                            dpi.TenantId == tenantId)
                .OrderBy(dpi => dpi.CreatedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetMedicationUsageStatsAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(dpi => dpi.CreatedAt >= startDate && 
                            dpi.CreatedAt <= endDate && 
                            dpi.TenantId == tenantId)
                .GroupBy(dpi => dpi.MedicationId)
                .Select(g => new { MedicationId = g.Key, TotalQuantity = g.Sum(dpi => dpi.Quantity) })
                .ToDictionaryAsync(x => x.MedicationId, x => x.TotalQuantity);
        }

        public async Task<int> GetTotalQuantityPrescribedAsync(Guid medicationId, DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(dpi => dpi.MedicationId == medicationId &&
                            dpi.CreatedAt >= startDate &&
                            dpi.CreatedAt <= endDate &&
                            dpi.TenantId == tenantId)
                .SumAsync(dpi => dpi.Quantity);
        }
    }
}
