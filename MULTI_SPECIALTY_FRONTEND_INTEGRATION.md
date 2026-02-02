# Multi-Specialty Frontend Integration

## Overview

This implementation provides the frontend integration for the multi-specialty adaptation system introduced in PR 608. It enables clinics to configure their business type, specialty, and features, while providing dynamic terminology and customizable document templates.

## Architecture

### Components

1. **Business Configuration Service** (`business-configuration.service.ts`)
   - Manages API communication for business configuration
   - Handles CRUD operations for clinic configurations
   - Supports feature flag management

2. **Terminology Service** (`terminology.service.ts`)
   - Fetches and caches terminology for clinics
   - Provides terminology lookup and replacement
   - Supports placeholder syntax (`{{key}}`)

3. **Terminology Pipe** (`terminology.pipe.ts`)
   - Angular pipe for inline terminology replacement
   - Used in templates for dynamic text
   - Example: `{{ 'appointment' | terminology }}`

4. **Business Configuration Component** (`business-configuration/`)
   - Full-featured UI for managing clinic configuration
   - Specialty selector with icons
   - Feature toggle by category
   - Real-time updates via API

5. **Template Editor Component** (`template-editor/`)
   - Visual editor for document templates
   - Pre-built templates for each specialty
   - Real-time preview with sample data
   - Placeholder insertion helper

6. **Onboarding Wizard Component** (`onboarding/`)
   - 4-step guided setup for new clinics
   - Business type selection
   - Specialty selection
   - Feature preview
   - Terminology confirmation

## Routes

| Route | Component | Description |
|-------|-----------|-------------|
| `/onboarding` | OnboardingWizardComponent | First-time clinic setup |
| `/clinic-admin/business-configuration` | BusinessConfigurationComponent | Manage clinic configuration |
| `/clinic-admin/template-editor` | TemplateEditorComponent | Customize document templates |

## Usage

### 1. Onboarding a New Clinic

```typescript
// Navigate to onboarding
router.navigate(['/onboarding']);

// The wizard will guide through:
// Step 1: Select business type (Solo/Small/Medium/Large)
// Step 2: Select specialty (Medical, Psychology, Nutrition, etc.)
// Step 3: Review recommended features
// Step 4: Preview terminology and confirm
```

### 2. Using Dynamic Terminology

```html
<!-- In templates -->
<h1>{{ 'appointment' | terminology }}</h1>
<!-- Outputs: "Consulta" for Medical, "Sessão" for Psychology -->

<!-- With placeholders -->
<p>{{ 'Agende sua {{appointment}} com nosso {{professional}}' | terminology }}</p>
```

```typescript
// In components
constructor(private terminologyService: TerminologyService) {}

ngOnInit() {
  // Load terminology for clinic
  this.terminologyService.loadTerminology(clinicId).subscribe();
  
  // Get a specific term
  const term = this.terminologyService.getTerm('appointment');
  
  // Replace placeholders
  const text = this.terminologyService.replacePlaceholders(
    'Próxima {{appointment}}: {{consultationDate}}'
  );
}
```

### 3. Managing Business Configuration

```typescript
// Get configuration for a clinic
businessConfigService.getByClinicId(clinicId).subscribe(config => {
  console.log(config.primarySpecialty); // ProfessionalSpecialty.Medico
  console.log(config.electronicPrescription); // true
});

// Update a feature
const dto: UpdateFeatureDto = {
  featureName: 'telemedicine',
  enabled: true
};
businessConfigService.updateFeature(configId, dto).subscribe();

// Check if feature is enabled
businessConfigService.isFeatureEnabled(clinicId, 'telemedicine')
  .subscribe(response => {
    console.log(response.enabled); // true/false
  });
```

### 4. Customizing Document Templates

```typescript
// Template editor supports these specialties:
// - Medical (Médico)
// - Psychology (Psicólogo)
// - Nutrition (Nutricionista)
// - Physiotherapy (Fisioterapeuta)
// - Dentistry (Dentista)

// Available placeholders:
{{patientName}}          // Patient name
{{patientBirthDate}}     // Patient birth date
{{consultationDate}}     // Consultation date
{{professionalName}}     // Professional name
{{professionalRegistration}} // Professional registration number
{{chiefComplaint}}       // Chief complaint
// ... and more
```

## Terminology Mapping

Each specialty has its own terminology:

| Specialty | Appointment | Professional | Registration | Main Document | Exit Document |
|-----------|-------------|--------------|--------------|---------------|---------------|
| Medical | Consulta | Médico | CRM | Prontuário Médico | Receita Médica |
| Psychology | Sessão | Psicólogo | CRP | Prontuário | Relatório Psicológico |
| Nutrition | Consulta | Nutricionista | CRN | Avaliação Nutricional | Plano Alimentar |
| Physiotherapy | Sessão | Fisioterapeuta | CREFITO | Avaliação Fisioterapêutica | Plano de Tratamento |
| Dentistry | Consulta | Dentista | CRO | Odontograma | Orçamento de Tratamento |

