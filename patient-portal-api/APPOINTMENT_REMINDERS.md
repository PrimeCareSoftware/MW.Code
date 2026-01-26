# Appointment Reminder Service

## Overview

The Appointment Reminder Service is a background service that automatically sends reminders to patients about upcoming appointments. It runs as a hosted service in the Patient Portal API and sends notifications via email, SMS, and WhatsApp.

## Features

- **Automated Reminders**: Sends reminders 24 hours before scheduled appointments
- **Multiple Channels**: Email (primary), SMS, and WhatsApp support
- **Smart Scheduling**: Runs every hour and checks for appointments needing reminders
- **Duplicate Prevention**: Tracks reminder status to avoid sending duplicates
- **Error Handling**: Comprehensive error handling and logging
- **Graceful Shutdown**: Supports cancellation tokens for clean service shutdown
- **Configurable**: All settings are configurable via appsettings.json

## Architecture

### Components

1. **AppointmentReminderService** - BackgroundService that orchestrates the reminder process
2. **INotificationService** - Interface for sending notifications
3. **NotificationService** - Implementation for email, SMS, and WhatsApp
4. **EmailTemplateHelper** - Generates HTML emails and text messages
5. **Configuration Models** - Settings for service and email configuration

### Database Changes

Two new columns added to the `Appointments` table:
- `ReminderSent` (boolean) - Tracks if reminder has been sent
- `ReminderSentAt` (timestamp) - When the reminder was sent

Indexes created for optimal query performance.

## Configuration

