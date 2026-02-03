# Authentication Documentation - Omni Care Software API

## Overview

The Omni Care Software API uses **JWT (JSON Web Token)** authentication with **HMAC-SHA256** encryption to secure all endpoints. This document describes how to authenticate and use the API.

## Authentication Endpoints

### 1. User Login
**Endpoint**: `POST /api/auth/login`

Used for regular users (doctors, secretaries, receptionists, etc.)

**Request Body**:
```json
{
  "username": "doctor@clinic.com",
  "password": "SecurePassword123!",
  "tenantId": "clinic-tenant-id"
}
```

**Success Response** (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "doctor@clinic.com",
  "tenantId": "clinic-tenant-id",
  "role": "Doctor",
  "clinicId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "isSystemOwner": false,
  "expiresAt": "2025-10-13T01:36:10Z"
}
```

**Error Response** (401 Unauthorized):
```json
{
  "message": "Invalid credentials or user not found"
}
```

---

### 2. Owner Login
**Endpoint**: `POST /api/auth/owner-login`

Used for clinic owners and system owners (administrators).

**Request Body**:
```json
{
  "username": "owner@clinic.com",
  "password": "SecurePassword123!",
  "tenantId": "clinic-tenant-id"
}
```

**Success Response** (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "owner@clinic.com",
  "tenantId": "clinic-tenant-id",
  "role": "Owner",
  "clinicId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "isSystemOwner": false,
  "expiresAt": "2025-10-13T01:36:10Z"
}
```

**System Owner Response** (no clinicId):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "igor",
  "tenantId": "system",
  "role": "Owner",
  "clinicId": null,
  "isSystemOwner": true,
  "expiresAt": "2025-10-13T01:36:10Z"
}
```

---

### 3. Token Validation
**Endpoint**: `POST /api/auth/validate`

Validates if a JWT token is still valid.

**Request Body**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Success Response** (200 OK):
```json
{
  "isValid": true,
  "username": "doctor@clinic.com",
  "role": "Doctor",
  "tenantId": "clinic-tenant-id"
}
```

**Invalid Token Response** (200 OK):
```json
{
  "isValid": false
}
```

---

## JWT Token Structure

### Token Claims

The JWT token includes the following claims:

| Claim | Type | Description | Example |
|-------|------|-------------|---------|
| `name` | string | Username | `doctor@clinic.com` |
| `nameid` | string | User/Owner ID (GUID) | `a1b2c3d4-e5f6-7890-abcd-ef1234567890` |
| `role` | string | User role | `Doctor`, `Owner`, `Secretary`, etc. |
| `tenant_id` | string | Tenant identifier | `clinic-tenant-id` |
| `clinic_id` | string? | Clinic ID (optional, null for system owners) | `a1b2c3d4-e5f6-7890-abcd-ef1234567890` |
| `is_system_owner` | string | Whether user is a system owner | `true` or `false` |
| `iss` | string | Token issuer | `Omni Care Software` |
| `aud` | string | Token audience | `Omni Care Software-API` |
| `exp` | number | Expiration timestamp | Unix timestamp |

### Token Example (Decoded)

**Header**:
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload**:
```json
{
  "name": "doctor@clinic.com",
  "nameid": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "role": "Doctor",
  "tenant_id": "clinic-tenant-id",
  "clinic_id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "is_system_owner": "false",
  "iss": "Omni Care Software",
  "aud": "Omni Care Software-API",
  "exp": 1697158570
}
```

**Signature**:
```
HMACSHA256(
  base64UrlEncode(header) + "." + base64UrlEncode(payload),
  secret
)
```

---

## Using the Token

### Authorization Header

All protected endpoints require the JWT token in the `Authorization` header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Example: cURL

```bash
curl -X GET "https://api.medicwarehouse.com/api/patients" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json"
```

### Example: JavaScript (Fetch)

```javascript
const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

fetch('https://api.medicwarehouse.com/api/patients', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
})
.then(response => response.json())
.then(data => console.log(data));
```

### Example: Axios

```javascript
import axios from 'axios';

const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";

