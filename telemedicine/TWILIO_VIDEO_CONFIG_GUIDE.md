# Twilio Video Configuration Guide

## Overview

The telemedicine microservice now supports Twilio Video as an alternative video provider to Daily.co. You can choose which provider to use by setting the `VIDEO_PROVIDER` environment variable.

## Choosing a Video Provider

### Option 1: Daily.co (Default)
```bash
VIDEO_PROVIDER=DailyCo
DAILYCO_API_KEY=your-daily-co-api-key
```

### Option 2: Twilio Video
```bash
VIDEO_PROVIDER=Twilio
TWILIO_ACCOUNT_SID=your-account-sid
TWILIO_API_KEY_SID=your-api-key-sid
TWILIO_API_KEY_SECRET=your-api-key-secret
```

## Getting Twilio Credentials

1. **Sign up for Twilio**: Visit [https://console.twilio.com](https://console.twilio.com)
2. **Navigate to API Keys**: Go to Account > API Keys & Tokens
3. **Create an API Key**: 
   - Click "Create API Key"
   - Give it a name (e.g., "Telemedicine Production")
   - Choose "Standard" for the key type
   - Save the SID and Secret securely
4. **Get Account SID**: Find your Account SID on the Console Dashboard

## Configuration Methods

### Method 1: Environment Variables (Recommended)

Set these environment variables in your deployment platform:

```bash
# Required for Twilio
VIDEO_PROVIDER=Twilio
TWILIO_ACCOUNT_SID=ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TWILIO_API_KEY_SID=SKxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
TWILIO_API_KEY_SECRET=your-secret-key-here
```

### Method 2: appsettings.Production.json

If using configuration files, update `appsettings.Production.json`:

```json
{
  "VideoProvider": "Twilio",
  "TwilioVideo": {
    "AccountSid": "${TWILIO_ACCOUNT_SID}",
    "ApiKeySid": "${TWILIO_API_KEY_SID}",
    "ApiKeySecret": "${TWILIO_API_KEY_SECRET}"
  }
}
```

The `${}` syntax will be replaced with environment variable values at runtime.

### Method 3: Azure Key Vault (Production Best Practice)

For enhanced security, store credentials in Azure Key Vault:

```bash
# Store secrets in Key Vault
az keyvault secret set --vault-name your-keyvault \
  --name TwilioAccountSid \
  --value "your-account-sid"

az keyvault secret set --vault-name your-keyvault \
  --name TwilioApiKeySid \
  --value "your-api-key-sid"

az keyvault secret set --vault-name your-keyvault \
  --name TwilioApiKeySecret \
  --value "your-api-key-secret"
```

Then reference in appsettings:

```json
{
  "TwilioVideo": {
    "AccountSid": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/TwilioAccountSid/)",
    "ApiKeySid": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/TwilioApiKeySid/)",
    "ApiKeySecret": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/TwilioApiKeySecret/)"
  }
}
```

## Deployment Instructions

### Docker/Container

```bash
docker run \
  -e VIDEO_PROVIDER=Twilio \
  -e TWILIO_ACCOUNT_SID=$TWILIO_ACCOUNT_SID \
  -e TWILIO_API_KEY_SID=$TWILIO_API_KEY_SID \
  -e TWILIO_API_KEY_SECRET=$TWILIO_API_KEY_SECRET \
  medicsoft-telemedicine-api
```

### Docker Compose

Add to `docker-compose.production.yml`:

```yaml
services:
  telemedicine-api:
    environment:
      - VIDEO_PROVIDER=Twilio
      - TWILIO_ACCOUNT_SID=${TWILIO_ACCOUNT_SID}
      - TWILIO_API_KEY_SID=${TWILIO_API_KEY_SID}
      - TWILIO_API_KEY_SECRET=${TWILIO_API_KEY_SECRET}
```

### Kubernetes

Create a secret:

```bash
kubectl create secret generic twilio-credentials \
  --from-literal=account-sid=$TWILIO_ACCOUNT_SID \
  --from-literal=api-key-sid=$TWILIO_API_KEY_SID \
  --from-literal=api-key-secret=$TWILIO_API_KEY_SECRET
```

Reference in deployment:

```yaml
env:
  - name: VIDEO_PROVIDER
    value: "Twilio"
  - name: TWILIO_ACCOUNT_SID
    valueFrom:
      secretKeyRef:
        name: twilio-credentials
        key: account-sid
  - name: TWILIO_API_KEY_SID
    valueFrom:
      secretKeyRef:
        name: twilio-credentials
        key: api-key-sid
  - name: TWILIO_API_KEY_SECRET
    valueFrom:
      secretKeyRef:
        name: twilio-credentials
        key: api-key-secret
```

## Testing the Configuration

### 1. Verify Configuration

Check that the service starts without errors and recognizes the Twilio configuration:

```bash
# Check logs for Twilio initialization
docker logs telemedicine-api | grep "Twilio Video Service initialized"
```

### 2. Test Room Creation

Use the API to create a test room:

```bash
curl -X POST https://your-api/api/telemedicine/sessions \
  -H "Content-Type: application/json" \
  -H "X-Tenant-Id: your-tenant" \
  -d '{
    "patientId": "test-patient",
    "professionalId": "test-doctor",
    "scheduledAt": "2026-02-18T10:00:00Z"
  }'
```

### 3. Test Token Generation

Generate an access token:

```bash
curl -X POST https://your-api/api/telemedicine/sessions/{sessionId}/token \
  -H "X-Tenant-Id: your-tenant"
```

## Features

### Twilio Video Features

- **Group Rooms**: Support for up to 10 participants
- **Video & Audio**: High-quality WebRTC streams
- **Screen Sharing**: Built-in screen sharing support
- **Chat**: Optional chat functionality
- **Recording**: Can be enabled (requires additional configuration)
- **Quality Monitoring**: Network quality indicators

### Differences from Daily.co

| Feature | Daily.co | Twilio |
|---------|----------|--------|
| Max Participants | Configurable | 10 (default) |
| Recording | Automatic | Manual setup required |
| Pricing | Free tier: 10k min/mo | Pay as you go |
| SDK Size | Smaller | Larger |
| Setup Complexity | Simple | Moderate |

## Monitoring

Monitor Twilio usage through:
- Twilio Console: https://console.twilio.com
- Usage API: Check room creation and participant counts
- Logs: Application logs show room lifecycle events

## Troubleshooting

### Issue: "TwilioVideo:AccountSid not configured"

**Solution**: Ensure environment variables are set correctly:
```bash
echo $TWILIO_ACCOUNT_SID
echo $TWILIO_API_KEY_SID
```

### Issue: Token generation fails

**Solution**: Verify API Key credentials are correct and active in Twilio Console

### Issue: Participants can't join room

**Solution**: Check:
1. Token is valid and not expired
2. Room was created successfully
3. Network allows WebRTC connections
4. CORS is properly configured

## Security Best Practices

✅ **DO:**
- Store credentials in environment variables or Key Vault
- Rotate API keys regularly (every 90 days)
- Use different credentials for dev/staging/production
- Monitor usage for anomalies
- Set up alerts for high usage

❌ **DON'T:**
- Commit credentials to source control
- Share credentials via email or chat
- Use the same credentials across environments
- Embed credentials in client-side code
- Leave default/example credentials in production

## Support Resources

- **Twilio Video Docs**: https://www.twilio.com/docs/video
- **Twilio Console**: https://console.twilio.com
- **Twilio Support**: https://support.twilio.com
- **Status Page**: https://status.twilio.com

## Implementation Details

The Twilio integration is implemented in:
- `TwilioVideoService.cs`: Main service implementation
- `Program.cs`: Service registration and provider selection
- `appsettings.json`: Configuration structure

For code-level details, see the source files in the telemedicine microservice.
