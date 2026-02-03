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
✅ **XML generation (ANVISA schema v2.1 COMPLETE)**
✅ **Alert persistence system for compliance monitoring**

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
- [x] ✅ **COMPLETE:** Implement full ANVISA XML schema v2.1
- [x] ✅ **COMPLETE:** PDF generation with prescription templates (QuestPDF)
- [x] ✅ **COMPLETE:** Alert persistence system for SNGPC compliance
- [ ] ⏳ **IN PROGRESS:** Complete ICP-Brasil digital signature integration (interface ready, awaiting production certificates)
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

## PDF Generation (✅ COMPLETE)

### Professional Templates by Prescription Type

The system includes **3 professional PDF templates** using QuestPDF:

#### 1. Simple Prescription Template
- Standard layout with clinic header
- Patient information section
- Medication list with dosage and instructions
- QR code for verification (top right)
- Doctor signature section (footer)
- A4/Letter/Half-page support

#### 2. Controlled Prescription Template
- Red "RECEITA CONTROLADA" watermark
- Notification number prominently displayed
- Control type indicator (A/B/C1)
- Complete prescriber identification
- Patient identification
- **Single medication per prescription** (ANVISA requirement)
- Issue date and expiration date highlighted
- Usage warnings

#### 3. Antimicrobial Prescription Template
- Yellow "USO SOB ORIENTAÇÃO MÉDICA" watermark
- "RDC 20/2011 ANVISA" header
- Patient information
- Antimicrobial medication list
- Yellow box with mandatory warnings:
  - 10-day validity
  - Second copy retention by pharmacy
  - Do not share medication

### PDF API Endpoints

```http
GET /api/DigitalPrescriptions/{id}/pdf?clinicName=MyClinic&clinicAddress=123 Main St&clinicPhone=(11) 1234-5678
# Downloads PDF with clinic information

GET /api/DigitalPrescriptions/{id}/pdf/preview
# Displays PDF inline in browser
```

### PDF Features
- ✅ QR code integration for authenticity verification
- ✅ Watermarks for controlled/antimicrobial prescriptions
- ✅ Optimized for print (A4, Letter, Half-page)
- ✅ Professional medical font (Arial)
- ✅ Proper spacing for readability
- ✅ Clinic branding support
- ✅ Digital signature timestamp display

### Usage Example

```csharp
// Generate PDF programmatically
var pdfBytes = await _pdfService.GeneratePdfAsync(
    prescriptionId,
    tenantId,
    new PrescriptionPdfOptions
    {
        ClinicName = "Clínica Exemplo",
        ClinicAddress = "Rua Exemplo, 123 - São Paulo/SP",
        ClinicPhone = "(11) 1234-5678",
        IncludeQRCode = true,
        IncludeWatermark = true,
        PaperSize = PaperSize.A4
    }
);

// Save to file or return as HTTP response
File.WriteAllBytes("receita.pdf", pdfBytes);
```

## XML ANVISA Schema v2.1 (✅ COMPLETE)

### SNGPC XML Generation

The system generates **fully compliant ANVISA XML** for controlled substance reporting:

#### XML Structure
```xml
<?xml version="1.0" encoding="UTF-8"?>
<SNGPC xmlns="http://www.anvisa.gov.br/sngpc/v2.1" 
       xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
       xsi:schemaLocation="http://www.anvisa.gov.br/sngpc/v2.1 SNGPC_v2.1.xsd"
       versao="2.1">
  <Cabecalho>
    <Versao>2.1</Versao>
    <TipoDocumento>ESCRITURACAO</TipoDocumento>
    <PeriodoInicio>2026-01-01</PeriodoInicio>
    <PeriodoFim>2026-01-31</PeriodoFim>
    <DataGeracao>2026-02-05T10:30:00</DataGeracao>
    <MesReferencia>01</MesReferencia>
    <AnoReferencia>2026</AnoReferencia>
    <QuantidadeReceitas>42</QuantidadeReceitas>
    <QuantidadeItens>42</QuantidadeItens>
  </Cabecalho>
  <Receitas>
    <Receita>
      <NumeroReceita>202601000001</NumeroReceita>
      <TipoReceituario>CONTROLE_ESPECIAL_B</TipoReceituario>
      <DataEmissao>2026-01-15</DataEmissao>
      <Prescritor>
        <Nome>Dr. João Silva</Nome>
        <CRM>12345</CRM>
        <UF>SP</UF>
      </Prescritor>
      <Paciente>
        <Nome>Maria Santos</Nome>
        <CPF>12345678900</CPF>
      </Paciente>
      <Itens>
        <Item>
          <Medicamento>Rivotril 2mg</Medicamento>
          <Quantidade>60</Quantidade>
          <Unidade>UN</Unidade>
          <NomeGenerico>Clonazepam</NomeGenerico>
          <PrincipioAtivo>Clonazepam</PrincipioAtivo>
          <ListaControlada>B1</ListaControlada>
          <Dosagem>2mg</Dosagem>
          <FormaFarmaceutica>Comprimido</FormaFarmaceutica>
          <Posologia>1 comprimido 2x ao dia, por 30 dias</Posologia>
        </Item>
      </Itens>
    </Receita>
  </Receitas>
</SNGPC>
```

