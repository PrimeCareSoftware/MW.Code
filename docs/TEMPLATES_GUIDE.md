# Document Templates Management System - Implementation Guide

## Overview

This system provides a comprehensive template management solution with two levels:
1. **Global Templates** - System-admin level templates available to all clinics
2. **Clinic Templates** - Clinic-specific templates that can be created from scratch or copied from global templates

## Backend Architecture

### Domain Layer

#### Entities

**GlobalDocumentTemplate** (`src/MedicSoft.Domain/Entities/GlobalDocumentTemplate.cs`)
- Properties: Name, Description, Type, Specialty, Content (HTML), Variables (JSON), IsActive, CreatedBy
- Multi-tenant support via TenantId
- Rich domain validation in constructor and Update method
- Cannot be modified by clinic users (SystemAdmin only)

**DocumentTemplate** (Updated)
- Added `GlobalTemplateId` property to reference the global template used as base
- Navigation property to GlobalDocumentTemplate
- Supports both custom templates and templates derived from global templates

#### Enums
- `DocumentTemplateType`: MedicalRecord, Prescription, MedicalCertificate, LabTestRequest, etc.
- `ProfessionalSpecialty`: Medico, Psicologo, Nutricionista, Fisioterapeuta, Dentista, etc.

### Infrastructure Layer

#### Repository Pattern

**GlobalDocumentTemplateRepository** (`src/MedicSoft.Repository/Repositories/GlobalDocumentTemplateRepository.cs`)
- Implements `IGlobalDocumentTemplateRepository`
- Methods:
  - `GetByTypeAsync()` - Filter by document type
  - `GetBySpecialtyAsync()` - Filter by professional specialty
  - `GetActiveTemplatesAsync()` - Get only active templates
  - `SetActiveStatusAsync()` - Activate/deactivate template
  - `ExistsByNameAndTypeAsync()` - Check for duplicate names

**Database Configuration**
- Table: `GlobalDocumentTemplates`
- Indexes on: TenantId, Type, Specialty, IsActive, (Name+Type+TenantId)
- Foreign key from `DocumentTemplates.GlobalTemplateId` to `GlobalDocumentTemplates.Id`

#### Migration

**20260207205000_AddGlobalDocumentTemplates.cs**
- Creates `GlobalDocumentTemplates` table
- Adds `GlobalTemplateId` column to `DocumentTemplates` table
- Adds foreign key constraint with RESTRICT delete behavior

### Application Layer (CQRS)

#### Commands

**GlobalDocumentTemplate Commands**:
- `CreateGlobalTemplateCommand` - Create new global template (with duplicate name validation)
- `UpdateGlobalTemplateCommand` - Update existing template
- `DeleteGlobalTemplateCommand` - Delete template (checks if in use by clinics)
- `SetGlobalTemplateActiveStatusCommand` - Activate/deactivate

**DocumentTemplate Commands** (Updated):
- `CreateDocumentTemplateFromGlobalCommand` - Copy global template to clinic

#### Queries

**GlobalDocumentTemplate Queries**:
- `GetAllGlobalTemplatesQuery` - List all with optional filters
- `GetGlobalTemplateByIdQuery` - Get single template
- `GetGlobalTemplatesByTypeQuery` - Filter by type
- `GetGlobalTemplatesBySpecialtyQuery` - Filter by specialty

#### DTOs

**GlobalDocumentTemplate DTOs**:
- `GlobalDocumentTemplateDto` - Read model
- `CreateGlobalTemplateDto` - Input with validation attributes
- `UpdateGlobalTemplateDto` - Update model
- `GlobalDocumentTemplateFilterDto` - Query filters

#### Handlers

All handlers follow MediatR pattern with:
- Dependency injection of repositories and AutoMapper
- Proper exception handling with meaningful messages
- Tenant-aware operations

### API Layer

#### Controllers

**GlobalDocumentTemplateController** (`src/MedicSoft.Api/Controllers/SystemAdmin/GlobalDocumentTemplateController.cs`)
- Route: `/api/system-admin/global-templates`
- Authorization: `[Authorize(Roles = "SystemAdmin")]`
- Endpoints:
  - `GET /` - List all global templates with filters
  - `GET /{id}` - Get specific template
  - `GET /type/{type}` - Get by document type
  - `GET /specialty/{specialty}` - Get by specialty
  - `POST /` - Create new global template
  - `PUT /{id}` - Update template
  - `DELETE /{id}` - Delete template (validates not in use)
  - `PATCH /{id}/active` - Set active status

**DocumentTemplatesController** (Updated)
- Added endpoint: `POST /from-global/{globalTemplateId}` - Create clinic template from global template
- Requires clinic context from JWT claims
- Validates global template is active before copying

#### Dependency Injection

Registered in `Program.cs`:
```csharp
builder.Services.AddScoped<IGlobalDocumentTemplateRepository, GlobalDocumentTemplateRepository>();
```

## Variable System

Templates support dynamic variable substitution:

### Patient Variables
- `{{patientName}}`, `{{patientCPF}}`, `{{patientRG}}`, `{{patientBirthDate}}`
- `{{patientAge}}`, `{{patientGender}}`, `{{patientAddress}}`
- `{{patientPhone}}`, `{{patientEmail}}`

### Professional Variables
- `{{professionalName}}`, `{{professionalRegistration}}`
- `{{professionalSpecialty}}`, `{{professionalPhone}}`

