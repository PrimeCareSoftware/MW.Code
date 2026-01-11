using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date, string tenantId)
        {
            return await _dbSet
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Where(a => a.ScheduledDate.Date == date.Date && a.TenantId == tenantId)
                .OrderBy(a => a.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Where(a => a.PatientId == patientId && a.TenantId == tenantId)
                .OrderByDescending(a => a.ScheduledDate)
                .ThenByDescending(a => a.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Where(a => a.ClinicId == clinicId && a.TenantId == tenantId)
                .OrderByDescending(a => a.ScheduledDate)
                .ThenBy(a => a.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Where(a => a.ScheduledDate.Date >= startDate.Date && 
                           a.ScheduledDate.Date <= endDate.Date && 
                           a.TenantId == tenantId)
                .OrderBy(a => a.ScheduledDate)
                .ThenBy(a => a.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetDailyAgendaAsync(DateTime date, Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Where(a => a.ScheduledDate.Date == date.Date && 
                           a.ClinicId == clinicId && 
                           a.TenantId == tenantId &&
                           a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.ScheduledTime)
                .ToListAsync();
        }

        public async Task<bool> HasConflictingAppointmentAsync(DateTime scheduledDate, TimeSpan scheduledTime, 
            int durationMinutes, Guid clinicId, string tenantId, Guid? excludeAppointmentId = null)
        {
            var endTime = scheduledTime.Add(TimeSpan.FromMinutes(durationMinutes));

            // First, filter by database-translatable criteria
            var query = _dbSet
                .Where(a => a.ScheduledDate.Date == scheduledDate.Date &&
                           a.ClinicId == clinicId &&
                           a.TenantId == tenantId &&
                           a.Status != AppointmentStatus.Cancelled);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            // Bring to memory and then check for time overlap
            var appointments = await query.ToListAsync();
            
            return appointments.Any(a =>
            {
                var appointmentEnd = a.ScheduledTime.Add(TimeSpan.FromMinutes(a.DurationMinutes));
                return a.ScheduledTime < endTime && appointmentEnd > scheduledTime;
            });
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(string tenantId, int days = 7)
        {
            var endDate = DateTime.Today.AddDays(days);

            return await _dbSet
                .Include(a => a.Patient)
                .Include(a => a.Clinic)
                .Where(a => a.ScheduledDate >= DateTime.Today &&
                           a.ScheduledDate <= endDate &&
                           a.TenantId == tenantId &&
                           (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed))
                .OrderBy(a => a.ScheduledDate)
                .ThenBy(a => a.ScheduledTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableSlotsAsync(DateTime date, Guid clinicId, 
            int durationMinutes, string tenantId)
        {
            // This is a simplified implementation - the actual logic would be in the domain service
            var existingAppointments = await GetDailyAgendaAsync(date, clinicId, tenantId);
            
            // Return the scheduled times of existing appointments (to be used by domain service)
            return existingAppointments.Select(a => a.ScheduledTime);
        }
    }
}