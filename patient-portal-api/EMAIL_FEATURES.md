# Email Verification and Notification Features

This document describes the email verification and notification features implemented in the Patient Portal API.

## Features Implemented

### 1. Email Verification on Registration

When a patient registers a new account, they receive an email with a verification link to confirm their email address.

**Workflow:**
1. Patient registers via `POST /api/auth/register`
2. System creates an email verification token (valid for 24 hours)
3. Verification email is sent with a link to confirm the email
4. Patient clicks the link, which calls `POST /api/auth/verify-email?token={token}`
5. System marks the email as confirmed

**API Endpoints:**
- `POST /api/auth/verify-email?token={token}` - Verify email with token
- `POST /api/auth/resend-verification` - Resend verification email

### 2. Password Reset

Patients can reset their password if they forget it.

**Workflow:**
1. Patient requests password reset via `POST /api/auth/forgot-password`
2. System creates a password reset token (valid for 1 hour)
3. Reset email is sent with a link containing the token
4. Patient clicks the link and enters a new password
5. Patient submits new password via `POST /api/auth/reset-password`
6. System validates token and updates password

**API Endpoints:**
- `POST /api/auth/forgot-password` - Request password reset email
- `POST /api/auth/reset-password` - Reset password with token

### 3. Appointment Confirmation Emails

When a patient books an appointment, they receive an immediate confirmation email.

**Workflow:**
1. Patient books appointment via `POST /api/appointments`
2. System creates appointment and sends confirmation email immediately
3. Email includes appointment details and a link to view the appointment

**Features:**
- HTML email with styled template
- Appointment details (date, time, doctor, clinic)
- Link to view appointment in portal
- Special indication for telehealth appointments

### 4. Appointment Reminder Emails (Pre-existing, Enhanced)

Patients receive reminder emails 24 hours before their appointment.

**Features:**
- Scheduled reminders sent 24 hours before appointment
- Links to confirm, reschedule, or cancel appointment
- Already existed, now consistent with new email templates

## Email Templates

All email templates use a consistent HTML design with:
- Professional styling with PrimeCare branding
- Responsive design for mobile devices
- Clear call-to-action buttons
- Important information highlighted
- Portuguese language content

### Available Templates

1. **Email Verification** - `GenerateEmailVerificationEmail()`
2. **Password Reset** - `GeneratePasswordResetEmail()`
3. **Appointment Confirmation** - `GenerateAppointmentConfirmationEmail()`
4. **Appointment Reminder** - `GenerateAppointmentReminderEmail()` (pre-existing)

## Configuration

### Email Settings

