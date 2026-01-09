# PrimeCare Software Microservices Architecture

This directory contains the microservices implementation of PrimeCare Software, migrated from the monolithic architecture.

## Architecture Overview

The application has been decomposed into the following microservices:

| Service | Port | Description |
|---------|------|-------------|
| Auth | 5001 | Authentication, JWT token generation, session management |
| Patients | 5002 | Patient management, patient-clinic links |
| Appointments | 5003 | Scheduling, agenda, check-in, consultation workflow |
| MedicalRecords | 5004 | Medical records, medications, prescriptions, exam requests |
| Billing | 5005 | Subscriptions, invoices, payments, expenses |
| SystemAdmin | 5006 | Multi-tenant management, clinics, system owners |

## Shared Authentication

All microservices use a shared authentication library (`MedicSoft.Shared.Authentication`) that provides:

- JWT token validation configuration
- Common claim constants (`tenant_id`, `clinic_id`, `session_id`, etc.)
- `MicroserviceBaseController` with authentication helpers
- Claims principal extension methods

### Authentication Flow

1. User authenticates via Auth Microservice (`/api/auth/login` or `/api/auth/owner-login`)
2. Auth service generates JWT token with claims: `tenant_id`, `clinic_id`, `session_id`, `role`
3. Frontend stores token and includes it in all requests via HTTP interceptor
4. Each microservice validates the JWT using shared configuration (same secret key)
5. Controllers extend `MicroserviceBaseController` to extract tenant/clinic context from claims

## Building Microservices

### Build Individual Service
```bash
cd microservices/{service-name}
dotnet build
```

### Build All Services
```bash
cd microservices
for dir in auth patients appointments medicalrecords billing systemadmin; do
  cd $dir && dotnet build && cd ..
done
```

## Running with Docker Compose

### Prerequisites
Set the required environment variables:
```bash
export POSTGRES_PASSWORD=your_secure_password
export JWT_SECRET_KEY=your_jwt_secret_key_minimum_32_chars
```

### Start All Services
```bash
docker-compose -f docker-compose.microservices.yml up -d
```

### Service URLs (Local Development)
- Auth API: http://localhost:5001
- Patients API: http://localhost:5002
- Appointments API: http://localhost:5003
- MedicalRecords API: http://localhost:5004
- Billing API: http://localhost:5005
- SystemAdmin API: http://localhost:5006

## Frontend Configuration

The frontend applications support both monolithic and microservices modes:

```typescript
// environment.ts
export const environment = {
  useMicroservices: false, // Set to true to use microservices
  apiUrl: 'http://localhost:5000/api', // Monolithic API
  microservices: {
    auth: 'http://localhost:5001/api',
    patients: 'http://localhost:5002/api',
    appointments: 'http://localhost:5003/api',
    medicalRecords: 'http://localhost:5004/api',
    billing: 'http://localhost:5005/api',
    systemAdmin: 'http://localhost:5006/api'
  }
};
```

Use `ApiConfigService` to get the correct URLs based on the configuration.

## Creating a New Microservice

1. Create a new solution:
   ```bash
   cd microservices
   mkdir new-service && cd new-service
   dotnet new sln -n MedicSoft.NewService
   dotnet new webapi -n MedicSoft.NewService.Api -f net8.0
   dotnet sln add MedicSoft.NewService.Api
   dotnet sln add ../shared/MedicSoft.Shared.Authentication
   ```

2. Add reference to shared authentication:
   ```xml
   <ProjectReference Include="../../shared/MedicSoft.Shared.Authentication/MedicSoft.Shared.Authentication.csproj" />
   ```

3. Configure authentication in Program.cs:
   ```csharp
   builder.Services.AddMicroserviceAuthentication(builder.Configuration);
   
   // In middleware pipeline:
   app.UseAuthentication();
   app.UseAuthorization();
   ```

4. Create controllers extending `MicroserviceBaseController`:
   ```csharp
   [Route("api/[controller]")]
   public class MyController : MicroserviceBaseController
   {
       [HttpGet]
       public async Task<ActionResult> Get()
       {
           var tenantId = GetTenantId();
           var clinicId = GetClinicId();
           // ...
       }
   }
   ```

## Configuration

Each microservice uses the same JWT settings to ensure token compatibility:

```json
{
  "JwtSettings": {
    "SecretKey": "your-shared-secret-key",
    "ExpiryMinutes": 60,
    "Issuer": "PrimeCare Software",
    "Audience": "PrimeCare Software-API"
  }
}
```

**Important**: All microservices must use the same `SecretKey`, `Issuer`, and `Audience` for authentication to work across services.
