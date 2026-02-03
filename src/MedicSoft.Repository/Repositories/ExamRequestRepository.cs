using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ExamRequestRepository : BaseRepository<ExamRequest>, IExamRequestRepository
    {
        public ExamRequestRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ExamRequest>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(e => e.AppointmentId == appointmentId && e.TenantId == tenantId)
                .Include(e => e.Patient)
                .AsNoTracking()
                .OrderBy(e => e.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamRequest>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(e => e.PatientId == patientId && e.TenantId == tenantId)
                .Include(e => e.Appointment)
                .AsNoTracking()
                .OrderByDescending(e => e.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamRequest>> GetByStatusAsync(ExamRequestStatus status, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Status == status && e.TenantId == tenantId)
                .Include(e => e.Patient)
                .Include(e => e.Appointment)
                .AsNoTracking()
                .OrderBy(e => e.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamRequest>> GetPendingExamsAsync(string tenantId)
        {
            return await _dbSet
                .Where(e => e.Status == ExamRequestStatus.Pending && e.TenantId == tenantId)
                .Include(e => e.Patient)
                .Include(e => e.Appointment)
                .AsNoTracking()
                .OrderBy(e => e.Urgency)
                .ThenBy(e => e.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamRequest>> GetUrgentExamsAsync(string tenantId)
        {
            return await _dbSet
                .Where(e => (e.Urgency == ExamUrgency.Urgent || e.Urgency == ExamUrgency.Emergency) 
                    && e.Status != ExamRequestStatus.Completed 
                    && e.Status != ExamRequestStatus.Cancelled
                    && e.TenantId == tenantId)
                .Include(e => e.Patient)
                .Include(e => e.Appointment)
                .AsNoTracking()
                .OrderBy(e => e.Urgency)
                .ThenBy(e => e.RequestedDate)
                .ToListAsync();
        }
    }
}
