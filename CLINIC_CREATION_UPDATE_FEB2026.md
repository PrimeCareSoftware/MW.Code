# Clinic Creation Update - February 2026

## Overview

This document describes the updates made to the clinic creation functionality in the System Admin module to properly support business type and primary specialty configuration.

## Problem Statement

> "Revise e implemente as novas regras de criacao de clinica no cadastro de clinica em system admin, pois esta desatualizado, apos isso revise os campos que nao estao com mascara"

**Translation:**
> "Revise and implement the new clinic creation rules in the clinic registration in system admin, as it is outdated, after that review the fields that do not have masks"

## Issues Identified

### 1. Business Configuration Not Respected
**Problem:** The frontend form collected `businessType` and `primarySpecialty` values, but the backend ignored them and always created clinics with hardcoded values (SmallClinic/Medico).

**Impact:** All clinics were created with the same configuration regardless of user selection, resulting in:
- Incorrect default features enabled/disabled
- Wrong access profiles created (always medical profiles)
- Inappropriate consultation forms and templates

### 2. Missing Input Masks
**Problem:** Critical fields like CNPJ and phone lacked input masks, making data entry error-prone.

**Impact:**
- Difficult for users to enter properly formatted data
- Inconsistent data format in database
- Harder to validate and display data

## Solution Implemented

### Backend Changes

**File:** `src/MedicSoft.Api/Controllers/SystemAdminController.cs`

#### 1. Updated CreateClinicRequest DTO

```csharp
public class CreateClinicRequest
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string OwnerUsername { get; set; } = string.Empty;
    public string OwnerPassword { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public string PlanId { get; set; } = string.Empty;
    public int? BusinessType { get; set; }        // NEW
    public int? PrimarySpecialty { get; set; }    // NEW
}
```

**Key Points:**
- Fields are optional (nullable) for backward compatibility
- Default to SmallClinic (2) and Medico (1) if not provided

#### 2. Updated Clinic Creation Logic

```csharp
// Use values from request, or default to SmallClinic and Medico
var businessType = request.BusinessType.HasValue 
    ? (Domain.Enums.BusinessType)request.BusinessType.Value 
    : Domain.Enums.BusinessType.SmallClinic;

var primarySpecialty = request.PrimarySpecialty.HasValue 
    ? (Domain.Enums.ProfessionalSpecialty)request.PrimarySpecialty.Value 
    : Domain.Enums.ProfessionalSpecialty.Medico;

await _businessConfigService.CreateAsync(
    clinic.Id,
    businessType,
    primarySpecialty,
    tenantId
);
```

**Benefits:**
- Respects user's selection from the form
- Creates appropriate business configuration
- Enables correct features for the selected clinic type
- Creates appropriate access profiles (e.g., Dentista for dental clinics)

#### 3. Enhanced Logging

```csharp
_logger.LogInformation(
    "Business configuration created successfully for clinic {ClinicId} with tenant {TenantId}, BusinessType: {BusinessType}, PrimarySpecialty: {PrimarySpecialty}",
    clinic.Id, tenantId, businessType, primarySpecialty);
```

**Benefits:**
- Better troubleshooting
- Audit trail for configuration choices
- Easier to identify misconfigured clinics

### Frontend Changes

**Files:**
- `frontend/mw-system-admin/src/app/pages/clinics/clinic-create.ts`
- `frontend/mw-system-admin/src/app/pages/clinics/clinic-create.html`
- `frontend/mw-system-admin/src/app/pages/clinics/clinic-create.scss`

#### 1. Added Input Masks

**CNPJ Mask:**
```html
<input
  type="text"
  id="document"
  [(ngModel)]="formData.document"
  name="document"
  required
  placeholder="00.000.000/0000-00"
  appCnpjMask
/>
```

**Result:** Automatically formats as user types: `12.345.678/0001-90`

**Phone Mask:**
```html
<input
  type="tel"
  id="phone"
  [(ngModel)]="formData.phone"
  name="phone"
  required
  placeholder="(00) 00000-0000"
  appPhoneMask
/>
```

**Result:** Automatically formats as user types: `(11) 98765-4321`

