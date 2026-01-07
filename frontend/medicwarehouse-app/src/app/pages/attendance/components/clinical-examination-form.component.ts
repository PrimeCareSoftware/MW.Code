import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ClinicalExaminationService } from '../../../services/clinical-examination.service';
import { CreateClinicalExamination, UpdateClinicalExamination, ClinicalExamination } from '../../../models/medical-record.model';

@Component({
  selector: 'app-clinical-examination-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="clinical-examination-form">
      <h3>Exame Clínico (CFM 1.821/2007)</h3>
      <p class="info-text">
        Registro obrigatório de sinais vitais e exame físico sistemático conforme Resolução CFM 1.821/2007.
      </p>

      @if (errorMessage()) {
        <div class="alert alert-error">{{ errorMessage() }}</div>
      }
      @if (successMessage()) {
        <div class="alert alert-success">{{ successMessage() }}</div>
      }

      <form [formGroup]="examinationForm" (ngSubmit)="onSubmit()">
        <!-- Vital Signs Section -->
        <div class="section">
          <h4>Sinais Vitais *</h4>
          
          <div class="form-row">
            <div class="form-group col-md-6">
              <label for="systolicBloodPressure">Pressão Arterial Sistólica (mmHg) *</label>
              <input
                type="number"
                id="systolicBloodPressure"
                formControlName="systolicBloodPressure"
                placeholder="Ex: 120"
                min="50"
                max="300"
                [class.invalid]="isFieldInvalid('systolicBloodPressure')"
                [class.warning]="isVitalSignAbnormal('systolicBloodPressure')"
              />
              @if (isFieldInvalid('systolicBloodPressure')) {
                <small class="error-text">Valor obrigatório (50-300 mmHg)</small>
              }
              @if (isVitalSignAbnormal('systolicBloodPressure')) {
                <small class="warning-text">⚠️ Valor fora da faixa normal (90-140 mmHg)</small>
              }
            </div>

            <div class="form-group col-md-6">
              <label for="diastolicBloodPressure">Pressão Arterial Diastólica (mmHg) *</label>
              <input
                type="number"
                id="diastolicBloodPressure"
                formControlName="diastolicBloodPressure"
                placeholder="Ex: 80"
                min="30"
                max="200"
                [class.invalid]="isFieldInvalid('diastolicBloodPressure')"
                [class.warning]="isVitalSignAbnormal('diastolicBloodPressure')"
              />
              @if (isFieldInvalid('diastolicBloodPressure')) {
                <small class="error-text">Valor obrigatório (30-200 mmHg)</small>
              }
              @if (isVitalSignAbnormal('diastolicBloodPressure')) {
                <small class="warning-text">⚠️ Valor fora da faixa normal (60-90 mmHg)</small>
              }
            </div>
          </div>

          <div class="form-row">
            <div class="form-group col-md-4">
              <label for="heartRate">Frequência Cardíaca (bpm) *</label>
              <input
                type="number"
                id="heartRate"
                formControlName="heartRate"
                placeholder="Ex: 75"
                min="30"
                max="220"
                [class.invalid]="isFieldInvalid('heartRate')"
                [class.warning]="isVitalSignAbnormal('heartRate')"
              />
              @if (isFieldInvalid('heartRate')) {
                <small class="error-text">Valor obrigatório (30-220 bpm)</small>
              }
              @if (isVitalSignAbnormal('heartRate')) {
                <small class="warning-text">⚠️ Valor fora da faixa normal (60-100 bpm)</small>
              }
            </div>

            <div class="form-group col-md-4">
              <label for="respiratoryRate">Freq. Respiratória (irpm) *</label>
              <input
                type="number"
                id="respiratoryRate"
                formControlName="respiratoryRate"
                placeholder="Ex: 16"
                min="8"
                max="60"
                [class.invalid]="isFieldInvalid('respiratoryRate')"
                [class.warning]="isVitalSignAbnormal('respiratoryRate')"
              />
              @if (isFieldInvalid('respiratoryRate')) {
                <small class="error-text">Valor obrigatório (8-60 irpm)</small>
              }
              @if (isVitalSignAbnormal('respiratoryRate')) {
                <small class="warning-text">⚠️ Valor fora da faixa normal (12-20 irpm)</small>
              }
            </div>

            <div class="form-group col-md-4">
              <label for="temperature">Temperatura (°C) *</label>
              <input
                type="number"
                id="temperature"
                formControlName="temperature"
                step="0.1"
                placeholder="Ex: 36.5"
                min="32"
                max="45"
                [class.invalid]="isFieldInvalid('temperature')"
                [class.warning]="isVitalSignAbnormal('temperature')"
              />
              @if (isFieldInvalid('temperature')) {
                <small class="error-text">Valor obrigatório (32-45 °C)</small>
              }
              @if (isVitalSignAbnormal('temperature')) {
                <small class="warning-text">⚠️ Valor fora da faixa normal (36-37.5 °C)</small>
              }
            </div>
          </div>

          <div class="form-row">
            <div class="form-group col-md-6">
              <label for="oxygenSaturation">Saturação de O₂ (%) *</label>
              <input
                type="number"
                id="oxygenSaturation"
                formControlName="oxygenSaturation"
                placeholder="Ex: 98"
                min="0"
                max="100"
                [class.invalid]="isFieldInvalid('oxygenSaturation')"
                [class.warning]="isVitalSignAbnormal('oxygenSaturation')"
              />
              @if (isFieldInvalid('oxygenSaturation')) {
                <small class="error-text">Valor obrigatório (0-100 %)</small>
              }
              @if (isVitalSignAbnormal('oxygenSaturation')) {
                <small class="warning-text">⚠️ Valor baixo (normal: >95%)</small>
              }
            </div>
          </div>
        </div>

        <!-- Physical Examination Section -->
        <div class="section">
          <h4>Exame Físico Sistemático *</h4>
          <div class="form-group">
            <textarea
              id="systematicExamination"
              formControlName="systematicExamination"
              rows="6"
              placeholder="Descreva o exame físico por sistemas (mínimo 20 caracteres)...&#10;&#10;Exemplos:&#10;- Cabeça e pescoço: ____&#10;- Aparelho respiratório: ____&#10;- Aparelho cardiovascular: ____&#10;- Abdome: ____&#10;- Extremidades: ____"
              [class.invalid]="isFieldInvalid('systematicExamination')"
            ></textarea>
            @if (isFieldInvalid('systematicExamination')) {
              <small class="error-text">
                Exame físico sistemático é obrigatório (mínimo 20 caracteres)
              </small>
            }
            <small class="char-count">
              {{ examinationForm.get('systematicExamination')?.value?.length || 0 }} caracteres
            </small>
          </div>
        </div>

        <!-- General State Section -->
        <div class="section">
          <h4>Estado Geral</h4>
          <div class="form-group">
            <textarea
              id="generalState"
              formControlName="generalState"
              rows="3"
              placeholder="Descreva o estado geral do paciente...&#10;Ex: Paciente consciente, orientado, corado, hidratado, acianótico, anictérico"
            ></textarea>
            <small class="help-text">
              Opcional: Descrição do estado geral e aspecto do paciente
            </small>
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
            [disabled]="examinationForm.invalid || isSaving()"
          >
            {{ isSaving() ? 'Salvando...' : (existingExamination ? 'Atualizar' : 'Registrar') }} Exame
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .clinical-examination-form {
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
      border-left: 3px solid #17a2b8;
      border-radius: 4px;
    }

    .section {
      margin-bottom: 2rem;
      padding: 1.5rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .section h4 {
      margin-bottom: 1.5rem;
      color: #333;
      font-size: 1.1rem;
    }

    .form-row {
      display: flex;
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .form-row:last-child {
      margin-bottom: 0;
    }

    .form-group {
      flex: 1;
      margin-bottom: 0;
    }

    .form-group.col-md-4 {
      flex: 0 0 calc(33.333% - 0.667rem);
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

    .form-group input[type="number"],
    .form-group textarea {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-family: inherit;
      font-size: 0.95rem;
    }

    .form-group input.invalid,
    .form-group textarea.invalid {
      border-color: #dc3545;
      background: #fff5f5;
    }

    .form-group input.warning {
      border-color: #ffc107;
      background: #fffbf0;
    }

    .help-text,
    .char-count {
      display: block;
      margin-top: 0.25rem;
      color: #666;
      font-size: 0.85rem;
    }

    .error-text {
      display: block;
      margin-top: 0.25rem;
      color: #dc3545;
      font-size: 0.85rem;
    }

    .warning-text {
      display: block;
      margin-top: 0.25rem;
      color: #856404;
      font-size: 0.85rem;
      font-weight: 500;
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
      background: #17a2b8;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #138496;
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

      .form-group.col-md-4,
      .form-group.col-md-6 {
        flex: 1;
      }
    }
  `]
})
export class ClinicalExaminationFormComponent implements OnInit {
  @Input() medicalRecordId!: string;
  @Input() existingExamination?: ClinicalExamination;
  @Output() examinationSaved = new EventEmitter<ClinicalExamination>();
  @Output() cancelled = new EventEmitter<void>();

  examinationForm: FormGroup;
  isSaving = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  // Normal ranges for vital signs
  private readonly normalRanges = {
    systolicBloodPressure: { min: 90, max: 140 },
    diastolicBloodPressure: { min: 60, max: 90 },
    heartRate: { min: 60, max: 100 },
    respiratoryRate: { min: 12, max: 20 },
    temperature: { min: 36.0, max: 37.5 },
    oxygenSaturation: { min: 95, max: 100 }
  };

  constructor(
    private fb: FormBuilder,
    private clinicalExaminationService: ClinicalExaminationService
  ) {
    this.examinationForm = this.fb.group({
      systolicBloodPressure: [null, [Validators.required, Validators.min(50), Validators.max(300)]],
      diastolicBloodPressure: [null, [Validators.required, Validators.min(30), Validators.max(200)]],
      heartRate: [null, [Validators.required, Validators.min(30), Validators.max(220)]],
      respiratoryRate: [null, [Validators.required, Validators.min(8), Validators.max(60)]],
      temperature: [null, [Validators.required, Validators.min(32), Validators.max(45)]],
      oxygenSaturation: [null, [Validators.required, Validators.min(0), Validators.max(100)]],
      systematicExamination: ['', [Validators.required, Validators.minLength(20)]],
      generalState: ['']
    });
  }

  ngOnInit() {
    if (this.existingExamination) {
      this.loadExistingData();
    }
  }

  loadExistingData() {
    if (!this.existingExamination) return;

    this.examinationForm.patchValue({
      systolicBloodPressure: this.existingExamination.systolicBloodPressure,
      diastolicBloodPressure: this.existingExamination.diastolicBloodPressure,
      heartRate: this.existingExamination.heartRate,
      respiratoryRate: this.existingExamination.respiratoryRate,
      temperature: this.existingExamination.temperature,
      oxygenSaturation: this.existingExamination.oxygenSaturation,
      systematicExamination: this.existingExamination.systematicExamination,
      generalState: this.existingExamination.generalState || ''
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.examinationForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  isVitalSignAbnormal(fieldName: string): boolean {
    const field = this.examinationForm.get(fieldName);
    if (!field || !field.value || field.invalid) return false;

    const value = parseFloat(field.value);
    const range = this.normalRanges[fieldName as keyof typeof this.normalRanges];
    
    if (!range) return false;
    return value < range.min || value > range.max;
  }

  onSubmit() {
    if (this.examinationForm.invalid) {
      Object.keys(this.examinationForm.controls).forEach(key => {
        this.examinationForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.examinationForm.value;

    if (this.existingExamination) {
      // Update existing
      const updateData: UpdateClinicalExamination = {
        ...formValue,
        generalState: formValue.generalState || null
      };

      this.clinicalExaminationService.update(this.existingExamination.id, updateData).subscribe({
        next: (examination) => {
          this.successMessage.set('Exame clínico atualizado com sucesso!');
          this.examinationSaved.emit(examination);
          this.isSaving.set(false);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao atualizar exame clínico: ' + (error.error?.message || 'Erro desconhecido'));
          this.isSaving.set(false);
        }
      });
    } else {
      // Create new
      const createData: CreateClinicalExamination = {
        medicalRecordId: this.medicalRecordId,
        ...formValue,
        generalState: formValue.generalState || null
      };

      this.clinicalExaminationService.create(createData).subscribe({
        next: (examination) => {
          this.successMessage.set('Exame clínico registrado com sucesso!');
          this.examinationSaved.emit(examination);
          this.isSaving.set(false);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao registrar exame clínico: ' + (error.error?.message || 'Erro desconhecido'));
          this.isSaving.set(false);
        }
      });
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}
