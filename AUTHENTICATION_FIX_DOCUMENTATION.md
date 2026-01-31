# Authentication Flow Fix Documentation

## Problem Description

All three authentication flows (system-admin, medicwarehouse-app, and patient portal) were returning 200 OK but not actually authenticating users. Even with correct credentials, the systems would return 200 OK without establishing authenticated sessions.

## Root Cause Analysis

### Issue Identified

The **Patient Portal frontend** was configured to call the **wrong API endpoint**:

- **Expected:** Patient Portal API at `http://localhost:5101/api`
- **Actual:** Main MedicSoft API at `http://localhost:5000/api`

This caused the Patient Portal frontend to receive authentication responses in the wrong format:

| System | API Endpoint | Response Format |
|--------|--------------|-----------------|
| MedicWarehouse App | Main API: `http://localhost:5293/api` | `{ token, username, tenantId, role, ... }` |
| System Admin | Main API: `http://localhost:5293/api` | `{ token, username, tenantId, role, ... }` |
| **Patient Portal** | **Patient API: `http://localhost:5101/api`** | `{ accessToken, refreshToken, user, ... }` |

### Why This Caused Authentication Failure

1. Patient Portal frontend expected `response.accessToken` and `response.refreshToken`
2. But it was calling the Main API which returns `response.token`
3. Frontend stored `undefined` in localStorage
4. Subsequent API requests had no valid token
5. Result: 200 OK on login, but no actual authentication

## Fixes Applied

### 1. Patient Portal Environment Configuration

**File:** `/frontend/patient-portal/src/environments/environment.ts`

```typescript
// BEFORE (WRONG)
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api', // ❌ Main API
  ...
};

// AFTER (CORRECT)
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5101/api', // ✅ Patient Portal API
  ...
};
```

### 2. Patient Portal Production Configuration

**File:** `/frontend/patient-portal/src/environments/environment.prod.ts`

```typescript
// BEFORE (WRONG)
export const environment = {
  production: true,
  apiUrl: '/api', // ❌ Would route to Main API
  ...
};

// AFTER (CORRECT)
export const environment = {
  production: true,
  apiUrl: '/patient-portal-api', // ✅ Specific Patient Portal API route
  ...
};
```

## API Endpoints Summary

### Main MedicSoft API (Port 5293/5000)

**Used by:** MedicWarehouse App, System Admin

**Endpoints:**
- `POST /api/auth/login` - User login (doctors, secretaries, etc.)
- `POST /api/auth/owner-login` - Owner login (clinic owners, system owners)
- `POST /api/auth/validate-session` - Session validation

**Response Format:**
```json
{
  "token": "eyJhbGc...",
  "username": "admin",
  "tenantId": "demo-clinic-001",
  "role": "Doctor",
  "clinicId": "...",
  "currentClinicId": "...",
  "availableClinics": [...],
  "expiresAt": "2024-01-01T00:00:00Z",
  "mfaEnabled": false,
  "requiresMfaSetup": false
}
```

### Patient Portal API (Port 5101)

**Used by:** Patient Portal

**Endpoints:**
- `POST /api/auth/login` - Patient login (email or CPF)
- `POST /api/auth/register` - Patient registration
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - Logout and revoke token
- `POST /api/auth/change-password` - Change password

**Response Format:**
```json
{
  "accessToken": "eyJhbGc...",
  "refreshToken": "refresh_token...",
  "expiresAt": "2024-01-01T00:00:00Z",
  "user": {
    "id": "...",
    "email": "patient@example.com",
    "fullName": "John Doe",
    "cpf": "12345678901",
    "phoneNumber": "+55 11 98765-4321",
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "twoFactorEnabled": false
  }
}
```

## Testing

### Manual Testing with Test Script

A comprehensive test script has been created to verify all three authentication flows:

```bash
./test-auth-flows.sh
```

This script tests:
1. MedicWarehouse App - User Login
2. System Admin - Owner Login
3. Patient Portal - Patient Login

### Expected Results

All three tests should:
1. Return **200 OK**
2. Include a valid **token** (or accessToken for patient portal)
3. Show decoded JWT claims with user information

### Manual Testing with cURL

#### Test MedicWarehouse App Login
```bash
curl -X POST http://localhost:5293/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "demo-clinic-001"
  }'
```

#### Test System Admin Owner Login
```bash
curl -X POST http://localhost:5293/api/auth/owner-login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "owner",
    "password": "Owner@123",
    "tenantId": "demo-clinic-001"
  }'
```

#### Test Patient Portal Login
```bash
curl -X POST http://localhost:5101/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrCPF": "patient@example.com",
    "password": "Patient@123"
  }'
```

### Frontend Testing

1. **Check localStorage after login:**
   - MedicWarehouse App: Should have `auth_token` key
   - System Admin: Should have `auth_token` key
   - Patient Portal: Should have `access_token` and `refresh_token` keys