### Clinic Variables
- `{{clinicName}}`, `{{clinicCNPJ}}`, `{{clinicAddress}}`
- `{{clinicPhone}}`, `{{clinicEmail}}`

### Consultation Variables
- `{{consultationDate}}`, `{{consultationTime}}`, `{{consultationReason}}`

### System Variables
- `{{currentDate}}`, `{{currentTime}}`, `{{currentDateTime}}`

Variables are stored as JSON in the `Variables` field for metadata and validation.

## Security

### Authorization
- Global templates require `SystemAdmin` role
- Clinic templates require `FormConfigurationManage` permission
- All operations are tenant-aware (multi-tenancy)

### Validation
- Name uniqueness per type and tenant
- Content and variables required (non-empty)
- Cannot delete global template if in use by clinics
- Cannot create from inactive global template

### Data Integrity
- Foreign key constraints with RESTRICT delete
- Transactions for complex operations
- Entity validation in domain layer

## Workflow

### System Admin Creates Global Template
1. Admin logs into System Admin portal
2. Navigates to Global Templates section
3. Clicks "New Template"
4. Fills in: Name, Description, Type, Specialty
5. Uses rich text editor to create content with variables
6. Saves template (becomes available to all clinics)

### Clinic Uses Global Template
1. Clinic admin/user views available global templates
2. Selects desired template
3. Clicks "Use as Base" or "Create from Template"
4. System copies template to clinic with reference to global template
5. Clinic can customize their copy without affecting global template

### Template Updates
- **Global Template Update**: Only affects future copies, not existing clinic templates
- **Clinic Template Update**: Only affects that clinic, maintains reference to original global template

## API Examples

### Create Global Template
```http
POST /api/system-admin/global-templates
Authorization: Bearer {systemAdminToken}
Content-Type: application/json

{
  "name": "Receita Médica Simples",
  "description": "Template padrão para receitas médicas",
  "type": 2,
  "specialty": 1,
  "content": "<h1>Receita Médica</h1><p>Paciente: {{patientName}}</p>",
  "variables": "[{\"key\":\"patientName\",\"label\":\"Nome do Paciente\"}]"
}
```

### List Global Templates with Filters
```http
GET /api/system-admin/global-templates?type=2&specialty=1&isActive=true
Authorization: Bearer {systemAdminToken}
```

### Create Clinic Template from Global
```http
POST /api/document-templates/from-global/{globalTemplateId}
Authorization: Bearer {clinicUserToken}
```

## Database Schema

### GlobalDocumentTemplates Table
```sql
CREATE TABLE GlobalDocumentTemplates (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Type INT NOT NULL,
    Specialty INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Variables NVARCHAR(MAX) NOT NULL,
    IsActive BIT DEFAULT 1 NOT NULL,
    TenantId NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(100) NOT NULL
);

-- Indexes
CREATE INDEX IX_GlobalDocumentTemplates_TenantId ON GlobalDocumentTemplates(TenantId);
CREATE INDEX IX_GlobalDocumentTemplates_Type ON GlobalDocumentTemplates(Type);
CREATE INDEX IX_GlobalDocumentTemplates_Specialty ON GlobalDocumentTemplates(Specialty);
CREATE INDEX IX_GlobalDocumentTemplates_IsActive ON GlobalDocumentTemplates(IsActive);
CREATE INDEX IX_GlobalDocumentTemplates_Name_Type_TenantId ON GlobalDocumentTemplates(Name, Type, TenantId);
```

### DocumentTemplates Table Update
```sql
ALTER TABLE DocumentTemplates 
ADD GlobalTemplateId UNIQUEIDENTIFIER NULL;

ALTER TABLE DocumentTemplates
ADD CONSTRAINT FK_DocumentTemplates_GlobalDocumentTemplates_GlobalTemplateId 
FOREIGN KEY (GlobalTemplateId) REFERENCES GlobalDocumentTemplates(Id) ON DELETE RESTRICT;

CREATE INDEX IX_DocumentTemplates_GlobalTemplateId ON DocumentTemplates(GlobalTemplateId);
```

## Next Steps

### Frontend Implementation Required:
1. System Admin UI with Quill.js rich text editor
2. MedicWarehouse App integration for using global templates
3. Template preview functionality
4. Variable insertion helpers

### Future Enhancements:
1. Template versioning
2. Template categories/tags
3. Template usage analytics
4. Template import/export
5. Template sharing between clinics
6. Rich template library with more examples

## Testing

### Unit Tests
- Repository methods
- Command/Query handlers
- Domain entity validation

### Integration Tests
- Controller endpoints
- End-to-end template creation and usage flow
- Security and authorization

### Manual Testing Checklist
- [ ] SystemAdmin can create global templates
- [ ] SystemAdmin can update/delete global templates
- [ ] Clinics can view global templates
- [ ] Clinics can create templates from global templates
- [ ] Cannot delete global template in use
- [ ] Cannot create from inactive global template
- [ ] Multi-tenancy isolation works correctly
- [ ] Variables are preserved when copying

## Support

For questions or issues:
- Technical documentation: See inline code comments
- API documentation: Swagger UI at `/swagger`
- Domain logic: Review entity classes for business rules

---

**Status**: ✅ Backend implementation complete and tested
**Next Phase**: Frontend implementation with Quill.js integration
