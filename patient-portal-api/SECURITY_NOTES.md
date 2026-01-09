# Security Configuration Notes

## ⚠️ Important Security Considerations

### 1. Configuration Files

#### JWT Secret Key
- **Current:** Hardcoded in `appsettings.json`
- **Production:** MUST use environment variables or Azure Key Vault
- **Action Required:** Set `JwtSettings__SecretKey` environment variable

```bash
# Example for production
export JwtSettings__SecretKey="your-super-secret-key-min-32-chars"
```

#### Database Connection String
- **Current:** Hardcoded credentials in `appsettings.json`
- **Production:** MUST use environment variables or Azure Key Vault
- **Action Required:** Set `ConnectionStrings__DefaultConnection` environment variable

```bash
# Example for production
export ConnectionStrings__DefaultConnection="Host=prod-server;Port=5432;Database=medicwarehouse;Username=user;Password=pass"
```

### 2. HTTPS Configuration

#### RequireHttpsMetadata
- **Current:** Disabled in `Program.cs` for development
- **Production:** MUST be enabled
- **Action Required:** Set environment-specific configuration

```csharp
// In Program.cs, make this environment-specific:
options.RequireHttpsMetadata = !app.Environment.IsDevelopment();
```

### 3. File Storage

#### Document Download
- **Current:** Placeholder implementation (returns null)
- **Production:** MUST implement actual file storage integration
- **Options:**
  - Azure Blob Storage
  - AWS S3
  - Local filesystem with proper security
- **Action Required:** Implement DocumentService.DownloadDocumentAsync

### 4. Database Views

#### SQL View Scripts
- **Current:** Template views with placeholder schema
- **Production:** MUST verify and adjust to match actual PrimeCare Software schema
- **Location:** `PatientPortal.Infrastructure/Migrations/Scripts/CreateViews.sql`
- **Action Required:** Review and adjust column names, table relationships, and data types

### 5. CORS Configuration

#### AllowAll Policy
- **Current:** Allows all origins for development
- **Production:** MUST restrict to specific domains
- **Action Required:** Configure specific allowed origins in production

```csharp
// Production example
policy.WithOrigins("https://patientportal.medicwarehouse.com")
      .AllowAnyMethod()
      .AllowAnyHeader();
```

## Production Checklist

Before deploying to production:

- [ ] Move JWT secret to environment variables or Key Vault
- [ ] Move database credentials to environment variables or Key Vault
- [ ] Enable HTTPS metadata validation
- [ ] Implement document download functionality
- [ ] Verify and adjust database view schemas
- [ ] Configure specific CORS origins
- [ ] Set up proper logging and monitoring
- [ ] Configure rate limiting
- [ ] Set up security headers
- [ ] Enable HTTPS redirection
- [ ] Configure proper error handling (don't expose stack traces)
- [ ] Set up audit logging for security events
- [ ] Configure proper backup strategy
- [ ] Set up intrusion detection

## Environment Variables

Recommended environment variables for production:

```bash
# JWT Configuration
JwtSettings__SecretKey="<generate-secure-random-key-min-32-chars>"
JwtSettings__ExpiryMinutes="15"
JwtSettings__Issuer="PatientPortal"
JwtSettings__Audience="PatientPortal-API"

# Database Configuration
ConnectionStrings__DefaultConnection="Host=<host>;Port=5432;Database=medicwarehouse;Username=<user>;Password=<password>"

# Application Configuration
ASPNETCORE_ENVIRONMENT="Production"
ASPNETCORE_URLS="https://+:443;http://+:80"

# CORS Configuration
Cors__AllowedOrigins__0="https://patientportal.medicwarehouse.com"
```

## Additional Security Measures

### Recommended for Production

1. **API Rate Limiting:** Already configured in appsettings.json - verify limits
2. **Input Validation:** Implement comprehensive validation for all endpoints
3. **SQL Injection Protection:** Using EF Core parameterized queries (already implemented)
4. **XSS Protection:** Configure security headers
5. **CSRF Protection:** Implement anti-forgery tokens for state-changing operations
6. **Audit Logging:** Log all authentication attempts and sensitive operations
7. **Account Security:** Implement 2FA (already prepared in domain model)
8. **Password Policy:** Enforce strong password requirements
9. **Session Management:** Implement proper session timeout and renewal
10. **Monitoring:** Set up alerts for suspicious activities
