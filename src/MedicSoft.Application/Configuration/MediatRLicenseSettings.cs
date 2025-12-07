namespace MedicSoft.Application.Configuration;

/// <summary>
/// Configuration settings for MediatR license from LuckyPennySoftware
/// </summary>
public class MediatRLicenseSettings
{
    /// <summary>
    /// The JWT license key for LuckyPennySoftware.MediatR
    /// </summary>
    public string LicenseKey { get; set; } = string.Empty;

    /// <summary>
    /// Validates if the license configuration is properly set
    /// </summary>
    public bool IsConfigured => !string.IsNullOrWhiteSpace(LicenseKey);
}
