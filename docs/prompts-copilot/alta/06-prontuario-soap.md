# 📝 Prompt: Prontuário SOAP Estruturado

## 📊 Status
- **Prioridade**: 🔥🔥 ALTA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 1-2 meses | 1 dev
- **Prazo**: Q1/2025

## 🎯 Contexto

Implementar prontuário médico estruturado no padrão SOAP (Subjetivo-Objetivo-Avaliação-Plano), que é o padrão internacional para documentação clínica. Isso melhora a qualidade dos registros, facilita pesquisas e prepara o sistema para futura análise por IA.

## 📋 Estrutura SOAP

### S - Subjetivo
Informações relatadas pelo paciente:
- Queixa principal
- História da doença atual (HDA)
- Sintomas atuais
- Duração dos sintomas
- Fatores de melhora/piora
- Revisão de sistemas

### O - Objetivo
Dados objetivos coletados pelo médico:
- Sinais vitais (PA, FC, FR, Temp, SpO2, Peso, Altura, IMC)
- Exame físico por sistemas
- Resultados de exames complementares
- Achados clínicos

### A - Avaliação
Interpretação médica:
- Hipóteses diagnósticas (principal + diferenciais)
- CID-10
- Raciocínio clínico
- Evolução do quadro

### P - Plano
Condutas a serem tomadas:
- Prescrições medicamentosas
- Exames solicitados
- Procedimentos
- Encaminhamentos
- Retorno
- Orientações ao paciente

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// Entidades
public class SoapRecord : Entity
{
    public Guid Id { get; set; }
    public Guid AttendanceId { get; set; }
    public string TenantId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime RecordDate { get; set; }
    
    // S - Subjetivo
    public SubjectiveData Subjective { get; set; }
    
    // O - Objetivo
    public ObjectiveData Objective { get; set; }
    
    // A - Avaliação
    public AssessmentData Assessment { get; set; }
    
    // P - Plano
    public PlanData Plan { get; set; }
    
    // Metadados
    public bool IsComplete { get; set; }
    public DateTime? CompletionDate { get; set; }
    public bool IsLocked { get; set; }  // Após conclusão
    
    // Navigation
    public virtual Attendance Attendance { get; set; }
    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }
}

// Value Objects
public class SubjectiveData : ValueObject
{
    public string ChiefComplaint { get; set; }  // Queixa principal
    public string HistoryOfPresentIllness { get; set; }  // HDA
    public string CurrentSymptoms { get; set; }
    public string SymptomDuration { get; set; }
    public string AggravatingFactors { get; set; }  // Fatores de piora
    public string RelievingFactors { get; set; }  // Fatores de melhora
    public string ReviewOfSystems { get; set; }  // Revisão de sistemas
    public string Allergies { get; set; }
    public string CurrentMedications { get; set; }
    public string PastMedicalHistory { get; set; }
    public string FamilyHistory { get; set; }
    public string SocialHistory { get; set; }  // Hábitos
}

public class ObjectiveData : ValueObject
{
    // Sinais Vitais
    public VitalSigns VitalSigns { get; set; }
    
    // Exame Físico
    public PhysicalExamination PhysicalExam { get; set; }
    
    // Exames Complementares
    public string LabResults { get; set; }
    public string ImagingResults { get; set; }
    public string OtherExamResults { get; set; }
}

public class VitalSigns : ValueObject
{
    public int? SystolicBP { get; set; }  // mmHg
    public int? DiastolicBP { get; set; }  // mmHg
    public int? HeartRate { get; set; }  // bpm
    public int? RespiratoryRate { get; set; }  // rpm
    public decimal? Temperature { get; set; }  // °C
    public int? OxygenSaturation { get; set; }  // %
    public decimal? Weight { get; set; }  // kg
    public decimal? Height { get; set; }  // cm
    public decimal? BMI { get; set; }  // calculado
    public int? Pain { get; set; }  // Escala 0-10
    
    public void CalculateBMI()
    {
        if (Weight.HasValue && Height.HasValue && Height.Value > 0)
        {
            var heightInMeters = Height.Value / 100;
            BMI = Weight.Value / (heightInMeters * heightInMeters);
        }
    }
}