#### 2. Added Help Text for Business Fields

**Business Type:**
```html
<select id="businessType" [(ngModel)]="formData.businessType" name="businessType" required>
  <option [value]="1">Profissional Solo</option>
  <option [value]="2">Clínica Pequena (2-5 profissionais)</option>
  <option [value]="3">Clínica Média (6-20 profissionais)</option>
  <option [value]="4">Clínica Grande (20+ profissionais)</option>
</select>
<small class="help-text">Define funcionalidades e configurações iniciais da clínica</small>
```

**Primary Specialty:**
```html
<select id="primarySpecialty" [(ngModel)]="formData.primarySpecialty" name="primarySpecialty" required>
  <option [value]="1">Médico</option>
  <option [value]="2">Psicólogo</option>
  <option [value]="3">Nutricionista</option>
  <option [value]="4">Fisioterapeuta</option>
  <option [value]="5">Dentista</option>
  <option [value]="6">Enfermeiro</option>
  <option [value]="7">Terapeuta Ocupacional</option>
  <option [value]="8">Fonoaudiólogo</option>
  <option [value]="9">Veterinário</option>
  <option [value]="99">Outro</option>
</select>
<small class="help-text">Configura perfis de acesso e formulários apropriados</small>
```

#### 3. Enhanced Password Validation

**Real-time Mismatch Detection:**
```html
<input type="password" id="confirmPassword" [(ngModel)]="confirmPassword" 
       name="confirmPassword" required placeholder="Repita a senha" minlength="8" />
@if (confirmPassword && formData.ownerPassword && confirmPassword !== formData.ownerPassword) {
  <small class="error-text">As senhas não coincidem</small>
}
```

**Accurate Help Text:**
```html
<small class="help-text">Mínimo 8 caracteres (recomendado: letras, números e símbolos)</small>
```

#### 4. Improved Section Description

```html
<div class="form-section">
  <h2>Configuração de Negócio</h2>
  <p class="section-description">
    Estes campos definem o perfil da clínica e determinam quais funcionalidades serão habilitadas automaticamente. 
    O sistema irá criar perfis de acesso apropriados (ex: Dentista para clínicas odontológicas, Psicólogo para clínicas de psicologia) 
    e configurar módulos específicos baseados no tipo de negócio escolhido.
  </p>
</div>
```

#### 5. Added CSS Styling

```scss
.help-text {
  font-size: 12px;
  color: var(--gray-500);
  margin-top: 4px;
  font-style: italic;
}

.error-text {
  font-size: 12px;
  color: var(--error-600);
  margin-top: 4px;
  font-weight: 500;
}
```

## Business Type Impact

When a clinic is created with a specific business type, the following occurs:

### 1. Solo Practitioner (Profissional Solo)
- **Features:** Basic features enabled, minimal overhead
- **Profiles:** Owner, Professional (1)
- **Use Case:** Individual practitioners

### 2. Small Clinic (Clínica Pequena) - 2-5 professionals
- **Features:** Standard features, basic reporting
- **Profiles:** Owner, Reception, Financial, Professional
- **Use Case:** Small practices

### 3. Medium Clinic (Clínica Média) - 6-20 professionals
- **Features:** Advanced features, multi-room support, queue management
- **Profiles:** Full set including specialized roles
- **Use Case:** Growing practices

### 4. Large Clinic (Clínica Grande) - 20+ professionals
- **Features:** All features, BI reports, API access
- **Profiles:** Complete role hierarchy
- **Use Case:** Large healthcare organizations

## Primary Specialty Impact

When a clinic is created with a specific specialty, appropriate access profiles are created:

| Specialty | Access Profile Created | Specific Features |
|-----------|----------------------|-------------------|
| Médico | Médico | Medical prescriptions, CID-10, TISS |
| Dentista | Dentista | Odontogram, dental procedures |
| Psicólogo | Psicólogo | Session notes, therapeutic assessment |
| Nutricionista | Nutricionista | Meal plans, anthropometric evaluation |
| Fisioterapeuta | Fisioterapeuta | Movement assessment, exercise protocols |
| Veterinário | Veterinário | Animal records, veterinary procedures |
| Others | Professional | Generic consultation forms |