### appsettings.json

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
    "From": "noreply@primecare.com",
    "FromName": "PrimeCare Portal"
  },
  "PortalSettings": {
    "BaseUrl": "https://portal.primecare.com"
  }
}
```

### Development Settings

For local development, use a local SMTP server like Papercut or MailHog:

```json
{
  "AppointmentReminder": {
    "CheckIntervalMinutes": 5
  },
  "Email": {
    "SmtpServer": "localhost",
    "SmtpPort": 1025,
    "UseSsl": false,
    "From": "noreply@primecare.local"
  },
  "PortalSettings": {
    "BaseUrl": "http://localhost:3000"
  }
}
```

## Email Setup

### Gmail SMTP

1. Enable 2-factor authentication in your Google account
2. Generate an App Password: https://myaccount.google.com/apppasswords
3. Use your Gmail address as `Username` and the app password as `Password`

### SendGrid (Alternative)

```json
{
  "Email": {
    "SendGridApiKey": "your-sendgrid-api-key"
  }
}
```

## Testing

### Local Email Testing

Use Papercut SMTP or MailHog for testing:

**Papercut SMTP:**
```bash
# Download from: https://github.com/ChangemakerStudios/Papercut-SMTP
# Run and set SmtpServer to "localhost", SmtpPort to 25
```

**MailHog (Docker):**
```bash
docker run -d -p 1025:1025 -p 8025:8025 mailhog/mailhog
# SMTP: localhost:1025
# Web UI: http://localhost:8025
```

### Running Tests

```bash
cd patient-portal-api
dotnet test PatientPortal.Tests/Services/NotificationServiceTests.cs
dotnet test PatientPortal.Tests/Services/EmailTemplateHelperTests.cs
dotnet test PatientPortal.Tests/Services/AppointmentReminderServiceTests.cs
```

## Database Migration

Run the migration script to add reminder tracking fields:

```bash
psql -h localhost -U postgres -d primecare -f migrations/001_add_reminder_fields.sql
```

Or using the provided script:
```bash
# From the project root
psql -h localhost -U postgres -d primecare < patient-portal-api/migrations/001_add_reminder_fields.sql
```

## Monitoring

### Logs

The service logs detailed information at various levels:

- **Information**: Service start/stop, reminder checks, successful sends
- **Warning**: Configuration issues, missing data
- **Error**: Failed notifications, database errors
- **Debug**: Detailed query information (Development only)

### Metrics to Monitor

1. Number of reminders sent per hour
2. Email delivery failures
3. Database query performance
4. Service restart frequency

### Log Examples

```
[INF] Appointment Reminder Service started. Check interval: 60 minutes, Advance notice: 24 hours
[INF] Found 5 appointments needing reminders
[INF] Sending reminder for appointment abc123 to patient João Silva (joao@example.com)
[INF] Successfully sent reminder for appointment abc123
```

## Email Template

The service generates beautiful HTML emails with:
- Patient name and appointment details
- Doctor information and specialty
- Clinic location
- Action buttons (Confirm, Reschedule, Cancel)
- Telehealth indicators when applicable
- Responsive design for mobile devices

## SMS/WhatsApp Integration

Currently implemented as placeholders. To enable:

### Twilio Integration

```csharp
// In NotificationService.cs
public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken)
{
    var twilioClient = new TwilioRestClient(_twilioAccountSid, _twilioAuthToken);
    var messageResource = await MessageResource.CreateAsync(
        to: new PhoneNumber(phoneNumber),
        from: new PhoneNumber(_twilioPhoneNumber),
        body: message
    );
}
```

### WhatsApp via Twilio

```csharp
public async Task SendWhatsAppAsync(string phoneNumber, string message, CancellationToken cancellationToken)
{
    var twilioClient = new TwilioRestClient(_twilioAccountSid, _twilioAuthToken);
    var messageResource = await MessageResource.CreateAsync(
        to: new PhoneNumber($"whatsapp:{phoneNumber}"),
        from: new PhoneNumber($"whatsapp:{_twilioWhatsAppNumber}"),
        body: message
    );
}
```

## Troubleshooting

### Service Not Starting

1. Check if `AppointmentReminder:Enabled` is set to `true`
2. Verify database connection string
3. Check logs for startup errors

### Emails Not Sending

1. Verify SMTP settings in appsettings.json
2. Check firewall/network allows SMTP port
3. Verify email credentials (for Gmail, use App Password)
4. Check logs for SMTP errors

### No Reminders Being Sent

1. Verify there are appointments scheduled for tomorrow
2. Check that appointments have `Status` = Scheduled or Confirmed
3. Verify patients have valid email addresses
4. Check that `ReminderSent` is not already true

### Performance Issues

1. Check database indexes are created
2. Monitor query execution time
3. Consider adjusting `CheckIntervalMinutes` if needed
4. Review log volume and adjust log levels

## Production Deployment

### Checklist

- [ ] Update SMTP settings with production credentials
- [ ] Set `PortalSettings:BaseUrl` to production URL
- [ ] Run database migration
- [ ] Configure SSL/TLS for SMTP (set `UseSsl: true`)
- [ ] Set up monitoring and alerts
- [ ] Test with a small group first
- [ ] Configure rate limits if using third-party email service
- [ ] Set appropriate `CheckIntervalMinutes` (60 recommended)
- [ ] Review and adjust log levels
- [ ] Set up log aggregation (e.g., Seq, ELK, Application Insights)

### Environment Variables

For security, avoid storing credentials in appsettings.json. Use environment variables:

```bash
export Email__Username="your-email@gmail.com"
export Email__Password="your-app-password"
export Email__SendGridApiKey="your-api-key"
```

Or use Azure Key Vault, AWS Secrets Manager, etc.

## Future Enhancements

- [ ] SMS/WhatsApp integration with Twilio
- [ ] Configurable reminder templates per clinic
- [ ] Multiple reminder times (e.g., 48h, 24h, 2h before)
- [ ] Patient preference management (opt-out, channel preference)
- [ ] Retry logic for failed deliveries
- [ ] Dashboard for monitoring reminder statistics
- [ ] A/B testing for email templates
- [ ] Multi-language support
- [ ] Push notifications for mobile app

## Support

For issues or questions:
- Check logs for error messages
- Review this documentation
- Contact the development team

## License

Proprietary - PrimeCare Software © 2026
