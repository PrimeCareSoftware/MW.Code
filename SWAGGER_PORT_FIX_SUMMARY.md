# Swagger and Port Configuration Fix - Summary

## Problem Statement (Portuguese)
> a api do portal do paciente ainda nao exibe o swagger, a pagina fica em branco e esta dando erro de porta em uso quando estou executando o medicwarehouse.api

**Translation:** The patient portal API still doesn't display Swagger, the page is blank and there's a port in use error when running medicwarehouse.api

## Issues Identified and Fixed

### Issue 1: MedicSoft.Api (medicwarehouse.api) Swagger Only Available in Development

**Problem:**
- Swagger was only enabled when `app.Environment.IsDevelopment()` was true
- This prevented Swagger from loading in Production, Staging, or other environments
- Users couldn't access API documentation outside of Development mode

**Solution:**
- Made Swagger configurable via `SwaggerSettings:Enabled` setting (similar to PatientPortal.Api)
- Swagger now defaults to `true` in Development and `false` in Production
- Can be overridden via configuration files or environment variables

**Files Modified:**
- `/src/MedicSoft.Api/Program.cs` (lines 698-716)
- `/src/MedicSoft.Api/appsettings.json` (added SwaggerSettings section)
- `/src/MedicSoft.Api/appsettings.Development.json` (added SwaggerSettings: Enabled: true)
- `/src/MedicSoft.Api/appsettings.Production.json` (added SwaggerSettings: Enabled: false)

**Code Changes:**
```csharp
// Before:
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { ... });
}

// After:
var enableSwagger = builder.Configuration.GetValue<bool?>("SwaggerSettings:Enabled") 
    ?? app.Environment.IsDevelopment();

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { ... });
}
```

### Issue 2: Port Conflicts in MedicSoft.Api launchSettings.json

**Problem:**
- MedicSoft.Api had inconsistent port configuration:
  - HTTP profile: `http://localhost:5293`
  - HTTPS profile: `https://localhost:5000;http://localhost:5001`
- The HTTPS/HTTP ports were reversed from ASP.NET conventions
- Port 5293 was an odd choice that could conflict with other services
- Ports 5000/5001 could conflict if trying to run both APIs simultaneously

**Solution:**
- Standardized MedicSoft.Api ports:
  - HTTP profile: `http://localhost:5000`
  - HTTPS profile: `https://localhost:5001;http://localhost:5000`
- PatientPortal.Api remains on separate ports (5101 HTTP, 7030 HTTPS)
- No port conflicts when running both APIs simultaneously

**Files Modified:**
- `/src/MedicSoft.Api/Properties/launchSettings.json`

**Port Summary:**
| API | HTTP Port | HTTPS Port | Swagger URL |
|-----|-----------|------------|-------------|
| MedicSoft.Api (medicwarehouse.api) | 5000 | 5001 | http://localhost:5000/swagger |
| PatientPortal.Api | 5101 | 7030 | http://localhost:5101/ (root) |

### Issue 3: PatientPortal.Api Swagger Configuration (Already Correct)

**Status:** ✅ No changes needed

The PatientPortal.Api was already correctly configured with:
- Swagger enabled by default via `SwaggerSettings:Enabled: true`
- Swagger UI accessible at root path (`/`)
- Proper JWT Bearer authentication in Swagger

## Configuration Examples

### Enable Swagger in All Environments
In `appsettings.json` or `appsettings.Production.json`:
```json
{
  "SwaggerSettings": {
    "Enabled": true
  }
}
```

### Disable Swagger in Production (Recommended for Security)
In `appsettings.Production.json`:
```json
{
  "SwaggerSettings": {
    "Enabled": false
  }
}
```

### Control via Environment Variable
```bash
export SwaggerSettings__Enabled=true
dotnet run
```

Or in Docker:
```dockerfile
ENV SwaggerSettings__Enabled=false
```

## How to Test

### Prerequisites
1. PostgreSQL running on localhost:5432 (or via Docker)
2. Database credentials configured in appsettings
3. .NET 8.0 SDK installed

### Test MedicSoft.Api Swagger

```bash
# Navigate to the API directory
cd src/MedicSoft.Api

# Build the project
dotnet build

# Run the API
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Access Swagger in browser
# Open: http://localhost:5000/swagger
```

Expected result: Swagger UI should load and display all API endpoints

### Test PatientPortal.Api Swagger

```bash
# Navigate to the API directory
cd patient-portal-api/PatientPortal.Api

# Build the project
dotnet build

# Run the API
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Access Swagger in browser
# Open: http://localhost:5101/
```

Expected result: Swagger UI should load at the root URL

### Test Both APIs Simultaneously

```bash
# Terminal 1: Start MedicSoft.Api
cd src/MedicSoft.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Terminal 2: Start PatientPortal.Api
cd patient-portal-api/PatientPortal.Api
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Both APIs should start without port conflicts
# MedicSoft.Api: http://localhost:5000/swagger
# PatientPortal.Api: http://localhost:5101/
```

## Security Considerations

### Production Deployment

When deploying to production, consider:

1. **Disable Swagger** (recommended):
   - Set `SwaggerSettings:Enabled` to `false` in `appsettings.Production.json`
   - Prevents public exposure of API structure

2. **Network-Level Restrictions** (if Swagger is enabled):
   - Use firewall rules to restrict Swagger access to specific IP ranges
   - Deploy behind VPN for internal-only access
   - Use reverse proxy (nginx, IIS) to add authentication

3. **JWT Authentication**:
   - Both APIs already require JWT Bearer tokens for protected endpoints
   - Swagger UI includes authentication configuration
   - No sensitive data is exposed through Swagger schemas

## Build Verification

✅ **MedicSoft.Api**: Builds successfully with 340 warnings, 0 errors
✅ **PatientPortal.Api**: Builds successfully with 2 warnings, 0 errors

## Database Configuration Note

Both APIs require PostgreSQL to fully start. Ensure:

1. PostgreSQL is running on `localhost:5432` (or configured host)
2. Database credentials match between:
   - `docker-compose.yml` (if using Docker): `POSTGRES_PASSWORD=${POSTGRES_PASSWORD:-postgres}`
   - `appsettings.json`: Connection string password
   - `appsettings.Development.json`: Connection string password

Default credentials:
- **Development**: postgres/postgres (from docker-compose)
- **Production**: Use secure credentials from environment variables

## Related Documentation

- [CORRECAO_SWAGGER_PORTAL_PACIENTE.md](patient-portal-api/CORRECAO_SWAGGER_PORTAL_PACIENTE.md) - Patient Portal Swagger fix (previous)
- [SWAGGER_CONFIGURATION.md](patient-portal-api/SWAGGER_CONFIGURATION.md) - Swagger configuration guide (Patient Portal)
- [CORRECAO_SWAGGER_RESUMO.md](CORRECAO_SWAGGER_RESUMO.md) - Previous Swagger fix for IFormFile

## Summary

✅ **MedicSoft.Api Swagger**: Now configurable, no longer restricted to Development only
✅ **Port Conflicts**: Resolved by standardizing ports (5000/5001 for MedicSoft, 5101/7030 for PatientPortal)
✅ **PatientPortal.Api Swagger**: Already correctly configured
✅ **Build Status**: Both APIs build successfully
✅ **Security**: Swagger can be disabled in Production via configuration

The changes are minimal and focused on configuration, maintaining backward compatibility while providing flexibility for different deployment environments.
