# Configurable Consultation Form - User Guide

## Overview

The Configurable Consultation Form feature allows clinic owners to customize their medical consultation forms based on their specialty (doctor, psychologist, nutritionist, etc.). This makes the PrimeCare Software adaptable to different types of healthcare professionals.

## Features

### 1. Predefined Profiles

The system includes predefined form profiles for common healthcare specialties:

- **Médico (Doctor)**: General medical consultation with all CFM 1.821 mandatory fields
- **Psicólogo (Psychologist)**: Psychology-focused consultation with custom fields for mental health assessment
- **Nutricionista (Nutritionist)**: Nutrition consultation with custom fields for dietary assessment
- **Fisioterapeuta (Physiotherapist)**: Physical therapy consultation
- **Dentista (Dentist)**: Dental consultation
- **Enfermeiro (Nurse)**: Nursing consultation
- **Terapeuta Ocupacional (Occupational Therapist)**
- **Fonoaudiólogo (Speech Therapist)**

### 2. Customizable Fields

Each profile can be customized with:

#### Standard CFM 1.821 Fields (Show/Hide)
- Queixa Principal (Chief Complaint)
- História da Doença Atual (History of Present Illness)
- História Patológica Pregressa (Past Medical History)
- História Familiar (Family History)
- Hábitos de Vida (Lifestyle Habits)
- Medicações em Uso (Current Medications)

#### Custom Fields
You can add custom fields with the following types:

- **TextoSimples**: Single line text input
- **TextoLongo**: Multi-line text area
- **Numero**: Numeric input
- **Data**: Date input
- **DataHora**: Date and time input
- **SelecaoUnica**: Single selection dropdown
- **SelecaoMultipla**: Multiple selection checkboxes
- **SimNao**: Yes/No checkbox
- **Email**: Email input
- **Telefone**: Phone number input
- **Url**: URL input

## API Endpoints

### Consultation Form Profiles

#### Get All Active Profiles
```http
GET /api/consultation-form-profiles
Authorization: Bearer {token}
```

#### Get Profile by ID
```http
GET /api/consultation-form-profiles/{id}
Authorization: Bearer {token}
```

#### Get Profiles by Specialty
```http
GET /api/consultation-form-profiles/specialty/{specialty}
Authorization: Bearer {token}
```

#### Create Profile
```http
POST /api/consultation-form-profiles
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Perfil Psicologia Clínica",
  "description": "Perfil para psicólogos clínicos",
  "specialty": 2,
  "showChiefComplaint": true,
  "showHistoryOfPresentIllness": true,
  "showPastMedicalHistory": false,
  "showFamilyHistory": true,
  "showLifestyleHabits": true,
  "showCurrentMedications": false,
  "customFields": [
    {
      "fieldKey": "sintomas_psicologicos",
      "label": "Sintomas Psicológicos",
      "fieldType": 2,
      "isRequired": true,
      "displayOrder": 1,
      "helpText": "Descreva os principais sintomas psicológicos apresentados"
    },
    {
      "fieldKey": "escala_ansiedade",
      "label": "Escala de Ansiedade (1-10)",
      "fieldType": 3,
      "isRequired": true,
      "displayOrder": 2,
      "placeholder": "1-10"
    }
  ]
}
```

#### Update Profile
```http
PUT /api/consultation-form-profiles/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "name": "Perfil Atualizado",
  "description": "Nova descrição",
  "showChiefComplaint": true,
  "showHistoryOfPresentIllness": true,
  "showPastMedicalHistory": true,
  "showFamilyHistory": true,
  "showLifestyleHabits": true,
  "showCurrentMedications": true,
  "customFields": []
}
```

#### Delete Profile
```http
DELETE /api/consultation-form-profiles/{id}
Authorization: Bearer {token}
```

### Consultation Form Configurations

#### Get Configuration by Clinic ID
```http
GET /api/consultation-form-configurations/clinic/{clinicId}
Authorization: Bearer {token}
```

