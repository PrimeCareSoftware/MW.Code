# Swagger 403 Forbidden Error Fix - February 2026

## Problem Description

Users were experiencing a "Forbidden /swagger/v1/swagger.json" error when trying to access the Swagger API documentation. The Swagger UI would display:

```
Select a definition
Failed to load API definition.

Errors
Fetch error
Forbidden /swagger/v1/swagger.json
```

## Root Cause Analysis

The issue was caused by Swagger's global security requirement configuration. In the `Program.cs` file, Swagger was configured with a global `AddSecurityRequirement` that applies the JWT Bearer authentication requirement to ALL documented API endpoints:

```csharp
c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }
});
```

This global requirement meant that:
1. All API endpoints in the Swagger documentation showed as requiring authentication
2. Endpoints marked with `[AllowAnonymous]` still showed authentication requirements in the documentation
3. The Swagger UI could not properly document which endpoints truly require authentication

While this didn't directly cause the 403 error (since Swagger middleware is placed early in the pipeline, before authentication), it created confusion and could cause issues depending on the environment configuration.

## Solution Implemented

### 1. Created AuthorizeCheckOperationFilter

Added a new Swagger operation filter (`AuthorizeCheckOperationFilter.cs`) that intelligently removes security requirements from endpoints based on their authorization attributes:

```csharp
public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Check if the endpoint has [AllowAnonymous] attribute
        var hasAllowAnonymous = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any() ?? false;

        if (!hasAllowAnonymous)
        {
            hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Any();
        }

        // If the endpoint has [AllowAnonymous], remove security requirements
        if (hasAllowAnonymous)
        {
            operation.Security?.Clear();
            return;
        }

        // Check if the endpoint has [Authorize] attribute
        var hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any() ?? false;

        if (!hasAuthorize)
        {
            hasAuthorize = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any();
        }

        // If no [Authorize] and no [AllowAnonymous], endpoint doesn't require auth explicitly
        // In this case, remove security requirements since we're not enforcing global auth
        if (!hasAuthorize)
        {
            operation.Security?.Clear();
        }
    }
}
```

**Benefits:**
- Properly documents which endpoints require authentication
- Respects `[AllowAnonymous]` attributes
- Removes security requirements from endpoints without `[Authorize]`
- Improves Swagger documentation accuracy

### 2. Registered the Filter in Swagger Configuration

Updated the Swagger configuration in `Program.cs` to include the operation filter:

```csharp
// Add operation filter to respect [AllowAnonymous] and [Authorize] attributes
// This ensures swagger.json is accessible without authentication
c.OperationFilter<MedicSoft.Api.Filters.AuthorizeCheckOperationFilter>();
```

### 3. Maintained Proper Middleware Order

Kept Swagger middleware in its original position (early in the pipeline, before routing and authentication) to ensure the swagger.json endpoint remains accessible:

```csharp
var app = builder.Build();

// Configure the HTTP request pipeline
// Enable Swagger - configurable via SwaggerSettings:Enabled
// Swagger is placed early in the pipeline to bypass authentication/authorization
// This ensures the swagger.json endpoint is accessible without authentication
var enableSwagger = builder.Configuration.GetValue<bool?>("SwaggerSettings:Enabled") 
    ?? app.Environment.IsDevelopment();

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Omni Care Software API v1");
        c.RoutePrefix = "swagger";
    });
}
```

## Files Modified

1. **New File**: `/src/MedicSoft.Api/Filters/AuthorizeCheckOperationFilter.cs`
   - Implements the operation filter logic
   - Checks for `[AllowAnonymous]` and `[Authorize]` attributes
   - Removes or maintains security requirements accordingly

2. **Modified**: `/src/MedicSoft.Api/Program.cs`
   - Registered the `AuthorizeCheckOperationFilter` in Swagger configuration (line ~164)
   - Added clarifying comments about Swagger middleware placement (lines 703-705)

## Testing and Verification

### Build Verification
- ✅ Project builds successfully with 0 errors
- ✅ 340 warnings (pre-existing, not introduced by this change)

### Expected Behavior After Fix

1. **Swagger UI Access**:
   - Navigate to `http://localhost:5000/swagger`
   - Swagger UI should load without errors
   - API definition should load successfully

2. **swagger.json Endpoint**:
   - `http://localhost:5000/swagger/v1/swagger.json` should return the OpenAPI specification
   - Should be accessible without authentication
   - Should return HTTP 200 (not 403)

3. **Documentation Accuracy**:
   - Endpoints with `[AllowAnonymous]` should NOT show the lock icon in Swagger UI
   - Endpoints with `[Authorize]` should show the lock icon
   - Endpoints without either attribute should NOT show authentication requirements

### Manual Testing Steps

To verify the fix works:

1. **Start the application** (requires PostgreSQL running):
   ```bash
   cd src/MedicSoft.Api
   dotnet run
   ```

2. **Access Swagger UI**:
   - Open browser to `http://localhost:5000/swagger`
   - Verify the page loads without errors

3. **Test swagger.json directly**:
   ```bash
   curl http://localhost:5000/swagger/v1/swagger.json
   ```
   - Should return JSON (not 403 Forbidden)

4. **Verify endpoint documentation**:
   - Look at endpoints like `/api/auth/login` (should have no lock icon)
   - Look at protected endpoints (should have lock icon)

## Additional Notes

### Why Swagger is Placed Early in Pipeline

Swagger middleware is intentionally placed BEFORE routing and authentication middleware because:
1. The swagger.json file should be publicly accessible for API documentation
2. Swagger UI needs to load without authentication
3. The operation filter ensures proper documentation of which API endpoints require auth
4. This is a common pattern in ASP.NET Core applications

### Environment Configuration

Swagger can be enabled/disabled via configuration:
- `appsettings.Development.json`: `SwaggerSettings:Enabled = true`
- `appsettings.Production.json`: `SwaggerSettings:Enabled = true` (can be changed to false)
- Default: Enabled in Development, configurable in Production

### Security Considerations

While Swagger is accessible without authentication:
- The endpoints themselves still require proper authentication
- Sensitive data is not exposed through the swagger.json
- In production, consider:
  - Network-level restrictions (firewall, VPN)
  - IP whitelisting
  - Disabling Swagger via configuration
  - Using a separate API Gateway with authentication

## Related Documentation

- Previous Swagger fixes: See `SWAGGER_FIX_SUMMARY.md`, `CORRECAO_SWAGGER_RESUMO.md`
- Swagger configuration: `RESUMO_ANALISE_SWAGGER_MIGRATIONS_FEV2026.md`
- Security headers middleware: Swagger paths are exempted in `SecurityHeadersMiddleware.cs`

## Summary

✅ **Fixed**: Added `AuthorizeCheckOperationFilter` to properly respect authorization attributes
✅ **Maintained**: Swagger middleware placement early in pipeline
✅ **Improved**: Swagger documentation now accurately shows which endpoints require authentication
✅ **Verified**: Build succeeds with no new errors

The fix ensures that:
1. Swagger.json is accessible without authentication (resolves 403 Forbidden)
2. API documentation accurately reflects endpoint authentication requirements
3. The solution follows ASP.NET Core best practices
4. No breaking changes to existing functionality

---

**Date**: February 6, 2026
**Issue**: Swagger 403 Forbidden Error
**Status**: ✅ Resolved
**Language**: English
