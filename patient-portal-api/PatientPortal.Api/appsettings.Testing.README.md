# Testing Environment Configuration

This file (`appsettings.Testing.json`) provides default configuration values for the Testing environment.

## Important Notes

### Credentials in Configuration Files

The credentials in this file are **default values for local testing only** and are intentionally non-sensitive:
- Database password: `postgres_test_password`
- JWT Secret: `TestSecretKey-MinLength32Chars-ForTestingOnly!`

### CI/CD and Docker

In CI/CD pipelines and Docker environments, these values are **overridden by environment variables** set in:
- `docker-compose.test.yml` - Sets proper environment variables
- GitHub Actions workflows - Uses secrets and environment-specific values

Example from `docker-compose.test.yml`:
```yaml
environment:
  - ASPNETCORE_ENVIRONMENT=Testing
  - ConnectionStrings__DefaultConnection=Host=postgres-test;...;Password=postgres_test_password
  - JwtSettings__SecretKey=TestSecretKey-MinLength32Chars-ForTestingOnly!
```

### AppointmentReminder Service

The most important setting in this file is:
```json
"AppointmentReminder": {
  "Enabled": false
}
```

This **disables the AppointmentReminderService** background service during testing because:
1. The test database doesn't have the main application tables (Appointments, Patients, etc.)
2. The service queries these tables from the main MedicWarehouse database
3. Disabling prevents PostgreSQL connection errors during CI/CD tests

## Usage

This configuration is automatically used when:
- Running with `ASPNETCORE_ENVIRONMENT=Testing`
- Using `docker-compose.test.yml`
- In CI/CD pipelines that set the Testing environment

For local development testing:
```bash
ASPNETCORE_ENVIRONMENT=Testing dotnet run
```

## Security

✅ **Safe**: Default values are non-sensitive and meant for testing
✅ **Overridden**: CI/CD and Docker override with environment variables
✅ **Documented**: Purpose and usage clearly documented
