namespace PatientPortal.Application.Configuration;

/// <summary>
/// Configuration settings for portal application
/// </summary>
public class PortalSettings
{
    public const string SectionName = "PortalSettings";

    /// <summary>
    /// Base URL for the patient portal (used in emails and links)
    /// </summary>
    public string BaseUrl { get; set; } = "https://portal.primecare.com";
}
