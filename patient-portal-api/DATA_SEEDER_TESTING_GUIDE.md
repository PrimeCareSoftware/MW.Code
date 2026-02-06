# Testing Guide: Patient Portal Data Seeder

This guide provides instructions for testing the Patient Portal data seeder functionality.

## Prerequisites

Before testing the data seeder, ensure:

1. **Main Database is Seeded**
   - The main application must have demo data
   - Run the main API seeder first:
     ```bash
     curl -X POST http://localhost:5000/api/data-seeder/seed-demo
     ```
   - This creates demo clinic (demo-clinic-001) with patients

2. **Patient Portal API is Running**
   ```bash
   cd patient-portal-api/PatientPortal.Api
   dotnet run
   ```
   - Default port: 5001 (HTTPS) or 5000 (HTTP)

3. **Database Connection**
   - Both APIs use the same PostgreSQL database
   - Connection string in `appsettings.Development.json`

## Testing Workflow

### 1. Verify Main Database Has Demo Data

Check that the main database has patients:

```bash
# Using the main API
curl -X GET http://localhost:5000/api/data-seeder/demo-info
```

Expected response should show patients in demo-clinic-001.

### 2. Seed Patient Portal Data

Create patient portal users from existing patients:

```bash
curl -X POST http://localhost:5001/api/data-seeder/seed-demo
```

**Expected Response:**
```json
{
  "message": "Demo data seeded successfully for Patient Portal",
  "tenantId": "demo-clinic-001",
  "credentials": {
    "note": "Use these credentials to login to the patient portal",
    "password": "Patient@123",
    "loginEndpoint": "POST /api/auth/login",
    "users": "All patients from demo clinic can login using their email or CPF and the password above"
  },
  "summary": {
    "patientUsers": "Created from existing patients in main database",
    "emailConfirmed": true,
    "twoFactorEnabled": false
  },
  "nextSteps": [
    "1. Use GET /api/data-seeder/demo-info to see all available patient emails",
    "2. Login with any patient email or CPF using password: Patient@123",
    "3. Test the patient portal features"
  ]
}
```

### 3. Get Demo Information

View all seeded patient users:

```bash
curl -X GET http://localhost:5001/api/data-seeder/demo-info
```

**Expected Response:**
```json
{
  "tenantId": "demo-clinic-001",
  "totalUsers": 6,
  "loginCredentials": {
    "password": "Patient@123",
    "note": "Use any patient email or CPF with this password",
    "endpoint": "POST /api/auth/login"
  },
  "patients": [
    {
      "email": "joao.silva@example.com",
      "cpf": "12345678900",
      "fullName": "João Silva",
      "emailConfirmed": true,
      "twoFactorEnabled": false
    }
    // ... more patients
  ],
  "availableEndpoints": [
    "POST /api/auth/login - Login with email/CPF and password",
    "GET /api/appointments - View patient appointments",
    "GET /api/documents - View patient documents",
    "GET /api/profile - View patient profile",
    "PUT /api/profile - Update patient profile"
  ]
}
```

### 4. Test Login with Seeded User

Login using a patient email from the demo-info response:

```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrCPF": "joao.silva@example.com",
    "password": "Patient@123"
  }'
```

**Expected Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "...",
  "expiresAt": "2026-02-06T03:30:00Z",
  "user": {
    "id": "...",
    "email": "joao.silva@example.com",
    "fullName": "João Silva",
    "cpf": "12345678900",
    "phoneNumber": "11999999999",
    "dateOfBirth": "1990-01-01",
    "twoFactorEnabled": false
  }
}
```

### 5. Test Protected Endpoints

Use the access token to call protected endpoints:

```bash
# Get appointments
curl -X GET http://localhost:5001/api/appointments \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"

# Get documents
curl -X GET http://localhost:5001/api/documents \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"

# Get profile
curl -X GET http://localhost:5001/api/profile \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

### 6. Clear Database (Optional)

Remove all patient portal data:

