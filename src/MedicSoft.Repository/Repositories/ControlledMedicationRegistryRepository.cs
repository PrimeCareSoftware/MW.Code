using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ControlledMedicationRegistryRepository : BaseRepository<ControlledMedicationRegistry>, IControlledMedicationRegistryRepository
    {
        public ControlledMedicationRegistryRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ControlledMedicationRegistry>> GetByPeriodAsync(
            DateTime startDate,
            DateTime endDate,
            string tenantId)
        {
            return await _dbSet
                .Where(cmr => cmr.TenantId == tenantId &&
                             cmr.Date >= startDate &&
                             cmr.Date <= endDate)
                .OrderBy(cmr => cmr.Date)
                .ThenBy(cmr => cmr.MedicationName)
                .ToListAsync();
        }

        public async Task<IEnumerable<ControlledMedicationRegistry>> GetByMedicationAsync(
            string medicationName,
            string tenantId)
        {
            return await _dbSet
                .Where(cmr => cmr.TenantId == tenantId &&
                             cmr.MedicationName == medicationName)
                .OrderByDescending(cmr => cmr.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<ControlledMedicationRegistry>> GetByAnvisaListAsync(
            string anvisaList,
            string tenantId)
        {
            return await _dbSet
                .Where(cmr => cmr.TenantId == tenantId &&
                             cmr.AnvisaList == anvisaList.ToUpperInvariant())
                .OrderByDescending(cmr => cmr.Date)
                .ToListAsync();
        }

        public async Task<decimal> GetLatestBalanceAsync(
            string medicationName,
            string tenantId)
        {
            var latestEntry = await _dbSet
                .Where(cmr => cmr.TenantId == tenantId &&
                             cmr.MedicationName == medicationName)
                .OrderByDescending(cmr => cmr.Date)
                .ThenByDescending(cmr => cmr.CreatedAt)
                .FirstOrDefaultAsync();

            return latestEntry?.Balance ?? 0;
        }

        public async Task<ControlledMedicationRegistry?> GetByPrescriptionIdAsync(
            Guid prescriptionId,
            string tenantId)
        {
            return await _dbSet
                .Where(cmr => cmr.TenantId == tenantId &&
                             cmr.PrescriptionId == prescriptionId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ControlledMedicationRegistry>> GetInboundEntriesAsync(
            DateTime startDate,
            DateTime endDate,
            string tenantId)
        {
            return await _dbSet
                .Where(cmr => cmr.TenantId == tenantId &&
                             cmr.Date >= startDate &&
                             cmr.Date <= endDate &&
                             cmr.RegistryType == RegistryType.Inbound)
                .OrderBy(cmr => cmr.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<ControlledMedicationRegistry>> GetOutboundEntriesAsync(
            DateTime startDate,
            DateTime endDate,
            string tenantId)
        {
            return await _dbSet
                .Where(cmr => cmr.TenantId == tenantId &&
                             cmr.Date >= startDate &&
                             cmr.Date <= endDate &&
                             cmr.RegistryType == RegistryType.Outbound)
                .OrderBy(cmr => cmr.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetControlledMedicationsAsync(string tenantId)
        {
            return await _dbSet
                .Where(cmr => cmr.TenantId == tenantId)
                .Select(cmr => cmr.MedicationName)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();
        }

        public async Task<bool> IsPrescriptionRegisteredAsync(Guid prescriptionId, string tenantId)
        {
            return await _dbSet
                .AnyAsync(cmr => cmr.TenantId == tenantId &&
                                cmr.PrescriptionId == prescriptionId);
        }
    }
}
