# Owner Dashboard and Permission System - Implementation Summary

## Overview

This implementation adds comprehensive owner management capabilities and a granular permission system to the PrimeCare Software platform, addressing the following requirements:

1. **Owner Screen**: Allow clinic owners to manage users, clients, and create users with various profiles
2. **Permission System**: Implement page and flow logic permissions to prevent unauthorized access
3. **Manual Override**: Allow keeping clinics active even if payment is overdue or not registered via website
4. **Environment Control**: Disable payment enforcement in development/staging environments

## Key Features

### 1. Manual Subscription Override

**Purpose**: Give system owners the ability to provide free access to friend doctors or special cases.

**Implementation**:
- New fields in `ClinicSubscription` entity:
  - `ManualOverrideActive`: Boolean flag
  - `ManualOverrideReason`: Documentation of why override was enabled
  - `ManualOverrideSetAt`: Timestamp of activation
  - `ManualOverrideSetBy`: Username who activated it

**Usage**:
```csharp
// Enable manual override
subscription.EnableManualOverride("Free access for friend doctor", "admin@system.com");

// Check access (will return true even if payment overdue)
bool canAccess = subscription.CanAccessWithOverride();

// Disable override
subscription.DisableManualOverride();
```

### 2. Environment-Based Payment Enforcement

**Development/Staging**: No payment enforcement, unlimited test clinics
**Production**: Normal subscription rules apply, manual overrides available

**Implementation**:
```csharp
// SubscriptionService now considers environment
public bool CanAccessSystem(ClinicSubscription subscription, string environment)
{
    // Dev/Staging always allow access
    if (environment is "Development" or "Staging" or "Homologacao")
        return true;
    
    // Check manual override
    if (subscription.ManualOverrideActive)
        return true;
    
    // Normal rules for production
    return subscription.IsActive();
}
```

### 3. Granular Permission System

**Implementation**: `RequirePermissionAttribute` for role-based authorization

**Permission Matrix**:

| Permission | SystemAdmin | ClinicOwner | Doctor/Dentist | Nurse | Secretary | Receptionist |
|-----------|------------|-------------|----------------|-------|-----------|--------------|
| ManageUsers | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| ManageMedicalRecords | ✅ | ✅ | ✅ | ✅ | ❌ | ❌ |
| ManagePatients | ✅ | ✅ | ✅ | ❌ | ✅ | ✅ |
| ManagePayments | ✅ | ✅ | ❌ | ❌ | ✅ | ❌ |
| ViewSystemAnalytics | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |

**Usage Examples**:
```csharp
// Protect medical records from secretaries
[HttpPost]
[RequirePermission(Permission.ManageMedicalRecords)]
public async Task<ActionResult> CreateMedicalRecord(...)

// Only owners can manage users
[HttpPost]
[RequirePermission(Permission.ManageUsers)]
public async Task<ActionResult> CreateUser(...)
```

### 4. Enhanced JWT Token

**Added `clinic_id` claim** for better authorization:

```json
{
  "token": "...",
  "username": "doctor@clinic.com",
  "role": "Doctor",
  "clinicId": "guid-here",
  "tenantId": "tenant-id"
}
```

### 5. System Admin Endpoints

New endpoints for system-wide management:

```
POST /api/system-admin/clinics/{id}/subscription/manual-override/enable
POST /api/system-admin/clinics/{id}/subscription/manual-override/disable
GET  /api/system-admin/clinics
GET  /api/system-admin/clinics/{id}
GET  /api/system-admin/analytics
POST /api/system-admin/users (create system admins)
```

## Database Changes

### Migration: 20251012195249_AddOwnerEntity

```sql
ALTER TABLE ClinicSubscriptions
ADD ManualOverrideActive bit NOT NULL DEFAULT 0,
    ManualOverrideReason nvarchar(500) NULL,
    ManualOverrideSetAt datetime2 NULL,
    ManualOverrideSetBy nvarchar(100) NULL;
```

## Testing

**Total Tests**: 692 (all passing ✅)
**New Tests**: 23

### Test Coverage:

1. **Manual Override Tests** (12 tests)
   - Enable/disable override
   - Field validation
   - Access control with override
   - Different subscription states

2. **Environment Tests** (11 tests)
   - Development environment behavior
   - Staging environment behavior
   - Production environment with/without override
   - Case-insensitive environment names

### Run Tests:

```bash
# All tests
dotnet test

# Manual override tests only
dotnet test --filter "FullyQualifiedName~ManualOverride"

# Environment tests only
dotnet test --filter "FullyQualifiedName~SubscriptionServiceEnvironment"
```

## Use Cases

### Use Case 1: Clinic Owner Creates Secretary

```typescript
// Secretary can schedule appointments and manage payments
// but CANNOT edit medical records or prescriptions
const response = await fetch('/api/users', {
  method: 'POST',
  headers: { 'Authorization': `Bearer ${token}` },
  body: JSON.stringify({
    role: 'Secretary',
    username: 'secretary',
    // ... other fields
  })
});
```

**Result**: Secretary user created with limited permissions:
- ✅ Can manage appointments
- ✅ Can manage payments
- ❌ Cannot edit medical records
- ❌ Cannot access prescriptions

### Use Case 2: Free Access for Friend Doctor

```bash
# System admin provides free access
curl -X POST /api/system-admin/clinics/{id}/subscription/manual-override/enable \
  -H "Authorization: Bearer {token}" \
  -d '{"reason": "Free access for Dr. John, personal friend"}'
```

