# 🔧 Telemedicine Troubleshooting Guide

## 📋 Overview

Comprehensive troubleshooting guide for common issues in the MedicSoft Telemedicine microservice.

## 🚨 Common Issues and Solutions

### Authentication Issues

#### Issue: "Unauthorized - Invalid JWT Token"

**Symptoms:**
- API returns 401 Unauthorized
- Error message: "Invalid or expired JWT token"

**Possible Causes:**
1. Token expired
2. Token signature invalid
3. Wrong signing key

**Solution:**
```bash
# Verify token is not expired
echo $TOKEN | cut -d. -f2 | base64 -d | jq .exp

# Check current timestamp
date +%s

# Token should expire after current time
# If expired, request new token

# Verify token signature matches configuration
# Check appsettings.json JWT:SecretKey
```

**Prevention:**
- Implement automatic token refresh
- Set appropriate expiration time (60 minutes recommended)
- Store tokens securely

#### Issue: "Missing X-Tenant-Id Header"

**Symptoms:**
- API returns 400 Bad Request
- Error: "X-Tenant-Id header is required"

**Solution:**
```javascript
// Always include tenant header in requests
const headers = {
  'Authorization': `Bearer ${token}`,
  'X-Tenant-Id': tenantId,
  'Content-Type': 'application/json'
};
```

### Consent Issues

#### Issue: "Patient has no valid consent"

**Symptoms:**
- Cannot start telemedicine session
- Error: "Patient consent not found or revoked"

**Diagnosis:**
```bash
# Check if consent exists
curl -X GET "https://api.medicsoft.com.br/api/telemedicine/consent/patient/${PATIENT_ID}/has-valid" \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: $TENANT_ID"
```

**Solution:**
1. Patient must register new consent:
```bash
curl -X POST "https://api.medicsoft.com.br/api/telemedicine/consent" \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: $TENANT_ID" \
  -H "Content-Type: application/json" \
  -d '{
    "patientId": "'$PATIENT_ID'",
    "acceptsRecording": true,
    "acceptsDataSharing": true,
    "digitalSignature": "patient_signature",
    "ipAddress": "192.168.1.1",
    "userAgent": "Mozilla/5.0..."
  }'
```

2. If consent was revoked, patient must provide new consent
3. Check consent validity period (no expiration, but can be revoked)

#### Issue: "Consent already exists for appointment"

**Symptoms:**
- Trying to create duplicate consent
- Error code: 409 Conflict

**Solution:**
- Retrieve existing consent instead of creating new one
- Use `GET /api/telemedicine/consent/patient/{patientId}/has-valid`

### Identity Verification Issues

#### Issue: "Identity verification expired"

**Symptoms:**
- Session validation fails
- Error: "Identity verification expired (older than 1 year)"

**Diagnosis:**
```bash
# Check verification status
curl -X GET "https://api.medicsoft.com.br/api/telemedicine/identityverification/user/${USER_ID}/latest" \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: $TENANT_ID"

# Check response:
# "isValid": false
# "expiresAt": "2025-01-01T00:00:00Z"  # Date in past
```

**Solution:**
User must upload new identity documents:
```bash
curl -X POST "https://api.medicsoft.com.br/api/telemedicine/identityverification" \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: $TENANT_ID" \
  -F "userId=$USER_ID" \
  -F "userType=Patient" \
  -F "documentType=CPF" \
  -F "documentNumber=12345678900" \
  -F "documentPhoto=@cpf_front.jpg" \
  -F "selfie=@selfie.jpg"
```

#### Issue: "Provider missing CRM information"

**Symptoms:**
- Provider verification fails
- Error: "CRM number and state required for Provider verification"

**Solution:**
```bash
# Provider verification requires CRM fields
curl -X POST "https://api.medicsoft.com.br/api/telemedicine/identityverification" \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: $TENANT_ID" \
  -F "userId=$PROVIDER_ID" \
  -F "userType=Provider" \
  -F "documentType=CPF" \
  -F "documentNumber=98765432100" \
  -F "crmNumber=12345" \
  -F "crmState=SP" \
  -F "documentPhoto=@cpf.jpg" \
  -F "crmCardPhoto=@crm_card.jpg" \  # Required for providers
  -F "selfie=@selfie.jpg"
```

### Session Issues

#### Issue: "Cannot start session - status not Scheduled"

**Symptoms:**
- Error when calling `/sessions/{id}/start`
- Session already started or completed

**Diagnosis:**
```bash
# Check session status
curl -X GET "https://api.medicsoft.com.br/api/telemedicine/sessions/${SESSION_ID}" \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: $TENANT_ID"
```

**Solution:**
- Sessions can only be started from `Scheduled` status
- If session is `InProgress`, user can join directly
- If `Completed` or `Cancelled`, create new session

**Valid Status Transitions:**
```
Scheduled → InProgress → Completed
Scheduled → Cancelled
InProgress → Failed
InProgress → Cancelled
```

#### Issue: "Session compliance validation failed"

**Symptoms:**
- Cannot start session
- Error: "Compliance check failed"

