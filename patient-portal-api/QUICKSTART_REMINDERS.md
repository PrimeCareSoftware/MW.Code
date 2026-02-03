# Appointment Reminder Service - Quick Start Guide

## What Was Implemented

✅ **Background Service** - Runs every hour checking for appointments 24 hours ahead  
✅ **Email Notifications** - Beautiful HTML emails with appointment details  
✅ **SMS/WhatsApp Placeholders** - Ready for integration with Twilio/AWS SNS  
✅ **Database Tracking** - Prevents duplicate reminders  
✅ **Configuration** - Fully configurable via appsettings.json  
✅ **Testing** - 14 unit tests, all passing  
✅ **Documentation** - Complete setup and deployment guides

## Quick Setup

### 1. Run Database Migration
```bash
psql -h localhost -U postgres -d primecare -f patient-portal-api/migrations/001_add_reminder_fields.sql
```

### 2. Configure Email Settings

**appsettings.json:**
```json
{
  "AppointmentReminder": {
    "Enabled": true,
    "CheckIntervalMinutes": 60,
    "AdvanceNoticeHours": 24
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "From": "noreply@omnicare.com",
    "FromName": "Omni Care Portal"
  },
  "PortalSettings": {
    "BaseUrl": "https://portal.omnicare.com"
  }
}
```

### 3. For Gmail (Development/Testing)

1. Enable 2-Factor Authentication in Google Account
2. Generate App Password: https://myaccount.google.com/apppasswords
3. Use your Gmail address as `Username`
4. Use the app password as `Password`

### 4. For Local Testing (Recommended)

Use MailHog to capture emails locally:
```bash
docker run -d -p 1025:1025 -p 8025:8025 mailhog/mailhog
```

Then update **appsettings.Development.json:**
```json
{
  "Email": {
    "SmtpServer": "localhost",
    "SmtpPort": 1025,
    "UseSsl": false,
    "Username": "",
    "Password": ""
  }
}
```

View emails at: http://localhost:8025

### 5. Start the API

```bash
cd patient-portal-api
dotnet run --project PatientPortal.Api
```

The service will start automatically and run in the background.

## How to Test

### 1. Create a Test Appointment

Insert an appointment scheduled for tomorrow:
```sql
INSERT INTO "Appointments" 
("Id", "PatientId", "ClinicId", "ProfessionalId", "ScheduledDate", "ScheduledTime", 
 "DurationMinutes", "Type", "Mode", "Status", "TenantId", "CreatedAt", "UpdatedAt", 
 "ReminderSent", "IsPaid")
SELECT 
    gen_random_uuid(),
    "Id",
    "ClinicId",
    (SELECT "Id" FROM "Professionals" LIMIT 1),
    CURRENT_DATE + INTERVAL '1 day',
    '14:30:00'::time,
    30,
    'Consulta',
    'Presencial',
    1, -- Scheduled
    "TenantId",
    NOW(),
    NOW(),
    false,
    false
FROM "Patients" 
WHERE "Email" IS NOT NULL 
LIMIT 1;
```

### 2. Wait for Service to Run

- Development: Runs every 5 minutes
- Production: Runs every 60 minutes
- Or restart the API to trigger immediately

### 3. Check Logs

```
[INF] Found X appointments needing reminders
[INF] Sending reminder for appointment {Id} to patient {Name}
[INF] Successfully sent reminder for appointment {Id}
```

### 4. Check Email

- **MailHog**: http://localhost:8025
- **Gmail**: Check your inbox
- **Database**: Verify `ReminderSent = true`

## Monitoring

### Check Service Status
```bash
# View logs in real-time
docker logs -f patient-portal-api
```

### Check Database
```sql
-- Count reminders sent today
SELECT COUNT(*) 
FROM "Appointments" 
WHERE "ReminderSent" = true 
AND DATE("ReminderSentAt") = CURRENT_DATE;

-- View upcoming appointments needing reminders
SELECT 
    a."Id",
    p."Name" as PatientName,
    p."Email",
    a."ScheduledDate",
    a."ScheduledTime",
    a."ReminderSent"
FROM "Appointments" a
JOIN "Patients" p ON a."PatientId" = p."Id"
WHERE a."ScheduledDate" = CURRENT_DATE + INTERVAL '1 day'
AND a."Status" IN (1, 2)
ORDER BY a."ScheduledTime";
```

## Troubleshooting

### Service Not Running
- Check `AppointmentReminder:Enabled` is `true`
- Check API logs for startup errors
- Verify database connection

### Emails Not Sending
- Verify SMTP settings
- Check firewall allows SMTP port
- For Gmail, ensure App Password is used
- Check logs for SMTP errors

### No Reminders Being Sent
- Verify appointments exist for tomorrow
- Check appointment status is Scheduled (1) or Confirmed (2)
- Ensure patients have email addresses
- Verify `ReminderSent` is `false`

## Production Checklist

- [ ] Run database migration
- [ ] Configure production SMTP settings
- [ ] Set correct `PortalSettings:BaseUrl`
- [ ] Enable SSL/TLS (`Email:UseSsl: true`)
- [ ] Use secure credential storage (Key Vault, Secrets Manager)
- [ ] Set `CheckIntervalMinutes` to 60
- [ ] Configure logging and monitoring
- [ ] Test with a small batch first
- [ ] Set up alerting for failures
- [ ] Review email templates for branding

## Files to Review

- **Documentation**: `APPOINTMENT_REMINDERS.md` - Complete guide
- **Implementation**: `APPOINTMENT_REMINDER_IMPLEMENTATION.md` - Technical details
- **Migration**: `migrations/001_add_reminder_fields.sql` - Database schema
- **Service**: `PatientPortal.Infrastructure/Services/AppointmentReminderService.cs`
- **Tests**: `PatientPortal.Tests/Services/*Tests.cs`

## Support

For issues:
1. Check logs for error messages
2. Review this guide and the detailed documentation
3. Verify configuration settings
4. Test with local SMTP server first
5. Contact development team if issues persist

---
**Status**: ✅ Ready for Production  
**Tests**: 14/14 passing (100%)  
**Build**: Success, 0 warnings  
**Security**: CodeQL passed, 0 alerts
