# 📡 Telemedicine API Complete Documentation

## 🎯 Overview

Complete API documentation for the MedicSoft Telemedicine microservice, including all endpoints, request/response formats, authentication, error handling, and best practices.

**Base URL (Production):** `https://api.medicsoft.com.br`  
**Base URL (Development):** `http://localhost:5000`

## 🔐 Authentication

All API requests require authentication using JWT tokens and tenant identification.

### Headers

```http
Authorization: Bearer <jwt-token>
X-Tenant-Id: <tenant-id>
X-User-Id: <user-id>
Content-Type: application/json
```

### JWT Token Format

```json
{
  "sub": "user-123",
  "email": "user@example.com",
  "role": ["Provider", "Admin"],
  "tenantId": "tenant-123",
  "exp": 1738195200
}
```

## 📋 API Endpoints

### Telemedicine Consent

#### 1. Create Consent

Registers informed consent from a patient for telemedicine services (CFM 2.314/2022 Art. 3º).

```http
POST /api/telemedicine/consent
```

**Request Body:**

```json
{
  "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "appointmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "acceptsRecording": true,
  "acceptsDataSharing": true,
  "digitalSignature": "base64-encoded-signature",
  "ipAddress": "192.168.1.1",
  "userAgent": "Mozilla/5.0..."
}
```

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "consentText": "Full consent text in Portuguese...",
  "consentedAt": "2026-01-29T10:30:00Z",
  "ipAddress": "192.168.1.1",
  "acceptsRecording": true,
  "acceptsDataSharing": true,
  "isRevoked": false,
  "version": 1
}
```

**Error Responses:**

- `400 Bad Request`: Invalid input data
- `401 Unauthorized`: Missing or invalid JWT token
- `409 Conflict`: Consent already exists for this appointment

#### 2. Get Consent by ID

```http
GET /api/telemedicine/consent/{id}
```

**Parameters:**
- `id` (path): Consent ID (GUID)

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "consentText": "Full consent text...",
  "consentedAt": "2026-01-29T10:30:00Z",
  "acceptsRecording": true,
  "isRevoked": false
}
```

#### 3. Check Valid Consent

Verifies if patient has valid active consent.

```http
GET /api/telemedicine/consent/patient/{patientId}/has-valid
```

**Parameters:**
- `patientId` (path): Patient ID (GUID)

**Response (200 OK):**

```json
{
  "hasValidConsent": true,
  "consentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "consentedAt": "2026-01-29T10:30:00Z"
}
```

#### 4. Revoke Consent

Allows patient to revoke their consent (LGPD compliance).

```http
POST /api/telemedicine/consent/{id}/revoke
```

**Request Body:**

```json
{
  "reason": "Patient no longer wishes to use telemedicine services",
  "revokedBy": "patient-id or admin-id"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "message": "Consent revoked successfully",
  "revokedAt": "2026-01-29T14:30:00Z"
}
```

#### 5. Get Consent Text

Retrieves the current version of the consent term.

```http
GET /api/telemedicine/consent/consent-text?includeRecording=true
```

**Query Parameters:**
- `includeRecording` (optional): Include recording consent clause

**Response (200 OK):**

```json
{
  "consentText": "TERMO DE CONSENTIMENTO INFORMADO...",
  "version": 1,
  "effectiveDate": "2026-01-01"
}
```

### Identity Verification

#### 6. Upload Identity Documents

Uploads identity verification documents (CFM 2.314/2022 Art. 4º).

```http
POST /api/telemedicine/identityverification
Content-Type: multipart/form-data
```

**Form Data:**

