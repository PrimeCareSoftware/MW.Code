# Quick Reference: Owner Permissions & Manual Override

## For Developers

### Using Permission Attributes

```csharp
// Protect endpoint - only specific roles can access
[RequirePermission(Permission.ManageMedicalRecords)]
public async Task<ActionResult> EditMedicalRecord(...)
{
    // Only Doctor, Dentist, Nurse, ClinicOwner can access
    // Secretary CANNOT access this endpoint
}

[RequirePermission(Permission.ManageUsers)]
public async Task<ActionResult> CreateUser(...)
{
    // Only ClinicOwner and SystemAdmin can access
}
```

### Available Permissions

```csharp
public enum Permission
{
    // System-level
    ViewAllClinics,          // SystemAdmin only
    ManageSubscriptions,     // SystemAdmin only
    ViewSystemAnalytics,     // SystemAdmin only
    ManagePlans,            // SystemAdmin only
    CrossTenantAccess,      // SystemAdmin only
    
    // Clinic-level
    ManageClinic,           // ClinicOwner, SystemAdmin
    ManageUsers,            // ClinicOwner, SystemAdmin
    ManageSubscription,     // ClinicOwner, SystemAdmin
    
    // Operations
    ViewPatients,           // Most roles
    ManagePatients,         // Doctor, Dentist, Secretary, Receptionist, ClinicOwner
    ViewAppointments,       // Most roles
    ManageAppointments,     // Doctor, Dentist, Secretary, Receptionist, ClinicOwner
    ViewMedicalRecords,     // Doctor, Dentist, Nurse, ClinicOwner
    ManageMedicalRecords,   // Doctor, Dentist, Nurse, ClinicOwner (NOT Secretary)
    ViewReports,            // ClinicOwner, SystemAdmin
    ManagePayments          // Secretary, ClinicOwner, SystemAdmin
}
```

### Manual Override Usage

```csharp
// Enable manual override (SystemAdmin only)
subscription.EnableManualOverride(
    reason: "Free access for friend doctor",
    setByUsername: "admin@system.com"
);

// Check if subscription allows access
bool canAccess = subscription.CanAccessWithOverride();

// Disable override
subscription.DisableManualOverride();
```

### Environment-Based Access

```csharp
// Inject IConfiguration and ISubscriptionService
private readonly IConfiguration _configuration;
private readonly ISubscriptionService _subscriptionService;

// Check access considering environment
var environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
bool canAccess = _subscriptionService.CanAccessSystem(subscription, environment);

// Development/Staging: Always returns true
// Production: Checks payment status and manual override
```

### JWT Token with Clinic ID

```csharp
// AuthController - Include clinic_id in token
var token = GenerateJwtToken(
    username: user.Username,
    tenantId: tenantId,
    userId: user.Id.ToString(),
    role: user.Role.ToString(),
    clinicId: user.ClinicId?.ToString() // Now included!
);

// Get clinic_id from token in controllers
var clinicIdClaim = User.FindFirst("clinic_id")?.Value;
var clinicId = Guid.Parse(clinicIdClaim);
```

## For API Consumers

### Enable Manual Override

```bash
curl -X POST https://api.medicwarehouse.com/api/system-admin/clinics/{clinicId}/subscription/manual-override/enable \
  -H "Authorization: Bearer {systemAdminToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "reason": "Free access for friend doctor"
  }'
```

### Disable Manual Override

```bash
curl -X POST https://api.medicwarehouse.com/api/system-admin/clinics/{clinicId}/subscription/manual-override/disable \
  -H "Authorization: Bearer {systemAdminToken}"
```

### Check Subscription Status

```bash
curl https://api.medicwarehouse.com/api/subscriptions/current \
  -H "Authorization: Bearer {token}"
```

Response:
```json
{
  "canAccess": true,
  "manualOverrideActive": true,
  "manualOverrideReason": "Free access for friend doctor",
  "status": "PaymentOverdue"
}
```

### Create User (Owner Only)

```bash
curl -X POST https://api.medicwarehouse.com/api/users \
  -H "Authorization: Bearer {clinicOwnerToken}" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "secretary",
    "email": "secretary@clinic.com",
    "password": "SecurePass123!",
    "fullName": "Maria Silva",
    "phone": "+5511999999999",
    "role": "Secretary"
  }'
```

## Environment Configuration

### Development/Staging (No Payment Enforcement)

```bash
# appsettings.Development.json or environment variable
ASPNETCORE_ENVIRONMENT=Development

# OR
ASPNETCORE_ENVIRONMENT=Staging

# OR
ASPNETCORE_ENVIRONMENT=Homologacao
```

**Result**: All clinics have free access, no payment checking

### Production (Normal Payment Rules)

```bash
ASPNETCORE_ENVIRONMENT=Production
```

**Result**: Payment rules enforced, manual overrides available

## Permission Matrix Quick View

