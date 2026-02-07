# Fix Summary: Module-Config "Invalid subscription plan" Error

## Problem Statement
When accessing the module configuration screen at `http://localhost:5000/api/module-config`, users were receiving:
```json
{
    "message": "Invalid subscription plan"
}
```
HTTP Status: 400 Bad Request

## Root Cause Analysis

### The Issue
1. Subscription plans (`SubscriptionPlan` entity) are **system-wide entities** created with `tenantId="system"`
2. They are created once during data seeding and shared across all tenants
3. When clinics subscribe to a plan, they store the plan's ID in their `ClinicSubscription` record

### The Bug
Multiple controllers and services were trying to retrieve subscription plans using:
```csharp
var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, tenantId);
```

Where `tenantId` was the clinic's tenant ID, not "system".

Since `BaseRepository.GetByIdAsync()` filters by tenant:
```csharp
public virtual async Task<T?> GetByIdAsync(Guid id, string tenantId)
{
    return await _dbSet
        .Where(e => e.Id == id && e.TenantId == tenantId)  // ← Filters by tenantId
        .FirstOrDefaultAsync();
}
```

The query would look for a plan with:
- `Id = subscription.SubscriptionPlanId`
- `TenantId = clinicTenantId` (e.g., "demo-clinic-001")

But the actual plan has:
- `Id = subscription.SubscriptionPlanId`
- `TenantId = "system"`

Result: Plan not found → "Invalid subscription plan" error.

## Solution

Changed all subscription plan lookups to use the tenant-agnostic method:
```csharp
// Before (WRONG - uses clinic's tenantId)
var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, tenantId);

// After (CORRECT - no tenant filtering)
var plan = await _planRepository.GetByIdWithoutTenantAsync(subscription.SubscriptionPlanId);
```

The `GetByIdWithoutTenantAsync` method does not apply tenant filtering:
```csharp
public virtual async Task<T?> GetByIdWithoutTenantAsync(Guid id)
{
    return await _dbSet
        .Where(e => e.Id == id)  // ← Only filters by ID
        .FirstOrDefaultAsync();
}
```

## Files Modified

### Controllers
1. **ModuleConfigController.cs**
   - `GetModules()` method (line 63)
   - `EnableModule()` method (line 124)

2. **UsersController.cs**
   - `CreateUser()` method (line 114)

3. **SubscriptionsController.cs**
   - `UpgradePlan()` method (line 87)
   - `DowngradePlan()` method (line 147)

4. **ClinicAdminController.cs**
   - User creation validation (line 357)
   - Clinic subscription check (line 744)

### Services
5. **ModuleConfigurationService.cs**
   - `GetModuleConfigAsync()` method (line 69)
   - `GetAllModulesForClinicAsync()` method (line 103)
   - `ValidateModuleConfigAsync()` method (line 439)

### Tests
6. **ModuleConfigControllerTests.cs**
   - Updated 5 test methods to mock `GetByIdWithoutTenantAsync` instead of `GetByIdAsync`

## Why This Is Safe

### Security Considerations
- Subscription plans are **intentionally system-wide** entities
- They contain pricing and feature configuration, not sensitive clinic data
- Clinics can only access plans they are subscribed to (verified via `ClinicSubscription` lookup)
- This change does NOT bypass tenant isolation for clinic-specific data

### Similar Patterns in Codebase
Other parts of the codebase already used the correct approach:
- `RegistrationService.cs` already used `GetByIdAsync(planId, "system")` (hardcoded)
- `ClinicAdminController.cs` already used `GetByIdAsync(planId, "system")` (hardcoded)

Our fix is more elegant as it uses the dedicated `GetByIdWithoutTenantAsync` method.

## Testing

### Build Status
✅ Build successful
- 344 warnings (pre-existing, unrelated)
- 0 errors

### Code Review
✅ No issues found

### Security Scan (CodeQL)
✅ No vulnerabilities detected

### Unit Tests
✅ Updated all affected tests
- `GetModules_WithValidSubscription_ShouldReturnOk`
- `GetModules_WithInvalidPlan_ShouldReturnBadRequest`
- `EnableModule_WithValidModule_ShouldReturnOk`
- `EnableModule_WithModuleNotInPlan_ShouldReturnBadRequest`
- `EnableModule_WhenAlreadyEnabled_ShouldUpdateConfig`

## Impact

### What's Fixed
✅ Module configuration screen now loads correctly
✅ Users can view available modules and their status
✅ Module enable/disable functionality works
✅ Subscription plan validation works correctly

### No Breaking Changes
- All existing functionality preserved
- Only fixes the broken behavior
- No API contract changes
- No database schema changes

## Prevention

### Why This Happened
The issue occurred because:
1. Subscription plans started as tenant-specific but became system-wide
2. Not all code was updated when the architecture changed
3. Tests used in-memory data where plans had the same tenantId as clinics

### Recommendations
1. **Documentation**: Clearly document which entities are system-wide vs tenant-specific
2. **Naming**: Consider using `SystemPlan` or similar to make it obvious
3. **Testing**: Add integration tests with realistic data seeding (system-wide entities)
4. **Code Review**: Watch for `GetByIdAsync` calls on system-wide entities

## Related Documentation
- `DataSeederService.cs` - See how subscription plans are created with `tenantId="system"`
- `BaseRepository.cs` - Understand tenant filtering behavior
- `SubscriptionPlan.cs` - Entity definition

## Verification Steps

To verify the fix works:
1. Start the application
2. Navigate to `http://localhost:5000/api/module-config`
3. Verify response is 200 OK with module list
4. Try enabling/disabling modules
5. Check that subscription plan validation works

## Date
February 7, 2026

## Author
GitHub Copilot Workspace
