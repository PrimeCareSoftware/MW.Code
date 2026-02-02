using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class RecurringAppointmentPatternRepository : BaseRepository<RecurringAppointmentPattern>, IRecurringAppointmentPatternRepository
    {
        public RecurringAppointmentPatternRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RecurringAppointmentPattern>> GetActivePatternsByClinicAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(p => p.Clinic)
                .Include(p => p.Professional)
                .Include(p => p.Patient)
                .Where(p => p.ClinicId == clinicId && 
                           p.IsActive && 
                           p.TenantId == tenantId)
                .OrderBy(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<RecurringAppointmentPattern>> GetActivePatternsByProfessionalAsync(Guid professionalId, string tenantId)
        {
            return await _dbSet
                .Include(p => p.Clinic)
                .Include(p => p.Professional)
                .Include(p => p.Patient)
                .Where(p => p.ProfessionalId == professionalId && 
                           p.IsActive && 
                           p.TenantId == tenantId)
                .OrderBy(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<RecurringAppointmentPattern>> GetActivePatternsByPatientAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Include(p => p.Clinic)
                .Include(p => p.Professional)
                .Include(p => p.Patient)
                .Where(p => p.PatientId == patientId && 
                           p.IsActive && 
                           p.TenantId == tenantId)
                .OrderBy(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<RecurringAppointmentPattern?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Include(p => p.Clinic)
                .Include(p => p.Professional)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Id == id && p.TenantId == tenantId);
        }
    }
}
