# Consultation Form Profile Linking Guide

## Overview

This guide documents the implementation that links Access Profiles to Consultation Form Profiles, enabling automatic assignment of appropriate form templates based on clinic type during registration.

## Problem Solved

Previously, the system had:
- Access profiles for different clinic types (Medical, Dental, Psychology, etc.)
- Consultation form profiles for different specialties (Médico, Dentista, Psicólogo, etc.)
- **No connection between the two**

This meant that:
- Clinics couldn't automatically use the correct form templates for their specialty
- Manual configuration was required to link profiles to forms
- No standardized forms were pre-assigned during registration

## Solution

We now automatically link Access Profiles to Consultation Form Profiles based on clinic type during registration.

## Technical Implementation

### 1. Database Schema Changes

Added `ConsultationFormProfileId` column to `AccessProfiles` table:

```sql
ALTER TABLE "AccessProfiles"
ADD COLUMN "ConsultationFormProfileId" uuid NULL;

CREATE INDEX "IX_AccessProfiles_ConsultationFormProfileId"
ON "AccessProfiles" ("ConsultationFormProfileId");

ALTER TABLE "AccessProfiles"
ADD CONSTRAINT "FK_AccessProfiles_ConsultationFormProfiles_ConsultationFormPro~"
FOREIGN KEY ("ConsultationFormProfileId")
REFERENCES "ConsultationFormProfiles" ("Id")
ON DELETE SET NULL;
```

### 2. Domain Layer Changes

#### AccessProfile Entity

Added navigation property and helper methods:

```csharp
public Guid? ConsultationFormProfileId { get; private set; }
public ConsultationFormProfile? ConsultationFormProfile { get; private set; }

public void SetConsultationFormProfile(Guid? consultationFormProfileId)
{
    ConsultationFormProfileId = consultationFormProfileId;
    UpdateTimestamp();
}

public bool IsProfessionalProfile()
{
    return !Name.Contains("Proprietário") && 
           !Name.Contains("Recepção") && 
           !Name.Contains("Financeiro");
}

public static ProfessionalSpecialty GetProfessionalSpecialtyForClinicType(ClinicType clinicType)
{
    return clinicType switch
    {
        ClinicType.Medical => ProfessionalSpecialty.Medico,
        ClinicType.Dental => ProfessionalSpecialty.Dentista,
        ClinicType.Nutritionist => ProfessionalSpecialty.Nutricionista,
        ClinicType.Psychology => ProfessionalSpecialty.Psicologo,
        ClinicType.PhysicalTherapy => ProfessionalSpecialty.Fisioterapeuta,
        ClinicType.Veterinary => ProfessionalSpecialty.Veterinario,
        ClinicType.Other => ProfessionalSpecialty.Outro,
        _ => ProfessionalSpecialty.Medico
    };
}
```

#### ProfessionalSpecialty Enum

Added new specialty for veterinary clinics:

```csharp
public enum ProfessionalSpecialty
{
    Medico = 1,
    Psicologo = 2,
    Nutricionista = 3,
    Fisioterapeuta = 4,
    Dentista = 5,
    Enfermeiro = 6,
    TerapeutaOcupacional = 7,
    Fonoaudiologo = 8,
    Veterinario = 9,  // NEW
    Outro = 99
}
```

### 3. Application Layer Changes

#### RegistrationService

Updated to automatically link consultation form profiles during registration:

```csharp
// Get the appropriate consultation form profile for this clinic type
var specialty = AccessProfile.GetProfessionalSpecialtyForClinicType(clinicType);
var allSystemProfiles = await _consultationFormProfileRepository.GetSystemDefaultProfilesAsync("system");
var consultationFormProfile = allSystemProfiles.FirstOrDefault(p => p.Specialty == specialty);

// Create default access profiles
var defaultProfiles = AccessProfile.GetDefaultProfilesForClinicType(tenantId, clinic.Id, clinicType);

foreach (var profile in defaultProfiles)
{
    // Link consultation form profile to professional profiles only
    if (consultationFormProfile != null && profile.IsProfessionalProfile())
    {
        profile.SetConsultationFormProfile(consultationFormProfile.Id);
    }
    
    await _accessProfileRepository.AddAsync(profile);
}
```

#### AccessProfileService

Added method to update consultation form profile linkage:

