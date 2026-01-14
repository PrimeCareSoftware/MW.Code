# Digital Prescriptions & SNGPC Integration - Implementation Documentation

## Overview

This document describes the implementation of Digital Medical Prescriptions and SNGPC (Sistema Nacional de Gerenciamento de Produtos Controlados) integration, completing the remaining 20% of these critical compliance features as outlined in PENDING_TASKS.md.

## Compliance Standards

### CFM (Conselho Federal de Medicina)
- **CFM 1.643/2002**: Receita Médica Digital
- **CFM 1.821/2007**: Prontuário Médico Eletrônico

### ANVISA (Agência Nacional de Vigilância Sanitária)
- **Portaria 344/1998**: Medicamentos Controlados
- **RDC 22/2014**: Sistema Nacional de Gerenciamento de Produtos Controlados (SNGPC)
- **SNGPC Schema v2.1**: XML format specification

## Implementation Summary

### 1. SNGPC XML Generator Service

**File**: `src/MedicSoft.Application/Services/SNGPCXmlGeneratorService.cs`

#### Features:
- ✅ Full ANVISA schema v2.1 compliance
- ✅ Supports all prescription types (Simple, SpecialControlA/B/C1, Antimicrobial)
- ✅ Includes all required ANVISA fields:
  - Header with report metadata
  - Prescriber information (Doctor name, CRM, State)
  - Patient information (Name, CPF/RG)
  - Item details with controlled substance classification
  - Dosage and posology information
- ✅ XML validation and hash generation (SHA-256)
- ✅ Proper text sanitization for XML compliance

#### XML Structure:
```xml
<?xml version="1.0" encoding="UTF-8"?>
<SNGPC xmlns="http://www.anvisa.gov.br/sngpc/v2.1" 
       xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
       xsi:schemaLocation="http://www.anvisa.gov.br/sngpc/v2.1 SNGPC_v2.1.xsd"
       versao="2.1">
  <Cabecalho>
    <Versao>2.1</Versao>
    <TipoDocumento>ESCRITURACAO</TipoDocumento>
    <PeriodoInicio>2024-01-01</PeriodoInicio>
    <PeriodoFim>2024-01-31</PeriodoFim>
    <DataGeracao>2024-01-31T23:59:59</DataGeracao>
    <MesReferencia>01</MesReferencia>
    <AnoReferencia>2024</AnoReferencia>
    <QuantidadeReceitas>10</QuantidadeReceitas>
    <QuantidadeItens>25</QuantidadeItens>
  </Cabecalho>
  <Receitas>
    <Receita>
      <NumeroReceita>001/2024</NumeroReceita>
      <TipoReceituario>CONTROLE_ESPECIAL_B</TipoReceituario>
      <DataEmissao>2024-01-15</DataEmissao>
      <Prescritor>
        <Nome>Dr. João Silva</Nome>
        <CRM>123456</CRM>
        <UF>SP</UF>
      </Prescritor>
      <Paciente>
        <Nome>Maria Santos</Nome>
        <CPF>12345678900</CPF>
      </Paciente>
      <Itens>
        <Item>
          <Medicamento>Diazepam 10mg</Medicamento>
          <Quantidade>30</Quantidade>
          <Unidade>UN</Unidade>
          <NomeGenerico>Diazepam</NomeGenerico>
          <ListaControlada>B1</ListaControlada>
          <Dosagem>10mg</Dosagem>
          <FormaFarmaceutica>Comprimido</FormaFarmaceutica>
          <Posologia>1 comprimido 2x ao dia, por 15 dias</Posologia>
        </Item>
      </Itens>
    </Receita>
  </Receitas>
</SNGPC>
```

#### Controlled Substance Lists Mapping:
| ANVISA List | Description | System Enum |
|-------------|-------------|-------------|
| A1 | Entorpecentes (narcóticos) | A1_Narcotics |
| A2 | Entorpecentes (psicotrópicos) | A2_Psychotropics |
| A3 | Psicotrópicos | A3_Psychotropics |
| B1 | Psicotrópicos | B1_Psychotropics |
| B2 | Psicotrópicos anorexígenos | B2_Anorexigenics |
| C1 | Outras substâncias controladas | C1_OtherControlled |
| C2 | Retinóides | C2_Retinoids |
| C3 | Imunossupressores | C3_Immunosuppressants |
| C4 | Antirretrovirais | C4_Antiretrovirals |
| C5 | Anabolizantes | C5_Anabolics |

