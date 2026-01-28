# System Admin - Phase 2 Features User Guide

## Overview
This guide covers the new features added in Phase 2 of the System Admin improvements, focusing on enhanced client management capabilities.

---

## Multiple View Modes

### List View (Default)
The traditional table view with comprehensive information about each clinic.

**Features:**
- Sortable columns
- Quick action buttons
- Bulk selection checkboxes
- Pagination controls

**How to Use:**
1. Click the "📋 Lista" button in the view switcher
2. Use checkboxes to select multiple clinics
3. Click action buttons for individual clinics

### Cards View
A visual card-based layout showing clinic information in a grid.

**Features:**
- Visual cards with clinic details
- Color-coded health status badges
- Tag display
- Easy scanning of multiple clinics

**How to Use:**
1. Click the "🎴 Cards" button in the view switcher
2. Click any card to view clinic details
3. Hover over cards for visual feedback

### Kanban View
Board-style view organized by health status.

**Features:**
- 4 columns: Trial, Healthy, Needs Attention, At Risk
- Automatic categorization by health status
- Quick visual overview of clinic health distribution
- Click cards to view details

**How to Use:**
1. Click the "📊 Kanban" button in the view switcher
2. View clinics organized by health status
3. Each column shows count of clinics in that status

### Map View
Geographic visualization of clinics (placeholder for future integration).

**Features:**
- Sidebar list of all clinics
- Geographic location display (to be implemented)
- Clinic count summary

**How to Use:**
1. Click the "🗺️ Mapa" button in the view switcher
2. Use the sidebar to navigate to specific clinics
3. View address information for each clinic

---

## Bulk Actions

### Selecting Clinics
You can select multiple clinics for bulk operations:

1. **Select Individual Clinics:**
   - Click the checkbox next to each clinic in list view

2. **Select All Clinics:**
   - Click the checkbox in the table header
   - All clinics on the current page will be selected

3. **Selection Indicator:**
   - A bulk actions bar appears showing the number of selected clinics
   - Example: "3 selecionado(s)"

### Bulk Activate/Deactivate

**To Activate Multiple Clinics:**
1. Select the clinics you want to activate
2. Click the "✅ Ativar" button in the bulk actions bar
3. Confirm the action
4. View the success/failure summary

**To Deactivate Multiple Clinics:**
1. Select the clinics you want to deactivate
2. Click the "🚫 Desativar" button in the bulk actions bar
3. Confirm the action
4. View the success/failure summary

### Bulk Tag Assignment

**To Add Tags to Multiple Clinics:**
1. Select the clinics
2. Click the "🏷️ Adicionar Tag" button
3. Enter the tag ID when prompted
4. View the success/failure summary

**Note:** Future versions will include a tag picker UI instead of requiring the tag ID.

### Clearing Selection
Click the "✕ Limpar" button to clear all selections and hide the bulk actions bar.

---

## Export Functionality

### Exporting Clinic Data

You can export selected clinics to various formats:

**Available Formats:**
- **CSV**: Comma-separated values (ideal for Excel, Google Sheets)
- **Excel**: Microsoft Excel format (.xlsx)
- **PDF**: Portable Document Format (ideal for printing)

**How to Export:**
1. Select the clinics you want to export
2. Click the "💾 Exportar" button in the bulk actions bar
3. Hover to see the dropdown menu
4. Click your preferred format:
   - 📄 CSV
   - 📊 Excel
   - 📑 PDF
5. The file will download automatically

**Exported Data Includes:**
- Clinic basic information (name, CNPJ, email, phone)
- Active status
- Creation date
- Health score (if enabled)
- Tags (if enabled)
- Usage metrics (if enabled)

**Export Progress:**
- While exporting, the button shows "⏳ Exportando..."
- Large exports may take a few seconds

---

## Automatic Tagging

The system automatically applies tags to clinics based on their characteristics.

### Automatic Tag Categories