Configure SMTP settings in `appsettings.json`:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "Username": "your-email@example.com",
    "Password": "your-app-password",
    "From": "noreply@primecare.com",
    "FromName": "PrimeCare Portal",
    "SendGridApiKey": ""  // Optional: Use SendGrid instead of SMTP
  },
  "PortalBaseUrl": "https://portal.primecare.com"
}
```

### Gmail Configuration

If using Gmail:
1. Enable 2-factor authentication on your Google account
2. Create an App Password: https://myaccount.google.com/apppasswords
3. Use the app password in the `Password` field

### SendGrid Configuration (Alternative)

If using SendGrid API:
1. Sign up for SendGrid: https://sendgrid.com/
2. Create an API key
3. Add the API key to `SendGridApiKey` in configuration
4. Leave SMTP settings empty (SendGrid takes precedence)

## Database Schema

### EmailVerificationTokens Table

| Column | Type | Description |
|--------|------|-------------|
| Id | Guid | Primary key |
| PatientUserId | Guid | Foreign key to PatientUsers |
| Token | string(512) | Verification token |
| ExpiresAt | DateTime | Token expiration (24 hours from creation) |
| CreatedAt | DateTime | Token creation timestamp |
| IsUsed | bool | Whether token has been used |
| UsedAt | DateTime? | When token was used |

### PasswordResetTokens Table

| Column | Type | Description |
|--------|------|-------------|
| Id | Guid | Primary key |
| PatientUserId | Guid | Foreign key to PatientUsers |
| Token | string(512) | Reset token |
| ExpiresAt | DateTime | Token expiration (1 hour from creation) |
| CreatedAt | DateTime | Token creation timestamp |
| IsUsed | bool | Whether token has been used |
| UsedAt | DateTime? | When token was used |
| CreatedByIp | string(50) | IP address of requestor |

## Security Considerations

1. **Token Security:**
   - Tokens are cryptographically secure (32 random bytes, Base64 encoded)
   - Tokens are single-use only
   - Tokens have short expiration times (1-24 hours)
   - All active tokens are revoked after successful use

2. **Email Enumeration Protection:**
   - Password reset always returns success, even if email doesn't exist
   - Prevents attackers from discovering valid email addresses

3. **Rate Limiting:**
   - Consider implementing rate limiting on email endpoints
   - Prevents abuse and spam

4. **HTTPS Only:**
   - All email links must use HTTPS
   - Tokens should never be sent over unencrypted connections

## Testing

### Unit Tests

Run the email-related unit tests:

```bash
dotnet test --filter "FullyQualifiedName~EmailTests"
dotnet test --filter "FullyQualifiedName~EmailTemplateHelperTests"
```

### Manual Testing

1. **Email Verification:**
   ```bash
   # Register a new user
   POST /api/auth/register
   {
     "email": "test@example.com",
     "cpf": "12345678901",
     "fullName": "Test User",
     "password": "Password123!",
     "confirmPassword": "Password123!",
     "phoneNumber": "+5511999999999",
     "dateOfBirth": "1990-01-01"
   }
   
   # Check email for verification link
   # Click link or call:
   POST /api/auth/verify-email?token={token-from-email}
   ```

2. **Password Reset:**
   ```bash
   # Request reset
   POST /api/auth/forgot-password
   {
     "email": "test@example.com"
   }
   
   # Check email for reset link
   # Submit new password:
   POST /api/auth/reset-password
   {
     "token": "{token-from-email}",
     "newPassword": "NewPassword123!"
   }
   ```

3. **Appointment Confirmation:**
   ```bash
   # Book an appointment (requires authentication)
   POST /api/appointments
   {
     "clinicId": "{guid}",
     "doctorId": "{guid}",
     "appointmentDate": "2026-02-01",
     "appointmentTime": "14:30:00",
     "durationMinutes": 30,
     "appointmentType": "Consulta",
     "appointmentMode": "InPerson",
     "paymentType": "HealthInsurance"
   }
   
   # Check email for confirmation
   ```

## Troubleshooting

### Emails Not Being Sent

1. **Check SMTP Configuration:**
   - Verify SMTP server, port, and credentials
   - Test SMTP connection separately
   - Check firewall settings

2. **Check Logs:**
   ```bash
   # Look for email sending errors in application logs
   grep "Failed to send.*email" logs/*.log
   ```

3. **Check Email Service:**
   - Ensure NotificationService is registered in DI container
   - Verify configuration is being loaded correctly

### Token Invalid or Expired

1. **Check Token Expiration:**
   - Email verification: 24 hours
   - Password reset: 1 hour
   - Tokens are single-use only

2. **Request New Token:**
   - Resend verification email
   - Request new password reset

## API Documentation

Full API documentation is available via Swagger when running the application:

```
http://localhost:5000/swagger
```

The Swagger UI includes detailed information about all authentication endpoints, including:
- Request/response schemas
- Example requests
- Security requirements
- Error codes

## Future Enhancements

Possible future improvements:
1. Email verification requirement before login (currently optional)
2. Email templates in multiple languages
3. SMS verification as an alternative
4. Customizable email templates via admin panel
5. Email delivery tracking and analytics
6. Bulk email operations for clinic communications
