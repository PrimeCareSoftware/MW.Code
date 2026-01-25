# 📋 Prompt: Anamnese Guiada por Especialidade

## 📊 Status
- **Prioridade**: 🔥 MÉDIA
- **Progresso**: 0% (Não iniciado)
- **Esforço**: 1 mês | 1 dev
- **Prazo**: Q3/2026

## 🎯 Contexto

Implementar sistema de anamnese estruturada com perguntas padronizadas e checklist de sintomas específicos por especialidade médica. Este sistema melhora a qualidade do atendimento, padroniza registros, aumenta produtividade dos médicos e prepara a base para futuras análises por IA e machine learning.

## 📋 Justificativa

### Problemas Atuais
- ❌ Anamnese em texto livre sem padrão
- ❌ Médicos esquecem perguntas importantes
- ❌ Falta de padronização entre profissionais
- ❌ Difícil análise de dados clínicos
- ❌ Compliance com protocolos clínicos

### Benefícios
- ✅ Atendimento mais rápido e eficiente
- ✅ Padronização de registros clínicos
- ✅ Redução de erros por omissão
- ✅ Base para protocolos clínicos
- ✅ Facilita análise de dados (BI)
- ✅ Suporte para pesquisa clínica
- ✅ Compliance com melhores práticas
- ✅ Preparação para IA diagnóstica

## 🏗️ Arquitetura

### Camada de Domínio (Domain Layer)

```csharp
// src/Domain/Entities/AnamnesisTemplate.cs
public class AnamnesisTemplate : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public string Name { get; set; }
    public MedicalSpecialty Specialty { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public List<QuestionSection> Sections { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}

public class QuestionSection
{
    public string SectionName { get; set; }
    public int Order { get; set; }
    public List<Question> Questions { get; set; }
}

public class Question
{
    public string QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public bool IsRequired { get; set; }
    public List<string> Options { get; set; }  // Para múltipla escolha
    public string Unit { get; set; }  // Para campos numéricos (kg, cm, etc)
    public int Order { get; set; }
    public string HelpText { get; set; }
    public string SnomedCode { get; set; }  // Código SNOMED CT (opcional)
}

public enum QuestionType
{
    Text,           // Texto livre
    Number,         // Número
    YesNo,          // Sim/Não
    SingleChoice,   // Múltipla escolha (uma opção)
    MultipleChoice, // Múltipla escolha (várias opções)
    Date,           // Data
    Scale           // Escala (ex: dor 0-10)
}

public enum MedicalSpecialty
{
    Cardiology,
    Pediatrics,
    Gynecology,
    Dermatology,
    Orthopedics,
    Psychiatry,
    Endocrinology,
    Neurology,
    Ophthalmology,
    Otorhinolaryngology,  // Otorrino
    GeneralMedicine,
    Other
}

// src/Domain/Entities/AnamnesisResponse.cs
public class AnamnesisResponse : Entity
{
    public Guid Id { get; set; }
    public string TenantId { get; set; }
    public Guid AttendanceId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid TemplateId { get; set; }
    public DateTime ResponseDate { get; set; }
    public List<QuestionAnswer> Answers { get; set; }
    public bool IsComplete { get; set; }
    
    // Navigation
    public virtual Attendance Attendance { get; set; }
    public virtual Patient Patient { get; set; }
    public virtual Doctor Doctor { get; set; }
    public virtual AnamnesisTemplate Template { get; set; }
}

public class QuestionAnswer
{
    public string QuestionText { get; set; }
    public QuestionType Type { get; set; }
    public string Answer { get; set; }  // JSON para respostas complexas
    public List<string> SelectedOptions { get; set; }
    public int? NumericValue { get; set; }
    public bool? BooleanValue { get; set; }
    public DateTime? DateValue { get; set; }
}
```

### Camada de Aplicação (Application Layer)

