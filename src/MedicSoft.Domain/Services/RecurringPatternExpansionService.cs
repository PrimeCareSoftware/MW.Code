using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Domain.Services
{
    /// <summary>
    /// Service to expand recurring patterns into individual occurrences
    /// </summary>
    public class RecurringPatternExpansionService
    {
        private readonly IRecurrenceExceptionRepository? _exceptionRepository;

        /// <summary>
        /// Maximum number of occurrences to prevent infinite loops or excessive memory usage
        /// </summary>
        public const int MaxOccurrences = 1000;

        public RecurringPatternExpansionService()
        {
            // Parameterless constructor for backward compatibility
        }

        public RecurringPatternExpansionService(IRecurrenceExceptionRepository exceptionRepository)
        {
            _exceptionRepository = exceptionRepository;
        }

        /// <summary>
        /// Generates dates based on a recurring pattern within a date range
        /// </summary>
        public IEnumerable<DateTime> ExpandPattern(RecurringAppointmentPattern pattern, DateTime? endDate = null)
        {
            if (!pattern.IsActive)
                return Enumerable.Empty<DateTime>();

            var effectiveEndDate = endDate ?? pattern.EndDate ?? DateTime.Today.AddYears(1);
            var dates = new List<DateTime>();
            var currentDate = pattern.StartDate;
            var occurrenceCount = 0;

            while (currentDate <= effectiveEndDate)
            {
                if (pattern.OccurrencesCount.HasValue && occurrenceCount >= pattern.OccurrencesCount.Value)
                    break;

                if (ShouldIncludeDate(pattern, currentDate))
                {
                    dates.Add(currentDate);
                    occurrenceCount++;
                }

                currentDate = GetNextDate(pattern, currentDate);

                // Safety check to prevent infinite loops or excessive memory usage
                // If you need more than MaxOccurrences, consider splitting into multiple patterns
                if (dates.Count >= MaxOccurrences)
                    break;
            }

            return dates;
        }

        private bool ShouldIncludeDate(RecurringAppointmentPattern pattern, DateTime date)
        {
            switch (pattern.Frequency)
            {
                case RecurrenceFrequency.Daily:
                    return true;

                case RecurrenceFrequency.Weekly:
                    if (!pattern.DaysOfWeek.HasValue)
                        return false;
                    
                    var dayFlag = GetDayOfWeekFlag(date.DayOfWeek);
                    return pattern.DaysOfWeek.Value.HasFlag(dayFlag);

                case RecurrenceFrequency.Biweekly:
                    if (!pattern.DaysOfWeek.HasValue)
                        return false;
                    
                    var biweeklyDayFlag = GetDayOfWeekFlag(date.DayOfWeek);
                    var weeksDiff = (int)Math.Floor((date - pattern.StartDate).TotalDays / 7.0);
                    return weeksDiff % 2 == 0 && pattern.DaysOfWeek.Value.HasFlag(biweeklyDayFlag);

                case RecurrenceFrequency.Monthly:
                    return pattern.DayOfMonth.HasValue && date.Day == pattern.DayOfMonth.Value;

                default:
                    return false;
            }
        }

        private DateTime GetNextDate(RecurringAppointmentPattern pattern, DateTime currentDate)
        {
            switch (pattern.Frequency)
            {
                case RecurrenceFrequency.Daily:
                    return currentDate.AddDays(pattern.Interval);

                case RecurrenceFrequency.Weekly:
                case RecurrenceFrequency.Biweekly:
                    return currentDate.AddDays(1); // Check each day, filter in ShouldIncludeDate

                case RecurrenceFrequency.Monthly:
                    var nextMonth = currentDate.AddMonths(pattern.Interval);
                    // Handle edge case where day doesn't exist in next month (e.g., Jan 31 -> Feb 31)
                    if (pattern.DayOfMonth.HasValue)
                    {
                        var daysInMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                        var targetDay = Math.Min(pattern.DayOfMonth.Value, daysInMonth);
                        return new DateTime(nextMonth.Year, nextMonth.Month, targetDay);
                    }
                    return nextMonth;

                default:
                    return currentDate.AddDays(1);
            }
        }

        private RecurrenceDays GetDayOfWeekFlag(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => RecurrenceDays.Sunday,
                DayOfWeek.Monday => RecurrenceDays.Monday,
                DayOfWeek.Tuesday => RecurrenceDays.Tuesday,
                DayOfWeek.Wednesday => RecurrenceDays.Wednesday,
                DayOfWeek.Thursday => RecurrenceDays.Thursday,
                DayOfWeek.Friday => RecurrenceDays.Friday,
                DayOfWeek.Saturday => RecurrenceDays.Saturday,
                _ => RecurrenceDays.None
            };
        }

        /// <summary>
        /// Generates appointments from a recurring pattern
        /// </summary>
        public IEnumerable<Appointment> GenerateAppointments(
            RecurringAppointmentPattern pattern,
            DateTime? endDate = null)
        {
            if (!pattern.IsForAppointments() || !pattern.PatientId.HasValue || !pattern.DurationMinutes.HasValue)
                return Enumerable.Empty<Appointment>();

            var dates = ExpandPattern(pattern, endDate);
            var appointments = new List<Appointment>();

            foreach (var date in dates)
            {
                var appointment = new Appointment(
                    patientId: pattern.PatientId.Value,
                    clinicId: pattern.ClinicId,
                    scheduledDate: date,
                    scheduledTime: pattern.StartTime,
                    durationMinutes: pattern.DurationMinutes.Value,
                    type: pattern.AppointmentType ?? AppointmentType.Regular,
                    tenantId: pattern.TenantId,
                    professionalId: pattern.ProfessionalId,
                    notes: pattern.Notes,
                    allowHistoricalData: true // Allow past dates for pattern expansion
                );
                appointments.Add(appointment);
            }

            return appointments;
        }

        /// <summary>
        /// Generates blocked time slots from a recurring pattern with unique series ID
        /// </summary>
        /// <param name="pattern">The recurring pattern</param>
        /// <param name="endDate">Optional end date to limit expansion</param>
        /// <param name="seriesId">Optional explicit series ID. If not provided, a new one will be generated</param>
        /// <param name="tenantId">Tenant ID for exception checking</param>
        public IEnumerable<BlockedTimeSlot> GenerateBlockedTimeSlots(
            RecurringAppointmentPattern pattern,
            DateTime? endDate = null,
            Guid? seriesId = null,
            string? tenantId = null)
        {
            if (!pattern.IsForBlockedSlots())
                return Enumerable.Empty<BlockedTimeSlot>();

            var dates = ExpandPattern(pattern, endDate);
            var blockedSlots = new List<BlockedTimeSlot>();
            
            // Generate unique series ID for this expansion
            var actualSeriesId = seriesId ?? Guid.NewGuid();

            // Get exceptions for this series if repository is available and tenant ID is provided
            HashSet<DateTime>? exceptionDates = null;
            if (_exceptionRepository != null && !string.IsNullOrEmpty(tenantId) && seriesId.HasValue)
            {
                // Note: This is async operation but we can't await in non-async method
                // For now, we'll check exceptions in the handler layer
                // This remains for backward compatibility
            }

            foreach (var date in dates)
            {
                // Skip if this date is an exception (if we have exception dates loaded)
                if (exceptionDates != null && exceptionDates.Contains(date.Date))
                    continue;

                var blockedSlot = new BlockedTimeSlot(
                    clinicId: pattern.ClinicId,
                    date: date,
                    startTime: pattern.StartTime,
                    endTime: pattern.EndTime,
                    type: pattern.BlockedSlotType ?? BlockedTimeSlotType.Unavailable,
                    tenantId: pattern.TenantId,
                    professionalId: pattern.ProfessionalId,
                    reason: pattern.Notes,
                    isRecurring: true,
                    recurringPatternId: pattern.Id,
                    recurringSeriesId: actualSeriesId);
                blockedSlots.Add(blockedSlot);
            }

            return blockedSlots;
        }

        /// <summary>
        /// Async version that properly checks for exceptions
        /// </summary>
        public async Task<IEnumerable<BlockedTimeSlot>> GenerateBlockedTimeSlotsAsync(
            RecurringAppointmentPattern pattern,
            DateTime? endDate = null,
            Guid? seriesId = null)
        {
            if (!pattern.IsForBlockedSlots())
                return Enumerable.Empty<BlockedTimeSlot>();

            var dates = ExpandPattern(pattern, endDate);
            var blockedSlots = new List<BlockedTimeSlot>();
            
            // Generate unique series ID for this expansion
            var actualSeriesId = seriesId ?? Guid.NewGuid();

            // Get exceptions for this series if repository is available
            HashSet<DateTime> exceptionDates = new HashSet<DateTime>();
            if (_exceptionRepository != null && seriesId.HasValue)
            {
                var exceptions = await _exceptionRepository.GetByRecurringSeriesIdAsync(seriesId.Value, pattern.TenantId);
                foreach (var ex in exceptions)
                {
                    exceptionDates.Add(ex.OriginalDate.Date);
                }
            }

            foreach (var date in dates)
            {
                // Skip if this date is an exception
                if (exceptionDates.Contains(date.Date))
                    continue;

                var blockedSlot = new BlockedTimeSlot(
                    clinicId: pattern.ClinicId,
                    date: date,
                    startTime: pattern.StartTime,
                    endTime: pattern.EndTime,
                    type: pattern.BlockedSlotType ?? BlockedTimeSlotType.Unavailable,
                    tenantId: pattern.TenantId,
                    professionalId: pattern.ProfessionalId,
                    reason: pattern.Notes,
                    isRecurring: true,
                    recurringPatternId: pattern.Id,
                    recurringSeriesId: actualSeriesId);
                blockedSlots.Add(blockedSlot);
            }

            return blockedSlots;
        }
    }
}
