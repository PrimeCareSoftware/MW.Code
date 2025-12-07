# Configuration Classes

This directory contains configuration classes for various application settings.

## MediatRLicenseSettings.cs

Configuration class for LuckyPennySoftware MediatR license key.

**Properties:**
- `LicenseKey` (string): The JWT license key from LuckyPennySoftware
- `IsConfigured` (bool): Indicates if the license key is properly configured

**Usage:**
```csharp
// Configure in Program.cs
builder.Services.Configure<MediatRLicenseSettings>(
    builder.Configuration.GetSection("MediatRLicense"));

// Inject in services
public MyService(IOptions<MediatRLicenseSettings> settings)
{
    _settings = settings.Value;
}
```

**Configuration in appsettings.json:**
```json
{
  "MediatRLicense": {
    "LicenseKey": "eyJhbGciOiJSUzI1NiIs..."
  }
}
```

For complete documentation, see: [/docs/MEDIATR_LICENSE_CONFIGURATION.md](../../../docs/MEDIATR_LICENSE_CONFIGURATION.md)