public class PhysicalExamination : ValueObject
{
    public string GeneralAppearance { get; set; }
    public string Head { get; set; }
    public string Eyes { get; set; }
    public string Ears { get; set; }
    public string Nose { get; set; }
    public string Throat { get; set; }
    public string Neck { get; set; }
    public string Cardiovascular { get; set; }
    public string Respiratory { get; set; }
    public string Abdomen { get; set; }
    public string Musculoskeletal { get; set; }
    public string Neurological { get; set; }
    public string Skin { get; set; }
    public string OtherFindings { get; set; }
}

public class AssessmentData : ValueObject
{
    public string PrimaryDiagnosis { get; set; }
    public string PrimaryDiagnosisIcd10 { get; set; }
    
    public List<DifferentialDiagnosis> DifferentialDiagnoses { get; set; }
    
    public string ClinicalReasoning { get; set; }  // Raciocínio clínico
    public string Prognosis { get; set; }
    public string Evolution { get; set; }  // Evolução do quadro
}

public class DifferentialDiagnosis : ValueObject
{
    public string Diagnosis { get; set; }
    public string Icd10Code { get; set; }
    public string Justification { get; set; }
    public int Priority { get; set; }  // 1 = mais provável
}

public class PlanData : ValueObject
{
    public List<Prescription> Prescriptions { get; set; }
    public List<ExamRequest> ExamRequests { get; set; }
    public List<Procedure> Procedures { get; set; }
    public List<Referral> Referrals { get; set; }
    
    public string ReturnInstructions { get; set; }
    public DateTime? NextAppointmentDate { get; set; }
    public string PatientInstructions { get; set; }  // Orientações
    public string DietaryRecommendations { get; set; }
    public string ActivityRestrictions { get; set; }
    public string WarningSymptoms { get; set; }  // Sinais de alerta
}

public class Prescription : ValueObject
{
    public string MedicationName { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public string Duration { get; set; }
    public string Instructions { get; set; }
}

public class ExamRequest : ValueObject
{
    public string ExamName { get; set; }
    public string ExamType { get; set; }  // Lab, Imaging, etc.
    public string ClinicalIndication { get; set; }
    public bool IsUrgent { get; set; }
}

public class Procedure : ValueObject
{
    public string ProcedureName { get; set; }
    public string Description { get; set; }
    public DateTime? ScheduledDate { get; set; }
}

public class Referral : ValueObject
{
    public string SpecialtyName { get; set; }
    public string Reason { get; set; }
    public string Priority { get; set; }  // Routine, Urgent, Emergency
}
```

### Camada de Aplicação (Application Layer)

```csharp
// Service Interface
public interface ISoapRecordService
{
    Task<SoapRecord> CreateSoapRecord(Guid attendanceId);
    Task<SoapRecord> UpdateSubjective(Guid soapId, SubjectiveData data);
    Task<SoapRecord> UpdateObjective(Guid soapId, ObjectiveData data);
    Task<SoapRecord> UpdateAssessment(Guid soapId, AssessmentData data);
    Task<SoapRecord> UpdatePlan(Guid soapId, PlanData data);
    Task<SoapRecord> CompleteSoapRecord(Guid soapId);
    Task<SoapRecord> GetBySoapId(Guid soapId);
    Task<List<SoapRecord>> GetByPatientId(Guid patientId);
    Task<SoapRecordValidation> ValidateCompleteness(Guid soapId);
}

// DTOs
public record UpdateSubjectiveCommand(
    Guid SoapId,
    string ChiefComplaint,
    string HistoryOfPresentIllness,
    string CurrentSymptoms,
    string SymptomDuration,
    string Allergies,
    string CurrentMedications
);

public record UpdateObjectiveCommand(
    Guid SoapId,
    VitalSigns VitalSigns,
    PhysicalExamination PhysicalExam,
    string LabResults,
    string ImagingResults
);

public record UpdateAssessmentCommand(
    Guid SoapId,
    string PrimaryDiagnosis,
    string PrimaryDiagnosisIcd10,
    List<DifferentialDiagnosis> DifferentialDiagnoses,
    string ClinicalReasoning
);

public record UpdatePlanCommand(
    Guid SoapId,
    List<Prescription> Prescriptions,
    List<ExamRequest> ExamRequests,
    string ReturnInstructions,
    string PatientInstructions
);

// Validation
public class SoapRecordValidation
{
    public bool IsValid { get; set; }
    public List<string> MissingFields { get; set; }
    public List<string> Warnings { get; set; }
    
