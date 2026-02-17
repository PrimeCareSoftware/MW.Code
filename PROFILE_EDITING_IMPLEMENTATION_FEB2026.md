# Profile Editing Implementation - February 2026

## Problem Statement (Portuguese)
Ao tentar editar um perfil através de uma clínica, estava ocorrendo o erro "message": "Cannot modify default profiles". Era necessário permitir a edição, porém mantendo a configuração do perfil salva somente para a respectiva clínica.

## Problem Statement (English)
When trying to edit a profile through a clinic, an error was occurring: "Cannot modify default profiles". It was necessary to allow editing, but keep the profile configuration saved only for the respective clinic.

## Solution

The solution implements a **copy-on-write** pattern for default profiles:

### Before
- Attempting to edit a default profile threw an error: "Cannot modify default profiles"
- Clinic owners could not customize default profiles for their specific needs
- The only option was to create entirely new custom profiles from scratch

### After
- When a clinic owner edits a default profile, the system automatically creates a clinic-specific copy
- The copy is customized with the requested changes
- The original default profile remains unchanged for other clinics
- Each clinic can have its own customized version of any default profile

## Technical Implementation

### Modified File
`src/MedicSoft.Application/Services/AccessProfileService.cs`

### Changes to `UpdateAsync` Method

```csharp
public async Task<AccessProfileDto> UpdateAsync(Guid id, UpdateAccessProfileDto dto, string tenantId)
{
    var profile = await _profileRepository.GetByIdAsync(id, tenantId);
    if (profile == null)
        throw new InvalidOperationException("Profile not found");

    // NEW: If trying to edit a default profile, create a clinic-specific copy instead
    if (profile.IsDefault)
    {
        if (!profile.ClinicId.HasValue)
            throw new InvalidOperationException("Cannot modify default profiles without a clinic context");

        // Create a clinic-specific copy of the default profile
        var clinicSpecificProfile = new AccessProfile(
            dto.Name, 
            dto.Description, 
            tenantId, 
            profile.ClinicId.Value, 
            isDefault: false,  // Mark as non-default
            consultationFormProfileId: profile.ConsultationFormProfileId
        );

        // Copy or update permissions
        if (dto.Permissions != null && dto.Permissions.Any())
        {
            clinicSpecificProfile.SetPermissions(dto.Permissions);
        }
        else
        {
            // Copy permissions from the default profile
            clinicSpecificProfile.SetPermissions(profile.GetPermissionKeys());
        }

        await _profileRepository.AddAsync(clinicSpecificProfile);
        return MapToDto(clinicSpecificProfile);
    }

    // Existing behavior for custom profiles (IsDefault = false)
    profile.Update(dto.Name, dto.Description);
    if (dto.Permissions != null)
    {
        profile.SetPermissions(dto.Permissions);
    }
    await _profileRepository.UpdateAsync(profile);
    return MapToDto(profile);
}
```

## Key Features

1. **Automatic Copy Creation**: When editing a default profile, the system creates a new clinic-specific profile automatically
2. **Preserves Original**: The original default profile remains available for other clinics
3. **Maintains Associations**: The consultation form profile link is preserved in the copy
4. **Permission Flexibility**: Permissions can be updated or copied from the original
5. **Clear Identification**: Copied profiles are marked with `IsDefault = false`

## Testing

### Unit Tests Added
Location: `tests/MedicSoft.Test/Services/AccessProfileServiceTests.cs`

Test Cases:
1. **UpdateAsync_WithDefaultProfile_CreatesClinicSpecificCopy**: Verifies that editing a default profile creates a new clinic-specific profile
2. **UpdateAsync_WithCustomProfile_UpdatesDirectly**: Confirms existing custom profiles are updated in place
3. **UpdateAsync_WithDefaultProfile_PreservesConsultationFormProfile**: Ensures consultation form links are maintained
4. **UpdateAsync_WithDefaultProfile_CopiesPermissionsWhenNotProvided**: Tests permission copying behavior
5. **UpdateAsync_WithDefaultProfileWithoutClinic_ThrowsException**: Validates error handling for system-wide defaults

## API Behavior

### Endpoint
`PUT /api/AccessProfiles/{id}`

### Request Example
```json
{
  "name": "Médico Customizado",
  "description": "Perfil médico customizado para esta clínica",
  "permissions": [
    "patients.view",
    "appointments.view",
    "medical-records.view"
  ]
}
```

### Response (Success)
```json
{
  "id": "new-guid-here",
  "name": "Médico Customizado",
  "description": "Perfil médico customizado para esta clínica",
  "isDefault": false,
  "isActive": true,
  "clinicId": "clinic-guid",
  "permissions": [
    "patients.view",
    "appointments.view",
    "medical-records.view"
  ]
}
```

### Response (Error - No Clinic Context)
```json
{
  "message": "Cannot modify default profiles without a clinic context"
}
```

## Impact Analysis

### Security
✅ **No Security Concerns**
- Tenant isolation is maintained through existing filters
- Only clinic owners can edit profiles (existing authorization)
- New profiles are properly associated with the clinic

### Performance
✅ **Minimal Impact**
- Only one additional database insert per profile customization
- No impact on read operations
- No additional queries for existing custom profiles

### Database
✅ **No Schema Changes Required**
- Uses existing `AccessProfiles` table
- No migrations needed
- Existing data remains valid

### Backwards Compatibility
✅ **Fully Compatible**
- Existing custom profiles continue to work unchanged
- Default profiles remain available
- No breaking changes to API contracts

## User Experience

### Before
❌ Clinic owners saw error message when trying to edit default profiles
❌ Had to create entirely new profiles from scratch
❌ Lost default profile configurations

### After
✅ Clinic owners can customize default profiles seamlessly
✅ Changes are saved only for their clinic
✅ Default profile permissions serve as a starting point
✅ No error messages for legitimate operations

## Future Considerations

### Potential Enhancements
1. **Profile Versioning**: Track version history of profile customizations
2. **Reset to Default**: Add ability to reset a customized profile back to the system default
3. **Bulk Operations**: Allow applying customizations to multiple profiles at once
4. **Template System**: Create profile templates that can be shared across clinics

### Monitoring
- Track frequency of profile customizations
- Monitor number of custom profiles per clinic
- Identify commonly customized default profiles for potential improvements

## Related Files

- Core Service: `src/MedicSoft.Application/Services/AccessProfileService.cs`
- Entity: `src/MedicSoft.Domain/Entities/AccessProfile.cs`
- Controller: `src/MedicSoft.Api/Controllers/AccessProfilesController.cs`
- DTOs: `src/MedicSoft.Application/DTOs/AccessProfileDto.cs`
- Repository: `src/MedicSoft.Repository/Repositories/AccessProfileRepository.cs`
- Tests: `tests/MedicSoft.Test/Services/AccessProfileServiceTests.cs`

## Migration Path

No database migrations or data migrations are required. The feature works with the existing schema and data.

## Rollback Plan

If needed, the change can be rolled back by:
1. Reverting the `UpdateAsync` method in `AccessProfileService.cs`
2. Custom profiles created through this feature will remain in the database
3. Users can continue to use these custom profiles, but won't be able to create new ones via editing defaults

## Conclusion

This implementation successfully resolves the issue where clinic owners could not edit default profiles. The solution maintains data integrity, preserves the original default profiles for other clinics, and provides a seamless user experience through automatic copy-on-write behavior.