## Testing

### Backend Build
```bash
cd /home/runner/work/MW.Code/MW.Code
dotnet restore src/MedicSoft.Api/MedicSoft.Api.csproj
dotnet build src/MedicSoft.Api/MedicSoft.Api.csproj
```

**Result:** ✅ Build succeeded with 0 errors (339 pre-existing warnings)

### Frontend Build
```bash
cd /home/runner/work/MW.Code/MW.Code/frontend/mw-system-admin
npm install
npm run build
```

**Result:** ✅ Build succeeded with 0 errors (bundle size warnings only)

### Code Review
**Result:** ✅ Passed - 1 suggestion addressed (password help text accuracy)

### Security Scan (CodeQL)
**Result:** ✅ Passed - 0 vulnerabilities found

## Migration Impact

### Existing Clinics
- No impact on existing clinics
- Existing configurations remain unchanged
- Can be updated via business configuration screen

### New Clinics
- Must select business type and specialty during creation
- Appropriate configuration created automatically
- Can be customized later if needed

## Backward Compatibility

The implementation maintains full backward compatibility:

1. **API Compatibility:** BusinessType and PrimarySpecialty are optional fields
2. **Default Behavior:** Defaults to SmallClinic/Medico if not provided
3. **Existing Clients:** Continue to work without changes

## API Documentation

### Create Clinic Endpoint

**Endpoint:** `POST /api/system-admin/clinics`

**Request Body:**
```json
{
  "name": "Clínica Exemplo",
  "document": "12.345.678/0001-90",
  "email": "contato@exemplo.com",
  "phone": "(11) 98765-4321",
  "address": "Rua Exemplo, 123 - São Paulo, SP",
  "ownerUsername": "admin.exemplo",
  "ownerPassword": "SenhaForte123!",
  "ownerFullName": "João da Silva",
  "planId": "uuid-do-plano",
  "businessType": 2,
  "primarySpecialty": 5
}
```

**Business Type Values:**
- `1`: Solo Practitioner
- `2`: Small Clinic (2-5 professionals)
- `3`: Medium Clinic (6-20 professionals)
- `4`: Large Clinic (20+ professionals)

**Primary Specialty Values:**
- `1`: Médico
- `2`: Psicólogo
- `3`: Nutricionista
- `4`: Fisioterapeuta
- `5`: Dentista
- `6`: Enfermeiro
- `7`: Terapeuta Ocupacional
- `8`: Fonoaudiólogo
- `9`: Veterinário
- `99`: Outro

**Response:**
```json
{
  "message": "Clínica criada com sucesso",
  "clinicId": "uuid-da-clinica",
  "tenantId": "uuid-do-tenant"
}
```

## Benefits

### For System Admins
1. ✅ More control over clinic configuration
2. ✅ Better data entry with auto-formatting
3. ✅ Clearer understanding of configuration impact
4. ✅ Fewer data entry errors

### For Clinic Owners
1. ✅ Appropriate features enabled from the start
2. ✅ Correct access profiles created automatically
3. ✅ Relevant consultation forms and templates
4. ✅ Better user experience

### For the System
1. ✅ Consistent data format in database
2. ✅ Better logging and troubleshooting
3. ✅ More accurate analytics and reporting
4. ✅ Easier to identify misconfigured clinics

## Related Documentation

- [Business Configuration Guide](BUSINESS_CONFIG_AUTO_CREATE_IMPLEMENTATION.md)
- [Clinic Type Profiles](CLINIC_TYPE_PROFILES_GUIDE.md)
- [System Admin Guide](SYSTEM_ADMIN_USER_GUIDE.md)

## Conclusion

The clinic creation process has been successfully updated to:
1. ✅ Properly use business type and primary specialty selections
2. ✅ Add input masks for CNPJ and phone fields
3. ✅ Improve user experience with help text and validation
4. ✅ Maintain backward compatibility
5. ✅ Pass all quality checks (build, code review, security scan)

The implementation is complete and ready for deployment.