    public bool HasSubjective { get; set; }
    public bool HasObjective { get; set; }
    public bool HasAssessment { get; set; }
    public bool HasPlan { get; set; }
}
```

### Camada de API (API Layer)

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SoapRecordsController : ControllerBase
{
    private readonly ISoapRecordService _soapRecordService;
    
    [HttpPost("attendance/{attendanceId}")]
    public async Task<IActionResult> CreateSoapRecord(Guid attendanceId)
    {
        var soapRecord = await _soapRecordService.CreateSoapRecord(attendanceId);
        return CreatedAtAction(nameof(GetSoapRecord), new { id = soapRecord.Id }, soapRecord);
    }
    
    [HttpPut("{id}/subjective")]
    public async Task<IActionResult> UpdateSubjective(
        Guid id, 
        [FromBody] UpdateSubjectiveCommand command)
    {
        command = command with { SoapId = id };
        var soapRecord = await _soapRecordService.UpdateSubjective(id, command.ToData());
        return Ok(soapRecord);
    }
    
    [HttpPut("{id}/objective")]
    public async Task<IActionResult> UpdateObjective(
        Guid id, 
        [FromBody] UpdateObjectiveCommand command)
    {
        command = command with { SoapId = id };
        var soapRecord = await _soapRecordService.UpdateObjective(id, command.ToData());
        return Ok(soapRecord);
    }
    
    [HttpPut("{id}/assessment")]
    public async Task<IActionResult> UpdateAssessment(
        Guid id, 
        [FromBody] UpdateAssessmentCommand command)
    {
        command = command with { SoapId = id };
        var soapRecord = await _soapRecordService.UpdateAssessment(id, command.ToData());
        return Ok(soapRecord);
    }
    
    [HttpPut("{id}/plan")]
    public async Task<IActionResult> UpdatePlan(
        Guid id, 
        [FromBody] UpdatePlanCommand command)
    {
        command = command with { SoapId = id };
        var soapRecord = await _soapRecordService.UpdatePlan(id, command.ToData());
        return Ok(soapRecord);
    }
    
    [HttpPost("{id}/complete")]
    public async Task<IActionResult> CompleteSoapRecord(Guid id)
    {
        // Validate completeness
        var validation = await _soapRecordService.ValidateCompleteness(id);
        
        if (!validation.IsValid)
        {
            return BadRequest(new { 
                message = "Prontuário incompleto", 
                missingFields = validation.MissingFields 
            });
        }
        
        var soapRecord = await _soapRecordService.CompleteSoapRecord(id);
        return Ok(soapRecord);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSoapRecord(Guid id)
    {
        var soapRecord = await _soapRecordService.GetBySoapId(id);
        if (soapRecord == null)
            return NotFound();
        
        return Ok(soapRecord);
    }
    
    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetPatientSoapRecords(Guid patientId)
    {
        var records = await _soapRecordService.GetByPatientId(patientId);
        return Ok(records);
    }
    
    [HttpGet("{id}/validate")]
    public async Task<IActionResult> ValidateSoapRecord(Guid id)
    {
        var validation = await _soapRecordService.ValidateCompleteness(id);
        return Ok(validation);
    }
}
```

## 🎨 Frontend (Angular)

### Componentes Necessários

```typescript
// SOAP Record Component
@Component({
  selector: 'app-soap-record',
  template: `
    <mat-stepper [linear]="true" #stepper>
      <!-- S - Subjective -->
      <mat-step [stepControl]="subjectiveForm">
        <ng-template matStepLabel>Subjetivo</ng-template>
        <app-subjective-form [soapId]="soapId" (saved)="stepper.next()"></app-subjective-form>
      </mat-step>
      
      <!-- O - Objective -->
      <mat-step [stepControl]="objectiveForm">
        <ng-template matStepLabel>Objetivo</ng-template>
        <app-objective-form [soapId]="soapId" (saved)="stepper.next()"></app-objective-form>
      </mat-step>
      
      <!-- A - Assessment -->
      <mat-step [stepControl]="assessmentForm">
        <ng-template matStepLabel>Avaliação</ng-template>
        <app-assessment-form [soapId]="soapId" (saved)="stepper.next()"></app-assessment-form>
      </mat-step>
      
