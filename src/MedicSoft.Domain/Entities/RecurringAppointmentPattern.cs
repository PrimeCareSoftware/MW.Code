using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a pattern for recurring appointments or blocked time slots
    /// </summary>
    public class RecurringAppointmentPattern : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public Guid? ProfessionalId { get; private set; }
        public Guid? PatientId { get; private set; }  // If for recurring appointments
        public RecurrenceFrequency Frequency { get; private set; }
        public int Interval { get; private set; } = 1; // Every N days/weeks/months
        public RecurrenceDays? DaysOfWeek { get; private set; }  // For weekly recurrence
        public int? DayOfMonth { get; private set; }  // For monthly recurrence (1-31)
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public int? OccurrencesCount { get; private set; }  // Alternative to EndDate
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public int? DurationMinutes { get; private set; }  // For appointments
        public AppointmentType? AppointmentType { get; private set; }  // If for appointments
        public BlockedTimeSlotType? BlockedSlotType { get; private set; }  // If for blocks
        public string? Notes { get; private set; }
        public bool IsActive { get; private set; } = true;
        
        // Navigation properties
        public Clinic Clinic { get; private set; } = null!;
        public User? Professional { get; private set; }
        public Patient? Patient { get; private set; }

        private RecurringAppointmentPattern() 
        { 
            // EF Constructor
        }

        public RecurringAppointmentPattern(
            Guid clinicId,
            RecurrenceFrequency frequency,
            DateTime startDate,
            TimeSpan startTime,
            TimeSpan endTime,
            string tenantId,
            int interval = 1,
            Guid? professionalId = null,
            Guid? patientId = null,
            RecurrenceDays? daysOfWeek = null,
            int? dayOfMonth = null,
            DateTime? endDate = null,
            int? occurrencesCount = null,
            int? durationMinutes = null,
            AppointmentType? appointmentType = null,
            BlockedTimeSlotType? blockedSlotType = null,
            string? notes = null) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (interval < 1)
                throw new ArgumentException("Interval must be at least 1", nameof(interval));

            if (startTime >= endTime)
                throw new ArgumentException("Start time must be before end time", nameof(startTime));

            if (endDate.HasValue && endDate.Value < startDate)
                throw new ArgumentException("End date must be after start date", nameof(endDate));

            if (occurrencesCount.HasValue && occurrencesCount.Value < 1)
                throw new ArgumentException("Occurrences count must be at least 1", nameof(occurrencesCount));

            if (frequency == RecurrenceFrequency.Weekly && !daysOfWeek.HasValue)
                throw new ArgumentException("Days of week must be specified for weekly recurrence", nameof(daysOfWeek));

            if (frequency == RecurrenceFrequency.Monthly && !dayOfMonth.HasValue)
                throw new ArgumentException("Day of month must be specified for monthly recurrence", nameof(dayOfMonth));

            if (dayOfMonth.HasValue && (dayOfMonth.Value < 1 || dayOfMonth.Value > 31))
                throw new ArgumentException("Day of month must be between 1 and 31", nameof(dayOfMonth));

            // Must be either for appointments OR for blocked slots, not both
            if (appointmentType.HasValue && blockedSlotType.HasValue)
                throw new ArgumentException("Pattern cannot be both appointment and blocked slot");

            if (appointmentType.HasValue && !patientId.HasValue)
                throw new ArgumentException("Patient ID is required for recurring appointments", nameof(patientId));

            if (appointmentType.HasValue && !durationMinutes.HasValue)
                throw new ArgumentException("Duration is required for recurring appointments", nameof(durationMinutes));

            ClinicId = clinicId;
            ProfessionalId = professionalId;
            PatientId = patientId;
            Frequency = frequency;
            Interval = interval;
            DaysOfWeek = daysOfWeek;
            DayOfMonth = dayOfMonth;
            StartDate = startDate;
            EndDate = endDate;
            OccurrencesCount = occurrencesCount;
            StartTime = startTime;
            EndTime = endTime;
            DurationMinutes = durationMinutes;
            AppointmentType = appointmentType;
            BlockedSlotType = blockedSlotType;
            Notes = notes?.Trim();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void UpdateEndDate(DateTime? endDate)
        {
            if (endDate.HasValue && endDate.Value < StartDate)
                throw new ArgumentException("End date must be after start date", nameof(endDate));

            EndDate = endDate;
            OccurrencesCount = null; // Clear occurrences if setting end date
            UpdateTimestamp();
        }

        public void UpdateOccurrencesCount(int? count)
        {
            if (count.HasValue && count.Value < 1)
                throw new ArgumentException("Occurrences count must be at least 1", nameof(count));

            OccurrencesCount = count;
            EndDate = null; // Clear end date if setting occurrences
            UpdateTimestamp();
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }

        public bool IsForAppointments()
        {
            return AppointmentType.HasValue;
        }

        public bool IsForBlockedSlots()
        {
            return BlockedSlotType.HasValue;
        }
    }
}
