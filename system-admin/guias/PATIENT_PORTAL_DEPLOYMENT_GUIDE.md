# Patient Portal - Deployment Guide

## Prerequisites

### Software Requirements
- .NET 8 SDK
- PostgreSQL 14+
- Node.js 18+ (for frontend)
- Angular CLI 20

### Infrastructure Requirements
- PostgreSQL database (same as Omni Care Software main application)
- HTTPS certificate for production
- Environment variable configuration system (Azure Key Vault recommended)

## Database Setup

### 1. Apply Migrations

```bash
cd patient-portal-api

# Apply migrations to create PatientUsers and RefreshTokens tables
dotnet ef database update --project PatientPortal.Infrastructure --startup-project PatientPortal.Api
```

### 2. Create Database Views

**IMPORTANT:** Before running the SQL scripts, verify the schema matches your Omni Care Software database.

```bash
# Review and adjust the SQL script
# Location: PatientPortal.Infrastructure/Migrations/Scripts/CreateViews.sql

# Then apply the script to your database
psql -U postgres -d medicwarehouse -f PatientPortal.Infrastructure/Migrations/Scripts/CreateViews.sql
```

The views provide read-only access to:
- `vw_PatientAppointments` - Patient appointments from the main application
- `vw_PatientDocuments` - Patient documents from the main application

### 3. Verify Database Setup

```sql
-- Check if tables were created
SELECT * FROM "PatientUsers" LIMIT 1;
SELECT * FROM "RefreshTokens" LIMIT 1;

-- Check if views were created
SELECT * FROM vw_PatientAppointments LIMIT 1;
SELECT * FROM vw_PatientDocuments LIMIT 1;
```

## Application Configuration

### 1. Environment Variables (Production)

Create a secure configuration with the following environment variables:

```bash
# JWT Configuration
export JwtSettings__SecretKey="<generate-secure-random-key-min-32-chars>"
export JwtSettings__ExpiryMinutes="15"
export JwtSettings__Issuer="PatientPortal"
export JwtSettings__Audience="PatientPortal-API"

# Database Configuration
export ConnectionStrings__DefaultConnection="Host=<host>;Port=5432;Database=medicwarehouse;Username=<user>;Password=<password>"

# Application Configuration
export ASPNETCORE_ENVIRONMENT="Production"
export ASPNETCORE_URLS="https://+:443;http://+:80"

# CORS Configuration
export Cors__AllowedOrigins__0="https://patientportal.yourdomain.com"
```

### 2. Development Configuration

For development, update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=medicwarehouse;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "SecretKey": "Development-Key-Min-32-Chars-Long!",
    "ExpiryMinutes": 15
  }
}
```

## Running the Application

### Development

```bash
cd PatientPortal.Api

# Run the API
dotnet run

# The API will be available at:
# - HTTP: http://localhost:5000
# - HTTPS: https://localhost:5001
# - Swagger UI: http://localhost:5000 (root)
```

### Production

```bash
# Build the application
dotnet publish -c Release -o ./publish

# Run the application
cd publish
./PatientPortal.Api
```

Or use a process manager like systemd:

```ini
[Unit]
Description=Patient Portal API
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/patient-portal
ExecStart=/usr/bin/dotnet /var/www/patient-portal/PatientPortal.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=patient-portal-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

## Testing the API

### Using Swagger UI

1. Navigate to `http://localhost:5000`
2. Click "Authorize" button
3. Register a new user via `/api/auth/register`
4. Login via `/api/auth/login`
5. Copy the access token
6. Enter token in authorization dialog: `Bearer <token>`
7. Test protected endpoints

### Using cURL

```bash
# Register a new user
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "patient@example.com",
    "cpf": "12345678901",
    "fullName": "John Doe",
    "password": "Password123!",
    "phoneNumber": "+55 11 98765-4321",
    "dateOfBirth": "1990-01-01",
    "patientId": "00000000-0000-0000-0000-000000000001",
    "clinicId": "00000000-0000-0000-0000-000000000001"
  }'

# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrCPF": "patient@example.com",
    "password": "Password123!"
  }'

# Use the access token from login response
TOKEN="<your-access-token>"

# Get appointments
curl -X GET http://localhost:5000/api/appointments \
  -H "Authorization: Bearer $TOKEN"

# Get profile
curl -X GET http://localhost:5000/api/profile/me \
  -H "Authorization: Bearer $TOKEN"
```

## Monitoring and Logging

### Application Insights (Recommended for Azure)

Add to `appsettings.json`:

```json
{
  "ApplicationInsights": {
    "InstrumentationKey": "<your-key>"
  }
}
```

### File Logging

Add Serilog for structured logging:

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

## Security Checklist

Before deploying to production:

- [ ] Configure HTTPS certificate
- [ ] Move secrets to environment variables or Key Vault
- [ ] Configure specific CORS origins
- [ ] Enable HTTPS metadata validation
- [ ] Set up rate limiting
- [ ] Configure security headers
- [ ] Set up monitoring and alerts
- [ ] Configure backup strategy
- [ ] Implement document storage integration
- [ ] Review and test SQL views
- [ ] Set up audit logging
- [ ] Configure proper error handling

## Troubleshooting

### Database Connection Issues

```bash
# Test PostgreSQL connection
psql -h localhost -U postgres -d medicwarehouse

# Check connection string
echo $ConnectionStrings__DefaultConnection
```

### JWT Authentication Issues

```bash
# Verify JWT secret is set
echo $JwtSettings__SecretKey

# Check token expiry
# Tokens expire after 15 minutes by default
```

### Migration Issues

```bash
# List migrations
dotnet ef migrations list --project PatientPortal.Infrastructure --startup-project PatientPortal.Api

# Remove last migration (if needed)
dotnet ef migrations remove --project PatientPortal.Infrastructure --startup-project PatientPortal.Api

# Update database to specific migration
dotnet ef database update <MigrationName> --project PatientPortal.Infrastructure --startup-project PatientPortal.Api
```

## Support

For issues or questions:
- Check the documentation in `patient-portal-api/README.md`
- Review `SECURITY_NOTES.md` for security guidance
- Check `ARCHITECTURE.md` for architecture details
- Consult the team via GitHub Issues

## Next Steps

After successful deployment:

1. **Frontend Development:**
   - Deploy Angular application
   - Configure API endpoint URL
   - Test end-to-end functionality

2. **Integration:**
   - Verify data flow from main application
   - Test appointment synchronization
   - Test document access

3. **User Acceptance Testing:**
   - Invite test users
   - Gather feedback
   - Fix issues

4. **Production Launch:**
   - Monitor application performance
   - Set up alerts
   - Prepare support documentation
