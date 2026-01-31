# Fix: 403 Forbidden Error for SaaS Metrics Dashboard

## Problem
When attempting to access the endpoint `http://localhost:5293/api/system-admin/saas-metrics/dashboard`, users receive a **403 Forbidden** error even when authenticated as "admin".

## Root Cause
The system has two types of admin users:

1. **System Owner** (Owner entity)
   - Authenticates via `/api/auth/owner-login` with `tenantId="system"`
   - Has `is_system_owner=true` in JWT claims
   - Intended for platform administrators who manage the SaaS platform

2. **Clinic Admin** (User entity)  
   - Authenticates via `/api/auth/login` with clinic's tenantId
   - Has `UserRole.SystemAdmin` role
   - Intended for administrators of individual clinics

Both types receive `role="SystemAdmin"` in their JWT tokens, so they both pass the `[Authorize(Roles = "SystemAdmin")]` check on the controller. However, the SaaS metrics endpoints show **system-wide data across ALL tenants** (using `.IgnoreQueryFilters()`), so they should only be accessible to System Owners.

## Solution
Created a new `[RequireSystemOwner]` authorization attribute that verifies the `is_system_owner` claim in the JWT token. This attribute:

- ✅ Returns **401 Unauthorized** for unauthenticated users
- ✅ Returns **403 Forbidden** for authenticated users who are not System Owners
- ✅ Uses case-sensitive validation (exact match on "true")
- ✅ Logs unauthorized access attempts for security monitoring
- ✅ Provides generic error messages that don't reveal implementation details

## Files Changed
- `src/MedicSoft.CrossCutting/Authorization/CustomClaimTypes.cs` - NEW: Centralized claim constants
- `src/MedicSoft.CrossCutting/Authorization/RequireSystemOwnerAttribute.cs` - NEW: Authorization attribute
- `src/MedicSoft.Api/Controllers/SystemAdmin/SaasMetricsController.cs` - Added `[RequireSystemOwner]`
- `src/MedicSoft.Application/Services/JwtTokenService.cs` - Updated to use claim constants
- `src/MedicSoft.Api/Controllers/BaseController.cs` - Updated to use claim constants

## How to Access SaaS Metrics

### Correct Way (System Owner)
```bash
# 1. Authenticate as System Owner
curl -X POST http://localhost:5293/api/auth/owner-login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "system"
  }'

# 2. Use the returned token
curl http://localhost:5293/api/system-admin/saas-metrics/dashboard \
  -H "Authorization: Bearer <token>"
```

### What Won't Work (Clinic Admin)
```bash
# This will return 403 even with SystemAdmin role
curl -X POST http://localhost:5293/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123",
    "tenantId": "<demo-clinic-tenant-id>"
  }'

# The token won't have is_system_owner=true, so access is denied
curl http://localhost:5293/api/system-admin/saas-metrics/dashboard \
  -H "Authorization: Bearer <token>"
# Returns: 403 Forbidden - "Access denied. Insufficient permissions to access this resource."
```

## Security Benefits
1. **Prevents data leakage** - Clinic admins can't see cross-tenant metrics
2. **Proper HTTP status codes** - 401 for auth, 403 for insufficient permissions
3. **No information disclosure** - Error messages don't reveal endpoints or tenant IDs
4. **Security monitoring** - All denied access attempts are logged
5. **Maintainability** - Centralized claim constants prevent typos

## Testing
```bash
# Test 1: Unauthenticated request (should return 401)
curl http://localhost:5293/api/system-admin/saas-metrics/dashboard

# Test 2: Clinic admin (should return 403)
# First get clinic admin token via /api/auth/login
# Then access endpoint - should be denied

# Test 3: System Owner (should return 200 with metrics)
# First get system owner token via /api/auth/owner-login with tenantId="system"
# Then access endpoint - should succeed
```

## Rollout Notes
- **No breaking changes** - Only affects who can access SaaS metrics endpoints
- **Backward compatible** - Existing System Owner authentication continues to work
- **Security improvement** - Closes potential authorization gap
- **Reusable** - The `[RequireSystemOwner]` attribute can be applied to other sensitive endpoints

## Future Improvements
Consider applying `[RequireSystemOwner]` to other system-admin endpoints that show cross-tenant data:
- `/api/system-admin/clinic-management/*`
- `/api/system-admin/cohort-analysis/*`
- Any other endpoints that use `.IgnoreQueryFilters()`