```
userId: 3fa85f64-5717-4562-b3fc-2c963f66afa6
userType: Provider | Patient
documentType: RG | CNH | CPF | RNE | Passport
documentNumber: 123456789
crmNumber: 12345 (required for Provider)
crmState: SP (required for Provider)
documentPhoto: [file] (JPEG, PNG, PDF - max 10MB)
crmCardPhoto: [file] (required for Provider)
selfie: [file] (optional but recommended)
```

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userType": "Provider",
  "documentType": "CPF",
  "verificationStatus": "Pending",
  "uploadedAt": "2026-01-29T10:30:00Z",
  "expiresAt": "2027-01-29T10:30:00Z"
}
```

**Error Responses:**

- `400 Bad Request`: Invalid file type or size
- `413 Payload Too Large`: File exceeds 10MB limit
- `422 Unprocessable Entity`: Missing CRM information for Provider

#### 7. Get Identity Verification

```http
GET /api/telemedicine/identityverification/{id}
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userType": "Provider",
  "verificationStatus": "Verified",
  "verifiedAt": "2026-01-29T12:00:00Z",
  "verifiedBy": "admin-123",
  "expiresAt": "2027-01-29T12:00:00Z",
  "isValid": true
}
```

#### 8. Verify/Reject Identity

Admin endpoint to approve or reject identity verification.

```http
POST /api/telemedicine/identityverification/{id}/verify
```

**Request Body:**

```json
{
  "approve": true,
  "verifiedBy": "admin-123",
  "notes": "Documents verified successfully"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "verificationStatus": "Verified",
  "verifiedAt": "2026-01-29T12:00:00Z"
}
```

#### 9. Get Latest Verification

```http
GET /api/telemedicine/identityverification/user/{userId}/latest
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "verificationStatus": "Verified",
  "isValid": true,
  "expiresAt": "2027-01-29T12:00:00Z"
}
```

### Telemedicine Sessions

#### 10. Create Session

Creates a new telemedicine video session.

```http
POST /api/telemedicine/sessions
```

**Request Body:**

```json
{
  "appointmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "clinicId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "providerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "scheduledStartTime": "2026-01-30T14:00:00Z",
  "notes": "Initial consultation"
}
```

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "appointmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "roomId": "session-abc123",
  "roomUrl": "https://medicsoft.daily.co/session-abc123",
  "status": "Scheduled",
  "scheduledStartTime": "2026-01-30T14:00:00Z",
  "createdAt": "2026-01-29T10:30:00Z"
}
```

#### 11. Join Session

Generates access token for joining a video room.

```http
POST /api/telemedicine/sessions/{id}/join
```

**Request Body:**

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userName": "Dr. João Silva",
  "userRole": "Provider | Patient"
}
```

**Response (200 OK):**

```json
{
  "roomUrl": "https://medicsoft.daily.co/session-abc123",
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-01-30T16:00:00Z",
  "iceServers": [
    {
      "urls": "stun:stun.l.google.com:19302"
    }
  ]
}
```

#### 12. Start Session

Starts a scheduled session (validates compliance).

```http
POST /api/telemedicine/sessions/{id}/start
```

**Request Body:**

```json
{
  "notes": "Session started on time"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "InProgress",
  "startTime": "2026-01-30T14:00:00Z",
  "complianceValidation": {
    "hasConsent": true,
    "patientVerified": true,
    "providerVerified": true,
    "allChecksPass": true
  }
}
```

**Error Responses:**

- `400 Bad Request`: Session not in Scheduled status
- `403 Forbidden`: Compliance validation failed (missing consent or verification)

#### 13. Validate Session Compliance

Pre-flight check before starting session.

```http
GET /api/telemedicine/sessions/{id}/validate-compliance
```

**Response (200 OK):**

```json
{
  "isCompliant": true,
  "checks": {
    "patientConsent": {
      "passed": true,
      "message": "Valid consent found"
    },
    "patientIdentityVerification": {
      "passed": true,
      "expiresAt": "2027-01-29T10:00:00Z"
    },
    "providerIdentityVerification": {
      "passed": true,
      "crmNumber": "12345",
      "crmState": "SP"
    },
    "firstAppointment": {
      "isFirst": false,
      "requiresJustification": false
    }
  }
}
```

#### 14. Complete Session

Marks session as completed.

```http
POST /api/telemedicine/sessions/{id}/complete
```

**Request Body:**

```json
{
  "summary": "Consultation completed successfully. Diagnosis: ...",
  "followUpRequired": true,
  "followUpNotes": "Schedule follow-up in 2 weeks",
  "prescriptionIssued": true
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Completed",
  "duration": "00:45:32",
  "completedAt": "2026-01-30T14:45:32Z"
}
```

#### 15. Cancel Session

Cancels a scheduled or in-progress session.

```http
POST /api/telemedicine/sessions/{id}/cancel
```

**Request Body:**

```json
{
  "reason": "Patient cancelled appointment",
  "cancelledBy": "patient-id"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "status": "Cancelled",
  "cancelledAt": "2026-01-30T13:00:00Z"
}
```

### Telemedicine Recordings

#### 16. Create Recording

Initializes a recording for a session (with patient consent).

```http
POST /api/telemedicine/recordings
```

**Request Body:**

```json
{
  "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "encryptionKeyId": "key-123",
  "estimatedDurationMinutes": 60
}
```

**Response (201 Created):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Initialized",
  "createdAt": "2026-01-30T14:00:00Z",
  "retentionUntil": "2046-01-30T14:00:00Z"
}
```