```csharp
public async Task<AccessProfileDto> SetConsultationFormProfileAsync(
    Guid profileId, 
    Guid? consultationFormProfileId, 
    string tenantId)
{
    var profile = await _profileRepository.GetByIdAsync(profileId, tenantId);
    if (profile == null)
        throw new InvalidOperationException("Profile not found");

    // Validate consultation form profile exists if provided
    if (consultationFormProfileId.HasValue)
    {
        var formProfile = await _consultationFormProfileRepository
            .GetAllQueryable()
            .Where(p => p.Id == consultationFormProfileId.Value && 
                       (p.TenantId == "system" || p.TenantId == tenantId))
            .FirstOrDefaultAsync();
        
        if (formProfile == null)
            throw new InvalidOperationException("Consultation form profile not found");
    }

    profile.SetConsultationFormProfile(consultationFormProfileId);
    await _profileRepository.UpdateAsync(profile);

    return MapToDto(profile);
}
```

#### AccessProfileDto

Added fields to expose consultation form profile information:

```csharp
public class AccessProfileDto
{
    // ... existing fields ...
    public Guid? ConsultationFormProfileId { get; set; }
    public string? ConsultationFormProfileName { get; set; }
}
```

### 4. API Layer Changes

#### New Endpoint

Added endpoint to update consultation form profile for an access profile:

```http
PUT /api/accessprofiles/{id}/consultation-form-profile
Content-Type: application/json
Authorization: Bearer {token}

{
  "consultationFormProfileId": "guid-here"  // or null to unlink
}
```

**Response:**
```json
{
  "id": "profile-guid",
  "name": "Dentista",
  "description": "...",
  "consultationFormProfileId": "form-profile-guid",
  "consultationFormProfileName": "Dentista - Atendimento Odontológico",
  // ... other fields
}
```

### 5. Data Seeding

Added Veterinario consultation form profile to system defaults:

```csharp
var veterinarioProfile = new ConsultationFormProfile(
    "Veterinário - Atendimento Veterinário",
    "Perfil para veterinários com foco em atendimento de animais",
    ProfessionalSpecialty.Veterinario,
    "system",
    customFields: new List<CustomField>
    {
        new CustomField("especie_animal", "Espécie do Animal", CustomFieldType.TextoSimples, isRequired: true),
        new CustomField("raca", "Raça", CustomFieldType.TextoSimples),
        new CustomField("idade_animal", "Idade do Animal", CustomFieldType.TextoSimples, isRequired: true),
        new CustomField("peso", "Peso (kg)", CustomFieldType.Numero, isRequired: true),
        new CustomField("vacinacao_atualizada", "Vacinação Atualizada?", CustomFieldType.SimNao, isRequired: true),
        new CustomField("condicao_corporal", "Condição Corporal", CustomFieldType.SelecaoUnica, isRequired: true,
            options: new List<string> { "Muito magro", "Magro", "Ideal", "Acima do peso", "Obeso" })
    },
    isSystemDefault: true
);
```

## Clinic Type to Specialty Mapping

| Clinic Type | Professional Specialty | Consultation Form Profile |
|------------|----------------------|--------------------------|
| Medical | Medico | Médico - Padrão CFM 1.821 |
| Dental | Dentista | Dentista - Atendimento Odontológico |
| Nutritionist | Nutricionista | Nutricionista - Avaliação Nutricional |
| Psychology | Psicologo | Psicólogo - Saúde Mental |
| PhysicalTherapy | Fisioterapeuta | Fisioterapeuta - Avaliação Física |
| Veterinary | Veterinario | Veterinário - Atendimento Veterinário |
| Other | Outro | (None by default) |

## Usage Examples

### For New Clinics

When registering a new clinic, the system automatically:

1. Detects the clinic type selected
2. Maps it to the appropriate professional specialty
3. Finds the system default consultation form profile for that specialty
4. Creates access profiles (Owner, Reception, Financial, Professional)
5. Links the professional profile to the consultation form profile

**Example Registration Flow:**

```
Clinic Type: Dental
↓
Professional Specialty: Dentista
↓
Consultation Form Profile: "Dentista - Atendimento Odontológico"
↓
Access Profiles Created:
- Proprietário (no form profile linked)
- Recepção/Secretaria (no form profile linked)
- Financeiro (no form profile linked)
- Dentista (linked to "Dentista - Atendimento Odontológico")
```

### For Existing Clinics

#### Create Missing Profiles

```bash
POST /api/accessprofiles/create-defaults-by-type
Authorization: Bearer {token}
```

This will:
- Check the clinic's type
- Create any missing default profiles
- Link appropriate consultation form profiles

#### Change Consultation Form Profile

```bash
PUT /api/accessprofiles/{profile-id}/consultation-form-profile
Authorization: Bearer {token}
Content-Type: application/json

{
  "consultationFormProfileId": "new-form-profile-guid"
}
```

### Retrieve Profile Information

