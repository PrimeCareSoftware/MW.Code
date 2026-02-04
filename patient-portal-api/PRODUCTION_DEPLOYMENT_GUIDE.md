# Production Deployment Configuration Guide

## Overview

This guide explains how to configure the Patient Portal API for production deployment with the AppointmentReminderService enabled.

## Important: Sensitive Configuration

**NEVER** commit sensitive credentials to source control. Use environment variables, Azure Key Vault, AWS Secrets Manager, or similar secure storage for production credentials.

## Configuration Strategy

The `appsettings.Production.json` file only contains production-specific overrides. Sensitive settings should be configured using:

1. **Environment Variables** (recommended)
2. **User Secrets** (for local development)
3. **Key Vault/Secrets Manager** (for cloud deployments)
4. **Docker secrets** (for containerized deployments)

## AppointmentReminderService Configuration

The reminder service is **disabled by default**. To enable in production:

### Method 1: Environment Variables (Recommended)

Set the following environment variables:

```bash
# Database Connection
ConnectionStrings__DefaultConnection="Host=your-db-host;Port=5432;Database=primecare;Username=db_user;Password=your_secure_password;Include Error Detail=false"

# JWT Settings
JwtSettings__SecretKey="your-super-secret-jwt-key-min-32-chars"
JwtSettings__Issuer="PatientPortal"
JwtSettings__Audience="PatientPortal-API"

# Email Settings  
Email__SmtpServer="smtp.your-provider.com"
Email__SmtpPort="587"
Email__UseSsl="true"
Email__Username="your-smtp-username"
Email__Password="your-smtp-password"
Email__From="noreply@yourdomain.com"
Email__FromName="Your Company Portal"

# Portal Settings
PortalSettings__BaseUrl="https://portal.yourdomain.com"

# Reminder Service - ENABLE IN PRODUCTION
AppointmentReminder__Enabled="true"
AppointmentReminder__CheckIntervalMinutes="60"
AppointmentReminder__AdvanceNoticeHours="24"
```

### Method 2: Azure App Service Configuration

In Azure Portal > Configuration > Application settings:

```
ConnectionStrings:DefaultConnection = Host=...
JwtSettings:SecretKey = your-secret-key
Email:SmtpServer = smtp.your-provider.com
AppointmentReminder:Enabled = true
```

### Method 3: Docker Compose (with secrets)

Create a `docker-compose.prod.yml`:

```yaml
version: '3.8'

services:
  patient-portal-api:
    image: medicwarehouse/patient-portal-api:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=primecare;Username=postgres;Password_FILE=/run/secrets/db_password
      - JwtSettings__SecretKey_FILE=/run/secrets/jwt_secret
      - Email__SmtpPassword_FILE=/run/secrets/smtp_password
      - AppointmentReminder__Enabled=true
    secrets:
      - db_password
      - jwt_secret
      - smtp_password
    depends_on:
      - postgres

secrets:
  db_password:
    external: true
  jwt_secret:
    external: true
  smtp_password:
    external: true
```

## Verification Checklist

Before deploying to production:

- [ ] Database connection string is valid and tested
- [ ] JWT secret key is at least 32 characters and secure
- [ ] SMTP credentials are configured and tested
- [ ] Portal base URL matches your domain
- [ ] AppointmentReminder:Enabled is set to `true`
- [ ] Database migrations have been applied
- [ ] SSL/TLS is enabled (Email:UseSsl = true)
- [ ] Error detail is disabled in connection string
- [ ] Logging level is set to Warning or Error
- [ ] All secrets are stored securely (not in source code)

## Testing the Configuration

### 1. Check Service Status

After deployment, check logs for:

```
[INF] Appointment Reminder Service started. Check interval: 60 minutes, Advance notice: 24 hours
```

If you see "disabled" instead, the service is not enabled.

### 2. Verify Database Connection

The service should connect to the database without errors. Look for:

```
[INF] Found X appointments needing reminders
```

If you see authentication errors (28P01), verify your connection string.

### 3. Test Email Sending

Insert a test appointment for tomorrow and wait for the service to run. Check logs for:

```
[INF] Sending reminder for appointment {Id} to patient {Name}
[INF] Successfully sent reminder for appointment {Id}
```

## Troubleshooting

### Service Not Starting

**Error**: "Appointment Reminder Service is disabled"

**Solution**: Set environment variable `AppointmentReminder__Enabled=true`

### Database Authentication Failed (28P01)

**Error**: "password authentication failed for user postgres"

**Solution**: 
1. Verify connection string format
2. Check username and password are correct
3. Ensure database user has necessary permissions
4. Verify PostgreSQL allows connections from your host

### Email Not Sending

**Error**: "Failed to send email" or SMTP timeout

**Solution**:
1. Verify SMTP server and port are correct
2. Check firewall allows outbound SMTP connections
3. Ensure SMTP credentials are valid
4. For Gmail, use App Password (not regular password)
5. Verify SSL/TLS settings match your provider

## Security Best Practices

1. **Never commit credentials** to version control
2. **Use strong passwords** for database and JWT secret
3. **Rotate secrets regularly** (every 90 days)
4. **Limit database user permissions** (only what's needed)
5. **Enable SSL/TLS** for all external connections
6. **Monitor logs** for authentication failures
7. **Use managed identity** (Azure) or IAM roles (AWS) when possible
8. **Set up alerts** for service failures

## Monitoring

Set up monitoring for:

- Service health (is it running?)
- Reminder send rate (how many per hour?)
- Email delivery failures
- Database connection issues
- SMTP authentication failures

## Support

For production deployment issues:

1. Check application logs for specific errors
2. Verify all environment variables are set
3. Test database connection separately
4. Test SMTP connection separately
5. Review this configuration guide
6. Contact DevOps or development team

---

**Last Updated**: 2026-02-04  
**Status**: Production Ready  
**Service**: AppointmentReminderService v1.0