      <!-- P - Plan -->
      <mat-step [stepControl]="planForm">
        <ng-template matStepLabel>Plano</ng-template>
        <app-plan-form [soapId]="soapId" (saved)="completeSoap()"></app-plan-form>
      </mat-step>
      
      <!-- Summary -->
      <mat-step>
        <ng-template matStepLabel>Revisar</ng-template>
        <app-soap-summary [soapId]="soapId"></app-soap-summary>
        <button mat-raised-button color="primary" (click)="completeAndLock()">
          Concluir e Bloquear Prontuário
        </button>
      </mat-step>
    </mat-stepper>
  `
})
export class SoapRecordComponent { }

// Subjective Form Component
@Component({
  selector: 'app-subjective-form',
  template: `
    <form [formGroup]="form" (ngSubmit)="save()">
      <h3>Dados Subjetivos</h3>
      
      <mat-form-field class="full-width">
        <textarea matInput placeholder="Queixa Principal" 
                  formControlName="chiefComplaint" rows="2"></textarea>
      </mat-form-field>
      
      <mat-form-field class="full-width">
        <textarea matInput placeholder="História da Doença Atual" 
                  formControlName="historyOfPresentIllness" rows="4"></textarea>
      </mat-form-field>
      
      <mat-form-field class="full-width">
        <textarea matInput placeholder="Sintomas Atuais" 
                  formControlName="currentSymptoms" rows="3"></textarea>
      </mat-form-field>
      
      <mat-form-field>
        <input matInput placeholder="Duração dos Sintomas" formControlName="symptomDuration">
      </mat-form-field>
      
      <mat-form-field class="full-width">
        <textarea matInput placeholder="Alergias" formControlName="allergies" rows="2"></textarea>
      </mat-form-field>
      
      <mat-form-field class="full-width">
        <textarea matInput placeholder="Medicamentos em Uso" 
                  formControlName="currentMedications" rows="2"></textarea>
      </mat-form-field>
      
      <button mat-raised-button color="primary" type="submit">Salvar e Avançar</button>
    </form>
  `
})
export class SubjectiveFormComponent { }

// Objective Form Component (Vital Signs)
@Component({
  selector: 'app-objective-form',
  template: `
    <form [formGroup]="form" (ngSubmit)="save()">
      <h3>Sinais Vitais</h3>
      
      <div class="vital-signs-grid">
        <mat-form-field>
          <input matInput placeholder="PA Sistólica" formControlName="systolicBP" type="number">
          <span matSuffix>mmHg</span>
        </mat-form-field>
        
        <mat-form-field>
          <input matInput placeholder="PA Diastólica" formControlName="diastolicBP" type="number">
          <span matSuffix>mmHg</span>
        </mat-form-field>
        
        <mat-form-field>
          <input matInput placeholder="Frequência Cardíaca" formControlName="heartRate" type="number">
          <span matSuffix>bpm</span>
        </mat-form-field>
        
        <mat-form-field>
          <input matInput placeholder="Frequência Respiratória" formControlName="respiratoryRate" type="number">
          <span matSuffix>rpm</span>
        </mat-form-field>
        
        <mat-form-field>
          <input matInput placeholder="Temperatura" formControlName="temperature" type="number" step="0.1">
          <span matSuffix>°C</span>
        </mat-form-field>
        
        <mat-form-field>
          <input matInput placeholder="SpO2" formControlName="oxygenSaturation" type="number">
          <span matSuffix>%</span>
        </mat-form-field>
        
        <mat-form-field>
          <input matInput placeholder="Peso" formControlName="weight" type="number" step="0.1">
          <span matSuffix>kg</span>
        </mat-form-field>
        
        <mat-form-field>
          <input matInput placeholder="Altura" formControlName="height" type="number">
          <span matSuffix>cm</span>
        </mat-form-field>
        
        <div class="bmi-display">
          <strong>IMC:</strong> {{ calculateBMI() | number:'1.1-1' }}
        </div>
      </div>
      
      <h3>Exame Físico</h3>
      