1. **At-Risk Tag (⚠️)**
   - Applied to clinics inactive for 30+ days
   - Removed when clinic becomes active again

2. **High-Value Tag (💎)**
   - Applied to clinics with subscriptions ≥ R$ 1000/month
   - Helps identify premium clients

3. **New Tag (🆕)**
   - Applied to clinics created in last 30 days
   - Automatically removed after 30 days

4. **Active-User Tag (👥)**
   - Applied to clinics with recent user activity (last 7 days)
   - Indicates strong engagement

5. **Support-Heavy Tag (💬)**
   - Applied to clinics with 5+ support tickets in last 30 days
   - Helps prioritize support efforts

6. **Trial Tag (🔄)**
   - Applied to clinics in trial period
   - Removed when trial ends or converts to paid

### Tag Update Schedule
- Automatic tags are updated daily
- Tags are recalculated based on current clinic status
- Manual tags are never automatically removed

---

## Ownership Transfer

### Transferring Clinic Ownership

**Prerequisites:**
- Both users must be from the same clinic
- Current user must have Owner role
- New owner must be an active user

**How to Transfer:**
1. Go to the Cross-Tenant Users page
2. Find the new owner user
3. Click the ownership transfer option (feature being added)
4. Confirm the transfer
5. The system will:
   - Change new user's role to Owner
   - Change current owner's role to Admin
   - Log the transfer in audit trail

**Important Notes:**
- Ownership transfers are permanent
- The action is logged for security and auditing
- Current owner loses owner privileges but retains admin access
- Only one owner per clinic is allowed

---

## Advanced Filters

### Using Advanced Filters

**Available Filters:**
1. **Search**: Search by name, CNPJ, or email
2. **Health Status**: Filter by Healthy, Needs Attention, or At Risk
3. **Subscription Status**: Filter by Active, Trial, Expired, or Suspended
4. **Tags**: Filter by one or more tags

**How to Use:**
1. Click "🔍 Filtros Avançados" button
2. Set your filter criteria
3. Click "Aplicar Filtros"
4. View filtered results
5. Use "Limpar" to reset filters

### Segment Quick Filters

Quick access buttons for common filters:
- 🆕 **Novos**: Clinics created in last 30 days
- 🔄 **Trial**: Clinics in trial period
- ⚠️ **Em Risco**: Clinics with health issues
- ✅ **Saudáveis**: Healthy clinics
- 👀 **Precisa Atenção**: Clinics needing attention

Simply click any segment chip to instantly filter clinics.

---

## Performance Tips

1. **Large Datasets**: Use filters to narrow results before selecting all
2. **Bulk Operations**: Process in batches of 50-100 clinics at a time
3. **Export**: For exports >500 clinics, consider using filters to split exports
4. **View Modes**: Cards and Kanban views work best with <200 clinics per page

---

## Troubleshooting

### Common Issues

**Problem**: Bulk action fails for some clinics
- **Solution**: Check the error messages in the result summary
- Some clinics may be in a state that prevents the action

**Problem**: Export takes too long
- **Solution**: Reduce the number of selected clinics or disable optional data

**Problem**: Tags not updating automatically
- **Solution**: Tags update daily - wait 24 hours or contact support

**Problem**: Cannot transfer ownership
- **Solution**: Verify both users are from same clinic and new owner is active

---

## Best Practices

1. **Use Filters**: Always filter before bulk operations on large datasets
2. **Review Selection**: Double-check your selection before bulk operations
3. **Tag Management**: Use automatic tags to identify clinics needing attention
4. **Regular Exports**: Export clinic data monthly for reporting purposes
5. **View Modes**: Switch views based on your task:
   - List for detailed work
   - Cards for quick scanning
   - Kanban for health overview
   - Map for geographic insights

---

**Need Help?**
Contact the System Admin support team for assistance with any features described in this guide.

**Version:** 1.0  
**Last Updated:** January 2026
