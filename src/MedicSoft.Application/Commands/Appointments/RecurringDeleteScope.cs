namespace MedicSoft.Application.Commands.Appointments
{
    /// <summary>
    /// Defines the scope of deletion for recurring appointments or blocked time slots
    /// Inspired by Google Calendar, Microsoft Outlook, and RFC 5545 (iCalendar)
    /// </summary>
    public enum RecurringDeleteScope
    {
        /// <summary>
        /// Delete only this single occurrence (soft delete via exception)
        /// Other occurrences in the series remain intact
        /// </summary>
        ThisOccurrence = 1,

        /// <summary>
        /// Delete this occurrence and all future occurrences
        /// Past occurrences remain intact
        /// </summary>
        ThisAndFuture = 2,

        /// <summary>
        /// Delete the entire series
        /// All occurrences (past, present, and future) are removed
        /// </summary>
        AllInSeries = 3
    }
}