axios.get('https://api.medicwarehouse.com/api/patients', {
  headers: {
    'Authorization': `Bearer ${token}`
  }
})
.then(response => console.log(response.data));
```

---

## Security Features

### 1. HMAC-SHA256 Encryption
- Tokens are signed using HMAC-SHA256 algorithm
- Secret key must be at least 32 characters (256 bits)
- Secret key is stored securely in configuration (never in code)

### 2. Token Expiration
- Default expiration: **60 minutes**
- Configurable via `JwtSettings:ExpiryMinutes` in appsettings.json
- **Zero clock skew** - expired tokens are immediately rejected

### 3. Token Validation
- Validates signature
- Validates issuer (`Omni Care Software`)
- Validates audience (`Omni Care Software-API`)
- Validates expiration time
- No tolerance for expired tokens (ClockSkew = 0)

### 4. BCrypt Password Hashing
- All passwords are hashed using BCrypt
- Work factor: 12
- Passwords are never stored in plain text

---

## Business Rules

### System Owners vs Clinic Owners

#### System Owner (e.g., Igor)
- **ClinicId**: `null`
- **TenantId**: `"system"`
- **IsSystemOwner**: `true`
- **Permissions**: Can manage all clinics and system-wide operations
- **Restrictions**: Cannot be assigned to a specific clinic after creation

#### Clinic Owner
- **ClinicId**: `<guid-da-clinica>`
- **TenantId**: `<tenant-da-clinica>`
- **IsSystemOwner**: `false`
- **Permissions**: Can manage only their specific clinic
- **Restrictions**: Cannot become a system owner after creation

**Important**: The `ClinicId` property is **readonly** after owner creation. System owners cannot "join" a clinic, and clinic owners cannot become system owners.

---

## Configuration

### appsettings.json

```json
{
  "JwtSettings": {
    "SecretKey": "YourSecretKey-MustBe-AtLeast32Characters-ForSecurity!",
    "ExpiryMinutes": 60,
    "Issuer": "Omni Care Software",
    "Audience": "Omni Care Software-API"
  }
}
```

### Production Configuration

For production, **never** hardcode secrets:

1. **Use Environment Variables**:
```bash
export JWT_SECRET_KEY="YourProductionSecretKey..."
```

2. **Use Azure Key Vault** (Recommended):
```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

---

## Error Handling

### Common Error Responses

#### 401 Unauthorized
```json
{
  "message": "Invalid credentials or user not found"
}
```

**Causes**:
- Invalid username or password
- User is inactive
- Owner is inactive

#### 400 Bad Request
```json
{
  "message": "Username, password, and tenantId are required"
}
```

**Causes**:
- Missing required fields in request body

---

## Swagger Integration

The API includes Swagger UI with JWT authentication support:

1. Navigate to `/swagger` in your browser
2. Click the **Authorize** button
3. Enter: `Bearer <your-token-here>`
4. Click **Authorize**
5. All subsequent requests will include the token

---

## Testing

### Unit Tests

The JWT service includes comprehensive unit tests:

```bash
dotnet test --filter "FullyQualifiedName~JwtTokenServiceTests"
```

**Test Coverage**:
- Token generation for regular users
- Token generation for system owners
- Token generation for clinic owners
- Token validation
- Invalid token handling
- HMAC-SHA256 algorithm verification

---

## Migration from No Authentication

If you're migrating from a version without authentication:

1. **Update all API calls** to include `Authorization` header
2. **Implement login flow** in your frontend
3. **Store token securely** (sessionStorage or memory, not localStorage for security)
4. **Handle token expiration** (refresh or re-login)
5. **Update tests** to use authentication

---

## Best Practices

1. ✅ **Use HTTPS in production** - Never send tokens over HTTP
2. ✅ **Store tokens securely** - Prefer memory or sessionStorage over localStorage
3. ✅ **Implement token refresh** - Prompt user to re-login before expiration
4. ✅ **Validate tokens on server** - Never trust client-side validation
5. ✅ **Use strong secret keys** - Minimum 32 characters, randomly generated
6. ✅ **Rotate secret keys regularly** - Especially after security incidents
7. ✅ **Log authentication attempts** - For security auditing
8. ✅ **Implement rate limiting** - Prevent brute force attacks (already configured)

---

## Support

For issues or questions:
- Email: contato@omnicaresoftware.com
- GitHub: https://github.com/Omni Care Software/MW.Code
