# 📨 CRM Messaging Integrations - Implementation Guide

**Status:** ✅ Implemented (PR #440 Completion)  
**Date:** January 28, 2026  
**Version:** 1.0

---

## 📋 Overview

This document describes the implementation of real messaging integrations for the CRM Advanced module, replacing the stub implementations with production-ready services for Email (SendGrid), SMS (Twilio), and WhatsApp (WhatsApp Business API).

## 🎯 What Was Implemented

### 1. Real Service Implementations

#### ✅ SendGridEmailService
- **File:** `src/MedicSoft.Api/Services/CRM/SendGridEmailService.cs`
- **Features:**
  - SendGrid API integration for email delivery
  - Template-based email support
  - Sandbox mode for testing
  - Comprehensive error handling and logging
  - Enable/disable toggle via configuration

#### ✅ TwilioSmsService
- **File:** `src/MedicSoft.Api/Services/CRM/TwilioSmsService.cs`
- **Features:**
  - Twilio API integration for SMS delivery
  - Automatic phone number formatting (Brazilian numbers)
  - Enable/disable toggle via configuration
  - Comprehensive error handling and logging

#### ✅ WhatsAppBusinessService
- **File:** `src/MedicSoft.Api/Services/CRM/WhatsAppBusinessService.cs`
- **Features:**
  - WhatsApp Business API integration
  - Message delivery via Meta Graph API
  - Automatic phone number formatting
  - Enable/disable toggle via configuration
  - Comprehensive error handling and logging

### 2. Configuration Infrastructure

#### ✅ MessagingConfiguration
- **File:** `src/MedicSoft.Api/Configuration/MessagingConfiguration.cs`
- **Features:**
  - Centralized configuration for all messaging services
  - Type-safe configuration classes
  - Support for service enable/disable flags
  - Sandbox mode for email testing

#### ✅ appsettings.json Integration
- **File:** `src/MedicSoft.Api/appsettings.json`
- **Configuration Structure:**
```json
{
  "Messaging": {
    "Email": {
      "ApiKey": "",
      "FromEmail": "no-reply@omnicare.com.br",
      "FromName": "Omni Care Software",
      "UseSandbox": true,
      "Enabled": false
    },
    "Sms": {
      "AccountSid": "",
      "AuthToken": "",
      "FromPhoneNumber": "",
      "Enabled": false
    },
    "WhatsApp": {
      "ApiUrl": "https://graph.facebook.com/v18.0",
      "AccessToken": "",
      "PhoneNumberId": "",
      "Enabled": false
    }
  }
}
```

### 3. Service Registration

#### ✅ Dynamic Service Selection
- **File:** `src/MedicSoft.Api/Program.cs`
- **Features:**
  - Conditional registration based on `Enabled` flag
  - Falls back to stub implementations when disabled
  - Maintains backward compatibility
  - No code changes required to switch implementations

### 4. NuGet Package Integration

#### ✅ Added Packages
- **SendGrid:** v9.29.3 (no vulnerabilities)
- **Twilio:** v7.8.0 (no vulnerabilities)
- **File:** `src/MedicSoft.Api/MedicSoft.Api.csproj`

### 5. Comprehensive Unit Tests

#### ✅ Test Coverage (42 tests total)
- **SendGridEmailServiceTests:** 14 tests
  - Configuration validation
  - Enable/disable state handling
  - Email sending with different inputs
  - Sandbox mode configuration
  
- **TwilioSmsServiceTests:** 14 tests
  - Configuration validation
  - Enable/disable state handling
  - Phone number formatting tests
  - Credential validation
  
- **WhatsAppBusinessServiceTests:** 14 tests
  - Configuration validation
  - Enable/disable state handling
  - Phone number formatting tests
  - API configuration validation

**Files:**
- `tests/MedicSoft.Test/Services/CRM/SendGridEmailServiceTests.cs`
- `tests/MedicSoft.Test/Services/CRM/TwilioSmsServiceTests.cs`
- `tests/MedicSoft.Test/Services/CRM/WhatsAppBusinessServiceTests.cs`

---

## 🚀 How to Enable Real Integrations

### Step 1: Configure Credentials

Edit `appsettings.json` or use environment variables/Azure Key Vault in production:

```json
{
  "Messaging": {
    "Email": {
      "ApiKey": "SG.your_sendgrid_api_key_here",
      "FromEmail": "notifications@yourcompany.com",
      "FromName": "Your Company Name",
      "UseSandbox": false,
      "Enabled": true
    },
    "Sms": {
      "AccountSid": "ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
      "AuthToken": "your_twilio_auth_token",
      "FromPhoneNumber": "+5511999999999",
      "Enabled": true
    },
    "WhatsApp": {
      "ApiUrl": "https://graph.facebook.com/v18.0",
      "AccessToken": "your_whatsapp_access_token",
      "PhoneNumberId": "123456789012345",
      "Enabled": true
    }
  }
}
```

### Step 2: Set Enabled Flags

- Set `Enabled: true` for each service you want to use
- Set `Enabled: false` to use stub implementations (for development)

### Step 3: Restart Application

The application will automatically use the real implementations when enabled.

---

## 🔒 Security Considerations

### ⚠️ DO NOT commit credentials to source control

**Best Practices:**

1. **Development:** Keep `Enabled: false` and use empty credentials
2. **Staging/Production:** Use environment variables or Azure Key Vault:
   ```bash
   export Messaging__Email__ApiKey="SG.xxxxx"
   export Messaging__Sms__AccountSid="ACxxxxx"
   export Messaging__WhatsApp__AccessToken="xxxxx"
   ```

3. **Azure App Service:** Configure in Application Settings
4. **Docker:** Use secrets or environment variables

### 🔐 Credentials Management

- Email API keys should be restricted to Mail Send only
- Twilio tokens should use project-specific keys
- WhatsApp tokens should be properly scoped
- Rotate credentials regularly
- Use read-only keys when possible

---

## 📊 Testing

### Development Testing (Stub Services)

```json
{
  "Messaging": {
    "Email": { "Enabled": false },
    "Sms": { "Enabled": false },
    "WhatsApp": { "Enabled": false }
  }
}
```

All messages will be logged but not actually sent.

### Sandbox Testing (Real APIs, Test Mode)

```json
{
  "Messaging": {
    "Email": {
      "ApiKey": "SG.test_key",
      "UseSandbox": true,
      "Enabled": true
    }
  }
}
```

SendGrid will validate but not deliver emails.

### Production Testing

Enable services with real credentials and test with controlled recipients.

---

## 📝 Usage Examples

### Email Service

```csharp
// Inject IEmailService
private readonly IEmailService _emailService;

// Send simple email
await _emailService.SendEmailAsync(
    to: "patient@example.com",
    subject: "Appointment Reminder",
    body: "<h1>Your appointment is tomorrow at 10:00 AM</h1>"
);

// Send template email
var variables = new Dictionary<string, string>
{
    { "patientName", "John Doe" },
    { "appointmentDate", "2026-01-29" }
};

await _emailService.SendEmailWithTemplateAsync(
    to: "patient@example.com",
    templateId: templateGuid,
    variables: variables
);
```

### SMS Service

```csharp
// Inject ISmsService
private readonly ISmsService _smsService;

// Send SMS (supports multiple phone formats)
await _smsService.SendSmsAsync(
    to: "11987654321",  // Automatically formatted to +5511987654321
    message: "Your appointment is confirmed for tomorrow at 10:00 AM"
);
```

### WhatsApp Service

```csharp
// Inject IWhatsAppService
private readonly IWhatsAppService _whatsAppService;

// Send WhatsApp message
await _whatsAppService.SendWhatsAppAsync(
    to: "+5511987654321",  // Formatted automatically
    message: "Hello! Your appointment is confirmed."
);
```

---

## 🧪 Test Execution

### Run Unit Tests

```bash
cd tests/MedicSoft.Test
dotnet test --filter "FullyQualifiedName~SendGridEmailServiceTests|FullyQualifiedName~TwilioSmsServiceTests|FullyQualifiedName~WhatsAppBusinessServiceTests"
```

### Expected Results
- ✅ 42 tests should pass
- All tests verify configuration, enable/disable states, and error handling
- Tests use mocked dependencies to avoid external API calls

---

## 📚 Related Documentation

- **Configuration Guide:** `/CRM_CONFIGURATION_GUIDE.md`
- **User Guide:** `/CRM_USER_GUIDE.md`
- **Implementation Status:** `/CRM_IMPLEMENTATION_STATUS.md`
- **Development Plan:** `/Plano_Desenvolvimento/fase-4-analytics-otimizacao/17-crm-avancado.md`

---

## 🎯 Next Steps

### Remaining Work

- [ ] Implement webhooks for message status tracking
- [ ] Add end-to-end integration tests
- [ ] Add message queue for high-volume scenarios

### ✅ Recently Completed (PR #442 Pending Items - January 2026)

- [x] **Email template loading from database** - Complete
  - Created IEmailTemplateRepository and implementation
  - SendGridEmailService now loads templates from EmailTemplate entity
  - Template variables replaced using {{variableName}} format with HTML encoding
  - Falls back to simple template if database template not found
  - Updated interface to support optional tenantId parameter
- [x] **Rate limiting and retry logic** - Complete
  - Integrated Polly 8.5.0 for resilience patterns
  - All messaging services (Email, SMS, WhatsApp) now include automatic retry with exponential backoff
  - Handles transient errors (5xx, rate limit 429) with intelligent retry
  - 3 retry attempts with jitter to prevent thundering herd
  - Service-specific error detection (Twilio error codes, HTTP status codes)

### Optional Enhancements

- [ ] Support for SendGrid Dynamic Templates
- [ ] Support for Twilio WhatsApp integration
- [ ] Message delivery status tracking
- [ ] Analytics dashboard for message metrics
- [ ] Multi-language template support

---

## 🐛 Troubleshooting

### Email Not Sending

1. Check `Messaging:Email:Enabled` is `true`
2. Verify API key is correct and has Mail Send permissions
3. Check SendGrid dashboard for blocked sends
4. Review application logs for error details

### SMS Not Sending

1. Check `Messaging:Sms:Enabled` is `true`
2. Verify Twilio credentials (AccountSid and AuthToken)
3. Ensure phone number is in E.164 format
4. Check Twilio console for error messages

### WhatsApp Not Sending

1. Check `Messaging:WhatsApp:Enabled` is `true`
2. Verify WhatsApp Business API credentials
3. Ensure phone number has been verified
4. Check Meta Business Manager for API errors

---

## 📞 Support

For questions or issues:
- Review the configuration guide: `/CRM_CONFIGURATION_GUIDE.md`
- Check application logs in `Logs/` directory
- Contact the development team

---

**Implementation completed as part of PR #442 - Sprint 7 Integrations**

**Updates - PR #442 Pending Items (January 2026):**

✅ **Completed:**
1. **Email Template Loading from Database** - Email templates are now loaded from the EmailTemplate entity with variable replacement
2. **Rate Limiting and Retry Logic** - All messaging services include automatic retry with exponential backoff for transient errors

📝 **Remaining (Lower Priority):**
- Webhooks for message status tracking (requires webhook endpoint infrastructure)
- End-to-end integration tests (requires test credentials and environments)
- Message queue for high-volume scenarios (requires queue infrastructure like RabbitMQ)