### 2. ICP-Brasil Digital Signature Service

**File**: `src/MedicSoft.Application/Services/ICPBrasilDigitalSignatureService.cs`

#### Status: Prepared for Future Implementation

This service provides a complete interface and stub implementation for ICP-Brasil digital signature integration. It's ready for future certificate integration when required.

#### Features:
- ✅ Interface defined for certificate-based signing
- ✅ Support planned for A1 (software) and A3 (token/smartcard) certificates
- ✅ Certificate validation structure
- ✅ Signature verification methods
- ⏳ Full implementation pending (requires ICP-Brasil certificate integration)

#### Interface:
```csharp
public interface IICPBrasilDigitalSignatureService
{
    Task<DigitalSignatureResult> SignDocumentAsync(
        string documentContent, 
        string? certificatePath = null, 
        string? certificatePassword = null);
    
    Task<bool> ValidateSignatureAsync(
        string documentContent, 
        string signature, 
        string certificateThumbprint);
    
    Task<CertificateInfo> GetCertificateInfoAsync(string? certificatePath = null);
}
```

#### Integration Notes:
For production deployment, consider these libraries:
- **Lacuna PKI SDK** (commercial, full ICP-Brasil support)
- **DigitalSignature.NET** (open source)
- Direct **PKCS#11** integration for A3 tokens

### 3. Frontend Integration

#### Routes Added (`app.routes.ts`):
```typescript
// Digital Prescription Routes
{ 
  path: 'prescriptions/new/:medicalRecordId', 
  loadComponent: () => import('./pages/prescriptions/digital-prescription-form.component')
}
{ 
  path: 'prescriptions/view/:id', 
  loadComponent: () => import('./pages/prescriptions/digital-prescription-view.component')
}
{ 
  path: 'sngpc/dashboard', 
  loadComponent: () => import('./pages/prescriptions/sngpc-dashboard.component')
}
```

#### Attendance Flow Integration

The attendance page now includes a dedicated section for digital prescriptions:

**Location**: After exam requests section  
**Features**:
- Link to create new digital prescription (requires saved medical record)
- Visual indication of CFM and ANVISA compliance
- Automatic routing with medical record ID
- Warning if medical record not yet saved

**UI Components**:
```html
<div class="card">
  <div class="card-header-flex">
    <h3>Receitas Médicas Digitais - CFM 1.643/2002 + ANVISA</h3>
    <button [routerLink]="['/prescriptions/new', medicalRecord()!.id]">
      Nova Receita Digital
    </button>
  </div>
  <div class="prescription-info">
    <p>Sistema de Receitas Digitais: Crie receitas em conformidade...</p>
  </div>
</div>
```

#### Navigation Menu

Added SNGPC dashboard to navbar under new "Compliance" section:

```html
<div class="nav-section-title">
  <span class="nav-text">Compliance</span>
</div>
<a routerLink="/sngpc/dashboard" routerLinkActive="active">
  <svg>...</svg>
  <span class="nav-text">SNGPC - ANVISA</span>
</a>
```

### 4. Backend Updates

#### Controller Enhancement (`SNGPCReportsController.cs`):

Updated the XML generation endpoint to use the new service:

```csharp
[HttpPost("{id}/generate-xml")]
public async Task<ActionResult> GenerateXML(Guid id)
{
    var report = await _reportRepository.GetByIdAsync(id, GetTenantId());
    var prescriptions = new List<DigitalPrescription>();
    
    // Fetch all prescriptions with items
    foreach (var prescriptionId in report.PrescriptionIds)
    {
        var prescription = await _prescriptionRepository
            .GetByIdWithItemsAsync(prescriptionId, GetTenantId());
        if (prescription != null)
            prescriptions.Add(prescription);
    }
    
    // Generate ANVISA-compliant XML
    var xmlContent = await _xmlGeneratorService.GenerateXmlAsync(report, prescriptions);
    var totalItems = prescriptions.Sum(p => p.Items.Count(i => i.IsControlledSubstance));
    
    report.GenerateXML(xmlContent, totalItems);
    await _reportRepository.UpdateAsync(report);
    
    return Ok(new { message = "XML generated successfully using ANVISA schema v2.1" });
}
```

