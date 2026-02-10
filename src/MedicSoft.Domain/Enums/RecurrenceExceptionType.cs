namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Types of exceptions that can occur in a recurring appointment or blocked time slot series
    /// </summary>
    public enum RecurrenceExceptionType
    {
        /// <summary>
        /// The occurrence was deleted (soft delete)
        /// </summary>
        Deleted = 1,

        /// <summary>
        /// The occurrence was modified (time or other properties changed)
        /// </summary>
        Modified = 2,

        /// <summary>
        /// The occurrence was rescheduled to a different date/time
        /// </summary>
        Rescheduled = 3
    }
}