**Diagnosis:**
```bash
# Run compliance pre-flight check
curl -X GET "https://api.medicsoft.com.br/api/telemedicine/sessions/${SESSION_ID}/validate-compliance" \
  -H "Authorization: Bearer $TOKEN" \
  -H "X-Tenant-Id: $TENANT_ID"
```

**Response Example:**
```json
{
  "isCompliant": false,
  "checks": {
    "patientConsent": {
      "passed": false,
      "message": "No valid consent found"
    },
    "patientIdentityVerification": {
      "passed": false,
      "message": "Identity verification expired"
    },
    "providerIdentityVerification": {
      "passed": true
    }
  }
}
```

**Solution:**
Address each failed check:
1. **Missing consent**: Patient must provide consent
2. **Expired verification**: User must re-verify identity
3. **First appointment without justification**: Provide justification or schedule in-person visit

### Video Connection Issues

#### Issue: "Unable to join video room"

**Symptoms:**
- Room URL loads but video doesn't connect
- WebRTC connection timeout

**Diagnosis:**
```bash
# Check if Daily.co is accessible
curl -I https://api.daily.co/v1/

# Test STUN/TURN connectivity
# Use browser console:
const pc = new RTCPeerConnection({
  iceServers: [{urls: 'stun:stun.l.google.com:19302'}]
});
pc.createOffer().then(offer => pc.setLocalDescription(offer));
// Check console for ICE candidates
```

**Common Causes:**
1. **Firewall blocking WebRTC ports**
   - Required ports: UDP 3478 (STUN), TCP 80/443 (HTTPS)
   - Corporate firewalls may block WebRTC

2. **Browser permissions denied**
   - Camera/microphone permissions not granted

3. **Daily.co API key invalid**
   - Check API key in configuration

**Solution:**

For firewall issues:
```bash
# Allow WebRTC ports
sudo ufw allow 3478/udp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Test STUN server
stunclient --mode full stun.l.google.com 19302
```

For browser permissions:
```javascript
// Request permissions explicitly
navigator.mediaDevices.getUserMedia({ video: true, audio: true })
  .then(stream => {
    console.log('Permissions granted');
    stream.getTracks().forEach(track => track.stop());
  })
  .catch(err => console.error('Permission denied:', err));
```

#### Issue: "Poor video quality or lag"

**Symptoms:**
- Video freezes
- Audio breaks up
- High latency

**Diagnosis:**
```javascript
// Monitor connection quality
const connection = dailyCall.participants().local;
console.log('Connection stats:', {
  videoRecvBitsPerSecond: connection.tracks.video.recvBitsPerSecond,
  audioRecvBitsPerSecond: connection.tracks.audio.recvBitsPerSecond,
  videoSendBitsPerSecond: connection.tracks.video.sendBitsPerSecond,
  audioSendBitsPerSecond: connection.tracks.audio.sendBitsPerSecond
});
```

**Solution:**
1. **Check bandwidth:**
   ```bash
   # Minimum required: 2 Mbps upload, 2 Mbps download
   speedtest-cli
   ```

2. **Adjust video quality:**
   ```javascript
   // Lower video resolution
   dailyCall.setLocalVideo({
     width: 640,
     height: 480,
     frameRate: 15
   });
   ```

3. **Close other applications** using bandwidth

### File Upload Issues

#### Issue: "File upload failed - size too large"

**Symptoms:**
- Error 413: Payload Too Large
- Upload timeout

**Solution:**
```bash
# Maximum file size: 10MB
# Check file size before upload
FILE_SIZE=$(wc -c < document.pdf)
MAX_SIZE=$((10 * 1024 * 1024))  # 10MB in bytes

if [ $FILE_SIZE -gt $MAX_SIZE ]; then
  echo "File too large: $FILE_SIZE bytes (max: $MAX_SIZE)"
  # Compress file or split into parts
else
  # Upload file
  curl -X POST "https://api.medicsoft.com.br/api/telemedicine/identityverification" \
    -F "documentPhoto=@document.pdf"
fi
```

#### Issue: "Invalid file type"

**Symptoms:**
- Error 400: Invalid file type
- Unsupported file format

**Supported File Types:**
- Images: JPEG, PNG
- Documents: PDF
- Video (recordings): MP4, WebM

**Solution:**
```bash
# Convert image to supported format
convert document.tiff document.jpg

# Compress PDF
gs -sDEVICE=pdfwrite -dCompatibilityLevel=1.4 -dPDFSETTINGS=/ebook \
   -dNOPAUSE -dQUIET -dBATCH \
   -sOutputFile=compressed.pdf original.pdf
```

### Database Issues

#### Issue: "Database connection timeout"

**Symptoms:**
- API returns 500 Internal Server Error
- Logs show: "Timeout expired. The timeout period elapsed..."

**Diagnosis:**
```bash
# Test database connectivity
psql -h prod-db.postgres.database.azure.com \
     -U medicsoft \
     -d telemedicine \
     -c "SELECT 1;"

# Check connection pool
# Look for: "Connection pool exhausted"
```

**Solution:**

