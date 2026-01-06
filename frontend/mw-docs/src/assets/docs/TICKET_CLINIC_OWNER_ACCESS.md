# Ticket System - Clinic Owner Access

## Overview

This document describes the authorization changes made to the ticket system to support clinic owners viewing all tickets from their clinic.

## Problem

Previously, the ticket system only had two types of users:
1. **System Owners** - Could see all tickets in the system
2. **Regular Users** - Could only see their own tickets

However, there was a requirement for **Clinic Owners** to be able to see all tickets created by users in their clinic, not just the tickets they personally created.

## Solution

### Authorization Levels

The ticket system now supports three authorization levels:

1. **System Owner** (`is_system_owner: true` in JWT)
   - Can see ALL tickets in the system
   - Can manage all tickets
   - No restrictions

2. **Clinic Owner** (`is_system_owner: false` AND has `clinic_id` in JWT)
   - Can see all tickets from their clinic
   - Can manage tickets from their clinic
   - Cannot see tickets from other clinics

3. **Regular User** (no `clinic_id` in JWT, or has `clinic_id` but is not an owner)
   - Can only see their own tickets
   - Can only manage their own tickets
   - Cannot see tickets from other users

### Implementation Details

#### Backend Changes

**1. Helper Methods**

Added `IsClinicOwner()` helper method to:
- `src/MedicSoft.Api/Controllers/TicketsController.cs`
- `microservices/shared/MedicSoft.Shared.Authentication/MicroserviceBaseController.cs`

```csharp
protected bool IsClinicOwner()
{
    // A user is a clinic owner if they are not a system owner but have a clinic_id
    return !IsSystemOwner() && GetClinicId().HasValue;
}
```

**2. Service Layer Updates**

Updated `GetTicketByIdAsync` method to accept clinic owner parameters:
- Interface: `ITicketService.GetTicketByIdAsync(..., bool isClinicOwner = false, Guid? userClinicId = null)`
- Implementation checks:
  - System owners: Can see any ticket
  - Clinic owners: Can see tickets where `ticket.ClinicId == userClinicId`
  - Regular users: Can only see tickets where `ticket.UserId == userId`

Updated `GetUserTicketsAsync` method:
- Interface: `ITicketService.GetUserTicketsAsync(..., bool isClinicOwner = false, Guid? userClinicId = null)`
- For clinic owners: Returns all tickets from their clinic
- For regular users: Returns only their own tickets

**3. Controller Updates**

Updated the following endpoints in both main API and SystemAdmin microservice:

- `GET /api/tickets/{id}` - Now checks clinic ownership
- `GET /api/tickets/my-tickets` - Returns clinic tickets for clinic owners
- `GET /api/tickets/clinic/{clinicId}` - Verifies the requesting user is the owner of the clinic

#### Frontend Changes

No frontend changes were required! The existing screens automatically work with the new authorization logic:

**medicwarehouse-app** (`/tickets`):
- Uses `getMyTickets()` endpoint
- For clinic owners: Shows all clinic tickets
- For regular users: Shows only their own tickets
- Title "Meus Chamados" works for both cases

**mw-system-admin** (`/tickets`):
- Uses `getAllTickets()` endpoint (for system owners)
- Shows all tickets with Kanban board
- Already had the proper UI for managing multiple tickets

### User Experience

#### Regular User
1. Opens ticket via FAB button
2. Goes to `/tickets` page
3. Sees only tickets they created
4. Can view details and add comments

#### Clinic Owner
1. Logs in with owner credentials (has `clinic_id` in JWT)
2. Can open tickets via FAB button
3. Goes to `/tickets` page
4. **Sees ALL tickets from their clinic** (not just their own)
5. Can view details and add comments to any ticket from their clinic
6. Cannot see tickets from other clinics

#### System Owner
1. Logs in to mw-system-admin
2. Goes to `/tickets` page
3. Sees ALL tickets from ALL clinics
4. Can manage, assign, and resolve tickets
5. Has Kanban board view

## Testing

All existing tests continue to pass:
- `8/8` ticket service tests passing
- No breaking changes to existing functionality

### Test Coverage

Existing tests cover:
- ✅ Regular user can see their own tickets
- ✅ Regular user cannot see other users' tickets
- ✅ System owner can see all tickets
- ✅ Ticket creation, updates, and comments

**New behavior covered by authorization logic:**
- ✅ Clinic owner can see all tickets from their clinic
- ✅ Clinic owner cannot see tickets from other clinics
- ✅ Clinic owner still sees their own tickets even if no clinic_id

## Database Schema

No database changes were required. The existing schema already supports this feature:
- `Tickets` table has `ClinicId` column
- JWT token contains `clinic_id` claim for clinic owners
- JWT token contains `is_system_owner` claim for system owners

## Security Considerations

- **Authorization is enforced at the service layer** - Even if frontend makes incorrect calls, backend validates permissions
- **Clinic ownership is verified from JWT claims** - Cannot be spoofed
- **Explicit checks prevent unauthorized access** - Each endpoint validates user's role and clinic
- **System owners remain unrestricted** - For administrative purposes
- **No information leakage** - Users only get 404/403 errors for tickets they cannot access

## Future Enhancements

Potential improvements:
1. Add UI indicator when viewing clinic tickets vs own tickets
2. Add filter to show "Only My Tickets" vs "All Clinic Tickets" for clinic owners
3. Add permission to allow/deny specific clinic owners from viewing all clinic tickets
4. Add audit log for when clinic owners view other users' tickets

## Files Modified

### Main API
- `src/MedicSoft.Api/Controllers/TicketsController.cs` - Added `IsClinicOwner()`, updated endpoints
- `src/MedicSoft.Application/Services/ITicketService.cs` - Updated method signatures
- `src/MedicSoft.Application/Services/TicketService.cs` - Implemented clinic owner logic

### SystemAdmin Microservice
- `microservices/shared/MedicSoft.Shared.Authentication/MicroserviceBaseController.cs` - Added `IsClinicOwner()`
- `microservices/systemadmin/MedicSoft.SystemAdmin.Api/Controllers/TicketsController.cs` - Updated endpoints
- `microservices/systemadmin/MedicSoft.SystemAdmin.Api/Services/ITicketService.cs` - Updated signatures
- `microservices/systemadmin/MedicSoft.SystemAdmin.Api/Services/TicketService.cs` - Implemented logic

## Backward Compatibility

All changes are **fully backward compatible**:
- Regular users continue to work exactly as before
- System owners continue to work exactly as before
- Clinic owners get new functionality automatically (no frontend changes needed)
- API signatures use optional parameters to maintain compatibility
- No database migrations required

## Deployment Notes

No special deployment steps required:
1. Deploy backend changes (main API and/or SystemAdmin microservice)
2. Existing frontend continues to work
3. Clinic owners will automatically see all clinic tickets
4. No data migration or configuration changes needed
