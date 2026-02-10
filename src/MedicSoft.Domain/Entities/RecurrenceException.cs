using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an exception to a recurring appointment or blocked time slot pattern
    /// Used to track deleted, modified, or rescheduled occurrences
    /// </summary>
    public class RecurrenceException : BaseEntity
    {
        public Guid RecurringPatternId { get; private set; }
        public Guid RecurringSeriesId { get; private set; }
        public DateTime OriginalDate { get; private set; }
        public RecurrenceExceptionType ExceptionType { get; private set; }
        public DateTime? NewDate { get; private set; }
        public TimeSpan? NewStartTime { get; private set; }
        public TimeSpan? NewEndTime { get; private set; }
        public string? Reason { get; private set; }

        // Navigation properties
        public RecurringAppointmentPattern RecurringPattern { get; private set; } = null!;

        private RecurrenceException()
        {
            // EF Constructor
        }

        public RecurrenceException(
            Guid recurringPatternId,
            Guid recurringSeriesId,
            DateTime originalDate,
            RecurrenceExceptionType exceptionType,
            string tenantId,
            string? reason = null,
            DateTime? newDate = null,
            TimeSpan? newStartTime = null,
            TimeSpan? newEndTime = null) : base(tenantId)
        {
            if (recurringPatternId == Guid.Empty)
                throw new ArgumentException("Recurring pattern ID cannot be empty", nameof(recurringPatternId));

            if (recurringSeriesId == Guid.Empty)
                throw new ArgumentException("Recurring series ID cannot be empty", nameof(recurringSeriesId));

            RecurringPatternId = recurringPatternId;
            RecurringSeriesId = recurringSeriesId;
            OriginalDate = originalDate.Date; // Store only date part
            ExceptionType = exceptionType;
            Reason = reason?.Trim();
            NewDate = newDate?.Date;
            NewStartTime = newStartTime;
            NewEndTime = newEndTime;
        }

        public void UpdateReason(string? reason)
        {
            Reason = reason?.Trim();
            UpdateTimestamp();
        }

        public void UpdateNewSchedule(DateTime? newDate, TimeSpan? newStartTime, TimeSpan? newEndTime)
        {
            NewDate = newDate?.Date;
            NewStartTime = newStartTime;
            NewEndTime = newEndTime;
            UpdateTimestamp();
        }
    }
}
