using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PrescriptionItemRepository : BaseRepository<PrescriptionItem>, IPrescriptionItemRepository
    {
        public PrescriptionItemRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PrescriptionItem>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Include(pi => pi.Medication)
                .Where(pi => pi.MedicalRecordId == medicalRecordId && pi.TenantId == tenantId)
                .OrderBy(pi => pi.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PrescriptionItem>> GetByMedicationIdAsync(Guid medicationId, string tenantId)
        {
            return await _dbSet
                .Include(pi => pi.MedicalRecord)
                .Where(pi => pi.MedicationId == medicationId && pi.TenantId == tenantId)
                .OrderByDescending(pi => pi.CreatedAt)
                .ToListAsync();
        }
    }
}