#### Repository Enhancement:

Added method to fetch prescriptions with items included:

```csharp
public async Task<DigitalPrescription?> GetByIdWithItemsAsync(Guid id, string tenantId)
{
    return await _dbSet
        .Include(dp => dp.Items)
        .FirstOrDefaultAsync(dp => dp.Id == id && dp.TenantId == tenantId);
}
```

#### Dependency Injection (`Program.cs`):

```csharp
// Digital Prescriptions and SNGPC - CFM 1.643/2002 + ANVISA
builder.Services.AddScoped<IDigitalPrescriptionRepository, DigitalPrescriptionRepository>();
builder.Services.AddScoped<IDigitalPrescriptionItemRepository, DigitalPrescriptionItemRepository>();
builder.Services.AddScoped<ISNGPCReportRepository, SNGPCReportRepository>();
builder.Services.AddScoped<IPrescriptionSequenceControlRepository, PrescriptionSequenceControlRepository>();

// Digital Prescriptions and SNGPC Services
builder.Services.AddScoped<ISNGPCXmlGeneratorService, SNGPCXmlGeneratorService>();
builder.Services.AddScoped<IICPBrasilDigitalSignatureService, ICPBrasilDigitalSignatureService>();
```

## Usage Workflow

### Creating a Digital Prescription

1. **Start Attendance**: Doctor opens patient attendance
2. **Fill Clinical Information**: Complete all CFM 1.821 required fields
3. **Save Medical Record**: Save the medical record first
4. **Create Prescription**: Click "Nova Receita Digital" button
5. **Select Type**: Choose prescription type (Simple, ControlledA/B/C1, Antimicrobial)
6. **Add Medications**: Add controlled substances with proper ANVISA classification
7. **Sign (Optional)**: Use ICP-Brasil signature if available
8. **Generate**: System creates prescription with automatic sequence number

### Generating SNGPC Report

1. **Navigate**: Go to SNGPC Dashboard via navbar
2. **Create Report**: Click "Create New Report" for specific month/year
3. **System Collects**: Automatically includes all unreported controlled prescriptions
4. **Generate XML**: Click "Generate XML" to create ANVISA-compliant file
5. **Review**: Download and review generated XML
6. **Transmit**: Mark as transmitted when sent to ANVISA system
7. **Track**: Monitor transmission status and deadlines

### SNGPC Reporting Timeline

- **Frequency**: Monthly
- **Deadline**: 10th day of following month
- **Scope**: All controlled substance prescriptions (Types A, B, C1)
- **Format**: XML file following ANVISA schema v2.1
- **Validation**: System checks deadline compliance and provides alerts

## Database Schema

### Tables Used:

1. **DigitalPrescriptions**
   - Id, MedicalRecordId, PatientId, DoctorId
   - Type, SequenceNumber, IssuedAt, ExpiresAt
   - DoctorName, DoctorCRM, DoctorCRMState
   - PatientName, PatientDocument
   - DigitalSignature, SignedAt, SignatureCertificate
   - VerificationCode, RequiresSNGPCReport, ReportedToSNGPCAt

2. **DigitalPrescriptionItems**
   - Id, DigitalPrescriptionId, MedicationId
   - MedicationName, GenericName, ActiveIngredient
   - Dosage, PharmaceuticalForm, Frequency, DurationDays, Quantity
   - IsControlledSubstance, ControlledList
   - AnvisaRegistration, AdministrationRoute, Instructions