#### 17. Start Recording

Begins actual recording.

```http
POST /api/telemedicine/recordings/{id}/start
```

**Response (200 OK):**

```json
{
  "success": true,
  "recordingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Recording",
  "startedAt": "2026-01-30T14:05:00Z"
}
```

#### 18. Complete Recording

Finalizes and uploads recording.

```http
POST /api/telemedicine/recordings/{id}/complete
```

**Request Body:**

```json
{
  "filePath": "/recordings/session-abc123.mp4",
  "durationSeconds": 2732,
  "fileSizeBytes": 524288000
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "recordingId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Completed",
  "encryptedFilePath": "https://storage.example.com/recordings/session-abc123-encrypted.mp4",
  "duration": "00:45:32",
  "completedAt": "2026-01-30T14:50:00Z"
}
```

#### 19. Get Recording

Retrieves recording metadata and download URL.

```http
GET /api/telemedicine/recordings/{id}
```

**Response (200 OK):**

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "sessionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Completed",
  "duration": "00:45:32",
  "fileSize": 524288000,
  "downloadUrl": "https://storage.example.com/recordings/session-abc123?sas=...",
  "urlExpiresAt": "2026-01-30T16:00:00Z",
  "retentionUntil": "2046-01-30T14:00:00Z"
}
```

**Note:** Download URL is temporary (SAS token) and expires after 1 hour.

#### 20. Delete Recording

Soft delete with mandatory justification (LGPD compliance).

```http
DELETE /api/telemedicine/recordings/{id}
```

**Request Body:**

```json
{
  "reason": "Patient requested data deletion under LGPD",
  "deletedBy": "admin-123"
}
```

**Response (200 OK):**

```json
{
  "success": true,
  "deletedAt": "2026-01-30T16:00:00Z",
  "permanentDeletionScheduled": "2026-02-29T16:00:00Z"
}
```

## 🔍 Error Handling

### Standard Error Response

All errors follow this format:

```json
{
  "error": {
    "code": "ERROR_CODE",
    "message": "Human-readable error message",
    "details": {
      "field": "specificField",
      "reason": "Detailed reason for error"
    },
    "traceId": "00-abc123def456-789-00"
  }
}
```

### Error Codes

| Code | Status | Description |
|------|--------|-------------|
| `UNAUTHORIZED` | 401 | Missing or invalid JWT token |
| `FORBIDDEN` | 403 | Insufficient permissions |
| `NOT_FOUND` | 404 | Resource not found |
| `VALIDATION_ERROR` | 400 | Invalid input data |
| `CONFLICT` | 409 | Resource already exists |
| `COMPLIANCE_ERROR` | 403 | CFM 2.314 compliance check failed |
| `RATE_LIMIT_EXCEEDED` | 429 | Too many requests |
| `INTERNAL_ERROR` | 500 | Server error |

## 📊 Rate Limiting

### Limits by Endpoint Type

| Endpoint Category | Limit | Window |
|-------------------|-------|--------|
| Authentication | 10 requests | 1 minute |
| Read Operations (GET) | 100 requests | 1 minute |
| Write Operations (POST/PUT) | 50 requests | 1 minute |
| File Uploads | 10 requests | 1 minute |
| Video Sessions | 20 requests | 1 minute |

### Rate Limit Headers

```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 85
X-RateLimit-Reset: 1738195200
```

## 🔐 Security Best Practices

### 1. Token Management

- Always use HTTPS in production
- Store JWT tokens securely (httpOnly cookies recommended)
- Implement token refresh mechanism
- Set appropriate token expiration (1 hour recommended)

### 2. File Uploads

- Validate file types on client and server
- Scan files for viruses before processing
- Use signed URLs for file downloads
- Implement file size limits

### 3. Audit Logging

All sensitive operations are logged:

```json
{
  "timestamp": "2026-01-30T14:00:00Z",
  "action": "consent.revoked",
  "userId": "patient-123",
  "tenantId": "tenant-123",
  "ipAddress": "192.168.1.1",
  "userAgent": "Mozilla/5.0...",
  "resource": {
    "type": "Consent",
    "id": "consent-456"
  }
}
```

## 📝 Compliance Notes

### CFM 2.314/2022 Requirements

1. **Informed Consent (Art. 3º):**
   - Implemented via `/api/telemedicine/consent` endpoints
   - Consent required before first session
   - Patient can revoke anytime

2. **Identity Verification (Art. 4º):**
   - Bidirectional verification required
   - Valid for 1 year
   - CRM mandatory for providers

3. **First Appointment (Art. 5º):**
   - System validates if first appointment
   - Justification required for telemedicine on first visit
   - Exceptions: remote areas, emergencies

4. **Recordings (Art. 12º):**
   - Optional with explicit consent
   - 20-year retention required
   - AES-256 encryption mandatory

### LGPD Compliance

- Patient consent required for data processing
- Right to access personal data
- Right to delete data (soft delete with audit trail)
- Data minimization principles followed
- Audit trail for all sensitive operations

## 🧪 Testing

### Example: Complete Flow Test

```bash
# 1. Create consent
CONSENT=$(curl -X POST https://api.medicsoft.com.br/api/telemedicine/consent \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: tenant-123" \
  -H "Content-Type: application/json" \
  -d '{"patientId":"'$PATIENT_ID'","acceptsRecording":true,"acceptsDataSharing":true,"digitalSignature":"sig","ipAddress":"192.168.1.1","userAgent":"curl"}')

