namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Types of SNGPC alerts
    /// </summary>
    public enum AlertType
    {
        DeadlineApproaching = 1,
        DeadlineOverdue = 2,
        MissingReport = 3,
        InvalidBalance = 4,
        NegativeBalance = 5,
        MissingRegistryEntry = 6,
        TransmissionFailed = 7,
        UnusualMovement = 8,
        ExcessiveDispensing = 9,
        ComplianceViolation = 10,
        SystemError = 11
    }

    /// <summary>
    /// Severity levels for alerts
    /// </summary>
    public enum AlertSeverity
    {
        Info = 1,
        Warning = 2,
        Error = 3,
        Critical = 4
    }
}
