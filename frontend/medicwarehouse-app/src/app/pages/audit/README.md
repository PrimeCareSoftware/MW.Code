# Audit Components

Angular components for the LGPD Audit System in PrimeCare Software.

## Components

### 1. AuditLogListComponent
Main component displaying audit logs in a table with filtering capabilities.

**Features:**
- Paginated table with 50 items per page (configurable: 25, 50, 100)
- Filtering by:
  - Date range (start/end date)
  - Action type (CREATE, READ, UPDATE, DELETE, etc.)
  - Result (SUCCESS, FAILED, UNAUTHORIZED, PARTIAL_SUCCESS)
  - Severity (INFO, WARNING, ERROR, CRITICAL)
  - Entity type
- Sortable columns
- Click row to view details
- Material Design UI
- Responsive layout

**Columns:**
- Timestamp
- User Name/Email
- Action (with color-coded chips)
- Entity Type/Name
- Result (with icons)
- IP Address
- Details button

**Usage:**
```typescript
import { AuditLogListComponent } from './pages/audit';

// In routes
{
  path: 'audit',
  component: AuditLogListComponent
}
```

### 2. AuditLogDetailsDialogComponent
Dialog component showing detailed audit log information.

**Features:**
- Three tabs:
  1. **General Information**: Complete audit details including user, action, entity, security info, and LGPD compliance data
  2. **Changes**: Shows before/after values, changed fields list, and field-by-field comparison table
  3. **Raw Data**: Complete JSON log for technical debugging
- Color-coded chips for actions, results, severity
- LGPD compliance information (data category, purpose)
- Security information (IP, User Agent, HTTP method/path)
- JSON diff viewer for modified data

**Usage:**
```typescript
// Opened automatically when clicking table row or details button
this.dialog.open(AuditLogDetailsDialogComponent, {
  width: '800px',
  maxHeight: '90vh',
  data: auditLog
});
```

## API Integration

These components use the `AuditService` which calls the backend API at `/api/Audit`:

- `POST /api/Audit/query` - Query logs with filters
- `GET /api/Audit/user/{userId}` - Get user activity
- `GET /api/Audit/entity/{entityType}/{entityId}` - Get entity history
- `GET /api/Audit/security-events` - Get security events
- `GET /api/Audit/lgpd-report/{userId}` - Get LGPD report

## LGPD Compliance

These components support full LGPD (Lei 13.709/2018) compliance:
- Tracks all data access and modifications
- Records data category (PUBLIC, PERSONAL, SENSITIVE, CONFIDENTIAL)
- Records processing purpose (HEALTHCARE, BILLING, LEGAL_OBLIGATION, etc.)
- Maintains complete audit trail
- Supports data subject access requests (relatório LGPD)

## Files

```
src/app/pages/audit/
├── audit-log-list.component.ts          # Main list component
├── audit-log-list.component.html        # List template
├── audit-log-list.component.scss        # List styles
├── audit-log-details-dialog.component.ts    # Dialog component
├── audit-log-details-dialog.component.html  # Dialog template
├── audit-log-details-dialog.component.scss  # Dialog styles
├── index.ts                             # Exports
└── README.md                            # This file
```

## Dependencies

### Angular Material Modules Used
- MatTableModule - Data table
- MatPaginatorModule - Pagination
- MatSortModule - Column sorting
- MatCardModule - Card containers
- MatFormFieldModule - Form fields
- MatInputModule - Text inputs
- MatSelectModule - Dropdowns
- MatDatepickerModule - Date pickers
- MatNativeDateModule - Date adapter
- MatButtonModule - Buttons
- MatIconModule - Material icons
- MatChipsModule - Colored chips
- MatProgressSpinnerModule - Loading spinner
- MatTooltipModule - Tooltips
- MatDialogModule - Dialogs
- MatTabsModule - Tabbed interface
- MatDividerModule - Visual dividers

### Other Dependencies
- ReactiveFormsModule - Form handling
- CommonModule - Common directives
- Navbar - Shared navigation component

## Routing

Add to your routing module:

```typescript
{
  path: 'audit',
  component: AuditLogListComponent,
  canActivate: [AuthGuard],
  data: { roles: ['Admin', 'SystemAdmin'] }
}
```

## Security

- Only Admin and SystemAdmin roles should access audit logs
- Individual users can access their own LGPD report via separate endpoint
- All API calls are protected by JWT authentication
- Audit logs are write-only (never deleted)

## Responsive Design

Both components are fully responsive:
- Desktop: Full table with all columns
- Tablet: Stacked filters, scrollable table
- Mobile: Vertical filter layout, horizontal table scroll

## Color Coding

**Actions:**
- CREATE: Blue (primary)
- READ: Purple (accent)
- UPDATE: Orange (warn)
- DELETE: Red (error)
- LOGIN/LOGOUT: Light blue (info)
- EXPORT/DOWNLOAD/PRINT: Orange (warn)
- ACCESS_DENIED/LOGIN_FAILED: Red (error)

**Results:**
- SUCCESS: Green
- FAILED: Orange
- UNAUTHORIZED: Red
- PARTIAL_SUCCESS: Blue

**Severity:**
- INFO: Blue
- WARNING: Orange
- ERROR: Orange-red
- CRITICAL: Red

**Data Categories:**
- PUBLIC: Green
- PERSONAL: Blue
- SENSITIVE: Orange
- CONFIDENTIAL: Red

## Future Enhancements

Potential additions:
- Export to CSV/PDF
- Real-time updates via SignalR
- Dashboard with charts/statistics
- Advanced search with multiple entity IDs
- Bulk operations
- Email alerts for critical events
- User activity timeline view