### XML Features
- ✅ ANVISA namespace v2.1 compliance
- ✅ UTF-8 encoding with XML declaration
- ✅ Schema validation ready
- ✅ Complete prescription header (period, totals)
- ✅ Prescription type mapping
- ✅ Prescriber information (Name, CRM, State)
- ✅ Patient information (Name, CPF/RG)
- ✅ Controlled substance classification (A1-A3, B1-B2, C1-C5)
- ✅ Medication details (dosage, form, posology)
- ✅ Generic name (DCB/DCI)
- ✅ Active ingredient
- ✅ ANVISA registration number (when available)
- ✅ Special character sanitization

### XML API Endpoints

```http
GET /api/DigitalPrescriptions/{id}/xml
# Export single prescription as ANVISA XML

POST /api/SNGPCReports/{reportId}/generate-xml
# Generate monthly report XML for all controlled prescriptions

GET /api/SNGPCReports/{reportId}/download-xml
# Download generated monthly report XML
```

### Usage Example

```csharp
// Generate ANVISA XML for a prescription
var xmlContent = await _xmlGenerator.GenerateXmlAsync(report, prescriptions);

// Save or transmit to ANVISA
File.WriteAllText($"SNGPC_{report.Year}_{report.Month:D2}.xml", xmlContent);
```

## Alert Persistence System (✅ COMPLETE)

### SNGPC Compliance Monitoring

The system includes a **complete alert persistence layer** for SNGPC compliance:

#### Alert Types (11 types)
1. **DeadlineApproaching** - Report deadline approaching (5 days before)
2. **DeadlineOverdue** - Report deadline passed
3. **MissingReport** - Monthly report not created
4. **InvalidBalance** - Calculated balance doesn't match
5. **NegativeBalance** - Stock showing negative values
6. **MissingRegistryEntry** - Movement not registered
7. **TransmissionFailed** - ANVISA transmission failed
8. **UnusualMovement** - Unusual dispensing pattern detected
9. **ExcessiveDispensing** - Excessive quantities dispensed
10. **ComplianceViolation** - Regulatory violation detected
11. **SystemError** - System error in SNGPC processing

#### Severity Levels
- **Info** - Informational (green)
- **Warning** - Warning (yellow)
- **Error** - Error (orange)
- **Critical** - Critical action required (red)

#### Alert Workflow
```
Created → Active → Acknowledged → Resolved
```

#### Alert Features
- ✅ Persistent storage in database
- ✅ Complete audit trail (who, when, why)
- ✅ Acknowledgment tracking with notes
- ✅ Resolution tracking with description
- ✅ Relationships to reports, registries, balances
- ✅ Multi-tenancy isolation
- ✅ Optimized queries with indexes
- ✅ Age calculation in days

### Alert API Endpoints

```http
GET /api/SNGPCReports/active-alerts?severity=Critical
# Get active alerts filtered by severity

GET /api/SNGPCReports/approaching-deadlines?daysBeforeDeadline=5
# Get reports with approaching deadlines

GET /api/SNGPCReports/overdue
# Get overdue reports that need immediate attention

GET /api/SNGPCReports/validate-compliance
# Run compliance validation and generate alerts

GET /api/SNGPCReports/detect-anomalies
# Detect unusual patterns and generate alerts
```

### Usage Example

```csharp
// Get critical alerts
var criticalAlerts = await _alertService.GetActiveAlertsAsync(
    tenantId, 
    severity: AlertSeverity.Critical
);

// Acknowledge an alert
await _alertService.AcknowledgeAlertAsync(
    alertId, 
    userId, 
    notes: "Checking with pharmacy team"
);

// Resolve an alert
await _alertService.ResolveAlertAsync(
    alertId, 
    userId, 
    resolution: "Transmitted successfully to ANVISA. Protocol: 123456"
);
```

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

Part of Omni Care Software system - Internal use only
