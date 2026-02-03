# Multi-Professional Specialized Attendance Screens - Complete Guide

## 📋 Overview

This implementation adds specialized attendance screens for different healthcare professional specialties to the Omni Care system. The system dynamically adapts the UI based on the professional's specialty, showing appropriate terminology and custom assessment fields.

**Status**: ✅ Complete and Ready for Testing  
**Implementation Date**: February 2026  
**Version**: 1.0

## 🎯 Supported Specialties

| # | Specialty | Portuguese | Terminology | Custom Fields |
|---|-----------|------------|-------------|---------------|
| 1 | Doctor | Médico | Consulta | Standard CFM 1.821 |
| 2 | Psychologist | Psicólogo | Sessão | Mental health assessment |
| 3 | Nutritionist | Nutricionista | Consulta | Dietary evaluation |
| 4 | Physiotherapist | Fisioterapeuta | Sessão | Mobility assessment |
| 5 | Dentist | Dentista | Consulta | Oral examination |
| 6 | Nurse | Enfermeiro | Atendimento | Vital signs & procedures |
| 7 | Occupational Therapist | Terapeuta Ocupacional | Sessão | Functional assessment |
| 8 | Speech Therapist | Fonoaudiólogo | Sessão | Communication assessment |

## ✨ Key Features

### 1. Dynamic Terminology

The UI automatically adapts terminology based on the professional's specialty:

- **Appointment** term changes: "Consulta" (Doctor) → "Sessão" (Psychologist)
- **Professional** term changes: "Médico" → "Psicólogo"  
- **Document** term changes: "Prontuário Médico" → "Relatório Psicológico"
- **Client** term changes (some specialties use "Cliente" instead of "Paciente")

### 2. Specialty-Specific Custom Fields

Each specialty has predefined custom fields that automatically appear:

#### 🧠 Psychologist
- Motivo da Consulta (Long text, required)
- Histórico Psiquiátrico (Long text)
- Nível de Ansiedade 0-10 (Number, required)
- Uso de Medicação Psiquiátrica (Yes/No, required)

#### 🥗 Nutritionist
- Peso Atual kg (Number, required)
- Altura cm (Number, required)  
- Objetivo Nutricional (Single choice, required)
- Restrições Alimentares (Multi-select)

#### 💪 Physiotherapist
- Área Acometida (Text, required)
- Nível de Dor 0-10 (Number, required)
- Avaliação de Mobilidade (Long text, required)

#### 🦷 Dentist
- Dente Acometido (Text)
- Higiene Bucal (Single choice, required)
- Usa fio dental regularmente (Yes/No, required)

#### 🩹 Nurse  
- Sinais Vitais (Long text, required)
- Procedimento Realizado (Long text)

## 🏗️ Technical Architecture

### Backend Components

```
ProfessionalSpecialty (Enum)
         ↓
ConsultationFormProfile (Templates)
         ↓
ConsultationFormConfiguration (Per Clinic)
         ↓
CustomFields (JSON)
```

#### Key Files
- `/src/MedicSoft.Domain/Enums/ProfessionalSpecialty.cs`
- `/src/MedicSoft.Domain/Entities/ConsultationFormProfile.cs`
- `/src/MedicSoft.Domain/Entities/ConsultationFormConfiguration.cs`
- `/src/MedicSoft.Domain/ValueObjects/TerminologyMap.cs`
- `/src/MedicSoft.Api/Controllers/ConsultationFormConfigurationsController.cs`

### Frontend Components

```
Appointment (with specialty)
         ↓
TerminologyService
         ↓  
CustomFieldsRenderer
         ↓
Attendance Screen (Dynamic UI)
```

#### Key Files
- `/frontend/medicwarehouse-app/src/app/services/terminology.service.ts`
- `/frontend/medicwarehouse-app/src/app/pages/attendance/attendance.ts`
- `/frontend/medicwarehouse-app/src/app/pages/attendance/components/custom-fields-renderer.*`

## 🔄 Data Flow

1. User opens attendance screen for appointment
2. System loads appointment with professional's specialty
3. Frontend requests terminology for specialty from API
4. System returns specialty-specific labels
5. Frontend updates UI with correct terminology
6. System loads ConsultationFormConfiguration for clinic
7. CustomFieldsRenderer displays specialty-specific fields
8. User fills standard + custom fields
9. System saves to MedicalRecord

## 🎨 Custom Field Types

| Type | Description | Example |
|------|-------------|---------|
| TextoCurto | Short text input | Name, ID |
| TextoLongo | Textarea | Notes, observations |
| Numero | Number input | Age, weight, pain level |
| Data | Date picker | Birth date, appointment date |
| SelecaoUnica | Radio buttons | Gender, blood type |
| SelecaoMultipla | Checkboxes | Allergies, symptoms |
| CheckBox | Single checkbox | Consent, agreement |
| SimNao | Yes/No radio | Binary questions |

## 🚀 Usage Examples

### Example 1: Psychology Session

