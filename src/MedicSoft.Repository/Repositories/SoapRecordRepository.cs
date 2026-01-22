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
                .Include(sr => sr.Patient)
                .Include(sr => sr.Appointment)
                .Include(sr => sr.Doctor)
                .Where(sr => sr.AppointmentId == appointmentId && sr.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SoapRecord>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Include(sr => sr.Appointment)
                .Include(sr => sr.Doctor)
                .Where(sr => sr.PatientId == patientId && sr.TenantId == tenantId)
                .OrderByDescending(sr => sr.RecordDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<SoapRecord>> GetByDoctorIdAsync(Guid doctorId, string tenantId)
        {
            return await _dbSet
                .Include(sr => sr.Patient)
                .Include(sr => sr.Appointment)
                .Where(sr => sr.DoctorId == doctorId && sr.TenantId == tenantId)
                .OrderByDescending(sr => sr.RecordDate)
                .ToListAsync();
        }
    }
}