1. **Increase connection pool size:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;MaxPoolSize=100;MinPoolSize=10;"
  }
}
```

2. **Check for connection leaks:**
```bash
# Monitor active connections
SELECT count(*), state 
FROM pg_stat_activity 
WHERE datname = 'telemedicine' 
GROUP BY state;
```

3. **Restart API service** if pool exhausted

#### Issue: "Migration failed"

**Symptoms:**
- Error applying database migrations
- Schema version mismatch

**Diagnosis:**
```bash
# Check applied migrations
dotnet ef migrations list --context TelemedicineDbContext

# Check database schema version
SELECT * FROM __EFMigrationsHistory ORDER BY migration_id;
```

**Solution:**

If migration partially applied:
```bash
# Rollback failed migration (if possible)
dotnet ef database update PreviousMigrationName --context TelemedicineDbContext

# Fix migration issues
# Re-apply migration
dotnet ef database update --context TelemedicineDbContext
```

If migration conflicts:
```bash
# Remove migration
dotnet ef migrations remove --context TelemedicineDbContext

# Create new migration
dotnet ef migrations add FixedMigration --context TelemedicineDbContext
```

### Performance Issues

#### Issue: "API response time > 2 seconds"

**Diagnosis:**
```bash
# Profile API response times
curl -w "@curl-format.txt" -o /dev/null -s "https://api.medicsoft.com.br/api/telemedicine/sessions"

# curl-format.txt:
# time_namelookup: %{time_namelookup}s
# time_connect: %{time_connect}s
# time_appconnect: %{time_appconnect}s
# time_pretransfer: %{time_pretransfer}s
# time_starttransfer: %{time_starttransfer}s
# time_total: %{time_total}s
```

**Common Causes:**
1. **Missing database indexes**
2. **N+1 query problem**
3. **Large result sets**
4. **Unoptimized queries**

**Solution:**

1. **Add indexes:**
```sql
CREATE INDEX idx_consent_patient_tenant 
ON telemedicine_consents(patient_id, tenant_id) 
WHERE is_revoked = false;

CREATE INDEX idx_session_appointment 
ON telemedicine_sessions(appointment_id, tenant_id);
```

2. **Enable query logging:**
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

3. **Implement caching:**
```csharp
// Cache consent text (rarely changes)
builder.Services.AddMemoryCache();

// In controller:
_cache.GetOrCreate("consent-text-v1", entry =>
{
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);
    return _consentService.GetConsentText();
});
```

#### Issue: "High memory usage"

**Diagnosis:**
```bash
# Check container memory
docker stats telemedicine-api

# Check process memory
ps aux | grep MedicSoft.Telemedicine.Api
```

**Solution:**

1. **Set memory limits:**
```yaml
# docker-compose.yml
services:
  telemedicine-api:
    deploy:
      resources:
        limits:
          memory: 512M
        reservations:
          memory: 256M
```

2. **Implement pagination:**
```csharp
// Instead of loading all records
var sessions = await _repository.GetAllAsync(tenantId);

// Use pagination
var sessions = await _repository.GetPagedAsync(tenantId, page: 1, pageSize: 20);
```

## 🔍 Debugging Tips

### Enable Detailed Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "MedicSoft.Telemedicine": "Trace",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Capture HTTP Traffic

```bash
# Use Fiddler or Charles Proxy
# Or use curl verbose mode
curl -v -X POST "https://api.medicsoft.com.br/api/telemedicine/sessions" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"appointmentId":"test"}'
```

### Check Application Logs

```bash
# Docker logs
docker logs telemedicine-api --tail=100 --follow

# File logs (if configured)
tail -f /var/log/telemedicine/log-$(date +%Y%m%d).txt
```

## 📞 Getting Help

### Before Contacting Support

1. Check this troubleshooting guide
2. Review API documentation
3. Check application logs
4. Verify configuration settings
5. Test with curl/Postman

### Support Channels

- **Technical Support:** support@medicsoft.com.br
- **API Issues:** api-support@medicsoft.com.br
- **Security Issues:** security@medicsoft.com.br
- **Emergency Hotline:** +55 11 9xxxx-xxxx (24/7)

### Information to Provide

When contacting support, include:

1. **Error Details:**
   - Error message
   - Status code
   - Trace ID (from response)

2. **Request Details:**
   - Endpoint URL
   - HTTP method
   - Request headers (excluding sensitive data)
   - Request body (excluding PII)

3. **Environment:**
   - Environment (production/staging/development)
   - API version
   - Client application version

4. **Logs:**
   - Relevant log entries
   - Timestamp of issue
   - Tenant ID (if multi-tenant issue)

## 📚 Additional Resources

- [API Documentation](./API_DOCUMENTATION_COMPLETE.md)
- [Production Deployment Guide](./PRODUCTION_DEPLOYMENT_GUIDE.md)
- [Security Summary](./SECURITY_SUMMARY.md)
- [CFM 2.314 Implementation](./CFM_2314_IMPLEMENTATION.md)

---

**Last Updated:** January 29, 2026  
**Version:** 1.0.0  
**Maintained by:** PrimeCare Software Support Team
