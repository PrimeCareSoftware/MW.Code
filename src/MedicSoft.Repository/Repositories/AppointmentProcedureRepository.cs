using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class AppointmentProcedureRepository : BaseRepository<AppointmentProcedure>, IAppointmentProcedureRepository
    {
        public AppointmentProcedureRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AppointmentProcedure>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Include(ap => ap.Procedure)
                .Include(ap => ap.Patient)
                .Where(ap => ap.AppointmentId == appointmentId && ap.TenantId == tenantId)
                .OrderBy(ap => ap.PerformedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentProcedure>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Include(ap => ap.Procedure)
                .Include(ap => ap.Appointment)
                .Where(ap => ap.PatientId == patientId && ap.TenantId == tenantId)
                .OrderByDescending(ap => ap.PerformedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetAppointmentTotalAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(ap => ap.AppointmentId == appointmentId && ap.TenantId == tenantId)
                .SumAsync(ap => ap.PriceCharged);
        }
    }
}
