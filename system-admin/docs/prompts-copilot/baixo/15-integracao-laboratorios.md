# 🔬 Prompt: Integração com Laboratórios (HL7 FHIR)

## 📊 Status
- **Prioridade**: BAIXA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 4-6 meses | 2 devs
- **Prazo**: Q4/2026

## 🎯 Contexto

Implementar integração bidirecional com laboratórios parceiros usando padrão HL7 FHIR para envio automático de requisições de exames e recebimento de resultados diretamente no prontuário eletrônico.

## 📋 Justificativa

### Problemas Atuais
- ❌ Requisições manuais (papel/fax)
- ❌ Resultados chegam por email/papel
- ❌ Digitação manual de resultados
- ❌ Demora na disponibilização
- ❌ Erros de transcrição

### Benefícios
- ✅ Automação completa
- ✅ Resultados em tempo real
- ✅ Zero papel
- ✅ Redução de erros
- ✅ Melhor experiência médico/paciente

## 🏗️ Arquitetura

### HL7 FHIR Resources

```csharp
// Service Request (Requisição de Exame)
public class LabServiceRequest : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid LabId { get; set; }
    public string FhirId { get; set; }
    public ServiceRequestStatus Status { get; set; }
    public List<RequestedTest> Tests { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CollectedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string Priority { get; set; }  // routine, urgent, asap
    public string ClinicalInfo { get; set; }
}

// Diagnostic Report (Resultado)
public class LabDiagnosticReport : Entity
{
    public Guid Id { get; set; }
    public Guid ServiceRequestId { get; set; }
    public string FhirId { get; set; }
    public DiagnosticReportStatus Status { get; set; }
    public DateTime IssuedAt { get; set; }
    public string PdfUrl { get; set; }
    public List<ObservationResult> Results { get; set; }
}

public class ObservationResult
{
    public string Code { get; set; }
    public string Display { get; set; }
    public string Value { get; set; }
    public string Unit { get; set; }
    public string ReferenceRange { get; set; }
    public string Interpretation { get; set; }  // Normal, High, Low
}
```

### FHIR API Integration

```csharp
public class FhirLabService : ILabService
{
    private readonly FhirClient _fhirClient;
    
    public async Task<string> SendServiceRequestAsync(LabServiceRequest request)
    {
        var fhirRequest = new ServiceRequest
        {
            Status = RequestStatus.Active,
            Intent = RequestIntent.Order,
            Priority = RequestPriority.Routine,
            Subject = new ResourceReference($"Patient/{request.PatientId}"),
            Requester = new ResourceReference($"Practitioner/{request.DoctorId}"),
            Code = new CodeableConcept("http://loinc.org", request.TestCode),
            Occurrence = new FhirDateTime(request.RequestedAt),
            Note = new List<Annotation>
            {
                new Annotation { Text = request.ClinicalInfo }
            }
        };
        
        var response = await _fhirClient.CreateAsync(fhirRequest);
        return response.Id;
    }
    
    public async Task<LabDiagnosticReport> GetDiagnosticReportAsync(string fhirId)
    {
        var report = await _fhirClient.ReadAsync<DiagnosticReport>(fhirId);
        
        return new LabDiagnosticReport
        {
            FhirId = report.Id,
            Status = report.Status.Value,
            IssuedAt = report.Issued.Value.DateTime,
            Results = report.Result.Select(r => ConvertObservation(r)).ToList()
        };
    }
}
```

### Laboratórios Parceiros

```csharp
public class Laboratory : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Cnpj { get; set; }
    public LabIntegrationType IntegrationType { get; set; }
    public string FhirEndpoint { get; set; }
    public string ApiKey { get; set; }
    public bool IsActive { get; set; }
    public List<AvailableTest> AvailableTests { get; set; }
}

public enum LabIntegrationType
{
    Fhir,           // HL7 FHIR
    ProprietaryApi, // API proprietária
    Manual          // Sem integração
}
```

## 🎨 Frontend (Angular)

```typescript
// Lab request component
@Component({
  selector: 'app-lab-request-form',
  template: `
    <h2>Solicitar Exames</h2>
    
    <mat-form-field>
      <mat-label>Laboratório</mat-label>
      <mat-select [(ngModel)]="selectedLab">
        <mat-option *ngFor="let lab of laboratories" [value]="lab">
          {{ lab.name }}
        </mat-option>
      </mat-select>
    </mat-form-field>
    
    <mat-selection-list [(ngModel)]="selectedTests">
      <mat-list-option *ngFor="let test of availableTests" [value]="test">
        {{ test.name }} - {{ test.price | currency:'BRL' }}
      </mat-list-option>
    </mat-selection-list>
    
    <button mat-raised-button color="primary" (click)="sendRequest()">
      Enviar Requisição
    </button>
  `
})
export class LabRequestFormComponent {
  laboratories: Laboratory[] = [];
  selectedLab: Laboratory;
  selectedTests: Test[] = [];
  
  async sendRequest() {
    const request = await this.labService.sendRequest({
      patientId: this.patientId,
      labId: this.selectedLab.id,
      tests: this.selectedTests
    });
    
    this.snackBar.open('Requisição enviada!', 'OK');
  }
}
```

## ✅ Checklist

### Backend
- [ ] Implementar FHIR client
- [ ] ServiceRequest mapping
- [ ] DiagnosticReport parser
- [ ] Webhook receiver (resultados)
- [ ] Laboratory entity
- [ ] Controllers REST
- [ ] Migrations

### Integrações
- [ ] Dasa
- [ ] Fleury
- [ ] Hermes Pardini
- [ ] Sabin
- [ ] DB Diagnósticos

### Frontend
- [ ] LabRequestFormComponent
- [ ] LabResultsViewerComponent
- [ ] LabHistoryComponent
- [ ] PDF viewer

### Testes
- [ ] Testes FHIR
- [ ] Testes de integração
- [ ] Mock lab endpoints

## 💰 Investimento

- **Esforço**: 4-6 meses | 2 devs
- **Custo**: R$ 180-270k

## 🎯 Critérios de Aceitação

- [ ] Envio automático de requisições
- [ ] Recebimento de resultados via webhook
- [ ] Resultados aparecem no prontuário
- [ ] Suporte HL7 FHIR
- [ ] 3+ laboratórios integrados
- [ ] PDF de resultados disponível
