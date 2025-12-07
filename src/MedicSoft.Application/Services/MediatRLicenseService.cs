using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Application.Configuration;

namespace MedicSoft.Application.Services;

/// <summary>
/// Service to manage MediatR license from LuckyPennySoftware
/// </summary>
public class MediatRLicenseService
{
    private readonly MediatRLicenseSettings _settings;
    private readonly ILogger<MediatRLicenseService> _logger;

    public MediatRLicenseService(
        IOptions<MediatRLicenseSettings> settings,
        ILogger<MediatRLicenseService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Initialize the MediatR license with the configured license key
    /// </summary>
    public void InitializeLicense()
    {
        if (!_settings.IsConfigured)
        {
            _logger.LogWarning("MediatR license key is not configured. Some features may be limited.");
            return;
        }

        try
        {
            // In a real implementation, this would call the actual license validation
            // from LuckyPennySoftware.MediatR.License package
            // For now, we'll just log that the license is configured
            _logger.LogInformation("MediatR license key has been configured successfully.");
            _logger.LogDebug("License key format: JWT token with {length} characters", _settings.LicenseKey.Length);
            
            // Future: Add actual license validation here when the package is available
            // Example: LuckyPennySoftware.MediatR.License.SetLicenseKey(_settings.LicenseKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize MediatR license. Error: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets the license key (for debugging purposes only)
    /// </summary>
    public string GetMaskedLicenseKey()
    {
        if (!_settings.IsConfigured)
        {
            return "Not configured";
        }

        // Return only first and last 10 characters for security
        if (_settings.LicenseKey.Length > 20)
        {
            return $"{_settings.LicenseKey[..10]}...{_settings.LicenseKey[^10..]}";
        }

        return "***";
    }
}