```csharp
// src/Application/Services/IAnamnesisService.cs
public interface IAnamnesisService
{
    // Templates
    Task<List<AnamnesisTemplate>> GetTemplatesBySpecialtyAsync(MedicalSpecialty specialty);
    Task<AnamnesisTemplate> GetTemplateByIdAsync(Guid templateId);
    Task<AnamnesisTemplate> CreateTemplateAsync(CreateTemplateRequest request);
    Task UpdateTemplateAsync(Guid templateId, UpdateTemplateRequest request);
    Task<AnamnesisTemplate> GetDefaultTemplateAsync(MedicalSpecialty specialty);
    
    // Respostas
    Task<AnamnesisResponse> CreateResponseAsync(Guid attendanceId, Guid templateId);
    Task<AnamnesisResponse> SaveAnswersAsync(Guid responseId, List<QuestionAnswer> answers);
    Task<AnamnesisResponse> GetResponseByIdAsync(Guid responseId);
    Task<List<AnamnesisResponse>> GetPatientHistoryAsync(Guid patientId);
    
    // Análise
    Task<SymptomAnalysis> AnalyzeSymptoms(List<QuestionAnswer> answers);
    Task<List<string>> GetSuggestedDiagnosesAsync(List<QuestionAnswer> answers);
}

// DTOs
public class CreateTemplateRequest
{
    public string Name { get; set; }
    public MedicalSpecialty Specialty { get; set; }
    public string Description { get; set; }
    public List<QuestionSection> Sections { get; set; }
}

public class SymptomAnalysis
{
    public List<string> IdentifiedSymptoms { get; set; }
    public List<string> RedFlags { get; set; }
    public List<string> SuggestedExams { get; set; }
    public double UrgencyScore { get; set; }
}
```

### Templates Pré-definidos

```json
// Cardiologia
{
  "name": "Anamnese Cardiológica",
  "specialty": "Cardiology",
  "sections": [
    {
      "sectionName": "Sintomas Cardiovasculares",
      "questions": [
        {
          "questionText": "Apresenta dor torácica?",
          "type": "YesNo",
          "isRequired": true
        },
        {
          "questionText": "Tipo de dor (se sim)",
          "type": "SingleChoice",
          "options": ["Aperto", "Queimação", "Pontada", "Peso"],
          "dependsOn": "anterior"
        },
        {
          "questionText": "Palpitações?",
          "type": "YesNo",
          "isRequired": true
        },
        {
          "questionText": "Dispneia aos esforços?",
          "type": "Scale",
          "helpText": "0 = Nenhuma, 10 = Extrema"
        },
        {
          "questionText": "Edema de membros inferiores?",
          "type": "YesNo"
        },
        {
          "questionText": "Síncope ou pré-síncope?",
          "type": "YesNo"
        }
      ]
    },
    {
      "sectionName": "Fatores de Risco",
      "questions": [
        {
          "questionText": "História familiar de cardiopatia?",
          "type": "YesNo",
          "isRequired": true
        },
        {
          "questionText": "Fumante?",
          "type": "SingleChoice",
          "options": ["Nunca fumou", "Ex-fumante", "Fumante atual"]
        },
        {
          "questionText": "Diabetes mellitus?",
          "type": "YesNo"
        },
        {
          "questionText": "Hipertensão arterial?",
          "type": "YesNo"
        },
        {
          "questionText": "Dislipidemia?",
          "type": "YesNo"
        },
        {
          "questionText": "Sedentarismo?",
          "type": "YesNo"
        }
      ]
    }
  ]
}

// Pediatria
{
  "name": "Anamnese Pediátrica",
  "specialty": "Pediatrics",
  "sections": [
    {
      "sectionName": "Desenvolvimento",
      "questions": [
        {
          "questionText": "Vacinação em dia?",
          "type": "YesNo",
          "isRequired": true
        },
        {
          "questionText": "Peso ao nascer (kg)",
          "type": "Number",
          "unit": "kg"
        },
        {
          "questionText": "Idade gestacional",
          "type": "Number",
          "unit": "semanas"
        },
        {
          "questionText": "Desenvolvimento neuropsicomotor adequado?",
          "type": "YesNo"
        },
        {
          "questionText": "Com quantos meses sentou sem apoio?",
          "type": "Number"
        },
        {
          "questionText": "Com quantos meses andou?",
          "type": "Number"
        },
        {
          "questionText": "Já fala palavras?",
          "type": "YesNo"
        }
      ]
    },
    {
      "sectionName": "Alimentação",
      "questions": [
        {
          "questionText": "Tipo de alimentação",
          "type": "SingleChoice",
          "options": ["Aleitamento materno exclusivo", "Fórmula", "Misto", "Alimentação complementar"]
        },
        {
          "questionText": "Idade de introdução alimentar",
          "type": "Number",
          "unit": "meses"
        },
        {
          "questionText": "Alergias alimentares conhecidas?",
          "type": "MultipleChoice",
          "options": ["Nenhuma", "Leite", "Ovo", "Glúten", "Frutos do mar", "Outras"]
        }
      ]
    }
  ]
}

// Dermatologia
{
  "name": "Anamnese Dermatológica",
  "specialty": "Dermatology",
  "sections": [
    {
      "sectionName": "Lesão Cutânea",
      "questions": [
        {
          "questionText": "Tipo de lesão",
          "type": "MultipleChoice",
          "options": ["Mancha", "Pápula", "Vesícula", "Pústula", "Nódulo", "Úlcera"],
          "isRequired": true
        },
        {
          "questionText": "Localização",
          "type": "Text",
          "isRequired": true
        },
        {
          "questionText": "Tempo de evolução",
          "type": "Text",
          "helpText": "Ex: 2 semanas, 1 mês"
        },
        {
          "questionText": "Prurido (coceira)?",
          "type": "Scale",
          "helpText": "0 = Nenhum, 10 = Intenso"
        },
        {
          "questionText": "Dor?",
          "type": "YesNo"
        },
        {
          "questionText": "Alteração de cor?",
          "type": "YesNo"
        },
        {
          "questionText": "Crescimento progressivo?",
          "type": "YesNo"
        }
      ]
    },
    {
      "sectionName": "Histórico Dermatológico",
      "questions": [
        {
          "questionText": "História familiar de câncer de pele?",
          "type": "YesNo"
        },
        {
          "questionText": "Exposição solar",
          "type": "SingleChoice",
          "options": ["Mínima", "Moderada", "Intensa"]
        },
        {
          "questionText": "Uso de protetor solar?",
          "type": "SingleChoice",
          "options": ["Nunca", "Às vezes", "Diariamente"]
        },
        {
          "questionText": "Alergias conhecidas?",
          "type": "MultipleChoice",
          "options": ["Nenhuma", "Cosméticos", "Medicamentos", "Metais", "Outras"]
        }
      ]
    }
  ]
}
```

