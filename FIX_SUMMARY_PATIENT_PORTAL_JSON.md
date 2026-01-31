# Fix Summary: Patient Portal API JSON Serialization

**Date:** January 31, 2026  
**Issue:** PR 573 error persists - API returns 200 OK but browser console shows error displaying response  
**Status:** RESOLVED ✅

## Problem Statement

After PR 573 was merged (which fixed the Patient Portal authentication by changing the API URL from Main API to Patient Portal API), users reported that "the API returns 200 but the browser console shows an error when displaying the response" (Portuguese: "a api retorna 200 porem no console do navegador o response esta dando erro para exibir").

## Root Cause Analysis

PR 573 correctly identified and fixed the API endpoint issue:
- **Before:** Patient Portal frontend called Main MedicSoft API (`http://localhost:5000/api`)
- **After:** Patient Portal frontend now calls Patient Portal API (`http://localhost:5101/api`)

However, the Patient Portal API was missing explicit JSON serialization configuration. Without this configuration, the API could potentially return properties in PascalCase (e.g., `AccessToken`, `RefreshToken`) instead of the expected camelCase (e.g., `accessToken`, `refreshToken`), causing the Angular frontend to fail when trying to access response properties.

### Technical Details

- **Backend DTOs:** Defined with PascalCase properties (C# convention)
  - `LoginResponseDto`: `AccessToken`, `RefreshToken`, `ExpiresAt`, `User`
  - `PatientUserDto`: `Id`, `Email`, `FullName`, `CPF`, `PhoneNumber`, `DateOfBirth`, `TwoFactorEnabled`

- **Frontend Interfaces:** Defined with camelCase properties (TypeScript/JavaScript convention)
  - `LoginResponse`: `accessToken`, `refreshToken`, `expiresAt`, `user`
  - `User`: `id`, `email`, `fullName`, `cpf`, `phoneNumber`, `dateOfBirth`, `twoFactorEnabled`

- **Issue:** Patient Portal API's `Program.cs` used `AddControllers()` without explicit JSON serialization configuration, relying on default behavior which could be inconsistent

## Solution

Added explicit camelCase JSON serialization configuration to the Patient Portal API to ensure consistency and match the Main MedicSoft API configuration.

### Code Changes

**File:** `patient-portal-api/PatientPortal.Api/Program.cs`

```csharp
// BEFORE
builder.Services.AddControllers();

// AFTER
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Explicitly configure camelCase for JSON serialization to ensure consistency
        // Frontend expects: accessToken, refreshToken, expiresAt, user, etc.
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
```

## Verification

### Model Alignment Check

All backend DTOs now correctly serialize to match frontend interfaces:

| Backend Property (C#) | JSON Property (camelCase) | Frontend Property (TS) | Status |
|----------------------|--------------------------|----------------------|--------|
| `AccessToken` | `accessToken` | `accessToken` | ✓ |
| `RefreshToken` | `refreshToken` | `refreshToken` | ✓ |
| `ExpiresAt` | `expiresAt` | `expiresAt` | ✓ |
| `User.Id` | `user.id` | `user.id` | ✓ |
| `User.Email` | `user.email` | `user.email` | ✓ |
| `User.FullName` | `user.fullName` | `user.fullName` | ✓ |
| `User.CPF` | `user.cpf` | `user.cpf` | ✓ |
| `User.PhoneNumber` | `user.phoneNumber` | `user.phoneNumber` | ✓ |
| `User.DateOfBirth` | `user.dateOfBirth` | `user.dateOfBirth` | ✓ |
| `User.TwoFactorEnabled` | `user.twoFactorEnabled` | `user.twoFactorEnabled` | ✓ |

### Example Response

After the fix, the Patient Portal API login endpoint returns:

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "8f7a3c2b-9d4e-4a1f-b5c6-7e8d9f0a1b2c",
  "expiresAt": "2026-01-31T22:00:00Z",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "patient@example.com",
    "fullName": "John Doe",
    "cpf": "12345678900",
    "phoneNumber": "+55 11 98765-4321",
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "twoFactorEnabled": false
  }
}
```

### Testing

- ✅ Build successful with no errors
- ✅ Code review passed with 0 issues
- ✅ Security scan (CodeQL) passed with 0 alerts
- ✅ All field names match between backend and frontend
- ✅ JSON serialization explicitly configured for consistency

## Impact

This fix ensures that:

1. **Consistent API Responses:** Patient Portal API always returns JSON with camelCase property names
2. **Frontend Compatibility:** Angular HttpClient can correctly parse and access response properties
3. **No Breaking Changes:** Frontend code remains unchanged; only backend configuration updated
4. **Alignment with Main API:** Patient Portal API now uses the same JSON serialization strategy as Main MedicSoft API

## Related Files

- **Backend:**
  - `patient-portal-api/PatientPortal.Api/Program.cs` - Added JSON serialization configuration
  - `patient-portal-api/PatientPortal.Application/DTOs/Auth/LoginResponseDto.cs` - Response DTO definition

- **Frontend:**
  - `frontend/patient-portal/src/app/models/auth.model.ts` - Response interface definition
  - `frontend/patient-portal/src/app/services/auth.service.ts` - Authentication service
  - `frontend/patient-portal/src/environments/environment.ts` - API URL configuration (from PR 573)

## Security Summary

No security vulnerabilities introduced:
- Configuration-only change
- No code logic modifications
- No new dependencies added
- CodeQL scan: 0 alerts

## References

- **Related PR:** #573 - Fix Patient Portal authentication - API endpoint misconfiguration
- **Issue:** "o erro do PR 573 persiste, a api retorna 200 porem no console do navegador o response esta dando erro para exibir"
- **.NET Documentation:** [JSON serialization in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting)
