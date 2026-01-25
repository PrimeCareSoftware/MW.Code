# Missing Database Tables - Migration Issues

## Overview
During investigation of the Subdomain column issue, multiple missing database tables were discovered. The EF Core migrations reference tables that are never created.

## Issue
Several migrations contain `AlterColumn` statements for tables that don't have corresponding `CreateTable` statements in any prior migration. This causes migration failures when running against a fresh database.

## Affected Migrations
The following migration files try to ALTER tables without creating them first:
- `20251221154116_AddTicketSystem.cs` - Alters WaitingQueueEntries and WaitingQueueConfigurations
- Other migrations may have similar issues

## Missing Tables (Preliminary List)
Based on model snapshot analysis, the following tables may be missing CREATE statements:

### Confirmed Missing (cause migration failures):
- **WaitingQueueEntries** - Referenced but never created
- **WaitingQueueConfigurations** - Referenced but never created

### Needs Verification:
- AccessProfiles
- ClinicCustomizations
- ClinicalExaminations  
- DiagnosticHypotheses
- DigitalPrescriptionItems
- DigitalPrescriptions
- ExamCatalogs
- InformedConsents
- Notifications
- OwnerClinicLinks
- PrescriptionSequenceControls
- ProfilePermissions
- SNGPCReports
- SalesFunnelMetrics
- TherapeuticPlans
- TicketAttachments
- TicketComments
- TicketHistory
- Tickets
- owner_sessions
- user_sessions

**Note**: Some of these may exist in the initial migration using raw SQL with different naming conventions. A thorough audit is needed.

## Impact
- Fresh database installations will fail during migration
- API endpoints that query these tables will fail with "relation does not exist" errors
- Similar to the Subdomain column issue

## Recommended Actions
1. **Immediate**: Apply the Subdomain column fix (already implemented)
2. **Short-term**: Create idempotent SQL scripts for critical missing tables
3. **Long-term**: 
   - Audit all migrations for consistency
   - Ensure all entity classes have corresponding table creation migrations
   - Consider migrating away from raw SQL to standard EF Core migrations
   - Add integration tests for migration scenarios

## Related Issues
- Issue #1: Column c.Subdomain does not exist (FIXED)
- Issue #2: Multiple tables missing CREATE statements (THIS DOCUMENT)

## Status
🔴 **CRITICAL** - Blocks fresh installations and some features

## Files to Review
- `/src/MedicSoft.Repository/Migrations/PostgreSQL/*.cs`
- `/src/MedicSoft.Repository/Context/MedicSoftDbContext.cs`
- `/src/MedicSoft.Domain/Entities/*.cs`
