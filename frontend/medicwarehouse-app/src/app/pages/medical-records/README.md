# Medical Record CFM 1.638/2002 Components

This directory contains Angular components for CFM 1.638/2002 versioning and audit functionality.

## Components

### 1. MedicalRecordVersionHistoryComponent

Displays version history of medical records with timeline view.

**Usage:**
```html
<app-medical-record-version-history 
  [medicalRecordId]="recordId">
</app-medical-record-version-history>
```

**Features:**
- Timeline display of all versions
- Shows version number, change type, date/time, and user
- Displays change summary and justification when available
- Material Design icons for different change types:
  - 🕐 `history` - Created
  - ✏️ `edit` - Updated
  - 🔒 `lock` - Closed
  - 🔓 `lock_open` - Reopened
- Loading and error states
- Empty state when no versions exist

**Input:**
- `medicalRecordId: string` - ID of the medical record

### 2. MedicalRecordAccessLogComponent

Displays access logs for medical records in a filterable table.

**Usage:**
```html
<app-medical-record-access-log 
  [medicalRecordId]="recordId">
</app-medical-record-access-log>
```

**Features:**
- Material table with sorting and pagination
- Date range filtering (optional start/end dates)
- Columns: Date/Time, User, Access Type, IP Address
- Access type icons:
  - 👁️ `visibility` - View
  - ✏️ `edit` - Edit
  - 🔒 `lock` - Close
  - 🔓 `lock_open` - Reopen
  - 🖨️ `print` - Print
  - ⬇️ `download` - Export
- Refresh button to reload logs
- Clear filter functionality
- Loading and error states
- Empty state when no logs found

**Input:**
- `medicalRecordId: string` - ID of the medical record

## Dependencies

Both components use:
- Angular Material (standalone components)
- Angular CommonModule
- Angular FormsModule (access log component)
- MedicalRecordService from `../../services/medical-record`
- Models from `../../models/medical-record.model`

## API Integration

Components integrate with backend endpoints:
- `GET /api/medical-records/{id}/versions` - Version history
- `GET /api/medical-records/{id}/access-logs?startDate&endDate` - Access logs

## Styling

Components follow Material Design guidelines and use consistent styling with:
- Card containers
- Material color palette
- Responsive layouts
- Loading spinners
- Error/empty states with icons

## Example Integration

```typescript
import { Component } from '@angular/core';
import { MedicalRecordVersionHistoryComponent } from './medical-record-version-history.component';
import { MedicalRecordAccessLogComponent } from './medical-record-access-log.component';

@Component({
  selector: 'app-medical-record-audit',
  standalone: true,
  imports: [
    MedicalRecordVersionHistoryComponent,
    MedicalRecordAccessLogComponent
  ],
  template: `
    <div class="audit-container">
      <app-medical-record-version-history 
        [medicalRecordId]="medicalRecordId">
      </app-medical-record-version-history>
      
      <app-medical-record-access-log 
        [medicalRecordId]="medicalRecordId">
      </app-medical-record-access-log>
    </div>
  `
})
export class MedicalRecordAuditComponent {
  medicalRecordId = 'your-record-id';
}
```