3. **SNGPCReports**
   - Id, Month, Year, ReportPeriodStart, ReportPeriodEnd
   - Status, GeneratedAt, TransmittedAt, TransmissionProtocol
   - XmlContent, XmlHash
   - TotalPrescriptions, TotalItems
   - ErrorMessage, LastAttemptAt, AttemptCount

4. **PrescriptionSequenceControl**
   - Id, PrescriptionType, Year, CurrentSequence, Prefix

## Security Considerations

### Data Protection:
- ✅ Prescriptions stored with tenant isolation
- ✅ Patient information sanitized in XML
- ✅ SHA-256 hash for XML integrity
- ✅ Soft-delete for 20-year retention (CFM requirement)

### Access Control:
- ✅ Authentication required for all endpoints
- ✅ Tenant-based authorization
- ✅ Medical record ownership validation

### Compliance:
- ✅ CFM 1.643/2002: Digital prescription structure
- ✅ ANVISA 344/1998: Controlled substance classification
- ✅ SNGPC v2.1: XML schema compliance
- ⏳ ICP-Brasil: Prepared for digital signature (not yet implemented)

## Testing

### Manual Testing Steps:

1. **Build Verification**:
   ```bash
   cd /home/runner/work/MW.Code/MW.Code
   dotnet restore src/MedicSoft.Api/MedicSoft.Api.csproj
   dotnet build src/MedicSoft.Api/MedicSoft.Api.csproj
   ```

2. **API Testing**:
   - Create digital prescription via POST /api/digitalprescriptions
   - Retrieve prescription via GET /api/digitalprescriptions/{id}
   - Create SNGPC report via POST /api/sngpcreports
   - Generate XML via POST /api/sngpcreports/{id}/generate-xml
   - Download XML via GET /api/sngpcreports/{id}/download-xml

3. **Frontend Testing**:
   - Open attendance page
   - Verify prescription section visibility
   - Click "Nova Receita Digital" (after saving medical record)
   - Navigate to SNGPC dashboard
   - Verify compliance section in navbar

### Expected Results:
- ✅ All endpoints return 200 OK for valid requests
- ✅ XML validates against ANVISA schema v2.1
- ✅ Controlled substances properly classified
- ✅ Sequence numbers generated correctly
- ✅ UI displays prescription integration

## Future Enhancements

### Short Term (Q1 2026):
1. **ICP-Brasil Integration**: Complete certificate-based signing
2. **Prescription Templates**: Common prescription templates
3. **Drug Database**: Integration with ANVISA drug database
4. **Validation Rules**: Enhanced CFM/ANVISA validation

### Medium Term (Q2-Q3 2026):
1. **ANVISA Integration**: Direct transmission to SNGPC system
2. **Electronic Verification**: QR code verification for pharmacies
3. **Batch Operations**: Bulk prescription processing
4. **Audit Trail**: Complete prescription lifecycle tracking

### Long Term (Q4 2026+):
1. **AI Assistance**: Drug interaction checking
2. **Patient Portal**: Prescription access for patients
3. **Pharmacy Integration**: Direct e-prescription to pharmacies
4. **Analytics**: Prescription patterns and compliance reports

## References

### Regulatory Documents:
- CFM 1.643/2002: https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2002/1643
- CFM 1.821/2007: https://sistemas.cfm.org.br/normas/visualizar/resolucoes/BR/2007/1821
- ANVISA Portaria 344/1998: http://www.anvisa.gov.br/legis/portarias/344_98.htm
- SNGPC RDC 22/2014: https://www.in.gov.br/materia/-/asset_publisher/Kujrw0TZC2Mb/content/id/31898851

### Technical Documentation:
- SNGPC XML Schema v2.1: http://www.anvisa.gov.br/sngpc/
- ICP-Brasil Standards: https://www.gov.br/iti/pt-br/centrais-de-conteudo/documentos-de-referencia

## Support

For questions or issues related to this implementation:
- Review the PENDING_TASKS.md document
- Check the implementation summary above
- Consult the regulatory references
- Contact the development team

---

**Implementation Date**: January 2026  
**Version**: 1.0  
**Status**: ✅ Complete (Backend + Frontend Integration)  
**Remaining**: ICP-Brasil full implementation (prepared)