| Role | Manage Users | Edit Medical Records | Manage Payments | System Analytics |
|------|--------------|---------------------|-----------------|------------------|
| **SystemAdmin** | ✅ | ✅ | ✅ | ✅ |
| **ClinicOwner** | ✅ | ✅ | ✅ | ❌ |
| **Doctor/Dentist** | ❌ | ✅ | ❌ | ❌ |
| **Nurse** | ❌ | ✅ (limited) | ❌ | ❌ |
| **Secretary** | ❌ | ❌ | ✅ | ❌ |
| **Receptionist** | ❌ | ❌ | ❌ | ❌ |

## Testing

```bash
# Run all tests
dotnet test

# Test manual override functionality
dotnet test --filter "ManualOverride"

# Test environment-based access
dotnet test --filter "SubscriptionServiceEnvironment"

# Test permissions
dotnet test --filter "Permission"
```

## Common Scenarios

### Scenario 1: Prevent Secretary from Editing Medical Records

```csharp
[HttpPut("{id}")]
[RequirePermission(Permission.ManageMedicalRecords)]
public async Task<ActionResult> UpdateMedicalRecord(...)
{
    // Secretary role does NOT have ManageMedicalRecords permission
    // Will return 403 Forbidden
}
```

### Scenario 2: Allow Owner to Manage Users

```csharp
[HttpPost]
[RequirePermission(Permission.ManageUsers)]
public async Task<ActionResult> CreateUser(...)
{
    // ClinicOwner has ManageUsers permission
    // Will succeed
}
```

### Scenario 3: Free Access for Test Clinic in Development

```csharp
// In Development environment
var canAccess = subscriptionService.CanAccessSystem(subscription, "Development");
// Always returns true, regardless of payment status
```

### Scenario 4: Free Access for Friend Doctor in Production

```csharp
// SystemAdmin enables manual override
subscription.EnableManualOverride("Friend doctor", "admin");

// Now clinic has access even with overdue payment
var canAccess = subscriptionService.CanAccessSystem(subscription, "Production");
// Returns true because of manual override
```

## Database Queries

### Check All Manual Overrides

```sql
SELECT 
    c.Name AS ClinicName,
    cs.ManualOverrideActive,
    cs.ManualOverrideReason,
    cs.ManualOverrideSetBy,
    cs.ManualOverrideSetAt,
    cs.Status
FROM ClinicSubscriptions cs
JOIN Clinics c ON cs.ClinicId = c.Id
WHERE cs.ManualOverrideActive = 1
ORDER BY cs.ManualOverrideSetAt DESC;
```

### Count Subscriptions by Environment Behavior

```sql
-- Production behavior
SELECT 
    Status,
    COUNT(*) AS Count,
    SUM(CASE WHEN ManualOverrideActive = 1 THEN 1 ELSE 0 END) AS WithOverride
FROM ClinicSubscriptions
GROUP BY Status;
```

## Troubleshooting

### Issue: User Cannot Access Endpoint

**Check**:
1. Does user have correct role?
2. Does role have required permission?
3. Is JWT token valid and includes correct claims?

```csharp
// Debug permission check
var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
var user = await _userRepository.GetByIdAsync(userId);
var hasPermission = user.HasPermission(Permission.ManageMedicalRecords);
```

### Issue: Clinic Blocked Despite Manual Override

**Check**:
1. Is manual override actually active in database?
2. Is environment variable set correctly?
3. Is SubscriptionService using correct environment?

```sql
SELECT ManualOverrideActive, Status 
FROM ClinicSubscriptions 
WHERE ClinicId = '{clinicId}';
```

### Issue: Development Environment Not Allowing Access

**Check**:
1. Environment variable: `ASPNETCORE_ENVIRONMENT=Development`
2. Service configuration in Program.cs
3. Correct environment passed to CanAccessSystem

```csharp
// Verify environment
var environment = _configuration["ASPNETCORE_ENVIRONMENT"];
Console.WriteLine($"Current environment: {environment}");
```

## Best Practices

1. **Always use RequirePermission**: Don't check permissions manually in code
2. **Document override reasons**: Always provide clear reason for manual overrides
3. **Test in all environments**: Verify behavior in Dev, Staging, and Production
4. **Audit regularly**: Check manual overrides and remove when no longer needed
5. **Minimal permissions**: Grant only the permissions each role needs

## Links

- Full Documentation: [OWNER_DASHBOARD_PERMISSIONS.md](OWNER_DASHBOARD_PERMISSIONS.md)
- Implementation Details: [IMPLEMENTATION_OWNER_PERMISSIONS.md](IMPLEMENTATION_OWNER_PERMISSIONS.md)
- API Guide: [frontend/mw-docs/src/assets/docs/SUBSCRIPTION_SYSTEM.md](frontend/mw-docs/src/assets/docs/SUBSCRIPTION_SYSTEM.md)