## Feature Flags

Features are automatically configured based on specialty and business type:

### Clinical Features
- Electronic Prescription
- Lab Integration
- Vaccine Control
- Inventory Management

### Administrative Features
- Multi Room
- Reception Queue
- Financial Module
- Health Insurance

### Consultation Features
- Telemedicine
- Home Visit
- Group Sessions

### Marketing Features
- Public Profile
- Online Booking
- Patient Reviews

### Advanced Features
- BI Reports
- API Access
- White Label

## API Endpoints

### Business Configuration
- `GET /api/BusinessConfiguration/clinic/{clinicId}` - Get configuration
- `POST /api/BusinessConfiguration` - Create configuration
- `PUT /api/BusinessConfiguration/{id}/business-type` - Update business type
- `PUT /api/BusinessConfiguration/{id}/primary-specialty` - Update specialty
- `PUT /api/BusinessConfiguration/{id}/feature` - Update feature flag
- `GET /api/BusinessConfiguration/clinic/{clinicId}/feature/{featureName}` - Check feature
- `GET /api/BusinessConfiguration/clinic/{clinicId}/terminology` - Get terminology

## Integration Example

Here's a complete example of integrating the multi-specialty system into an existing component:

```typescript
import { Component, OnInit } from '@angular/core';
import { TerminologyService } from '../../services/terminology.service';
import { TerminologyPipe } from '../../pipes/terminology.pipe';
import { BusinessConfigurationService } from '../../services/business-configuration.service';

@Component({
  selector: 'app-appointments',
  standalone: true,
  imports: [CommonModule, TerminologyPipe],
  template: `
    <h1>{{ 'appointment' | terminology }} List</h1>
    <p *ngIf="hasTelemedicine">Telemedicina disponível</p>
    
    <button>
      Agendar {{ 'appointment' | terminology }}
    </button>
  `
})
export class AppointmentsComponent implements OnInit {
  hasTelemedicine = false;

  constructor(
    private terminologyService: TerminologyService,
    private businessConfig: BusinessConfigurationService,
    private clinicSelection: ClinicSelectionService
  ) {}

  ngOnInit() {
    const clinic = this.clinicSelection.getSelectedClinic();
    
    // Load terminology
    this.terminologyService.loadTerminology(clinic.id).subscribe();
    
    // Check feature availability
    this.businessConfig.isFeatureEnabled(clinic.id, 'telemedicine')
      .subscribe(response => {
        this.hasTelemedicine = response.enabled;
      });
  }
}
```

## Testing

To test the components:

1. **Onboarding Flow**
   - Navigate to `/onboarding`
   - Complete all 4 steps
   - Verify configuration is created via API

2. **Business Configuration**
   - Navigate to `/clinic-admin/business-configuration`
   - Toggle features
   - Change specialty
   - Verify API calls are made

3. **Template Editor**
   - Navigate to `/clinic-admin/template-editor`
   - Select a specialty
   - Edit template
   - Verify preview updates in real-time

## Future Enhancements

1. **Template Persistence**
   - Save custom templates to backend
   - Load clinic-specific templates

2. **Multi-language Support**
   - Extend terminology for different languages
   - Support i18n for UI strings

3. **Template Variables**
   - Add more placeholder types
   - Support conditional sections
   - Add calculations (e.g., IMC)

4. **Advanced Features**
   - Template versioning
   - Template sharing between clinics
   - Template marketplace

## Dependencies

- Angular 20.x
- RxJS 7.8.x
- HttpClient (for API communication)
- FormsModule (for template binding)
- CommonModule (for directives)

## Related Files

### Backend (PR 608)
- `src/MedicSoft.Domain/Entities/BusinessConfiguration.cs`
- `src/MedicSoft.Domain/ValueObjects/TerminologyMap.cs`
- `src/MedicSoft.Domain/Enums/ProfessionalSpecialty.cs`
- `src/MedicSoft.Domain/Enums/BusinessType.cs`
- `src/MedicSoft.Api/Controllers/BusinessConfigurationController.cs`
- `src/MedicSoft.Application/Services/BusinessConfigurationService.cs`

### Frontend (This Implementation)
- `src/app/services/terminology.service.ts`
- `src/app/services/business-configuration.service.ts`
- `src/app/pipes/terminology.pipe.ts`
- `src/app/pages/onboarding/onboarding-wizard.component.*`
- `src/app/pages/clinic-admin/business-configuration/*`
- `src/app/pages/clinic-admin/template-editor/*`

## Support

For questions or issues, please refer to:
- Backend documentation: `BUSINESS_CONFIGURATION_GUIDE.md`
- API documentation: Swagger UI at `/swagger`
- Component examples: `/demo` routes
