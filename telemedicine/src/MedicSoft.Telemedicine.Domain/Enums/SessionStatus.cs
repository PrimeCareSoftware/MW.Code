namespace MedicSoft.Telemedicine.Domain.Enums;

/// <summary>
/// Status of a telemedicine session
/// </summary>
public enum SessionStatus
{
    /// <summary>
    /// Session is scheduled but not started
    /// </summary>
    Scheduled = 0,
    
    /// <summary>
    /// Session is active and in progress
    /// </summary>
    InProgress = 1,
    
    /// <summary>
    /// Session has ended normally
    /// </summary>
    Completed = 2,
    
    /// <summary>
    /// Session was cancelled
    /// </summary>
    Cancelled = 3,
    
    /// <summary>
    /// Session failed due to technical issues
    /// </summary>
    Failed = 4
}
