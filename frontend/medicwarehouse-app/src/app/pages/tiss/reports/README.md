# TISS Reports Component

## Overview
The TISS Reports component provides a comprehensive reporting interface for generating and exporting TISS (Troca de Informações na Saúde Suplementar) reports in the MedicWarehouse system.

## Location
- **TypeScript**: `/frontend/medicwarehouse-app/src/app/pages/tiss/reports/tiss-reports.ts`
- **Template**: `/frontend/medicwarehouse-app/src/app/pages/tiss/reports/tiss-reports.html`
- **Styles**: `/frontend/medicwarehouse-app/src/app/pages/tiss/reports/tiss-reports.scss`
- **Route**: `/tiss/reports` (Protected by authGuard)

## Features

### Report Types
1. **Faturamento por Operadora** (Billing by Operator)
   - Displays total billed, approved, and rejected amounts
   - Shows glosa (rejection) rates per operator
   - Includes guide counts

2. **Glosas Detalhadas** (Detailed Rejections)
   - Detailed breakdown of rejected claims by operator
   - Shows rejection percentages and guide counts

3. **Autorizações Negadas** (Denied Authorizations)
   - Lists authorization requests by operator
   - Shows approved, rejected, and pending counts
   - Displays approval rates

4. **Tempo de Aprovação** (Approval Times)
   - Average approval times per operator
   - Shows min/max approval times
   - Total processed authorizations

5. **Procedimentos Mais Utilizados** (Top Procedures)
   - Top 10 most frequently rejected procedures
   - Shows billing amounts and rejection rates
   - Displays occurrence counts

### Filters
- **Report Type**: Dropdown to select report type
- **Date Range**: Start and end date selectors
- **Operator**: Filter by specific health insurance operator (optional)

### Export Options
- **PDF Export**: Export report to PDF format (placeholder for jsPDF implementation)
- **Excel Export**: Export report to Excel format (placeholder for ExcelJS implementation)

### UI Features
- Clean, professional interface
- Responsive design (mobile-first)
- Loading states with spinners
- Error messages
- Report preview in table format
- Summary rows with totals
- Color-coded badges for rates and percentages

## Technical Details

### Architecture
- **Angular Version**: 20+
- **Component Type**: Standalone component with signals
- **State Management**: Angular signals for reactive state

### Dependencies
- `@angular/core`: Angular core functionality
- `@angular/common`: Common Angular directives (CommonModule)
- `@angular/forms`: Form handling (FormsModule)
- `TissAnalyticsService`: Data fetching service
- `HealthInsuranceOperatorService`: Operator data service
- `AuthService`: Authentication and user context

### Services Used

#### TissAnalyticsService Methods
- `getGlosasByOperator()`: Fetches billing/glosa data by operator
- `getAuthorizationRate()`: Fetches authorization statistics
- `getApprovalTime()`: Fetches approval time metrics
- `getProcedureGlosas()`: Fetches top rejected procedures

#### HealthInsuranceOperatorService Methods
- `getAll()`: Fetches all active health insurance operators

### Data Models
All models are imported from `../../../models/tiss.model`:
- `GlosasByOperator`
- `ProcedureGlosas`
- `ApprovalTime`
- `AuthorizationRate`
- `HealthInsuranceOperator`

## Usage

### Navigation
Access the component via the route: `/tiss/reports`

### Workflow
1. Component loads with default date range (last 30 days)
2. User selects report type from dropdown
3. User adjusts date range if needed
4. (Optional) User filters by specific operator
5. User clicks "Gerar Relatório" button
6. Component fetches data and displays preview
7. User can export to PDF or Excel using export buttons

### Code Example

```typescript
// Generating a billing report
selectedReportType.set('billing');
startDate.set('2024-01-01');
endDate.set('2024-01-31');
selectedOperatorId.set('all');
generateReport();
```

## Future Enhancements

### PDF Export Implementation
To implement PDF export, add the following:

```typescript
// Install jsPDF
npm install jspdf jspdf-autotable

// Import in component
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

// Update exportToPDF method
exportToPDF(): void {
  const doc = new jsPDF();
  doc.text(this.getReportTitle(), 14, 15);
  
  autoTable(doc, {
    head: [this.getTableHeaders()],
    body: this.getTableData(),
    startY: 25,
    theme: 'grid',
    styles: { fontSize: 9 }
  });
  
  doc.save(`tiss-report-${this.selectedReportType()}-${Date.now()}.pdf`);
}
```

### Excel Export Implementation
To implement Excel export, add the following:

```typescript
// Install ExcelJS
npm install exceljs

// Import in component
import * as ExcelJS from 'exceljs';

// Update exportToExcel method
async exportToExcel(): Promise<void> {
  const workbook = new ExcelJS.Workbook();
  const worksheet = workbook.addWorksheet('Report');
  
  // Add headers
  worksheet.columns = this.getExcelColumns();
  
  // Add data
  worksheet.addRows(this.getTableData());
  
  // Style header row
  worksheet.getRow(1).font = { bold: true };
  worksheet.getRow(1).fill = {
    type: 'pattern',
    pattern: 'solid',
    fgColor: { argb: 'FFE0E0E0' }
  };
  
  // Generate and download
  const buffer = await workbook.xlsx.writeBuffer();
  const blob = new Blob([buffer], { 
    type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' 
  });
  const url = window.URL.createObjectURL(blob);
  const anchor = document.createElement('a');
  anchor.href = url;
  anchor.download = `tiss-report-${this.selectedReportType()}-${Date.now()}.xlsx`;
  anchor.click();
  window.URL.revokeObjectURL(url);
}
```

## Styling

### Responsive Breakpoints
- Mobile: < 576px
- Tablet: 576px - 768px
- Desktop: > 768px

### Color Scheme
- Primary: `#0d6efd` (Bootstrap primary blue)
- Success: `#198754` (Green for approved amounts)
- Danger: `#dc3545` (Red for rejected amounts)
- Warning: `#ffc107` (Yellow for warnings)
- Muted: `#6c757d` (Gray for secondary text)

### Badge Colors
- Success (< 10% glosa): Green
- Warning (10-15% glosa): Yellow
- Danger (> 15% glosa): Red

## Testing Checklist
- [ ] Component loads without errors
- [ ] Default date range is set correctly
- [ ] All report types can be selected
- [ ] Data fetches correctly for each report type
- [ ] Operator filter works correctly
- [ ] Tables display data properly
- [ ] Export buttons are disabled when no data
- [ ] Loading spinner shows during data fetch
- [ ] Error messages display properly
- [ ] Responsive design works on mobile
- [ ] Summary totals calculate correctly

## Notes
- The component follows the same patterns as `glosas-dashboard.ts`
- Uses Angular 20+ standalone components
- Implements signals for reactive state management
- Export functions have placeholder alerts for future implementation
- All data fetching includes proper error handling
- Loading states provide good UX feedback
