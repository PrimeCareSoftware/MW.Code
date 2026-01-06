# Digital Medical Prescriptions - CFM 1.643/2002 & ANVISA 344/1998

## Overview

This implementation provides a comprehensive digital medical prescription system compliant with Brazilian medical regulations (CFM 1.643/2002) and controlled substances legislation (ANVISA Portaria 344/1998).

## Features Implemented

### Backend (C# / .NET 8)

#### Domain Entities
- **DigitalPrescription**: Main prescription entity with CFM compliance
- **DigitalPrescriptionItem**: Individual medication items with ANVISA classification
- **SNGPCReport**: ANVISA reporting for controlled substances
- **PrescriptionSequenceControl**: Sequential numbering for controlled prescriptions

#### Prescription Types
1. **Simple** (Receita Simples) - 30 days validity
2. **Special Control A** (Lista A1/A2/A3) - Narcotics - 30 days, SNGPC required
3. **Special Control B** (Lista B1/B2) - Psychotropics - 30 days, SNGPC required
4. **Special Control C1** (Lista C1) - Other controlled - 30 days, SNGPC required
5. **Antimicrobial** - Antibiotics - 10 days validity

#### API Endpoints

**Digital Prescriptions Controller** (`/api/DigitalPrescriptions`)
- `POST /` - Create new prescription
- `GET /{id}` - Get prescription by ID
- `GET /patient/{patientId}` - Get patient prescriptions
- `GET /patient/{patientId}/active` - Get active prescriptions
- `GET /medical-record/{medicalRecordId}` - Get prescriptions by medical record
- `GET /doctor/{doctorId}` - Get prescriptions by doctor
- `GET /verify/{verificationCode}` - Verify prescription by QR code
- `POST /{id}/sign` - Sign prescription (ICP-Brasil ready)
- `POST /{id}/deactivate` - Deactivate prescription
- `GET /sngpc/unreported` - Get unreported controlled prescriptions

**SNGPC Reports Controller** (`/api/SNGPCReports`)
- `POST /` - Create new monthly report
- `GET /{id}` - Get report by ID
- `GET /{year}/{month}` - Get report by period
- `GET /year/{year}` - Get all reports for year
- `GET /status/{status}` - Get reports by status
- `GET /overdue` - Get overdue reports
- `GET /latest` - Get most recent report
- `GET /history` - Get transmission history
- `POST /{id}/generate-xml` - Generate ANVISA XML
- `POST /{id}/transmit` - Mark as transmitted
- `POST /{id}/transmission-failed` - Mark transmission failed
- `GET /{id}/download-xml` - Download XML file

#### Database Schema
- PostgreSQL migrations included
- Entity Framework Core configurations
- Indexes optimized for common queries
- Tenant isolation support

#### Repositories
- `DigitalPrescriptionRepository` - 11 async methods
- `DigitalPrescriptionItemRepository` - 5 async methods
- `SNGPCReportRepository` - 10 async methods
- `PrescriptionSequenceControlRepository` - 5 async methods

### Frontend (Angular 18 / TypeScript)

#### Models
- TypeScript interfaces matching backend DTOs
- Prescription type enums with metadata
- Controlled substance classifications
- SNGPC report status tracking

#### Services
- `DigitalPrescriptionService` - 25+ API integration methods
- Full CRUD operations
- SNGPC workflow methods
- File download support

#### Components

**Prescription Type Selector**
- Visual cards for each prescription type
- Compliance information display
- Controlled medication warnings
- Type-specific characteristics (validity, SNGPC requirement)

**SNGPC Dashboard**
- Statistics cards (unreported count, overdue reports, transmissions)
- Reports table with filtering
- Status tracking with visual indicators
- Action menu (generate XML, transmit, download)
- Deadline countdown and alerts
- Compliance information panel

## Compliance Features

### CFM 1.643/2002
✅ Digital prescription format
✅ Doctor identification (Name, CRM, State)
✅ Patient identification (Name, Document)
✅ Medication details (dosage, frequency, duration, quantity)
✅ Digital signature support (ICP-Brasil ready)
✅ QR code verification
✅ 20-year retention period support