#### Create Configuration
```http
POST /api/consultation-form-configurations
Authorization: Bearer {token}
Content-Type: application/json

{
  "clinicId": "00000000-0000-0000-0000-000000000000",
  "profileId": null,
  "configurationName": "Configuração Principal da Clínica",
  "showChiefComplaint": true,
  "showHistoryOfPresentIllness": true,
  "showPastMedicalHistory": true,
  "showFamilyHistory": true,
  "showLifestyleHabits": true,
  "showCurrentMedications": true,
  "customFields": []
}
```

#### Create Configuration from Profile
```http
POST /api/consultation-form-configurations/from-profile
Authorization: Bearer {token}
Content-Type: application/json

{
  "clinicId": "00000000-0000-0000-0000-000000000000",
  "profileId": "00000000-0000-0000-0000-000000000000"
}
```

#### Update Configuration
```http
PUT /api/consultation-form-configurations/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "configurationName": "Configuração Atualizada",
  "showChiefComplaint": true,
  "showHistoryOfPresentIllness": true,
  "showPastMedicalHistory": false,
  "showFamilyHistory": false,
  "showLifestyleHabits": true,
  "showCurrentMedications": true,
  "customFields": []
}
```

## Permissions

The following permissions are required:

- **form-configuration.view**: View form configurations and profiles
- **form-configuration.manage**: Create, update, and delete configurations and profiles

These permissions should be assigned to clinic owners only.

## Examples

### Example 1: Psychology Consultation Profile

```json
{
  "name": "Psicologia Clínica",
  "description": "Perfil para atendimento psicológico",
  "specialty": 2,
  "customFields": [
    {
      "fieldKey": "motivo_consulta",
      "label": "Motivo da Consulta",
      "fieldType": 2,
      "isRequired": true,
      "displayOrder": 1
    },
    {
      "fieldKey": "historico_psiquiatrico",
      "label": "Histórico Psiquiátrico",
      "fieldType": 2,
      "isRequired": false,
      "displayOrder": 2
    },
    {
      "fieldKey": "nivel_ansiedade",
      "label": "Nível de Ansiedade (0-10)",
      "fieldType": 3,
      "isRequired": true,
      "displayOrder": 3
    },
    {
      "fieldKey": "uso_medicacao_psiquiatrica",
      "label": "Faz uso de medicação psiquiátrica?",
      "fieldType": 8,
      "isRequired": true,
      "displayOrder": 4
    }
  ]
}
```

### Example 2: Nutrition Consultation Profile

```json
{
  "name": "Nutrição Clínica",
  "description": "Perfil para consulta nutricional",
  "specialty": 3,
  "customFields": [
    {
      "fieldKey": "peso_atual",
      "label": "Peso Atual (kg)",
      "fieldType": 3,
      "isRequired": true,
      "displayOrder": 1
    },
    {
      "fieldKey": "altura",
      "label": "Altura (cm)",
      "fieldType": 3,
      "isRequired": true,
      "displayOrder": 2
    },
    {
      "fieldKey": "objetivo_nutricional",
      "label": "Objetivo Nutricional",
      "fieldType": 6,
      "isRequired": true,
      "displayOrder": 3,
      "options": ["Perda de peso", "Ganho de massa", "Manutenção", "Reeducação alimentar"]
    },
    {
      "fieldKey": "restricoes_alimentares",
      "label": "Restrições Alimentares",
      "fieldType": 7,
      "isRequired": false,
      "displayOrder": 4,
      "options": ["Lactose", "Glúten", "Frutos do mar", "Nozes", "Soja", "Outras"]
    }
  ]
}
```

## Implementation Notes

1. **System Default Profiles**: Some profiles are marked as system defaults and cannot be deleted or modified. They serve as templates for creating custom profiles.

2. **Field Keys**: Custom field keys must be unique within a profile/configuration and follow alphanumeric + underscore naming convention.

3. **CFM 1.821 Compliance**: For medical doctors, the system maintains compliance with Brazilian medical record regulations (CFM 1.821) by ensuring mandatory fields are always present.

4. **Multi-tenant**: All configurations are isolated by tenant (clinic) for security and data separation.

## Future Enhancements

- Import/Export profiles
- Field validation rules
- Conditional field display
- Form versioning
- Field analytics and usage statistics

## Support

For questions or issues, please contact support at support@primecaresoftware.com
