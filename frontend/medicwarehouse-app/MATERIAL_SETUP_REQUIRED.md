# Angular Material Setup Required

## Issue
Two components in this application use Angular Material components but Angular Material is not currently installed:

1. `src/app/pages/prescriptions/prescription-type-selector.component.ts`
2. `src/app/pages/prescriptions/sngpc-dashboard.component.ts`

These components were previously using inline templates with Material components, but Material was never properly configured in the project.

## Components Affected
- **prescription-type-selector**: Uses mat-card, mat-button, mat-icon, mat-chips
- **sngpc-dashboard**: Uses mat-card, mat-icon, mat-chip, mat-menu, mat-button, mat-table

## Solution
To fix these components, Angular Material needs to be installed:

```bash
cd frontend/medicwarehouse-app
ng add @angular/material
```

After installation, uncomment the Material module imports in:
- `src/app/pages/prescriptions/prescription-type-selector.component.ts`
- Add Material imports to `src/app/pages/prescriptions/sngpc-dashboard.component.ts`

## Temporary Status
The templates have been successfully extracted from inline to separate HTML files, but the components will not render correctly until Material is installed.

All other components (6 out of 8) have been successfully refactored and are working.
