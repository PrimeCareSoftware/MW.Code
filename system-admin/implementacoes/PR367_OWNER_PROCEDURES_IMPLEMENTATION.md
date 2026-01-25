# PR 367 - Owner Procedures Management Implementation

## Overview
This document describes the implementation of the Owner Procedures Management feature from PR 367, which enables clinic owners to view and manage procedures across all clinics they own.

## Problem Statement
Previously, the procedures management system only displayed procedures from a single clinic (filtered by `tenantId`). Clinic owners who manage multiple clinics needed cross-clinic visibility to have a consolidated view of all procedures across their clinic network.

## Solution Architecture

### Backend Implementation (PR 367)
The backend changes were implemented in PR 367 and include:

#### Domain Layer
- **Permission Key**: Added `ProceduresManage` permission (`procedures.manage`) to `PermissionKeys.cs`
  - Label: "Gerenciar Procedimentos (proprietário)"
  - Purpose: Owner-level permission to manage procedures across all owned clinics

#### Repository Layer
- **New Method**: `GetByOwnerAsync(Guid ownerId, bool activeOnly)` in `IProcedureRepository`
- **Implementation**: SQL JOIN query with `OwnerClinicLink` table
  ```csharp
  var query = from procedure in _dbSet
              join link in _context.OwnerClinicLinks
                  on procedure.TenantId equals link.ClinicId.ToString()
              where link.OwnerId == ownerId && link.IsActive
              select procedure;
  ```
- **Performance**: Single query with JOIN avoids N+1 problem
- **Safety**: Uses `Distinct()` to prevent duplicate results

#### Application Layer
- **Extended Query**: `GetProceduresByClinicQuery` now includes optional `OwnerId` parameter
- **Query Handler**: Routes to `GetByOwnerAsync()` when `OwnerId` is present, otherwise uses standard `GetByClinicAsync()`

#### API Layer
- **Controller Enhancement**: `ProceduresController.GetAll()` detects `ClinicOwner` role
- **Security**: Server-side database lookup verifies owner status (prevents role claim spoofing)
- **Automatic Routing**: When user has `ClinicOwner` role, automatically passes `ownerId` to query

### Frontend Implementation (This PR)

#### Component Structure
Created new Angular component at `/frontend/medicwarehouse-app/src/app/pages/procedures/`:
- `owner-procedure-management.ts` - Component logic
- `owner-procedure-management.html` - Template
- `owner-procedure-management.scss` - Styles

#### Key Features

##### 1. Cross-Clinic Procedure Visibility
- Automatically fetches procedures from all owned clinics
- Backend handles owner detection and clinic filtering
- No manual clinic selection needed

##### 2. Advanced Filtering
- **Search**: Filter by procedure code, name, or description
- **Category Filter**: Dropdown to filter by procedure category
- **Real-time Updates**: 300ms debounced search for smooth UX

##### 3. Statistics Dashboard
- **Total Procedures Count**: Shows all filtered procedures
- **Active Procedures Count**: Shows only active procedures
- Updates dynamically based on current filters

##### 4. Read-Only View
- View button to see procedure details
- Redirects to existing procedure edit form
- Maintains separation of concerns (owners view, clinic admins manage)

#### Routing
Added protected route:
```typescript
{ 
  path: 'procedures/owner-management', 
  loadComponent: () => import('./pages/procedures/owner-procedure-management').then(m => m.OwnerProcedureManagement),
  canActivate: [authGuard, ownerGuard]
}
```

#### Navigation Menu
Added menu item in the "Procedimentos" section:
- **Label**: "Gerenciar Procedimentos (Proprietário)"
- **Icon**: Document with plus symbol
- **Visibility**: Only shown to clinic owners (using `isOwner()` check)
- **Location**: Below "Procedimentos da Clínica" item

#### Permission Logic
The component leverages Angular's `ownerGuard` which checks:
1. User is authenticated
2. User has one of: `Owner` role, `ClinicOwner` role, or `isSystemOwner` flag

## User Experience

### Access Path
1. **Login** as clinic owner
2. **Navigate** to menu → Procedimentos → "Gerenciar Procedimentos (Proprietário)"
3. **View** consolidated list of procedures from all owned clinics

### Screen Layout
- **Header**: Title and description
- **Statistics Cards**: 
  - Total procedures count
  - Active procedures count
- **Search Bar**: Real-time search across code, name, description
- **Category Filter**: Dropdown with all procedure categories
- **Data Table**: 
  - Columns: Código, Nome, Categoria, Preço, Duração, Status, Ações
  - Sortable and filterable
  - View button for each procedure

### Responsive Design
- Desktop: Full table layout with all columns
- Tablet: Scrollable table
- Mobile: Horizontal scroll with pinned action column

## Technical Details

### Data Flow
1. Component calls `procedureService.getAll(false)` (includes inactive procedures)
2. Service makes GET request to `/api/procedures?activeOnly=false`
3. Backend detects `ClinicOwner` role from JWT token
4. Backend queries database with owner-specific filter
5. Returns procedures from all owned clinics
6. Frontend displays data with filtering and search capabilities