2. **Check Network tab in DevTools:**
   - Login request should return 200 OK
   - Response should contain token field
   - Subsequent requests should include `Authorization: Bearer {token}` header

3. **Check Console for errors:**
   - No 401 Unauthorized errors
   - No token undefined warnings
   - No CORS errors

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    Frontend Applications                     │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ MedicWarehouse  │  │  System Admin   │  │   Patient   │ │
│  │      App        │  │                 │  │   Portal    │ │
│  │  (Port 4200)    │  │  (Port 4201)    │  │ (Port 4202) │ │
│  └────────┬────────┘  └────────┬────────┘  └──────┬──────┘ │
│           │                    │                   │        │
└───────────┼────────────────────┼───────────────────┼────────┘
            │                    │                   │
            ▼                    ▼                   ▼
┌───────────────────────────────────────────────────────────────┐
│                     API Backends                              │
├───────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌───────────────────────────────┐  ┌───────────────────────┐│
│  │      Main MedicSoft API       │  │  Patient Portal API   ││
│  │      (Port 5293/5000)         │  │    (Port 5101)        ││
│  │                               │  │                       ││
│  │  • User Login                 │  │  • Patient Login      ││
│  │  • Owner Login                │  │  • Registration       ││
│  │  • All clinic operations      │  │  • Appointments       ││
│  │                               │  │  • Documents          ││
│  │  Returns: { token, ... }      │  │  Returns:             ││
│  │                               │  │  { accessToken, ... } ││
│  └───────────────────────────────┘  └───────────────────────┘│
│           ▲                                   ▲               │
│           │                                   │               │
└───────────┼───────────────────────────────────┼───────────────┘
            │                                   │
            └───────────────┬───────────────────┘
                           │
                           ▼
                ┌─────────────────────┐
                │   PostgreSQL DB     │
                │                     │
                │  • MedicSoft DB     │
                │  • PatientPortal DB │
                └─────────────────────┘
```

## Security Considerations

### JWT Token Storage

- **MedicWarehouse App & System Admin:** Store token in `localStorage` with key `auth_token`
- **Patient Portal:** Store `access_token` and `refresh_token` separately in `localStorage`

### Token Validation

All three systems implement:
- JWT token validation on every request
- Session validation (single session per user)
- Token expiration checking
- Automatic token refresh (Patient Portal only)

### CORS Configuration

All APIs are configured to accept requests from their respective frontends with proper CORS headers.

## Troubleshooting

### Issue: "200 OK but not authenticating"

**Symptoms:**
- Login returns 200 OK
- No error messages
- User not authenticated
- Subsequent API calls fail with 401

**Causes:**
1. Frontend calling wrong API endpoint
2. Token stored as `undefined` in localStorage
3. Wrong field name for token (token vs accessToken)

**Solution:**
1. Check `environment.ts` has correct API URL
2. Verify localStorage has valid token after login
3. Check browser Network tab for correct API endpoint

### Issue: "401 Unauthorized on all requests"

**Symptoms:**
- Login successful
- All subsequent API calls return 401

**Causes:**
1. Token not being sent in Authorization header
2. HTTP interceptor not working
3. Token expired

**Solution:**
1. Check HTTP interceptor is registered in app.config or app.module
2. Verify Authorization header is present in Network tab
3. Check token expiration in JWT payload

### Issue: "CORS errors"

**Symptoms:**
- CORS policy blocking requests
- Preflight OPTIONS requests failing

**Causes:**
1. API CORS configuration not allowing frontend origin
2. Wrong API URL in frontend

**Solution:**
1. Check API CORS configuration allows frontend origin
2. Verify API is running on correct port
3. Check environment.ts has correct API URL

## References

- **Main API Controller:** `/src/MedicSoft.Api/Controllers/AuthController.cs`
- **Patient Portal API Controller:** `/patient-portal-api/PatientPortal.Api/Controllers/AuthController.cs`
- **MedicWarehouse App Auth Service:** `/frontend/medicwarehouse-app/src/app/services/auth.ts`
- **System Admin Auth Service:** `/frontend/mw-system-admin/src/app/services/auth.ts`
- **Patient Portal Auth Service:** `/frontend/patient-portal/src/app/services/auth.service.ts`

## Future Improvements

1. **Unified Authentication Service:** Consider creating a shared authentication library for all frontends
2. **Better Error Messages:** Add more descriptive error messages for authentication failures
3. **Token Refresh for Main API:** Implement refresh token mechanism for MedicWarehouse App and System Admin
4. **Session Management UI:** Add UI to show active sessions and allow users to revoke them
5. **Multi-Factor Authentication:** Extend MFA support to all three systems (currently only Patient Portal has 2FA)

---

**Last Updated:** January 31, 2026
**Status:** Fixed ✅