## 🎨 Frontend (Angular)

### Componentes

```typescript
// src/app/features/anamnesis/template-selector/template-selector.component.ts
@Component({
  selector: 'app-anamnesis-template-selector',
  template: `
    <mat-card>
      <mat-card-header>
        <mat-card-title>Selecionar Template de Anamnese</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <mat-form-field appearance="outline" class="w-full">
          <mat-label>Especialidade</mat-label>
          <mat-select [(ngModel)]="selectedSpecialty" (selectionChange)="onSpecialtyChange()">
            <mat-option *ngFor="let specialty of specialties" [value]="specialty">
              {{ specialtyNames[specialty] }}
            </mat-option>
          </mat-select>
        </mat-form-field>
        
        <mat-list>
          <mat-list-item *ngFor="let template of templates" (click)="selectTemplate(template)">
            <mat-icon mat-list-icon>assignment</mat-icon>
            <div mat-line>{{ template.name }}</div>
            <div mat-line class="mat-caption">{{ template.description }}</div>
            <button mat-icon-button>
              <mat-icon>chevron_right</mat-icon>
            </button>
          </mat-list-item>
        </mat-list>
      </mat-card-content>
    </mat-card>
  `
})
export class AnamnesisTemplateSelectorComponent implements OnInit {
  @Output() templateSelected = new EventEmitter<AnamnesisTemplate>();
  
  specialties = Object.keys(MedicalSpecialty);
  selectedSpecialty: MedicalSpecialty;
  templates: AnamnesisTemplate[] = [];
  
  specialtyNames = {
    'Cardiology': 'Cardiologia',
    'Pediatrics': 'Pediatria',
    'Dermatology': 'Dermatologia',
    // ... outros
  };
  
  constructor(private anamnesisService: AnamnesisService) {}
  
  async ngOnInit() {
    // Carregar templates da especialidade padrão
    if (this.selectedSpecialty) {
      await this.onSpecialtyChange();
    }
  }
  
  async onSpecialtyChange() {
    this.templates = await this.anamnesisService.getTemplatesBySpecialty(this.selectedSpecialty);
  }
  
  selectTemplate(template: AnamnesisTemplate) {
    this.templateSelected.emit(template);
  }
}

// src/app/features/anamnesis/questionnaire/questionnaire.component.ts
@Component({
  selector: 'app-anamnesis-questionnaire',
  template: `
    <div class="anamnesis-questionnaire">
      <h2>{{ template.name }}</h2>
      
      <form [formGroup]="questionnaireForm">
        <div *ngFor="let section of template.sections" class="section">
          <h3>{{ section.sectionName }}</h3>
          
          <div *ngFor="let question of section.questions" class="question">
            <!-- Texto -->
            <mat-form-field *ngIf="question.type === 'Text'" appearance="outline" class="w-full">
              <mat-label>{{ question.questionText }}</mat-label>
              <textarea matInput [formControlName]="getQuestionKey(question)" rows="3"></textarea>
              <mat-hint *ngIf="question.helpText">{{ question.helpText }}</mat-hint>
            </mat-form-field>
            
            <!-- Número -->
            <mat-form-field *ngIf="question.type === 'Number'" appearance="outline">
              <mat-label>{{ question.questionText }}</mat-label>
              <input matInput type="number" [formControlName]="getQuestionKey(question)">
              <span matSuffix *ngIf="question.unit">{{ question.unit }}</span>
            </mat-form-field>
            
            <!-- Sim/Não -->
            <div *ngIf="question.type === 'YesNo'" class="yes-no-question">
              <label>{{ question.questionText }}</label>
              <mat-radio-group [formControlName]="getQuestionKey(question)">
                <mat-radio-button value="yes">Sim</mat-radio-button>
                <mat-radio-button value="no">Não</mat-radio-button>
              </mat-radio-group>
            </div>
            
            <!-- Escolha Única -->
            <mat-form-field *ngIf="question.type === 'SingleChoice'" appearance="outline" class="w-full">
              <mat-label>{{ question.questionText }}</mat-label>
              <mat-select [formControlName]="getQuestionKey(question)">
                <mat-option *ngFor="let option of question.options" [value]="option">
                  {{ option }}
                </mat-option>
              </mat-select>
            </mat-form-field>
            
            <!-- Múltipla Escolha -->
            <div *ngIf="question.type === 'MultipleChoice'" class="multiple-choice">
              <label>{{ question.questionText }}</label>
              <mat-selection-list [formControlName]="getQuestionKey(question)">
                <mat-list-option *ngFor="let option of question.options" [value]="option">
                  {{ option }}
                </mat-list-option>
              </mat-selection-list>
            </div>
            
            <!-- Escala -->
            <div *ngIf="question.type === 'Scale'" class="scale-question">
              <label>{{ question.questionText }}</label>
              <mat-slider min="0" max="10" step="1" [formControlName]="getQuestionKey(question)">
                <input matSliderThumb>
              </mat-slider>
              <div class="scale-labels">
                <span>0</span>
                <span>{{ questionnaireForm.get(getQuestionKey(question))?.value || 0 }}</span>
                <span>10</span>
              </div>
              <mat-hint *ngIf="question.helpText">{{ question.helpText }}</mat-hint>
            </div>
            
            <!-- Data -->
            <mat-form-field *ngIf="question.type === 'Date'" appearance="outline">
              <mat-label>{{ question.questionText }}</mat-label>
              <input matInput [matDatepicker]="picker" [formControlName]="getQuestionKey(question)">
              <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
              <mat-datepicker #picker></mat-datepicker>
            </mat-form-field>
          </div>
        </div>
      </form>
      
      <div class="actions">
        <button mat-button (click)="onCancel()">Cancelar</button>
        <button mat-raised-button color="accent" (click)="onSave(false)">
          Salvar Rascunho
        </button>
        <button mat-raised-button color="primary" (click)="onSave(true)">
          Finalizar
        </button>
      </div>
      
      <!-- Progresso -->
      <mat-progress-bar mode="determinate" [value]="getCompletionPercentage()"></mat-progress-bar>
      <p class="progress-text">{{ getCompletionPercentage() }}% completo</p>
    </div>
  `,
  styles: [`
    .section {
      margin: 2rem 0;
      padding: 1rem;
      border-left: 4px solid #3f51b5;
      background: #f5f5f5;
    }
    
    .question {
      margin: 1.5rem 0;
    }
    
    .scale-labels {
      display: flex;
      justify-content: space-between;
      margin-top: 0.5rem;
      font-size: 0.9rem;
      color: #666;
    }
    
    .progress-text {
      text-align: center;
      margin-top: 0.5rem;
      color: #666;
    }
  `]
})
export class AnamnesisQuestionnaireComponent implements OnInit {
  @Input() template: AnamnesisTemplate;
  @Input() attendanceId: string;
  @Output() completed = new EventEmitter<AnamnesisResponse>();
  
  questionnaireForm: FormGroup;
  response: AnamnesisResponse;
  
  constructor(
    private fb: FormBuilder,
    private anamnesisService: AnamnesisService,
    private snackBar: MatSnackBar
  ) {}
  
  ngOnInit() {
    this.buildForm();
    this.loadSavedResponse();
  }
  
  buildForm() {
    const group: any = {};
    
    this.template.sections.forEach(section => {
      section.questions.forEach(question => {
        const key = this.getQuestionKey(question);
        const validators = question.isRequired ? [Validators.required] : [];
        group[key] = [null, validators];
      });
    });
    
    this.questionnaireForm = this.fb.group(group);
  }
  
  getQuestionKey(question: Question): string {
    return question.questionText.replace(/[^a-zA-Z0-9]/g, '_').toLowerCase();
  }
  
  async loadSavedResponse() {
    // Carregar resposta salva anteriormente (rascunho)
    try {
      this.response = await this.anamnesisService.getResponseByAttendance(this.attendanceId);
      if (this.response) {
        this.populateForm(this.response.answers);
      }
    } catch (error) {
      // Sem resposta salva, continuar
    }
  }
  
  populateForm(answers: QuestionAnswer[]) {
    answers.forEach(answer => {
      const key = this.getQuestionKey({ questionText: answer.questionText } as Question);
      const control = this.questionnaireForm.get(key);
      if (control) {
        control.setValue(answer.answer);
      }
    });
  }
  
  async onSave(isComplete: boolean) {
    const answers = this.buildAnswers();
    
    try {
      if (!this.response) {
        this.response = await this.anamnesisService.createResponse(
          this.attendanceId,
          this.template.id
        );
      }
      
      this.response = await this.anamnesisService.saveAnswers(
        this.response.id,
        answers,
        isComplete
      );
      
      this.snackBar.open(
        isComplete ? 'Anamnese finalizada!' : 'Rascunho salvo!',
        'OK',
        { duration: 3000 }
      );
      
      if (isComplete) {
        this.completed.emit(this.response);
      }
    } catch (error) {
      this.snackBar.open('Erro ao salvar: ' + error.message, 'OK');
    }
  }
  
  buildAnswers(): QuestionAnswer[] {
    const answers: QuestionAnswer[] = [];
    const formValue = this.questionnaireForm.value;
    
    this.template.sections.forEach(section => {
      section.questions.forEach(question => {
        const key = this.getQuestionKey(question);
        const value = formValue[key];
        
        if (value !== null && value !== undefined) {
          answers.push({
            questionText: question.questionText,
            type: question.type,
            answer: value.toString(),
            selectedOptions: Array.isArray(value) ? value : undefined,
            numericValue: typeof value === 'number' ? value : undefined,
            booleanValue: typeof value === 'boolean' ? value : undefined,
            dateValue: value instanceof Date ? value : undefined
          });
        }
      });
    });
    
    return answers;
  }
  
  getCompletionPercentage(): number {
    const totalQuestions = this.getTotalQuestions();
    const answeredQuestions = this.getAnsweredQuestions();
    return Math.round((answeredQuestions / totalQuestions) * 100);
  }
  
  getTotalQuestions(): number {
    return this.template.sections.reduce(
      (sum, section) => sum + section.questions.length,
      0
    );
  }
  
  getAnsweredQuestions(): number {
    const formValue = this.questionnaireForm.value;
    return Object.values(formValue).filter(v => v !== null && v !== undefined && v !== '').length;
  }
  
  onCancel() {
    // Navegar de volta
  }
}
```