**Result**: Clinic has full access even without payment.

### Use Case 3: Testing in Development

```bash
# Set environment
export ASPNETCORE_ENVIRONMENT=Development

# Create unlimited test clinics
# No payment enforcement
# All clinics have access
```

## Security Considerations

1. **Audit Trail**: All manual overrides are logged with reason, timestamp, and user
2. **Role-Based Access**: Only SystemAdmin can enable manual overrides
3. **Tenant Isolation**: Users can only access their own clinic (except SystemAdmin)
4. **Permission Validation**: All sensitive operations require specific permissions

## API Examples

### Enable Manual Override

```http
POST /api/system-admin/clinics/{clinicId}/subscription/manual-override/enable
Authorization: Bearer {systemAdminToken}
Content-Type: application/json

{
  "reason": "Free access for friend doctor"
}
```

**Response**:
```json
{
  "message": "Override manual ativado com sucesso",
  "reason": "Free access for friend doctor",
  "setBy": "admin@system.com",
  "setAt": "2025-10-12T03:15:00Z"
}
```

### Get Subscription Status

```http
GET /api/subscriptions/current
Authorization: Bearer {token}
```

**Response**:
```json
{
  "id": "guid",
  "clinicId": "guid",
  "status": "PaymentOverdue",
  "canAccess": true,
  "manualOverrideActive": true,
  "manualOverrideReason": "Free access for friend doctor"
}
```

## Files Changed

### Domain Layer
- `src/MedicSoft.Domain/Entities/ClinicSubscription.cs` - Added manual override properties and methods
- `src/MedicSoft.Domain/Services/SubscriptionService.cs` - Added environment-aware access checking

### API Layer
- `src/MedicSoft.Api/Controllers/AuthController.cs` - Added clinic_id to JWT
- `src/MedicSoft.Api/Controllers/SubscriptionsController.cs` - Added manual override info to DTOs
- `src/MedicSoft.Api/Controllers/SystemAdminController.cs` - Added override management endpoints
- `src/MedicSoft.Api/Controllers/UsersController.cs` - Added permission attributes
- `src/MedicSoft.Api/Controllers/MedicalRecordsController.cs` - Protected sensitive endpoints
- `src/MedicSoft.Api/Program.cs` - Updated service registration

### Cross-Cutting
- `src/MedicSoft.CrossCutting/Authorization/RequirePermissionAttribute.cs` - New permission filter
- `src/MedicSoft.CrossCutting/MedicSoft.CrossCutting.csproj` - Added MVC package reference

### Repository Layer
- `src/MedicSoft.Repository/Configurations/ClinicSubscriptionConfiguration.cs` - Updated EF configuration
- `src/MedicSoft.Repository/Migrations/20251012195249_AddOwnerEntity.cs` - New migration

### Tests
- `tests/MedicSoft.Test/Entities/ClinicSubscriptionManualOverrideTests.cs` - 12 new tests
- `tests/MedicSoft.Test/Services/SubscriptionServiceEnvironmentTests.cs` - 11 new tests

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────┐
│                    Frontend/Client                       │
│              (Angular, React, etc.)                      │
└────────────────────┬────────────────────────────────────┘
                     │ JWT Token (includes clinic_id)
                     ▼
┌─────────────────────────────────────────────────────────┐
│                   API Controllers                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │  [RequirePermission(Permission.X)]                │  │
│  │  Authorization Filter validates role              │  │
│  └──────────────────────────────────────────────────┘  │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│              Subscription Service                        │
│  • Checks environment (Dev/Staging/Production)          │
│  • Checks manual override status                        │
│  • Applies business rules                               │
└────────────────────┬────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────┐
│                   Database                               │
│  ClinicSubscriptions table with override fields         │
└─────────────────────────────────────────────────────────┘
```

## Migration Guide

### For Existing Installations

1. **Backup Database**
   ```bash
   # Create backup before migration
   ```

2. **Apply Migration**
   ```bash
   dotnet ef database update
   ```

3. **Verify Migration**
   ```sql
   SELECT ManualOverrideActive, ManualOverrideReason 
   FROM ClinicSubscriptions;
   ```

4. **Update Environment Variables**
   ```bash
   # Set appropriate environment
   ASPNETCORE_ENVIRONMENT=Production
   ```

### For New Installations

Migration will run automatically on first startup.

## Future Enhancements

1. **Frontend Dashboard**
   - Owner management screen
   - User permission matrix visualization
   - Manual override management UI

2. **Advanced Permissions**
   - Time-based permissions
   - IP-based restrictions
   - Audit log viewer

3. **Notifications**
   - Email when override is enabled
   - Alert for permission changes
   - Subscription status notifications

## References

- Main Documentation: [OWNER_DASHBOARD_PERMISSIONS.md](OWNER_DASHBOARD_PERMISSIONS.md) (Portuguese)
- Subscription System: [frontend/mw-docs/src/assets/docs/SUBSCRIPTION_SYSTEM.md](frontend/mw-docs/src/assets/docs/SUBSCRIPTION_SYSTEM.md)
- Security Guide: [SECURITY_GUIDE.md](SECURITY_GUIDE.md)

## Support

- **Issues**: https://github.com/PrimeCare Software/MW.Code/issues
- **Docs**: https://docs.medicwarehouse.com
- **Email**: contato@primecaresoftware.com

---

**Version**: 2.0.0  
**Date**: 2025-10-12  
**Author**: GitHub Copilot  
**Co-authored-by**: igorleessa <13488628+igorleessa@users.noreply.github.com>
