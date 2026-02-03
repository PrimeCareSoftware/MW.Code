using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class MedicalRecordRepository : BaseRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<MedicalRecord?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(mr => mr.AppointmentId == appointmentId && mr.TenantId == tenantId)
                .Include(mr => mr.Patient)
                .Include(mr => mr.Appointment)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MedicalRecord>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(mr => mr.PatientId == patientId && mr.TenantId == tenantId)
                .Include(mr => mr.Appointment)
                .AsNoTracking()
                .OrderByDescending(mr => mr.ConsultationStartTime)
                .ToListAsync();
        }
    }
}
