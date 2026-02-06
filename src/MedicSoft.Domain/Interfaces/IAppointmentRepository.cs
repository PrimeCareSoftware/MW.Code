using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByDateAsync(DateTime date, string tenantId);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<Appointment>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<IEnumerable<Appointment>> GetDailyAgendaAsync(DateTime date, Guid clinicId, string tenantId);
        Task<bool> HasConflictingAppointmentAsync(DateTime scheduledDate, TimeSpan scheduledTime, 
            int durationMinutes, Guid clinicId, string tenantId, Guid? excludeAppointmentId = null);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(string tenantId, int days = 7);
        Task<IEnumerable<TimeSpan>> GetAvailableSlotsAsync(DateTime date, Guid clinicId, 
            int durationMinutes, string tenantId);
        
        // Optimized methods for performance
        Task<IEnumerable<Appointment>> GetDailyAgendaWithIncludesAsync(
            DateTime date, 
            Guid clinicId, 
            string tenantId, 
            Guid? professionalId = null);
        
        Task<int> GetDailyAppointmentCountAsync(
            DateTime date, 
            Guid clinicId, 
            string tenantId, 
            Guid? professionalId = null);
    }
}