### Service

```typescript
// src/app/core/services/anamnesis.service.ts
@Injectable({ providedIn: 'root' })
export class AnamnesisService {
  private apiUrl = '/api/anamnesis';
  
  constructor(private http: HttpClient) {}
  
  getTemplatesBySpecialty(specialty: MedicalSpecialty): Promise<AnamnesisTemplate[]> {
    return firstValueFrom(
      this.http.get<AnamnesisTemplate[]>(`${this.apiUrl}/templates`, {
        params: { specialty }
      })
    );
  }
  
  getTemplateById(templateId: string): Promise<AnamnesisTemplate> {
    return firstValueFrom(
      this.http.get<AnamnesisTemplate>(`${this.apiUrl}/templates/${templateId}`)
    );
  }
  
  createResponse(attendanceId: string, templateId: string): Promise<AnamnesisResponse> {
    return firstValueFrom(
      this.http.post<AnamnesisResponse>(`${this.apiUrl}/responses`, {
        attendanceId,
        templateId
      })
    );
  }
  
  saveAnswers(responseId: string, answers: QuestionAnswer[], isComplete: boolean): Promise<AnamnesisResponse> {
    return firstValueFrom(
      this.http.put<AnamnesisResponse>(`${this.apiUrl}/responses/${responseId}`, {
        answers,
        isComplete
      })
    );
  }
  
  getPatientHistory(patientId: string): Promise<AnamnesisResponse[]> {
    return firstValueFrom(
      this.http.get<AnamnesisResponse[]>(`${this.apiUrl}/responses/patient/${patientId}`)
    );
  }
}
```

