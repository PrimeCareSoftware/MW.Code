namespace MedicSoft.Telemedicine.Domain.Enums;

/// <summary>
/// Role of a participant in the telemedicine session
/// </summary>
public enum ParticipantRole
{
    /// <summary>
    /// Medical professional (doctor, dentist, etc.)
    /// </summary>
    Provider = 0,
    
    /// <summary>
    /// Patient receiving care
    /// </summary>
    Patient = 1
}