      <mat-expansion-panel>
        <mat-expansion-panel-header>
          <mat-panel-title>Aparência Geral</mat-panel-title>
        </mat-expansion-panel-header>
        <mat-form-field class="full-width">
          <textarea matInput formControlName="generalAppearance" rows="2"></textarea>
        </mat-form-field>
      </mat-expansion-panel>
      
      <mat-expansion-panel>
        <mat-expansion-panel-header>
          <mat-panel-title>Cardiovascular</mat-panel-title>
        </mat-expansion-panel-header>
        <mat-form-field class="full-width">
          <textarea matInput formControlName="cardiovascular" rows="2"></textarea>
        </mat-form-field>
      </mat-expansion-panel>
      
      <mat-expansion-panel>
        <mat-expansion-panel-header>
          <mat-panel-title>Respiratório</mat-panel-title>
        </mat-expansion-panel-header>
        <mat-form-field class="full-width">
          <textarea matInput formControlName="respiratory" rows="2"></textarea>
        </mat-form-field>
      </mat-expansion-panel>
      
      <!-- More exam panels... -->
      
      <button mat-raised-button color="primary" type="submit">Salvar e Avançar</button>
    </form>
  `
})
export class ObjectiveFormComponent { }
```

## 📋 Checklist de Implementação

### Backend

- [ ] Criar entidades de domínio (SoapRecord, SubjectiveData, etc.)
- [ ] Implementar Value Objects
- [ ] Criar repositórios
- [ ] Implementar serviços de aplicação
- [ ] Criar validações de completude
- [ ] Implementar bloqueio após conclusão
- [ ] Criar controllers REST
- [ ] Adicionar migrations
- [ ] Implementar testes unitários
- [ ] Implementar testes de integração

### Frontend

- [ ] Criar componente SOAP com stepper
- [ ] Implementar formulário Subjetivo
- [ ] Implementar formulário Objetivo (sinais vitais + exame físico)
- [ ] Implementar formulário Avaliação (diagnósticos + CID-10)
- [ ] Implementar formulário Plano (prescrições + exames)
- [ ] Criar visualizador de resumo
- [ ] Implementar validação de campos obrigatórios
- [ ] Adicionar busca de CID-10
- [ ] Criar histórico de SOAP por paciente
- [ ] Implementar impressão de prontuário SOAP

### Migração

- [ ] Manter prontuários antigos (texto livre)
- [ ] Permitir migração gradual
- [ ] Criar conversor de texto livre para SOAP (assistido por IA)
- [ ] Documentar processo de migração

## 🧪 Testes

### Testes Unitários
```csharp
public class SoapRecordServiceTests
{
    [Fact]
    public async Task ShouldCreateSoapRecord()
    {
        // Test SOAP creation
    }
    
    [Fact]
    public async Task ShouldValidateCompleteness()
    {
        // Test validation
    }
    
    [Fact]
    public async Task ShouldLockAfterCompletion()
    {
        // Test locking mechanism
    }
}
```

## 📚 Referências

- [PENDING_TASKS.md - Seção Prontuário SOAP](../../PENDING_TASKS.md#4-prontuário-soap-estruturado)
- [SOAP Note Wikipedia](https://en.wikipedia.org/wiki/SOAP_note)
- [ANALISE_MELHORIAS_SISTEMA.md](../../ANALISE_MELHORIAS_SISTEMA.md)

## 💰 Investimento

- **Desenvolvimento**: 1-2 meses, 1 dev
- **Custo**: R$ 30-45k
- **ROI Esperado**: Qualidade de registros, base para IA futura

## ✅ Critérios de Aceitação

1. ✅ Prontuário estruturado em 4 seções (S-O-A-P)
2. ✅ Sinais vitais são capturados de forma estruturada
3. ✅ Diagnósticos incluem código CID-10
4. ✅ Sistema valida completude antes de concluir
5. ✅ Prontuário é bloqueado após conclusão
6. ✅ Histórico de prontuários SOAP por paciente
7. ✅ Retrocompatibilidade com prontuários antigos
8. ✅ Impressão de prontuário formatado
9. ✅ Pesquisa por diagnóstico (CID-10)
10. ✅ Análise estatística de diagnósticos mais comuns

---

**Última Atualização**: Janeiro 2026
**Status**: Pronto para desenvolvimento
**Próximo Passo**: Iniciar implementação backend com entidades de domínio
