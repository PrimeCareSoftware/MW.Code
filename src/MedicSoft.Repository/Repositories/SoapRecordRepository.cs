using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class SoapRecordRepository : BaseRepository<SoapRecord>, ISoapRecordRepository
    {
        public SoapRecordRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<SoapRecord?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.AppointmentId == appointmentId && sr.TenantId == tenantId)
                .Include(sr => sr.Patient)
                .Include(sr => sr.Appointment)
                .Include(sr => sr.Doctor)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SoapRecord>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.PatientId == patientId && sr.TenantId == tenantId)
                .Include(sr => sr.Appointment)
                .Include(sr => sr.Doctor)
                .AsNoTracking()
                .OrderByDescending(sr => sr.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<SoapRecord>> GetByDoctorIdAsync(Guid doctorId, string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.DoctorId == doctorId && sr.TenantId == tenantId)
                .Include(sr => sr.Patient)
                .Include(sr => sr.Appointment)
                .AsNoTracking()
                .OrderByDescending(sr => sr.RecordDate)
                .ToListAsync();
        }
    }
}
