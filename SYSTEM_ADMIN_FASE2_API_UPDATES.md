# Phase 2: Client Management - API Documentation Update

## Overview
This document describes the new API endpoints added as part of Phase 2 Client Management improvements to the System Admin functionality.

## New Endpoints

### Clinic Management

#### POST /api/system-admin/clinic-management/bulk-action
Execute bulk actions on multiple clinics.

**Request Body:**
```json
{
  "clinicIds": ["guid1", "guid2", "guid3"],
  "action": "activate|deactivate|addTag|removeTag",
  "parameters": {
    "tagId": "guid" // Optional, required for addTag/removeTag actions
  }
}
```

**Response:**
```json
{
  "successCount": 3,
  "failureCount": 0,
  "errors": [],
  "message": "Processed 3 clinics. 3 succeeded, 0 failed."
}
```

**Available Actions:**
- `activate` - Activate selected clinics
- `deactivate` - Deactivate selected clinics
- `addTag` - Add a tag to selected clinics (requires `tagId` parameter)
- `removeTag` - Remove a tag from selected clinics (requires `tagId` parameter)

---

#### POST /api/system-admin/clinic-management/export
Export clinic data to various formats.

**Request Body:**
```json
{
  "clinicIds": ["guid1", "guid2", "guid3"],
  "format": "Csv|Excel|Pdf",
  "includeHealthScore": true,
  "includeTags": true,
  "includeUsageMetrics": false
}
```

**Response:**
Binary file download with appropriate content type:
- CSV: `text/csv`
- Excel: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
- PDF: `application/pdf`

---

### User Management

#### POST /api/system-admin/users/transfer-ownership
Transfer ownership from one user to another within the same clinic.

**Request Body:**
```json
{
  "currentOwnerId": "guid1",
  "newOwnerId": "guid2"
}
```

**Response:**
```json
{
  "message": "Ownership transferred successfully"
}
```

**Validations:**
- Both users must exist
- Both users must belong to the same clinic
- Current user must have Owner role
- New owner must be active
- Operation is logged in audit trail

---

## Background Jobs

### AutoTaggingJob
Automatically applies tags to clinics based on predefined rules.

**Schedule:** Daily

**Rules:**
1. **At-Risk Tag**: Clinics inactive for 30+ days
2. **High-Value Tag**: Clinics with subscription value ≥ R$ 1000
3. **New Tag**: Clinics created in last 30 days
4. **Active-User Tag**: Clinics with user activity in last 7 days
5. **Support-Heavy Tag**: Clinics with 5+ tickets in last 30 days
6. **Trial Tag**: Clinics with trial subscription status

---

## Changes to Existing Endpoints

### POST /api/system-admin/clinic-management/filter
No changes to the endpoint, but now supports enhanced filtering for the new view modes.

---

## Frontend Integration

### New Views
The frontend now supports 4 view modes:
1. **List View** (default) - Traditional table view with bulk selection
2. **Cards View** - Card-based grid layout
3. **Map View** - Geographic visualization (placeholder for future integration)
4. **Kanban View** - Board view organized by health status

### Bulk Operations
Users can now:
- Select multiple clinics using checkboxes
- Perform bulk activate/deactivate operations
- Add tags to multiple clinics at once
- Export selected clinics to CSV, Excel, or PDF

---

## Security Considerations

- All endpoints require `SystemAdmin` role authorization
- Bulk actions are logged in audit trail
- Export operations are limited to selected clinics only
- Ownership transfer creates audit log entry
- No sensitive data (passwords, tokens) included in exports

---

## Performance Notes

- Bulk actions process clinics sequentially to prevent database locks
- Export operations generate files in-memory (future: implement async generation for large datasets)
- Health score calculations are cached where appropriate
- Automatic tagging runs as background job to avoid blocking user requests

---

## Future Enhancements

1. **Bulk Email**: Send emails to multiple clinic owners
2. **Bulk Plan Change**: Change subscription plans for multiple clinics
3. **Map View Integration**: Integrate real mapping library (Leaflet/Google Maps)
4. **Async Export**: For large datasets (>1000 clinics), generate exports asynchronously
5. **Custom Tag Rules**: Allow admins to define custom automatic tagging rules via UI

---

**Version:** 1.0  
**Last Updated:** January 2026  
**Related Documentation:**
- [System Admin User Guide](SYSTEM_ADMIN_USER_GUIDE.md)
- [Phase 2 Implementation Plan](Plano_Desenvolvimento/fase-system-admin-melhorias/02-fase2-gestao-clientes.md)