### ANVISA 344/1998
✅ Controlled substance classification (Lists A, B, C)
✅ Sequential numbering for controlled prescriptions
✅ Special prescription forms indication
✅ SNGPC monthly reporting
✅ Deadline tracking (10th day of following month)
✅ Transmission protocol recording
✅ XML generation (placeholder for ANVISA schema)

## Usage Examples

### Creating a Digital Prescription

```csharp
var prescription = new DigitalPrescription(
    medicalRecordId: medicalRecordGuid,
    patientId: patientGuid,
    doctorId: doctorGuid,
    type: PrescriptionType.SpecialControlB,
    doctorName: "Dr. João Silva",
    doctorCRM: "12345",
    doctorCRMState: "SP",
    patientName: "Maria Santos",
    patientDocument: "12345678900",
    tenantId: "clinic-id"
);

// Add medication item
var item = new DigitalPrescriptionItem(
    prescription.Id,
    medicationGuid,
    medicationName: "Rivotril 2mg",
    dosage: "2mg",
    pharmaceuticalForm: "Comprimido",
    frequency: "2x ao dia",
    durationDays: 30,
    quantity: 60,
    tenantId: "clinic-id",
    isControlledSubstance: true,
    controlledList: ControlledSubstanceList.B1_Psychotropics
);

prescription.AddItem(item);
await prescriptionRepository.AddAsync(prescription);
```

### Frontend Usage

```typescript
// Create prescription
this.prescriptionService.createPrescription({
  medicalRecordId: '...',
  patientId: '...',
  doctorId: '...',
  type: PrescriptionType.SpecialControlB,
  doctorName: 'Dr. João Silva',
  doctorCRM: '12345',
  doctorCRMState: 'SP',
  patientName: 'Maria Santos',
  patientDocument: '12345678900',
  items: [/* prescription items */]
}).subscribe(prescription => {
  console.log('Created:', prescription.id);
});

// Create SNGPC report
this.prescriptionService.createSNGPCReport({
  month: 12,
  year: 2024
}).subscribe(report => {
  console.log('Report created:', report.id);
});
```

## Testing

### Unit Tests
- 15 tests for DigitalPrescription entity
- All tests passing ✅
- Covers: creation, validation, signing, SNGPC marking, expiration

Run tests:
```bash
dotnet test --filter "FullyQualifiedName~DigitalPrescriptionTests"
```

## Architecture

### Clean Architecture Layers
- **Domain**: Entities with business rules
- **Repository**: Data access with EF Core
- **Application**: DTOs, commands, queries
- **API**: REST controllers
- **Frontend**: Angular components and services

### Design Patterns
- Repository pattern
- Dependency injection
- Async/await throughout
- AutoMapper for DTOs
- Entity encapsulation with private setters

## Future Enhancements

### High Priority
- [ ] Implement full ANVISA XML schema v2.1
- [ ] PDF generation with prescription templates
- [ ] Complete ICP-Brasil digital signature integration
- [ ] Integration tests for XML and PDF services

### Medium Priority
- [ ] Batch prescription creation
- [ ] Prescription renewal workflow
- [ ] Pharmacy dispensing integration
- [ ] Mobile app support

### Low Priority
- [ ] OCR for prescription scanning
- [ ] Voice dictation for prescription creation
- [ ] ML-based medication interaction checking
- [ ] Analytics and reporting dashboard

## Security Considerations

✅ Tenant isolation enforced
✅ Digital signature support
✅ Audit trails (CreatedAt, UpdatedAt)
✅ Cannot modify signed prescriptions
✅ Sequential numbers prevent duplication
✅ Verification codes for authenticity

## Performance

- Async operations throughout
- Indexed database queries
- Batch operations supported
- Paginated API responses ready
- Efficient change tracking

## Regulatory Compliance

This implementation follows:
- **CFM Resolution 1.643/2002** - Digital prescriptions
- **ANVISA Portaria 344/1998** - Controlled substances
- **CFM Resolution 1.821/2007** - Medical record standards
- **Lei 6.360/1976** - Sanitary surveillance

## Support

For issues or questions:
1. Check API documentation (Swagger at `/swagger`)
2. Review entity unit tests for usage examples
3. Consult ANVISA and CFM official documentation

## License

Part of MedicWarehouse system - Internal use only
