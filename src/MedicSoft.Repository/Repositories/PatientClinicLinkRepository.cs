using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PatientClinicLinkRepository : BaseRepository<PatientClinicLink>, IPatientClinicLinkRepository
    {
        public PatientClinicLinkRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PatientClinicLink>> GetPatientClinicsAsync(Guid patientId)
        {
            return await _dbSet
                .Where(l => l.PatientId == patientId && l.IsActive)
                .Include(l => l.Clinic)
                .OrderBy(l => l.LinkedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PatientClinicLink>> GetClinicPatientsAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(l => l.ClinicId == clinicId && l.TenantId == tenantId && l.IsActive)
                .Include(l => l.Patient)
                .OrderBy(l => l.Patient.Name)
                .ToListAsync();
        }

        public async Task<PatientClinicLink?> GetLinkAsync(Guid patientId, Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(l => l.PatientId == patientId && l.ClinicId == clinicId && l.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsPatientLinkedToClinicAsync(Guid patientId, Guid clinicId, string tenantId)
        {
            return await _dbSet
                .AnyAsync(l => l.PatientId == patientId && l.ClinicId == clinicId && l.TenantId == tenantId && l.IsActive);
        }
    }
}