```bash
curl -X DELETE http://localhost:5001/api/data-seeder/clear-database
```

**Expected Response:**
```json
{
  "message": "Patient Portal database cleared successfully",
  "deletedTables": [
    "TwoFactorTokens",
    "PasswordResetTokens",
    "EmailVerificationTokens",
    "RefreshTokens",
    "PatientUsers"
  ],
  "note": "All patient portal data has been removed. You can now re-seed the database using POST /api/data-seeder/seed-demo"
}
```

## Testing via Swagger UI

1. Navigate to: `http://localhost:5001` (Swagger UI is at root)
2. Find the **DataSeeder** section
3. Test each endpoint:
   - `POST /api/data-seeder/seed-demo`
   - `GET /api/data-seeder/demo-info`
   - `DELETE /api/data-seeder/clear-database`

## Error Scenarios

### Data Already Exists
```bash
# Trying to seed when data exists
curl -X POST http://localhost:5001/api/data-seeder/seed-demo
```

**Expected Error:**
```json
{
  "error": "Demo data already exists. Clear the database first using DELETE /api/data-seeder/clear-database"
}
```

**Solution:** Clear database first, then seed again.

### No Patients in Main Database
```bash
# When main database has no demo data
curl -X POST http://localhost:5001/api/data-seeder/seed-demo
```

**Expected Error:**
```json
{
  "error": "No patients found in main database. Please seed the main application first."
}
```

**Solution:** Seed the main application first using its data seeder.

### Production Environment
```bash
# Trying to use endpoints in production
curl -X POST https://production-url/api/data-seeder/seed-demo
```

**Expected Error:**
```json
{
  "error": "This endpoint is only available in Development environment or when Development:EnableDevEndpoints is true"
}
```

## Validation Checklist

- [ ] Main database seeded with demo-clinic-001
- [ ] Patient portal data seeded successfully
- [ ] Demo info returns list of patient users
- [ ] Can login with patient email and password "Patient@123"
- [ ] Can login with patient CPF and password "Patient@123"
- [ ] Access token works for protected endpoints
- [ ] Can view appointments for logged-in patient
- [ ] Can view documents for logged-in patient
- [ ] Can view and update profile
- [ ] Database can be cleared successfully
- [ ] Can re-seed after clearing
- [ ] Endpoints are protected in production environment

## Database Verification

You can verify the data directly in PostgreSQL:

```sql
-- Check patient users
SELECT 
    "Id",
    "Email",
    "CPF",
    "FullName",
    "EmailConfirmed",
    "TwoFactorEnabled",
    "CreatedAt"
FROM "PatientUsers"
ORDER BY "CreatedAt" DESC;

-- Check relationship to main patients
SELECT 
    pu."Email",
    pu."FullName",
    p."Name" as "MainPatientName"
FROM "PatientUsers" pu
JOIN "Patients" p ON pu."PatientId" = p."Id"
WHERE pu."ClinicId"::text = 'demo-clinic-001';
```

## Notes

- **Default Password**: All seeded patient users have password `Patient@123`
- **Email Confirmed**: All users are pre-confirmed (no email verification needed)
- **2FA Disabled**: Two-factor authentication is disabled by default for demo users
- **Clinic ID**: All users belong to demo-clinic-001
- **Patient Limit**: Seeder fetches up to 10 most recent patients from main database

## Troubleshooting

### Connection Issues
- Verify PostgreSQL is running: `pg_isready -h localhost -p 5432`
- Check connection string in `appsettings.Development.json`

### Build Issues
```bash
cd patient-portal-api
dotnet build
```

### Migration Issues
```bash
cd PatientPortal.Api
dotnet ef database update
```

### Port Conflicts
- Check if port 5001 is available
- Modify `launchSettings.json` to use different port

## Integration Testing

For automated testing, see:
- `PatientPortal.Tests/Controllers/AuthControllerTests.cs`
- `PatientPortal.Tests/Services/AuthServiceTests.cs`

Run tests:
```bash
cd patient-portal-api
dotnet test
```