```typescript
// Appointment has professionalSpecialty: "Psicologo"
// System loads:
terminology = {
  appointment: "Sessão",
  professional: "Psicólogo",
  mainDocument: "Prontuário Psicológico"
}

// Custom fields rendered:
- Motivo da Consulta (required)
- Nível de Ansiedade 0-10 (required)
- Histórico Psiquiátrico
- Uso de Medicação Psiquiátrica (required)
```

### Example 2: Nutrition Consultation

```typescript
// Appointment has professionalSpecialty: "Nutricionista"
// System loads:
terminology = {
  appointment: "Consulta",
  professional: "Nutricionista",
  mainDocument: "Avaliação Nutricional"
}

// Custom fields rendered:
- Peso Atual kg (required)
- Altura cm (required)
- Objetivo Nutricional (required)
- Restrições Alimentares (multi-select)
```

## ⚙️ Configuration

### Creating Custom Profiles

1. Navigate to **Clinic Admin > Form Configuration**
2. Select **Create New Profile**
3. Choose base specialty template
4. Customize field visibility
5. Add custom fields
6. Save and activate

### System Default Profiles

Pre-configured profiles exist for all 8 specialties:
- Created automatically during data seeding
- Cannot be deleted (marked as `IsSystemDefault`)
- Can be cloned to create clinic-specific versions

## 🧪 Testing

### Manual Test Checklist

- [ ] Create appointments for each specialty
- [ ] Verify terminology updates (appointment term, professional term)
- [ ] Test custom fields appear for each specialty
- [ ] Validate required field enforcement
- [ ] Test form submission and data persistence
- [ ] Verify error messages display correctly
- [ ] Test with clinic-specific configurations
- [ ] Confirm doctor screens still work

### Test Scenarios

**Scenario 1**: Psychologist Session
1. Create appointment with psychologist
2. Open attendance → verify "Sessão" terminology
3. Fill anxiety level, psychiatric history
4. Save → reload → verify persistence

**Scenario 2**: Nutritionist with Restrictions
1. Create appointment with nutritionist
2. Open attendance → verify "Consulta Nutricional" 
3. Fill weight, height
4. Select multiple dietary restrictions
5. Save → verify multi-select persists correctly

## 🔒 Security

- ✅ Terminology endpoint is public (only translations)
- ✅ Configurations are tenant-isolated
- ✅ Custom fields validated on backend
- ✅ Proper permissions for configuration changes

## 📈 Performance

- ✅ Terminology cached after first load
- ✅ Form config loaded once per appointment
- ✅ Dynamic form controls with Angular signals
- ✅ No unnecessary re-renders

## 🐛 Troubleshooting

### Issue: Custom fields not appearing
**Solution**: 
- Verify ConsultationFormConfiguration exists for clinic
- Check customFields array is populated
- Review browser console for errors

### Issue: Wrong terminology
**Solution**:
- Clear browser cache
- Verify professional's specialty in database
- Check API response from `/api/consultation-form-configurations/terminology/{specialty}`

### Issue: Validation errors
**Solution**:
- Check custom field validators
- Review CustomFieldsRenderer initialization
- Check browser console

## 🔮 Future Enhancements

- [ ] Conditional field visibility
- [ ] Field-level permissions
- [ ] Multi-language custom fields
- [ ] Import/export profiles
- [ ] Visual form builder
- [ ] Analytics on field usage
- [ ] AI-powered suggestions

## 📚 API Reference

### Endpoints

#### Get Terminology
```
GET /api/consultation-form-configurations/terminology/{specialty}
```
Returns specialty-specific terminology mapping.

**Example Response**:
```json
{
  "appointment": "Sessão",
  "professional": "Psicólogo",
  "registration": "CRP",
  "client": "Paciente",
  "mainDocument": "Prontuário Psicológico",
  "exitDocument": "Relatório Psicológico"
}
```

#### Get Active Configuration
```
GET /api/consultation-form-configurations/clinic/{clinicId}
```
Returns active consultation form configuration for clinic.

**Example Response**:
```json
{
  "id": "...",
  "clinicId": "...",
  "configurationName": "Psicólogo - Padrão",
  "isActive": true,
  "customFields": [
    {
      "fieldKey": "nivel_ansiedade",
      "label": "Nível de Ansiedade (0-10)",
      "fieldType": 2,
      "isRequired": true,
      "displayOrder": 1,
      "helpText": "Escala de 0 (sem ansiedade) a 10 (ansiedade extrema)"
    }
  ]
}
```

## 👥 Contributing

To add a new specialty:

### Backend
1. Add to `ProfessionalSpecialty` enum
2. Add terminology in `TerminologyMap.For()`
3. Create profile in `DataSeederService`

### Frontend
1. Add to `TerminologyService.parseSpecialty()`
2. Add icon in `getSpecialtyIcon()`
3. Add name in `getSpecialtyName()`

## 📄 License

Part of Omni Care Software - Internal Use Only

---

**For Support**: Check GitHub issues or create new issue with [multi-professional] tag