```bash
GET /api/accessprofiles/{profile-id}
Authorization: Bearer {token}
```

**Response includes:**
```json
{
  "id": "profile-guid",
  "name": "Dentista",
  "consultationFormProfileId": "form-profile-guid",
  "consultationFormProfileName": "Dentista - Atendimento Odontológico",
  "permissions": ["patients.view", "patients.create", ...],
  // ...
}
```

## Security

### Authorization

- Only clinic owners and system administrators can:
  - Create access profiles
  - Link/update consultation form profiles
  - Assign profiles to users

### Data Validation

- Validates that consultation form profile exists before linking
- Supports both system default profiles (tenantId: "system") and clinic-specific profiles
- Prevents information disclosure by returning 404 for profiles in other clinics

### Database Constraints

- Foreign key with SET NULL on delete: If a consultation form profile is deleted, the reference is automatically cleared (not blocked)
- Nullable field: Access profiles can exist without a linked consultation form profile

## Benefits

1. **Automatic Configuration**: Clinics get the correct form templates immediately upon registration
2. **Type Safety**: Enum-based mapping prevents errors
3. **Flexibility**: Owners can change form profiles if needed
4. **Scalability**: Easy to add new clinic types and specialties
5. **Consistency**: All clinics of the same type use the same form structure by default
6. **Maintainability**: Centralized logic for profile-to-form mapping

## Migration Path

### For Existing Data

Existing clinics registered before this change will:
1. Continue to function with their existing access profiles
2. Can call `/api/accessprofiles/create-defaults-by-type` to add type-specific profiles with linked forms
3. Can manually link consultation form profiles using the new endpoint
4. No data loss or breaking changes

### Database Migration

The migration is backward compatible:
- `ConsultationFormProfileId` is nullable
- Existing records have NULL for this field
- No required changes to existing data

## Future Enhancements

Potential improvements:
1. **Frontend UI**: Dropdown to select/change consultation form profiles during user creation
2. **Specialty-Specific Screens**: Different attendance screens based on linked form profile
3. **Custom Form Profiles**: Allow clinics to create custom consultation form profiles
4. **Form Templates**: Multiple form templates per specialty
5. **Profile Inheritance**: Child profiles that inherit form settings from parent profiles

## Testing Considerations

### Unit Tests

Test cases to cover:
- Clinic type to specialty mapping
- Professional profile identification (IsProfessionalProfile)
- Form profile linkage during registration
- Form profile update functionality
- Validation of non-existent form profiles

### Integration Tests

Test scenarios:
1. Register clinic with each clinic type → Verify correct form profile linked
2. Create default profiles for existing clinic → Verify correct linking
3. Update form profile for access profile → Verify changes persisted
4. Delete consultation form profile → Verify SET NULL behavior
5. Attempt to link non-existent form profile → Verify error handling

### Manual Testing

Verification steps:
1. Register new dental clinic → Check if Dentista profile has dental form linked
2. Register new veterinary clinic → Check if Veterinário profile has vet form linked
3. Change form profile via API → Verify update in database
4. View profile via API → Verify form profile information returned

## Troubleshooting

### Issue: Professional Profile Not Linked

**Symptoms:** Access profile created but no consultation form profile linked

**Possible Causes:**
1. System default form profiles not seeded in database
2. Clinic type doesn't match any specialty
3. Profile name doesn't match expected pattern

**Solution:**
- Ensure DataSeederService has run to create system default profiles
- Verify clinic type is one of the supported types
- Manually link using the API endpoint

### Issue: Cannot Find Consultation Form Profile

**Symptoms:** Error when trying to link form profile

**Possible Causes:**
1. Form profile doesn't exist
2. Form profile has wrong tenantId
3. Form profile was deleted

**Solution:**
- Verify form profile exists in database
- Check that tenantId is either "system" or matches the clinic's tenant
- Create or restore the form profile if needed

## References

- **Migration**: `20260207152902_AddConsultationFormProfileIdToAccessProfile.cs`
- **Entity**: `src/MedicSoft.Domain/Entities/AccessProfile.cs`
- **Service**: `src/MedicSoft.Application/Services/AccessProfileService.cs`
- **Controller**: `src/MedicSoft.Api/Controllers/AccessProfilesController.cs`
- **Data Seeder**: `src/MedicSoft.Application/Services/DataSeederService.cs`

## Conclusion

This implementation successfully bridges the gap between Access Profiles and Consultation Form Profiles, enabling a truly multi-professional system where each clinic type automatically gets the correct form templates for their specialty. The solution is flexible, maintainable, and provides a solid foundation for future enhancements in specialty-specific workflows.
