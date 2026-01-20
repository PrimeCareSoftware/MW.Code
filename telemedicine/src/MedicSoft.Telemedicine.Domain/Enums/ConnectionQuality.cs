namespace MedicSoft.Telemedicine.Domain.Enums;

/// <summary>
/// Quality of connection during telemedicine session
/// Required for CFM 2.314/2022 compliance
/// </summary>
public enum ConnectionQuality
{
    /// <summary>
    /// Excellent connection quality (HD video, no issues)
    /// </summary>
    Excellent = 0,
    
    /// <summary>
    /// Good connection quality (stable video)
    /// </summary>
    Good = 1,
    
    /// <summary>
    /// Fair connection quality (some buffering)
    /// </summary>
    Fair = 2,
    
    /// <summary>
    /// Poor connection quality (frequent interruptions)
    /// </summary>
    Poor = 3,
    
    /// <summary>
    /// Connection failed
    /// </summary>
    Failed = 4,
    
    /// <summary>
    /// Not yet measured
    /// </summary>
    Unknown = 5
}
