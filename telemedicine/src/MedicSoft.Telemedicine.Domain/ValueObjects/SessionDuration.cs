namespace MedicSoft.Telemedicine.Domain.ValueObjects;

/// <summary>
/// Value Object representing the duration of a session
/// </summary>
public class SessionDuration
{
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    
    public SessionDuration(DateTime startTime)
    {
        if (startTime > DateTime.UtcNow.AddMinutes(5))
            throw new ArgumentException("Start time cannot be more than 5 minutes in the future");
            
        StartTime = startTime;
    }
    
    public void End()
    {
        if (EndTime.HasValue)
            throw new InvalidOperationException("Session already ended");
            
        EndTime = DateTime.UtcNow;
    }
    
    public TimeSpan? GetDuration()
    {
        if (!EndTime.HasValue)
            return null;
            
        return EndTime.Value - StartTime;
    }
    
    public int GetDurationInMinutes()
    {
        var duration = GetDuration();
        return duration.HasValue ? (int)Math.Ceiling(duration.Value.TotalMinutes) : 0;
    }
}
