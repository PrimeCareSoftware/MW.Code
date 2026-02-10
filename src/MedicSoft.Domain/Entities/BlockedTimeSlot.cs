using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a blocked time slot in the schedule that cannot be used for appointments
    /// </summary>
    public class BlockedTimeSlot : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public Guid? ProfessionalId { get; private set; }  // Null = blocks for entire clinic
        public DateTime Date { get; private set; }
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public BlockedTimeSlotType Type { get; private set; }
        public string? Reason { get; private set; }
        public bool IsRecurring { get; private set; }
        public Guid? RecurringPatternId { get; private set; }  // Link to recurring pattern if applicable
        public Guid? RecurringSeriesId { get; private set; }  // Unique identifier for this specific series
        public DateTime? OriginalOccurrenceDate { get; private set; }  // Original date for modified occurrences
        public bool IsException { get; private set; }  // Flag indicating this is an exception
        
        // Navigation properties
        public Clinic Clinic { get; private set; } = null!;
        public User? Professional { get; private set; }
        public RecurringAppointmentPattern? RecurringPattern { get; private set; }

        private BlockedTimeSlot() 
        { 
            // EF Constructor
        }

        public BlockedTimeSlot(
            Guid clinicId, 
            DateTime date, 
            TimeSpan startTime, 
            TimeSpan endTime,
            BlockedTimeSlotType type,
            string tenantId,
            Guid? professionalId = null,
            string? reason = null,
            bool isRecurring = false,
            Guid? recurringPatternId = null,
            Guid? recurringSeriesId = null) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (startTime >= endTime)
                throw new ArgumentException("Start time must be before end time", nameof(startTime));

            if (date < DateTime.Today && !isRecurring)
                throw new ArgumentException("Cannot block time slots in the past", nameof(date));

            ClinicId = clinicId;
            ProfessionalId = professionalId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Type = type;
            Reason = reason?.Trim();
            IsRecurring = isRecurring;
            RecurringPatternId = recurringPatternId;
            RecurringSeriesId = recurringSeriesId;
            IsException = false;
        }

        public void UpdateTimeSlot(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("Start time must be before end time", nameof(startTime));

            StartTime = startTime;
            EndTime = endTime;
            UpdateTimestamp();
        }

        public void UpdateReason(string? reason)
        {
            Reason = reason?.Trim();
            UpdateTimestamp();
        }

        public void UpdateType(BlockedTimeSlotType type)
        {
            Type = type;
            UpdateTimestamp();
        }

        public bool IsOverlapping(TimeSpan start, TimeSpan end)
        {
            return StartTime < end && EndTime > start;
        }

        public bool IsOverlappingWithDate(DateTime date, TimeSpan start, TimeSpan end)
        {
            return Date == date && IsOverlapping(start, end);
        }

        /// <summary>
        /// Marks this blocked time slot as an exception (soft delete or modification)
        /// </summary>
        public void MarkAsException(DateTime? originalDate = null)
        {
            IsException = true;
            if (originalDate.HasValue)
            {
                OriginalOccurrenceDate = originalDate.Value;
            }
            UpdateTimestamp();
        }

        /// <summary>
        /// Updates the recurring series ID for this blocked time slot
        /// </summary>
        public void UpdateSeriesId(Guid seriesId)
        {
            RecurringSeriesId = seriesId;
            UpdateTimestamp();
        }
    }
}
