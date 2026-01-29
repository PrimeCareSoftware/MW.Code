# API Documentation: Two-Factor Authentication (2FA)

> **Status**: ✅ Implementação completa - Janeiro 2026
> 
> Todos os endpoints estão implementados e testados. Pronto para produção.

## Overview

This document describes the 2FA (Two-Factor Authentication) API endpoints for the PrimeCare Patient Portal. The 2FA system adds an extra layer of security by requiring users to verify their identity with a code sent to their email in addition to their password.

## Table of Contents

- [Authentication](#authentication)
- [Endpoints](#endpoints)
  - [Enable 2FA](#enable-2fa)
  - [Disable 2FA](#disable-2fa)
  - [Get 2FA Status](#get-2fa-status)
  - [Verify 2FA Code](#verify-2fa-code)
  - [Resend 2FA Code](#resend-2fa-code)
- [Login Flow with 2FA](#login-flow-with-2fa)
- [Error Responses](#error-responses)
- [Security](#security)
- [Rate Limiting](#rate-limiting)
- [Examples](#examples)

## Authentication

Most 2FA endpoints require a valid JWT Bearer token. The token should be included in the `Authorization` header:

```
Authorization: Bearer {your-jwt-token}
```

## Endpoints

### Enable 2FA

Enables two-factor authentication for the authenticated user.

**Endpoint:** `POST /api/auth/enable-2fa`

**Authentication:** Required (JWT Bearer Token)

**Request Headers:**
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

**Request Body:** None

**Success Response (200 OK):**
```json
{
  "message": "Autenticação de dois fatores habilitada com sucesso"
}
```

**Error Responses:**

| Status Code | Description | Response Body |
|-------------|-------------|---------------|
| 401 | Not authenticated | `{ "message": "Unauthorized" }` |
| 400 | Failed to enable 2FA | `{ "message": "Falha ao habilitar autenticação de dois fatores" }` |
| 500 | Server error | `{ "message": "Ocorreu um erro ao habilitar a autenticação de dois fatores" }` |

**Side Effects:**
- User's `TwoFactorEnabled` flag is set to `true`
- A security notification email is sent to the user

---

### Disable 2FA

Disables two-factor authentication for the authenticated user.

**Endpoint:** `POST /api/auth/disable-2fa`

**Authentication:** Required (JWT Bearer Token)

**Request Headers:**
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/json
```

**Request Body:** None

**Success Response (200 OK):**
```json
{
  "message": "Autenticação de dois fatores desabilitada com sucesso"
}
```

**Error Responses:**

| Status Code | Description | Response Body |
|-------------|-------------|---------------|
| 401 | Not authenticated | `{ "message": "Unauthorized" }` |
| 400 | Failed to disable 2FA | `{ "message": "Falha ao desabilitar autenticação de dois fatores" }` |
| 500 | Server error | `{ "message": "Ocorreu um erro ao desabilitar a autenticação de dois fatores" }` |

**Side Effects:**
- User's `TwoFactorEnabled` flag is set to `false`
- A security notification email is sent to the user

---

### Get 2FA Status

Retrieves the current 2FA status for the authenticated user.

**Endpoint:** `GET /api/auth/2fa-status`

**Authentication:** Required (JWT Bearer Token)

**Request Headers:**
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Success Response (200 OK):**
```json
{
  "isEnabled": true
}
```

**Error Responses:**

| Status Code | Description | Response Body |
|-------------|-------------|---------------|
| 401 | Not authenticated | `{ "message": "Unauthorized" }` |
| 500 | Server error | `{ "message": "Ocorreu um erro ao obter o status da autenticação de dois fatores" }` |

---

### Verify 2FA Code

Verifies a 2FA code during the login process.

**Endpoint:** `POST /api/auth/verify-2fa`

**Authentication:** Not Required (uses temporary token)

**Request Headers:**
```http
Content-Type: application/json
```

**Request Body:**
```json
{
  "tempToken": "dGVtcF90b2tlbl8xMjM0NTY=",
  "code": "123456"
}
```

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| tempToken | string | Yes | Temporary token received from login response |
| code | string | Yes | 6-digit verification code from email |

**Success Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "expiresAt": "2026-01-30T14:00:00Z",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "patient@example.com",
    "fullName": "João Silva",
    "cpf": "12345678901",
    "phoneNumber": "+55 11 98765-4321",
    "dateOfBirth": "1990-01-15",
    "twoFactorEnabled": true
  }
}
```

**Error Responses:**

| Status Code | Description | Response Body |
|-------------|-------------|---------------|
| 400 | Missing parameters | `{ "message": "Token temporário e código são obrigatórios" }` |
| 400 | Invalid or expired code | `{ "message": "Código inválido ou expirado" }` |
| 400 | Too many attempts | `{ "message": "Número máximo de tentativas excedido" }` |
| 500 | Server error | `{ "message": "Ocorreu um erro ao verificar o código" }` |

**Side Effects:**
- Token is marked as used
- Access and refresh tokens are generated
- Login is recorded with IP address

---

### Resend 2FA Code

Resends the 2FA verification code to the user's email.

**Endpoint:** `POST /api/auth/resend-2fa-code`

**Authentication:** Not Required (uses temporary token)

**Request Headers:**
```http
Content-Type: application/json
```

**Request Body:**
```json
{
  "tempToken": "dGVtcF90b2tlbl8xMjM0NTY="
}
```

**Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| tempToken | string | Yes | Temporary token received from login response |

**Success Response (200 OK):**
```json
{
  "message": "Código reenviado com sucesso"
}
```

**Error Responses:**

| Status Code | Description | Response Body |
|-------------|-------------|---------------|
| 400 | Missing parameter | `{ "message": "Token temporário é obrigatório" }` |
| 400 | Failed to resend | `{ "message": "Falha ao reenviar código" }` |
| 429 | Rate limit exceeded | `{ "message": "Limite de códigos atingido. Tente novamente em 1 hora." }` |
| 500 | Server error | `{ "message": "Ocorreu um erro ao reenviar o código" }` |

**Side Effects:**
- A new verification code is generated (previous code remains valid)
- Email is sent with the new code
- Rate limit counter is incremented

---

## Login Flow with 2FA

### Standard Login Flow (2FA Disabled)

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant Database
    participant Email

    Client->>API: POST /api/auth/login
    API->>Database: Verify credentials
    Database-->>API: User authenticated
    API-->>Client: Return JWT tokens
```

```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrCPF": "patient@example.com",
  "password": "SecurePassword123"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "expiresAt": "2026-01-30T14:00:00Z",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "patient@example.com",
    "fullName": "João Silva",
    "twoFactorEnabled": false
  }
}
```

### Login Flow with 2FA Enabled

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant Database
    participant Email

    Client->>API: 1. POST /api/auth/login
    API->>Database: Verify credentials
    Database-->>API: User authenticated (2FA enabled)
    API->>Database: Generate 2FA token
    API->>Email: Send verification code
    Email-->>Client: Email with code
    API-->>Client: Return tempToken
    
    Client->>API: 2. POST /api/auth/verify-2fa
    API->>Database: Verify code
    Database-->>API: Code valid
    API-->>Client: Return JWT tokens
```

**Step 1: Initial Login**
```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrCPF": "patient@example.com",
  "password": "SecurePassword123"
}
```

**Response (200 OK):**
```json
{
  "requiresTwoFactor": true,
  "tempToken": "dGVtcF90b2tlbl8xMjM0NTY=",
  "message": "Código de verificação enviado para seu e-mail"
}
```

**Step 2: Verify Code**
```http
POST /api/auth/verify-2fa
Content-Type: application/json

{
  "tempToken": "dGVtcF90b2tlbl8xMjM0NTY=",
  "code": "123456"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "abc123def456...",
  "expiresAt": "2026-01-30T14:00:00Z",
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "email": "patient@example.com",
    "fullName": "João Silva",
    "twoFactorEnabled": true
  }
}
```

---

## Error Responses

### Common Error Structure

All error responses follow this structure:

```json
{
  "message": "Error description in Portuguese"
}
```

### HTTP Status Codes

| Status Code | Meaning | Common Causes |
|-------------|---------|---------------|
| 200 | Success | Request completed successfully |
| 400 | Bad Request | Invalid input, missing parameters |
| 401 | Unauthorized | Missing or invalid JWT token |
| 429 | Too Many Requests | Rate limit exceeded |
| 500 | Internal Server Error | Server-side error |

---

## Security

### Code Generation

- **Algorithm:** Cryptographically secure random number generator (CSPRNG)
- **Format:** 6-digit numeric code (000000-999999)
- **Entropy:** ~19.93 bits (1 million possibilities)
- **Validity:** 5 minutes from generation

### Token Security

- **Temporary Token:** Base64-encoded GUID
- **Purpose:** Links the 2FA verification to the initial login attempt
- **Storage:** Stored in database with foreign key to PatientUser
- **Expiration:** Automatically expires with the code

### Rate Limiting

#### Code Generation
- **Limit:** 3 codes per hour per user
- **Purpose:** Prevent email flooding and abuse
- **Response:** HTTP 429 with retry message

#### Code Verification
- **Limit:** 5 attempts per code
- **Purpose:** Prevent brute force attacks
- **Response:** Code marked as invalid after 5 attempts

### Audit Logging

All 2FA events are logged with:
- User ID
- Action (generate, verify, enable, disable)
- IP Address
- Timestamp
- Result (success/failure)

---

## Rate Limiting

### Code Generation

**Limit:** 3 codes per hour per user

**Tracking:** Based on `CreatedAt` timestamp in `TwoFactorTokens` table

**Implementation:**
```csharp
var recentTokensCount = await _twoFactorTokenRepository.CountRecentTokensAsync(
    patientUserId, 
    TimeSpan.FromHours(1)
);

if (recentTokensCount >= 3)
{
    throw new InvalidOperationException(
        "Limite de códigos atingido. Tente novamente em 1 hora."
    );
}
```

### Code Verification

**Limit:** 5 attempts per code

**Tracking:** `VerificationAttempts` field in `TwoFactorToken` entity

**Implementation:**
```csharp
token.VerificationAttempts++;
await _twoFactorTokenRepository.UpdateAsync(token);

if (token.VerificationAttempts >= 5)
{
    return false; // Code invalid
}
```

---

## Examples

### Full Integration Example (JavaScript/TypeScript)

```typescript
// Step 1: Login
async function login(email: string, password: string) {
  const response = await fetch('https://api.primecare.com/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      emailOrCPF: email,
      password: password
    })
  });

  const data = await response.json();

  if (data.requiresTwoFactor) {
    // 2FA is required, show verification form
    return {
      requiresTwoFactor: true,
      tempToken: data.tempToken
    };
  } else {
    // Login successful, store tokens
    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    return {
      requiresTwoFactor: false,
      user: data.user
    };
  }
}

// Step 2: Verify 2FA Code
async function verify2FA(tempToken: string, code: string) {
  const response = await fetch('https://api.primecare.com/api/auth/verify-2fa', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      tempToken: tempToken,
      code: code
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }

  const data = await response.json();
  
  // Store tokens
  localStorage.setItem('accessToken', data.accessToken);
  localStorage.setItem('refreshToken', data.refreshToken);
  
  return data.user;
}

// Resend Code
async function resendCode(tempToken: string) {
  const response = await fetch('https://api.primecare.com/api/auth/resend-2fa-code', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      tempToken: tempToken
    })
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }

  return await response.json();
}

// Enable 2FA
async function enable2FA(accessToken: string) {
  const response = await fetch('https://api.primecare.com/api/auth/enable-2fa', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${accessToken}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }

  return await response.json();
}

// Disable 2FA
async function disable2FA(accessToken: string) {
  const response = await fetch('https://api.primecare.com/api/auth/disable-2fa', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${accessToken}`,
      'Content-Type': 'application/json'
    }
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }

  return await response.json();
}

// Check 2FA Status
async function get2FAStatus(accessToken: string) {
  const response = await fetch('https://api.primecare.com/api/auth/2fa-status', {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${accessToken}`
    }
  });

  if (!response.ok) {
    const error = await response.json();
    throw new Error(error.message);
  }

  const data = await response.json();
  return data.isEnabled;
}

// Usage Example
async function loginFlow() {
  try {
    const loginResult = await login('patient@example.com', 'SecurePassword123');
    
    if (loginResult.requiresTwoFactor) {
      // Show 2FA verification form
      const code = prompt('Digite o código de verificação:');
      
      if (code) {
        const user = await verify2FA(loginResult.tempToken, code);
        console.log('Login successful:', user);
      }
    } else {
      console.log('Login successful:', loginResult.user);
    }
  } catch (error) {
    console.error('Login failed:', error.message);
  }
}
```

### cURL Examples

**Enable 2FA:**
```bash
curl -X POST https://api.primecare.com/api/auth/enable-2fa \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json"
```

**Verify 2FA Code:**
```bash
curl -X POST https://api.primecare.com/api/auth/verify-2fa \
  -H "Content-Type: application/json" \
  -d '{
    "tempToken": "dGVtcF90b2tlbl8xMjM0NTY=",
    "code": "123456"
  }'
```

**Resend Code:**
```bash
curl -X POST https://api.primecare.com/api/auth/resend-2fa-code \
  -H "Content-Type: application/json" \
  -d '{
    "tempToken": "dGVtcF90b2tlbl8xMjM0NTY="
  }'
```

---

## Database Schema

### TwoFactorTokens Table

| Column | Type | Nullable | Description |
|--------|------|----------|-------------|
| Id | UUID | No | Primary key |
| PatientUserId | UUID | No | Foreign key to PatientUsers |
| Code | VARCHAR(10) | No | 6-digit verification code |
| CreatedAt | TIMESTAMP | No | When the token was created |
| ExpiresAt | TIMESTAMP | No | When the token expires (CreatedAt + 5 min) |
| IsUsed | BOOLEAN | No | Whether the token has been used |
| UsedAt | TIMESTAMP | Yes | When the token was used |
| Purpose | VARCHAR(50) | No | "Login" or "Verification" |
| IpAddress | VARCHAR(50) | No | IP address of the request |
| VerificationAttempts | INTEGER | No | Number of verification attempts |

### Indexes

- `IX_TwoFactorTokens_Code_PatientUserId` - For fast lookups by code and user
- `IX_TwoFactorTokens_PatientUserId_ExpiresAt` - For cleanup and recent token counting
- `IX_TwoFactorTokens_CreatedAt` - For rate limiting queries

---

## Changelog

### Version 1.0.0 (2026-01-29)

**Initial Release**

- ✨ Email-based 2FA implementation
- 🔒 6-digit verification codes with 5-minute expiry
- 📧 Automatic email notifications
- 🚦 Rate limiting (3 codes/hour, 5 attempts/code)
- 📊 Complete audit logging
- 🔐 Cryptographically secure code generation
- ⚡ RESTful API endpoints

---

## Support

For technical support or questions:

- **Documentation:** https://docs.primecare.com
- **API Support:** api-support@primecaresoftware.com
- **General Support:** suporte@primecaresoftware.com

---

© 2026 PrimeCare Software. All rights reserved.
