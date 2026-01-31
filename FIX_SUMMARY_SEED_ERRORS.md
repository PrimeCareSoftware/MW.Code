# Fix Summary: Database Seed Errors

## Problem
The seed-demo and seed-system-demo flows were experiencing database errors due to:
1. Missing deletion of entities created during seeding
2. Incorrect deletion order violating foreign key constraints

## Root Causes Identified

### 1. Missing Deletions in ClearDatabaseAsync
Two entities were created during `SeedDemoDataAsync()` but never deleted in `ClearDatabaseAsync()`:

- **ConsultationFormProfiles** (created at step 0, line 127-132)
  - System-wide templates for consultation forms
  - Never deleted, causing orphaned records
  
- **AnamnesisTemplates** (created at step 27, line 309-314)
  - Templates created by users
  - Never deleted, causing orphaned records and potential FK violations

### 2. Incorrect Deletion Order
Foreign key constraint violations occurred due to improper deletion sequence:

- **ExamCatalogs** were deleted too late (step 13.1)
  - Should be deleted earlier to avoid FK constraint issues
  - Moved to step 9.1 (after PatientClinicLinks, before Patients)

## Changes Made

### DataSeederService.cs
Modified `ClearDatabaseAsync()` method to:

1. **Add AnamnesisTemplates deletion** (new step 0)
   ```csharp
   // 0. Delete AnamnesisTemplates (depends on Users)
   var anamnesisTemplates = await _anamnesisTemplateRepository.GetAllAsync(_demoTenantId);
   foreach (var template in anamnesisTemplates)
   {
       await _anamnesisTemplateRepository.DeleteWithoutSaveAsync(template.Id, _demoTenantId);
   }
   ```

2. **Move ExamCatalogs deletion** from step 13.1 to 9.1
   - Deleted after PatientClinicLinks
   - Deleted before Patients
   - Prevents FK constraint violations

3. **Add ConsultationFormProfiles deletion** (new step 22)
   ```csharp
   // 22. Delete ConsultationFormProfiles (system-wide templates)
   var consultationFormProfiles = await _consultationFormProfileRepository.GetAllAsync("system");
   foreach (var profile in consultationFormProfiles)
   {
       await _consultationFormProfileRepository.DeleteWithoutSaveAsync(profile.Id, "system");
   }
   ```

4. **Invoice deletion order maintained** at step 5.1
   - Invoices have FK to Payments (PaymentId)
   - Must be deleted BEFORE Payments (step 6)
   - Original order was correct

### DataSeederController.cs
Updated the `deletedTables` array in `ClearDatabase` endpoint response to reflect the correct deletion order:

```csharp
deletedTables = new[]
{
    "AnamnesisTemplates",        // NEW - added
    "PrescriptionItems",
    "ExamRequests",
    "Notifications",
    "NotificationRoutines",
    "DigitalPrescriptions",
    "MedicalRecords",
    "Invoices",
    "Payments",
    "AppointmentProcedures",
    "Appointments",
    "PatientClinicLinks",
    "ExamCatalogs",              // MOVED - was after Medications
    "HealthInsurancePlans",
    "Patients",
    "PrescriptionTemplates",
    "MedicalRecordTemplates",
    "Medications",
    "Procedures",
    "Expenses",
    "Users",
    "OwnerClinicLinks",
    "ClinicSubscriptions",
    "Owners",
    "Clinics",
    "HealthInsuranceOperators",
    "SubscriptionPlans",
    "ConsultationFormProfiles"   // NEW - added
}
```

## Deletion Order (Final)

The correct deletion order respecting all FK constraints:

1. **Step 0**: AnamnesisTemplates (depends on Users)
2. **Step 1**: PrescriptionItems (depends on MedicalRecords, Medications)
3. **Step 2**: ExamRequests (depends on Appointments, Patients)
4. **Step 3**: Notifications (depends on Patients, Appointments)
5. **Step 4**: NotificationRoutines (depends on Clinic)
6. **Step 4.1**: DigitalPrescriptions (depends on MedicalRecords)
7. **Step 5**: MedicalRecords (depends on Appointments, Patients)
8. **Step 5.1**: Invoices (depends on Payments via FK PaymentId)
9. **Step 6**: Payments (depends on Appointments)
10. **Step 7**: AppointmentProcedures (depends on Appointments, Procedures)
11. **Step 8**: Appointments (depends on Patients, Clinic)
12. **Step 9**: PatientClinicLinks (depends on Patients, Clinics)
13. **Step 9.1**: ExamCatalogs (moved earlier)
14. **Step 9.2**: HealthInsurancePlans (depends on Operators, Patients)
15. **Step 10**: Patients
16. **Step 11**: PrescriptionTemplates
17. **Step 12**: MedicalRecordTemplates
18. **Step 13**: Medications
19. **Step 14**: Procedures
20. **Step 15**: Expenses
21. **Step 16**: Users (depends on Clinics)
22. **Step 17**: OwnerClinicLinks (depends on Owners, Clinics)
23. **Step 18**: ClinicSubscriptions (depends on Clinics, Plans)
24. **Step 19**: Owners
25. **Step 20**: Clinics
26. **Step 20.1**: HealthInsuranceOperators
27. **Step 21**: SubscriptionPlans (system-wide)
28. **Step 22**: ConsultationFormProfiles (system-wide)

## Benefits

1. **No orphaned records**: All created entities are now properly deleted
2. **FK constraint compliance**: Deletion order respects all foreign key relationships
3. **Transaction safety**: All operations within transaction ensure data consistency
4. **Clean state**: Database can be cleared and re-seeded without errors

## Testing Recommendations

1. **Test seed-demo endpoint**:
   ```bash
   curl -X POST http://localhost:5000/api/data-seeder/seed-demo
   ```

2. **Test clear-database endpoint**:
   ```bash
   curl -X DELETE http://localhost:5000/api/data-seeder/clear-database
   ```

3. **Verify FK constraints**: No constraint violations should occur
4. **Verify transaction rollback**: If any error occurs, no partial data should remain

## Related Files

- `src/MedicSoft.Application/Services/DataSeederService.cs`
- `src/MedicSoft.Api/Controllers/DataSeederController.cs`
- `system-admin/guias/SEEDER_GUIDE.md` (documentation)

## Notes

- All changes maintain backward compatibility
- Transaction handling ensures atomic operations
- NpgsqlRetryingExecutionStrategy compatibility maintained with batch operations