## ✅ Checklist de Implementação

### Backend
- [ ] Criar entidades (AnamnesisTemplate, AnamnesisResponse, Question)
- [ ] Implementar repositórios
- [ ] Criar AnamnesisService
- [ ] Criar controllers REST
- [ ] Migrations
- [ ] Implementar 10+ templates pré-definidos
- [ ] Sistema de versionamento de templates
- [ ] Validações de respostas

### Frontend
- [ ] TemplateSelectorComponent
- [ ] QuestionnaireComponent
- [ ] TemplateEditorComponent (admin)
- [ ] AnamnesisHistoryComponent
- [ ] AnamnesisService (Angular)
- [ ] Integração no fluxo de atendimento
- [ ] Auto-save de rascunhos

### Integrações
- [ ] Integração com prontuário SOAP
- [ ] Exportação para PDF
- [ ] SNOMED CT (opcional)
- [ ] IA para sugestões diagnósticas (futuro)

### Testes
- [ ] Testes unitários
- [ ] Testes de serviços
- [ ] Testes de validação
- [ ] Testes de integração

## 💰 Investimento

- **Esforço**: 1 mês | 1 dev
- **Custo**: R$ 45k

### ROI Esperado
- Tempo de consulta: -20-30%
- Qualidade de registros: +40-50%
- Compliance com protocolos: 100%
- Base para IA diagnóstica

## 🎯 Critérios de Aceitação

- [ ] 10+ templates por especialidade implementados
- [ ] Formulários dinâmicos funcionam
- [ ] Auto-save de rascunhos
- [ ] Validação de campos obrigatórios
- [ ] Histórico de anamneses do paciente
- [ ] Integração com prontuário
- [ ] Interface intuitiva para médicos
- [ ] Progresso visual de preenchimento
- [ ] Editor de templates para admins
- [ ] Exportação PDF
