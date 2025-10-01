using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Domain.Services
{
    public class AppointmentSchedulingService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClinicRepository _clinicRepository;

        public AppointmentSchedulingService(
            IAppointmentRepository appointmentRepository,
            IClinicRepository clinicRepository)
        {
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableSlotsAsync(
            DateTime date, Guid clinicId, int durationMinutes, string tenantId)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found", nameof(clinicId));

            var availableSlots = new List<TimeSpan>();
            var currentTime = clinic.OpeningTime;

            // Get existing appointments for the day
            var existingAppointments = await _appointmentRepository.GetDailyAgendaAsync(date, clinicId, tenantId);
            var bookedSlots = existingAppointments
                .Where(a => a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.ScheduledTime)
                .ToList();

            while (currentTime.Add(TimeSpan.FromMinutes(durationMinutes)) <= clinic.ClosingTime)
            {
                var proposedEndTime = currentTime.Add(TimeSpan.FromMinutes(durationMinutes));
                var isSlotAvailable = !bookedSlots.Any(appointment =>
                {
                    var appointmentStart = appointment.ScheduledTime;
                    var appointmentEnd = appointmentStart.Add(TimeSpan.FromMinutes(appointment.DurationMinutes));
                    
                    // Check if proposed slot overlaps with existing appointment
                    return currentTime < appointmentEnd && proposedEndTime > appointmentStart;
                });

                if (isSlotAvailable)
                {
                    availableSlots.Add(currentTime);
                }

                currentTime = currentTime.Add(TimeSpan.FromMinutes(clinic.AppointmentDurationMinutes));
            }

            return availableSlots;
        }

        public async Task<bool> CanScheduleAppointmentAsync(
            DateTime scheduledDate, TimeSpan scheduledTime, int durationMinutes, 
            Guid clinicId, string tenantId, Guid? excludeAppointmentId = null)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
                return false;

            // Check if time is within clinic working hours
            var endTime = scheduledTime.Add(TimeSpan.FromMinutes(durationMinutes));
            if (!clinic.IsWithinWorkingHours(scheduledTime) || !clinic.IsWithinWorkingHours(endTime))
                return false;

            // Check for conflicts with existing appointments
            return !await _appointmentRepository.HasConflictingAppointmentAsync(
                scheduledDate, scheduledTime, durationMinutes, clinicId, tenantId, excludeAppointmentId);
        }

        public async Task<Appointment> ScheduleAppointmentAsync(
            Guid patientId, Guid clinicId, DateTime scheduledDate, TimeSpan scheduledTime,
            int durationMinutes, AppointmentType type, string tenantId, string? notes = null)
        {
            if (!await CanScheduleAppointmentAsync(scheduledDate, scheduledTime, durationMinutes, clinicId, tenantId))
            {
                throw new InvalidOperationException("Cannot schedule appointment at the requested time");
            }

            var appointment = new Appointment(
                patientId, clinicId, scheduledDate, scheduledTime, 
                durationMinutes, type, tenantId, notes);

            return await _appointmentRepository.AddAsync(appointment);
        }

        public async Task<Appointment> ScheduleEmergencyAppointmentAsync(
            Guid patientId, Guid clinicId, DateTime scheduledDate, 
            int durationMinutes, string tenantId, string? notes = null)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found", nameof(clinicId));

            if (!clinic.AllowEmergencySlots)
                throw new InvalidOperationException("Clinic does not allow emergency appointments");

            // Find the next available emergency slot
            var availableSlots = await GetAvailableSlotsAsync(scheduledDate, clinicId, durationMinutes, tenantId);
            if (!availableSlots.Any())
                throw new InvalidOperationException("No available emergency slots for the requested date");

            var emergencySlot = availableSlots.First();
            
            var appointment = new Appointment(
                patientId, clinicId, scheduledDate, emergencySlot, 
                durationMinutes, AppointmentType.Emergency, tenantId, notes);

            return await _appointmentRepository.AddAsync(appointment);
        }
    }
}