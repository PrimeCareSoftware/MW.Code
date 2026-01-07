import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TherapeuticPlanService } from '../../../services/therapeutic-plan.service';
import { CreateTherapeuticPlan, UpdateTherapeuticPlan, TherapeuticPlan } from '../../../models/medical-record.model';

@Component({
  selector: 'app-therapeutic-plan-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="therapeutic-plan-form">
      <h3>Plano Terapêutico (CFM 1.821/2007)</h3>
      <p class="info-text">
        Registro obrigatório do plano terapêutico detalhado conforme CFM 1.821/2007, incluindo
        tratamento, prescrições, exames solicitados e orientações ao paciente.
      </p>

      @if (errorMessage()) {
        <div class="alert alert-error">{{ errorMessage() }}</div>
      }
      @if (successMessage()) {
        <div class="alert alert-success">{{ successMessage() }}</div>
      }

      @if (existingPlan()) {
        <div class="existing-plan-notice">
          <p>✓ Um plano terapêutico já foi registrado. Você pode atualizá-lo abaixo.</p>
          <small>Última atualização: {{ existingPlan()!.updatedAt | date: 'dd/MM/yyyy HH:mm' }}</small>
        </div>
      }

      <form [formGroup]="planForm" (ngSubmit)="onSubmit()">
        <!-- Treatment/Conduct Section -->
        <div class="section">
          <h4>Tratamento / Conduta *</h4>
          <div class="form-group">
            <textarea
              id="treatment"
              formControlName="treatment"
              rows="5"
              placeholder="Descreva o tratamento e conduta recomendados (mínimo 20 caracteres)...&#10;&#10;Exemplo:&#10;- Repouso relativo por 3 dias&#10;- Hidratação adequada (2L água/dia)&#10;- Retorno em 7 dias ou se piora dos sintomas"
              [class.invalid]="isFieldInvalid('treatment')"
            ></textarea>
            @if (isFieldInvalid('treatment')) {
              <small class="error-text">
                Tratamento/Conduta é obrigatório (mínimo 20 caracteres)
              </small>
            }
            <small class="char-count">
              {{ planForm.get('treatment')?.value?.length || 0 }} caracteres
            </small>
          </div>
        </div>

        <!-- Medication Prescription Section -->
        <div class="section">
          <h4>Prescrição Medicamentosa</h4>
          <div class="form-group">
            <textarea
              id="medicationPrescription"
              formControlName="medicationPrescription"
              rows="6"
              placeholder="Liste os medicamentos prescritos com posologia completa...&#10;&#10;Exemplo:&#10;1. Paracetamol 750mg - 1 cp VO 8/8h por 3 dias (se dor ou febre)&#10;2. Dipirona 500mg - 1 cp VO 6/6h por 5 dias&#10;3. Omeprazol 20mg - 1 cp VO em jejum por 14 dias"
            ></textarea>
            <small class="help-text">
              💡 Dica: Use o formato padrão: Nome + Dosagem + Via + Frequência + Duração
            </small>
            @if (planForm.get('medicationPrescription')?.value) {
              <small class="char-count">
                {{ planForm.get('medicationPrescription')?.value.length }} caracteres
              </small>
            }
          </div>
        </div>

        <!-- Requested Exams Section -->
        <div class="section">
          <h4>Exames Solicitados</h4>
          <div class="form-group">
            <textarea
              id="requestedExams"
              formControlName="requestedExams"
              rows="5"
              placeholder="Liste os exames complementares solicitados...&#10;&#10;Exemplo:&#10;- Hemograma completo&#10;- Glicemia de jejum&#10;- TSH e T4 livre&#10;- ECG de repouso"
            ></textarea>
            <small class="help-text">
              Opcional: Exames laboratoriais, de imagem ou outros exames complementares
            </small>
            @if (planForm.get('requestedExams')?.value) {
              <small class="char-count">
                {{ planForm.get('requestedExams')?.value.length }} caracteres
              </small>
            }
          </div>
        </div>

        <!-- Referrals Section -->
        <div class="section">
          <h4>Encaminhamentos</h4>
          <div class="form-group">
            <textarea
              id="referrals"
              formControlName="referrals"
              rows="4"
              placeholder="Encaminhamentos para outros especialistas ou serviços...&#10;&#10;Exemplo:&#10;- Encaminhamento para Cardiologista para avaliação de sopro cardíaco&#10;- Fisioterapia respiratória - 10 sessões"
            ></textarea>
            <small class="help-text">
              Opcional: Referências para outros médicos, fisioterapia, nutrição, etc.
            </small>
            @if (planForm.get('referrals')?.value) {
              <small class="char-count">
                {{ planForm.get('referrals')?.value.length }} caracteres
              </small>
            }
          </div>
        </div>

        <!-- Patient Guidance Section -->
        <div class="section">
          <h4>Orientações ao Paciente</h4>
          <div class="form-group">
            <textarea
              id="patientGuidance"
              formControlName="patientGuidance"
              rows="6"
              placeholder="Orientações e instruções para o paciente...&#10;&#10;Exemplo:&#10;- Evitar esforços físicos intensos por 7 dias&#10;- Manter dieta leve e fracionada&#10;- Observar sinais de piora: febre persistente, dor intensa, falta de ar&#10;- Procurar PS se apresentar os sintomas acima&#10;- Trazer resultados dos exames no retorno"
            ></textarea>
            <small class="help-text">
              Opcional: Instruções sobre cuidados, sinais de alerta, quando procurar atendimento, etc.
            </small>
            @if (planForm.get('patientGuidance')?.value) {
              <small class="char-count">
                {{ planForm.get('patientGuidance')?.value.length }} caracteres
              </small>
            }
          </div>
        </div>

        <!-- Return Date Section -->
        <div class="section">
          <h4>Data de Retorno</h4>
          <div class="form-row">
            <div class="form-group col-md-6">
              <label for="returnDate">Agendar Retorno</label>
              <input
                type="date"
                id="returnDate"
                formControlName="returnDate"
                [min]="tomorrowDate"
              />
              <small class="help-text">
                Opcional: Data sugerida para retorno do paciente
              </small>
            </div>
          </div>
        </div>

        <div class="form-actions">
          <button
            type="button"
            class="btn btn-secondary"
            (click)="onCancel()"
            [disabled]="isSaving()"
          >
            Cancelar
          </button>
          <button
            type="submit"
            class="btn btn-primary"
            [disabled]="planForm.invalid || isSaving()"
          >
            {{ isSaving() ? 'Salvando...' : (existingPlan() ? 'Atualizar' : 'Registrar') }} Plano Terapêutico
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .therapeutic-plan-form {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }

    .info-text {
      color: #666;
      font-size: 0.9rem;
      margin-bottom: 1.5rem;
      padding: 0.75rem;
      background: #f8f9fa;
      border-left: 3px solid #6f42c1;
      border-radius: 4px;
    }

    .existing-plan-notice {
      padding: 1rem;
      margin-bottom: 1.5rem;
      background: #d4edda;
      border: 1px solid #c3e6cb;
      border-radius: 6px;
      color: #155724;
    }

    .existing-plan-notice p {
      margin: 0 0 0.5rem 0;
      font-weight: 500;
    }

    .existing-plan-notice small {
      color: #155724;
      font-size: 0.85rem;
    }

    .section {
      margin-bottom: 2rem;
      padding: 1.5rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .section h4 {
      margin-bottom: 1rem;
      color: #333;
      font-size: 1.05rem;
    }

    .form-row {
      display: flex;
      gap: 1rem;
      margin-bottom: 0;
    }

    .form-group {
      margin-bottom: 0;
    }

    .form-group.col-md-6 {
      flex: 0 0 calc(50% - 0.5rem);
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #333;
      font-size: 0.9rem;
    }

    .form-group textarea,
    .form-group input[type="date"] {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-family: inherit;
      font-size: 0.95rem;
      line-height: 1.5;
    }

    .form-group textarea {
      resize: vertical;
    }

    .form-group textarea.invalid {
      border-color: #dc3545;
      background: #fff5f5;
    }

    .help-text,
    .char-count {
      display: block;
      margin-top: 0.25rem;
      color: #666;
      font-size: 0.85rem;
    }

    .char-count {
      text-align: right;
      font-style: italic;
    }

    .error-text {
      display: block;
      margin-top: 0.25rem;
      color: #dc3545;
      font-size: 0.85rem;
    }

    .form-actions {
      margin-top: 2rem;
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
    }

    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      font-size: 1rem;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .btn-primary {
      background: #6f42c1;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #5a32a3;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #5a6268;
    }

    .btn:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .alert {
      padding: 0.75rem 1rem;
      margin-bottom: 1rem;
      border-radius: 4px;
    }

    .alert-error {
      background: #f8d7da;
      color: #721c24;
      border: 1px solid #f5c6cb;
    }

    .alert-success {
      background: #d4edda;
      color: #155724;
      border: 1px solid #c3e6cb;
    }

    @media (max-width: 768px) {
      .form-row {
        flex-direction: column;
      }

      .form-group.col-md-6 {
        flex: 1;
      }
    }
  `]
})
export class TherapeuticPlanFormComponent implements OnInit {
  @Input() medicalRecordId!: string;
  @Input() existingPlan?: TherapeuticPlan;
  @Output() planSaved = new EventEmitter<TherapeuticPlan>();
  @Output() cancelled = new EventEmitter<void>();

  planForm: FormGroup;
  isSaving = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  tomorrowDate: string;

  constructor(
    private fb: FormBuilder,
    private therapeuticPlanService: TherapeuticPlanService
  ) {
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.tomorrowDate = tomorrow.toISOString().split('T')[0];

    this.planForm = this.fb.group({
      treatment: ['', [Validators.required, Validators.minLength(20)]],
      medicationPrescription: [''],
      requestedExams: [''],
      referrals: [''],
      patientGuidance: [''],
      returnDate: ['']
    });
  }

  ngOnInit() {
    if (this.existingPlan) {
      this.loadExistingData();
    } else {
      this.loadExistingPlan();
    }
  }

  loadExistingPlan() {
    if (!this.medicalRecordId) return;

    this.therapeuticPlanService.getByMedicalRecord(this.medicalRecordId).subscribe({
      next: (plans) => {
        if (plans && plans.length > 0) {
          // Get the most recent plan
          const latestPlan = plans.sort((a, b) => 
            new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
          )[0];
          this.existingPlan = latestPlan;
          this.loadExistingData();
        }
      },
      error: (error) => {
        console.error('Error loading therapeutic plan:', error);
      }
    });
  }

  loadExistingData() {
    if (!this.existingPlan) return;

    this.planForm.patchValue({
      treatment: this.existingPlan.treatment,
      medicationPrescription: this.existingPlan.medicationPrescription || '',
      requestedExams: this.existingPlan.requestedExams || '',
      referrals: this.existingPlan.referrals || '',
      patientGuidance: this.existingPlan.patientGuidance || '',
      returnDate: this.existingPlan.returnDate ? 
        new Date(this.existingPlan.returnDate).toISOString().split('T')[0] : ''
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.planForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  onSubmit() {
    if (this.planForm.invalid) {
      Object.keys(this.planForm.controls).forEach(key => {
        this.planForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.planForm.value;

    const planData = {
      treatment: formValue.treatment.trim(),
      medicationPrescription: formValue.medicationPrescription?.trim() || null,
      requestedExams: formValue.requestedExams?.trim() || null,
      referrals: formValue.referrals?.trim() || null,
      patientGuidance: formValue.patientGuidance?.trim() || null,
      returnDate: formValue.returnDate || null
    };

    if (this.existingPlan) {
      // Update existing
      const updateData: UpdateTherapeuticPlan = planData;

      this.therapeuticPlanService.update(this.existingPlan.id, updateData).subscribe({
        next: (plan) => {
          this.successMessage.set('Plano terapêutico atualizado com sucesso!');
          this.planSaved.emit(plan);
          this.isSaving.set(false);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao atualizar plano terapêutico: ' + (error.error?.message || 'Erro desconhecido'));
          this.isSaving.set(false);
        }
      });
    } else {
      // Create new
      const createData: CreateTherapeuticPlan = {
        medicalRecordId: this.medicalRecordId,
        ...planData
      };

      this.therapeuticPlanService.create(createData).subscribe({
        next: (plan) => {
          this.successMessage.set('Plano terapêutico registrado com sucesso!');
          this.planSaved.emit(plan);
          this.isSaving.set(false);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao registrar plano terapêutico: ' + (error.error?.message || 'Erro desconhecido'));
          this.isSaving.set(false);
        }
      });
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}