### Security Considerations
- **Route Protection**: Both `authGuard` and `ownerGuard` required
- **Server-Side Verification**: Backend verifies owner status via database lookup
- **Token-Based Auth**: Uses JWT claims but validates against database
- **Read-Only Access**: Owners can view but must use clinic-specific screens to edit

### Performance Optimizations
- **Debounced Search**: 300ms delay prevents excessive API calls
- **Client-Side Filtering**: Category and search filters work on already-fetched data
- **Lazy Loading**: Component loaded only when route is accessed
- **Single Query**: Backend uses JOIN instead of multiple queries

## Integration with Existing Features

### Permissions System
- Works with existing `PermissionKeys` infrastructure
- Permission can be assigned to access profiles
- Appears in profile configuration under "Procedimentos" category

### Clinic Administration
- Complements existing clinic-specific procedures management
- Owners can view all procedures, clinic admins manage individual clinics
- Clear separation of concerns between viewing and editing

### Multi-Tenant Architecture
- Respects tenant boundaries (owner-clinic relationships)
- Uses `OwnerClinicLink` table to determine ownership
- Maintains data isolation between different clinic networks

## Testing Recommendations

### Manual Testing Scenarios
1. **Owner with Multiple Clinics**
   - Verify procedures from all owned clinics are displayed
   - Verify count matches total across all clinics

2. **Owner with Single Clinic**
   - Verify works same as multi-clinic scenario
   - Verify menu item still appears

3. **Non-Owner User**
   - Verify menu item is hidden
   - Verify direct URL navigation is blocked (403)

4. **Search and Filter**
   - Test search with various terms
   - Test category filter
   - Test combination of both filters
   - Verify statistics update correctly

5. **Navigation**
   - Test view button redirects to edit form
   - Verify back navigation works
   - Test deep-linking to route

### Security Testing
1. Attempt access with non-owner JWT token (should fail)
2. Modify JWT claims to fake owner role (backend should reject)
3. Test cross-clinic data isolation

## Documentation Updates

### PROCEDURES_IMPLEMENTATION.md
Updated to include:
- Section on Owner Procedures Management
- Access path for clinic owners
- Feature description and use cases
- Differences from clinic-specific management

### README.md
Consider adding:
- Note about multi-clinic owner features
- Link to procedures management documentation

## Future Enhancements

### Potential Improvements
1. **Clinic Column**: Add column showing which clinic owns each procedure
2. **Bulk Operations**: Export, print, or analyze procedures across clinics
3. **Comparative Analytics**: Compare procedure pricing/usage across clinics
4. **Procedure Synchronization**: Copy procedures between clinics
5. **Advanced Filters**: Filter by clinic, price range, duration, etc.

### Performance Considerations
- For owners with 50+ clinics and 1000+ procedures, consider:
  - Pagination implementation
  - Virtual scrolling for large datasets
  - Server-side filtering and search

## Changelog Entry

```markdown
### Added
- **Owner Procedures Management**: New screen for clinic owners to view procedures across all owned clinics
  - Consolidated view of all procedures from owned clinics
  - Advanced search and filtering by category
  - Statistics dashboard showing procedure counts
  - Read-only view mode for cross-clinic visibility
  - Automatic permission-based menu visibility
  - Protected route with owner authentication
  
### Backend (PR 367)
- Added `procedures.manage` permission for owner-level access
- Implemented `GetByOwnerAsync()` repository method
- Extended procedures API to support cross-clinic queries
- Added owner verification logic to prevent permission spoofing
```

## Support and Troubleshooting

### Common Issues

**Issue**: Menu item not appearing
- **Solution**: Verify user has `ClinicOwner` role or `isSystemOwner` flag

**Issue**: No procedures showing
- **Solution**: Verify owner has active links in `OwnerClinicLink` table

**Issue**: Permission denied error
- **Solution**: Verify route has `ownerGuard` and user authentication is valid

**Issue**: Showing procedures from only one clinic
- **Solution**: Backend might not be detecting owner role correctly; check JWT claims

## References
- **Original PR**: #367 - "Enable clinic owners to view procedures across all owned clinics"
- **Backend Files**: 
  - `src/MedicSoft.Domain/Common/PermissionKeys.cs`
  - `src/MedicSoft.Domain/Interfaces/IProcedureRepository.cs`
  - `src/MedicSoft.Repository/Repositories/ProcedureRepository.cs`
  - `src/MedicSoft.Application/Queries/Procedures/GetProceduresByClinicQuery.cs`
  - `src/MedicSoft.Api/Controllers/ProceduresController.cs`
- **Frontend Files**:
  - `frontend/medicwarehouse-app/src/app/pages/procedures/owner-procedure-management.ts`
  - `frontend/medicwarehouse-app/src/app/app.routes.ts`
  - `frontend/medicwarehouse-app/src/app/shared/navbar/navbar.html`

## Conclusion
This implementation successfully delivers cross-clinic procedure visibility for clinic owners while maintaining security, performance, and code quality standards. The feature integrates seamlessly with existing systems and provides a foundation for future multi-clinic management capabilities.
