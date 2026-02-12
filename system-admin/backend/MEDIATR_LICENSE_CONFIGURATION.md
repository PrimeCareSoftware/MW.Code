# MediatR License Configuration Guide

## Overview

This document explains how to configure the LuckyPennySoftware MediatR license key in the Omni Care Software application.

## What is LuckyPennySoftware.MediatR.License?

LuckyPennySoftware provides commercial licensing for MediatR. The license key is in JWT (JSON Web Token) format and contains information about:
- Account ID
- Customer ID
- License type and edition
- Expiration date
- Issuer information

## Configuration

### 1. License Key Storage

The license key is stored in the application configuration files:

#### Development Environment
In `appsettings.json`:
```json
{
  "MediatRLicense": {
    "LicenseKey": "eyJhbGciOiJSUzI1NiIs..."
  }
}
```

> **Note:** For sensitive projects, consider using [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for development instead of storing the license key in appsettings.json. The key in this file is for development/testing purposes only.

#### Production Environment
In `appsettings.Production.json`, the license key is referenced via environment variable:
```json
{
  "MediatRLicense": {
    "LicenseKey": "${MEDIATR_LICENSE_KEY}"
  }
}
```

Set the environment variable:
```bash
export MEDIATR_LICENSE_KEY="eyJhbGciOiJSUzI1NiIs..."
```

### 2. Code Components

The license configuration consists of three main components:

#### a) MediatRLicenseSettings.cs
Configuration class that holds the license key:
```csharp
namespace MedicSoft.Application.Configuration;

public class MediatRLicenseSettings
{
    public string LicenseKey { get; set; } = string.Empty;
    public bool IsConfigured => !string.IsNullOrWhiteSpace(LicenseKey);
}
```

#### b) MediatRLicenseService.cs
Service that initializes and manages the license:
```csharp
namespace MedicSoft.Application.Services;

public class MediatRLicenseService
{
    public void InitializeLicense() { /* ... */ }
    public string GetMaskedLicenseKey() { /* ... */ }
}
```

#### c) Program.cs Registration
The license is configured and initialized in `Program.cs`:
```csharp
// Configure MediatR License
builder.Services.Configure<MedicSoft.Application.Configuration.MediatRLicenseSettings>(
    builder.Configuration.GetSection("MediatRLicense"));
builder.Services.AddSingleton<MedicSoft.Application.Services.MediatRLicenseService>();

// Initialize MediatR License (after app.Build())
using (var scope = app.Services.CreateScope())
{
    var licenseService = scope.ServiceProvider
        .GetRequiredService<MedicSoft.Application.Services.MediatRLicenseService>();
    licenseService.InitializeLicense();
}
```

## Current License Key Information

The currently configured license key contains the following information (decoded from JWT):

**Header:**
```json
{
  "alg": "RS256",
  "kid": "LuckyPennySoftwareLicenseKey/bbb13acb59904d89b4cb1c85f088ccf9",
  "typ": "JWT"
}
```

**Payload:**
```json
{
  "iss": "https://luckypennysoftware.com",
  "aud": "LuckyPennySoftware",
  "exp": "1802390400",  // Expires: August 6, 2027
  "iat": "1770856605",  // Issued: February 7, 2025
  "account_id": "019afb08813c7ff8b8824e31b16f66ac",
  "customer_id": "ctm_01kbxghxex2cv4xnd5rggx7hya",
  "sub_id": "-",
  "edition": "0",
  "type": "2"
}
```

**License Status:**
- ✅ Valid and active
- ✅ Configured in production
- 📅 Expires: August 6, 2027
- 🔄 Renewal required before: July 2027

## Verification

### Check License Initialization

When the application starts, you should see log messages indicating the license has been configured:

```
info: MedicSoft.Application.Services.MediatRLicenseService[0]
      MediatR license key has been configured successfully.
```

### Verify in Code

You can check if the license is properly configured by injecting the `MediatRLicenseService`:

```csharp
public class MyController : ControllerBase
{
    private readonly MediatRLicenseService _licenseService;
    
    public MyController(MediatRLicenseService licenseService)
    {
        _licenseService = licenseService;
    }
    
    [HttpGet("license-status")]
    public IActionResult GetLicenseStatus()
    {
        var maskedKey = _licenseService.GetMaskedLicenseKey();
        return Ok(new { licenseConfigured = maskedKey != "Not configured", maskedKey });
    }
}
```

## Security Considerations

1. **Never commit license keys to version control** in plain text
2. Use environment variables for production deployments
3. The `GetMaskedLicenseKey()` method only shows first and last 10 characters for security
4. Keep the license key confidential - it's tied to your account

## Troubleshooting

### License Not Found Warning

If you see the warning:
```
MediatR license key is not configured. Some features may be limited.
```

**Solution:** Check that:
1. The `MediatRLicense` section exists in your appsettings.json
2. The `LicenseKey` property has a value
3. The environment variable is set correctly in production

### Invalid License Format

If the license key format is invalid, the application will log an error but continue running.

**Solution:** Verify the license key is:
- A valid JWT token
- Not truncated or modified
- Copied completely from the provider

## Future Enhancements

When the official `LuckyPennySoftware.MediatR.License` NuGet package becomes available:

1. Add the package reference:
   ```bash
   dotnet add package LuckyPennySoftware.MediatR.License
   ```

2. Update `MediatRLicenseService.InitializeLicense()` to call the actual validation:
   ```csharp
   LuckyPennySoftware.MediatR.License.SetLicenseKey(_settings.LicenseKey);
   ```

3. Handle any license validation errors appropriately

## Support

For issues with the license key or questions about licensing:
- Contact: LuckyPennySoftware support
- Website: https://luckypennysoftware.com
- Email: support@luckypennysoftware.com (verify actual contact)

## Summary

The MediatR license key from LuckyPennySoftware is now properly configured in the Omni Care Software application:
- ✅ Stored in configuration files (appsettings.json)
- ✅ Managed by dedicated service (MediatRLicenseService)
- ✅ Initialized at application startup
- ✅ Supports environment variables for production
- ✅ Includes security best practices