# 2. Upload identity documents
curl -X POST https://api.medicsoft.com.br/api/telemedicine/identityverification \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: tenant-123" \
  -F "userId=$PATIENT_ID" \
  -F "userType=Patient" \
  -F "documentType=CPF" \
  -F "documentNumber=12345678900" \
  -F "documentPhoto=@patient_cpf.jpg"

# 3. Create session
SESSION=$(curl -X POST https://api.medicsoft.com.br/api/telemedicine/sessions \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: tenant-123" \
  -H "Content-Type: application/json" \
  -d '{"appointmentId":"'$APPOINTMENT_ID'","patientId":"'$PATIENT_ID'","providerId":"'$PROVIDER_ID'","clinicId":"'$CLINIC_ID'"}')

# 4. Validate compliance
curl https://api.medicsoft.com.br/api/telemedicine/sessions/$SESSION_ID/validate-compliance \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: tenant-123"

# 5. Start session
curl -X POST https://api.medicsoft.com.br/api/telemedicine/sessions/$SESSION_ID/start \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: tenant-123" \
  -H "Content-Type: application/json" \
  -d '{"notes":"Starting consultation"}'
```

## 📞 Support

**API Documentation:** https://api.medicsoft.com.br/swagger  
**Technical Support:** api-support@medicsoft.com.br  
**Security Issues:** security@medicsoft.com.br

---

**Last Updated:** January 29, 2026  
**API Version:** 1.0.0  
**Maintained by:** Omni Care Software Engineering Team
