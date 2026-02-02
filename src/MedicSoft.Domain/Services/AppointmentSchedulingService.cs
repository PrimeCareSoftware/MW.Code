using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Domain.Services
{
    public class AppointmentSchedulingService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IBlockedTimeSlotRepository? _blockedTimeSlotRepository;

        public AppointmentSchedulingService(
            IAppointmentRepository appointmentRepository,
            IClinicRepository clinicRepository,
            IBlockedTimeSlotRepository? blockedTimeSlotRepository = null)
        {
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _blockedTimeSlotRepository = blockedTimeSlotRepository;
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableSlotsAsync(
            DateTime date, Guid clinicId, int durationMinutes, string tenantId, Guid? professionalId = null)
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

            // Get blocked time slots for the day
            var blockedSlots = _blockedTimeSlotRepository != null
                ? (await _blockedTimeSlotRepository.GetByDateAsync(date, clinicId, tenantId)).ToList()
                : new List<BlockedTimeSlot>();

            while (currentTime.Add(TimeSpan.FromMinutes(durationMinutes)) < clinic.ClosingTime)
            {
                var proposedEndTime = currentTime.Add(TimeSpan.FromMinutes(durationMinutes));
                
                // Check if proposed slot overlaps with existing appointment
                var hasAppointmentConflict = bookedSlots.Any(appointment =>
                {
                    var appointmentStart = appointment.ScheduledTime;
                    var appointmentEnd = appointmentStart.Add(TimeSpan.FromMinutes(appointment.DurationMinutes));
                    return currentTime < appointmentEnd && proposedEndTime > appointmentStart;
                });

                // Check if proposed slot overlaps with blocked time slot
                var hasBlockedConflict = blockedSlots.Any(block =>
                {
                    // Block applies if it's for entire clinic or for the specific professional
                    var appliesToProfessional = !block.ProfessionalId.HasValue || 
                                               (professionalId.HasValue && block.ProfessionalId == professionalId);
                    return appliesToProfessional && block.IsOverlapping(currentTime, proposedEndTime);
                });

                if (!hasAppointmentConflict && !hasBlockedConflict)
                {
                    availableSlots.Add(currentTime);
                }

                currentTime = currentTime.Add(TimeSpan.FromMinutes(clinic.AppointmentDurationMinutes));
            }

            return availableSlots;
        }

        public async Task<(bool IsValid, string? ErrorReason)> CanScheduleAppointmentWithReasonAsync(
            DateTime scheduledDate, TimeSpan scheduledTime, int durationMinutes, 
            Guid clinicId, string tenantId, Guid? professionalId = null, Guid? excludeAppointmentId = null)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
                return (false, "Clinic not found");

            // Check if time is within clinic working hours
            var endTime = scheduledTime.Add(TimeSpan.FromMinutes(durationMinutes));
            if (!clinic.IsWithinWorkingHours(scheduledTime))
                return (false, $"Appointment start time {scheduledTime:hh\\:mm} is outside clinic working hours ({clinic.OpeningTime:hh\\:mm} - {clinic.ClosingTime:hh\\:mm})");
            
            if (!clinic.IsWithinWorkingHours(endTime))
                return (false, $"Appointment end time {endTime:hh\\:mm} is outside clinic working hours ({clinic.OpeningTime:hh\\:mm} - {clinic.ClosingTime:hh\\:mm})");

            // Check for conflicts with existing appointments
            var hasConflict = await _appointmentRepository.HasConflictingAppointmentAsync(
                scheduledDate, scheduledTime, durationMinutes, clinicId, tenantId, excludeAppointmentId);
            
            if (hasConflict)
                return (false, $"Time slot {scheduledTime:hh\\:mm}-{endTime:hh\\:mm} is already booked");

            // Check for conflicts with blocked time slots
            if (_blockedTimeSlotRepository != null)
            {
                var hasBlockedConflict = await _blockedTimeSlotRepository.HasOverlappingBlockAsync(
                    clinicId, scheduledDate, scheduledTime, endTime, professionalId, tenantId);
                
                if (hasBlockedConflict)
                    return (false, $"Time slot {scheduledTime:hh\\:mm}-{endTime:hh\\:mm} is blocked");
            }

            return (true, null);
        }

        public async Task<bool> CanScheduleAppointmentAsync(
            DateTime scheduledDate, TimeSpan scheduledTime, int durationMinutes, 
            Guid clinicId, string tenantId, Guid? professionalId = null, Guid? excludeAppointmentId = null)
        {
            var (isValid, _) = await CanScheduleAppointmentWithReasonAsync(scheduledDate, scheduledTime, durationMinutes, clinicId, tenantId, professionalId, excludeAppointmentId);
            return isValid;
        }

        public async Task<Appointment> ScheduleAppointmentAsync(
            Guid patientId, Guid clinicId, DateTime scheduledDate, TimeSpan scheduledTime,
            int durationMinutes, AppointmentType type, string tenantId, Guid? professionalId = null, string? notes = null)
        {
            var (isValid, errorReason) = await CanScheduleAppointmentWithReasonAsync(scheduledDate, scheduledTime, durationMinutes, clinicId, tenantId, professionalId);
            if (!isValid)
            {
                throw new InvalidOperationException(errorReason ?? "Cannot schedule appointment at the requested time");
            }

            var appointment = new Appointment(
                patientId, clinicId, scheduledDate, scheduledTime, 
                durationMinutes, type, tenantId, AppointmentMode.InPerson, PaymentType.Private, professionalId, null, notes);

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

            var emergencySlot = availableSlots.FirstOrDefault();
            if (emergencySlot == default)
                throw new InvalidOperationException("Failed to retrieve emergency slot");
            
            var appointment = new Appointment(
                patientId, clinicId, scheduledDate, emergencySlot, 
                durationMinutes, AppointmentType.Emergency, tenantId, AppointmentMode.InPerson, PaymentType.Private, null, null, notes);

            return await _appointmentRepository.AddAsync(appointment);
        }
    }
}