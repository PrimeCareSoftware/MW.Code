import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { DiagnosticHypothesisService } from '../../../services/diagnostic-hypothesis.service';
import { CreateDiagnosticHypothesis, UpdateDiagnosticHypothesis, DiagnosticHypothesis, DiagnosisType } from '../../../models/medical-record.model';

@Component({
  selector: 'app-diagnostic-hypothesis-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="diagnostic-hypothesis-form">
      <h3>Hipóteses Diagnósticas (CFM 1.821/2007)</h3>
      <p class="info-text">
        Registro obrigatório de hipóteses diagnósticas com código CID-10 conforme CFM 1.821/2007.
        É necessário ao menos uma hipótese diagnóstica principal.
      </p>

      @if (errorMessage()) {
        <div class="alert alert-error">{{ errorMessage() }}</div>
      }
      @if (successMessage()) {
        <div class="alert alert-success">{{ successMessage() }}</div>
      }

      <!-- List of Existing Hypotheses -->
      @if (existingHypotheses().length > 0) {
        <div class="existing-hypotheses">
          <h4>Diagnósticos Registrados</h4>
          @for (hypothesis of existingHypotheses(); track hypothesis.id) {
            <div class="hypothesis-item" [class.principal]="hypothesis.type === 1">
              <div class="hypothesis-header">
                <div class="hypothesis-info">
                  <span class="hypothesis-type" [class.principal]="hypothesis.type === 1">
                    {{ hypothesis.type === 1 ? '⭐ Principal' : 'Secundário' }}
                  </span>
                  <span class="hypothesis-code">CID-10: {{ hypothesis.icd10Code }}</span>
                </div>
                <button
                  type="button"
                  class="btn-delete"
                  (click)="deleteHypothesis(hypothesis.id)"
                  [disabled]="isDeletingId() === hypothesis.id"
                  title="Excluir diagnóstico"
                >
                  {{ isDeletingId() === hypothesis.id ? '...' : '🗑️' }}
                </button>
              </div>
              <div class="hypothesis-description">{{ hypothesis.description }}</div>
              <div class="hypothesis-date">
                <small>Data: {{ hypothesis.diagnosedAt | date: 'dd/MM/yyyy' }}</small>
              </div>
            </div>
          }
        </div>
      }

      <!-- Form to Add New Hypothesis -->
      <div class="add-hypothesis-section">
        <h4>{{ editingHypothesis() ? 'Editar' : 'Adicionar' }} Diagnóstico</h4>
        
        <form [formGroup]="hypothesisForm" (ngSubmit)="onSubmit()">
          <div class="form-row">
            <div class="form-group col-md-8">
              <label for="description">Descrição do Diagnóstico *</label>
              <textarea
                id="description"
                formControlName="description"
                rows="3"
                placeholder="Descreva a hipótese diagnóstica..."
                [class.invalid]="isFieldInvalid('description')"
              ></textarea>
              @if (isFieldInvalid('description')) {
                <small class="error-text">Descrição é obrigatória (mínimo 5 caracteres)</small>
              }
            </div>

            <div class="form-group col-md-4">
              <label for="type">Tipo *</label>
              <select
                id="type"
                formControlName="type"
                [class.invalid]="isFieldInvalid('type')"
              >
                <option value="">Selecione...</option>
                <option value="1">⭐ Principal</option>
                <option value="2">Secundário</option>
              </select>
              @if (isFieldInvalid('type')) {
                <small class="error-text">Tipo é obrigatório</small>
              }
              @if (!hasPrincipalDiagnosis() && hypothesisForm.get('type')?.value !== 'Principal') {
                <small class="warning-text">
                  ⚠️ É necessário ao menos um diagnóstico principal
                </small>
              }
            </div>
          </div>

          <div class="form-row">
            <div class="form-group col-md-6">
              <label for="icdCode">Código CID-10 *</label>
              <input
                type="text"
                id="icdCode"
                formControlName="icdCode"
                placeholder="Ex: A00, J20.9, Z99.01"
                (input)="onIcdCodeInput($event)"
                [class.invalid]="isFieldInvalid('icdCode')"
                [class.valid]="hypothesisForm.get('icdCode')?.valid && hypothesisForm.get('icdCode')?.value"
              />
              @if (isFieldInvalid('icdCode')) {
                <small class="error-text">
                  Código CID-10 inválido. Formato: letra(s) seguida de números (ex: A00, J20.9)
                </small>
              }
              @if (hypothesisForm.get('icdCode')?.valid && hypothesisForm.get('icdCode')?.value) {
                <small class="success-text">✓ Formato válido</small>
              }
              <small class="help-text">
                Formato: 1-3 letras + 2 dígitos + opcional (.1 dígito)
              </small>
            </div>
          </div>

          <!-- CID-10 Quick Search Helper -->
          <div class="cid-search-helper">
            <h5>🔍 Busca Rápida CID-10</h5>
            <div class="search-examples">
              <p><strong>Exemplos comuns:</strong></p>
              <div class="examples-grid">
                <button type="button" class="example-btn" (click)="fillCidExample('J06.9')">
                  J06.9 - Infecção respiratória aguda
                </button>
                <button type="button" class="example-btn" (click)="fillCidExample('E11')">
                  E11 - Diabetes mellitus tipo 2
                </button>
                <button type="button" class="example-btn" (click)="fillCidExample('I10')">
                  I10 - Hipertensão essencial
                </button>
                <button type="button" class="example-btn" (click)="fillCidExample('K29.7')">
                  K29.7 - Gastrite não especificada
                </button>
                <button type="button" class="example-btn" (click)="fillCidExample('M79.1')">
                  M79.1 - Mialgia
                </button>
                <button type="button" class="example-btn" (click)="fillCidExample('R51')">
                  R51 - Cefaleia
                </button>
              </div>
              <small class="help-text">
                💡 Dica: Em produção, integrar com base de dados CID-10 completa para busca avançada
              </small>
            </div>
          </div>

          <div class="form-actions">
            @if (editingHypothesis()) {
              <button
                type="button"
                class="btn btn-secondary"
                (click)="cancelEdit()"
                [disabled]="isSaving()"
              >
                Cancelar Edição
              </button>
            }
            <button
              type="submit"
              class="btn btn-primary"
              [disabled]="hypothesisForm.invalid || isSaving()"
            >
              {{ isSaving() ? 'Salvando...' : (editingHypothesis() ? 'Atualizar' : 'Adicionar') }} Diagnóstico
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [`
    .diagnostic-hypothesis-form {
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
      border-left: 3px solid #28a745;
      border-radius: 4px;
    }

    .existing-hypotheses {
      margin-bottom: 2rem;
      padding: 1rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .existing-hypotheses h4 {
      margin-bottom: 1rem;
      color: #333;
    }

    .hypothesis-item {
      background: white;
      padding: 1rem;
      margin-bottom: 0.75rem;
      border-radius: 6px;
      border: 1px solid #ddd;
    }

    .hypothesis-item.principal {
      border-color: #28a745;
      border-width: 2px;
      background: #f8fff9;
    }

    .hypothesis-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.5rem;
    }

    .hypothesis-info {
      display: flex;
      gap: 1rem;
      align-items: center;
    }

    .hypothesis-type {
      padding: 0.25rem 0.75rem;
      border-radius: 12px;
      font-size: 0.8rem;
      background: #6c757d;
      color: white;
      font-weight: 500;
    }

    .hypothesis-type.principal {
      background: #28a745;
    }

    .hypothesis-code {
      font-family: 'Courier New', monospace;
      font-weight: 600;
      color: #007bff;
      font-size: 0.9rem;
    }

    .hypothesis-description {
      color: #333;
      margin-bottom: 0.5rem;
      line-height: 1.5;
    }

    .hypothesis-date small {
      color: #666;
      font-size: 0.8rem;
    }

    .btn-delete {
      background: #dc3545;
      color: white;
      border: none;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.9rem;
      transition: background-color 0.2s;
    }

    .btn-delete:hover:not(:disabled) {
      background: #c82333;
    }

    .btn-delete:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .add-hypothesis-section {
      padding: 1.5rem;
      background: #f8f9fa;
      border-radius: 6px;
    }

    .add-hypothesis-section h4 {
      margin-bottom: 1.5rem;
      color: #333;
    }

    .form-row {
      display: flex;
      gap: 1rem;
      margin-bottom: 1rem;
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

    .form-group.col-md-8 {
      flex: 0 0 calc(66.666% - 0.5rem);
    }

    .form-group label {
      display: block;
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #333;
      font-size: 0.9rem;
    }

    .form-group input,
    .form-group select,
    .form-group textarea {
      width: 100%;
      padding: 0.75rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-family: inherit;
      font-size: 0.95rem;
    }

    .form-group input.invalid,
    .form-group select.invalid,
    .form-group textarea.invalid {
      border-color: #dc3545;
      background: #fff5f5;
    }

    .form-group input.valid {
      border-color: #28a745;
      background: #f8fff9;
    }

    .help-text {
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

    .success-text {
      display: block;
      margin-top: 0.25rem;
      color: #28a745;
      font-size: 0.85rem;
      font-weight: 500;
    }

    .cid-search-helper {
      margin: 1.5rem 0;
      padding: 1rem;
      background: white;
      border-radius: 6px;
      border: 1px solid #ddd;
    }

    .cid-search-helper h5 {
      margin-bottom: 0.75rem;
      color: #333;
      font-size: 1rem;
    }

    .search-examples p {
      margin-bottom: 0.75rem;
      color: #666;
      font-size: 0.9rem;
    }

    .examples-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 0.5rem;
      margin-bottom: 0.75rem;
    }

    .example-btn {
      padding: 0.5rem;
      background: #e9ecef;
      border: 1px solid #ced4da;
      border-radius: 4px;
      cursor: pointer;
      font-size: 0.85rem;
      text-align: left;
      transition: all 0.2s;
    }

    .example-btn:hover {
      background: #dee2e6;
      border-color: #007bff;
    }

    .form-actions {
      margin-top: 1.5rem;
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
      background: #28a745;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #218838;
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
      .form-group.col-md-6,
      .form-group.col-md-8 {
        flex: 1;
      }

      .examples-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class DiagnosticHypothesisFormComponent implements OnInit {
  @Input() medicalRecordId!: string;
  @Output() hypothesisSaved = new EventEmitter<DiagnosticHypothesis>();
  @Output() hypothesisDeleted = new EventEmitter<string>();

  hypothesisForm: FormGroup;
  existingHypotheses = signal<DiagnosticHypothesis[]>([]);
  editingHypothesis = signal<DiagnosticHypothesis | null>(null);
  isSaving = signal(false);
  isDeletingId = signal<string | null>(null);
  errorMessage = signal('');
  successMessage = signal('');

  // CID-10 format validator: 1-3 letters + 2 digits + optional (.1 digit)
  private readonly cidPattern = /^[A-Z]{1,3}\d{2}(\.\d{1,2})?$/;

  constructor(
    private fb: FormBuilder,
    private diagnosticHypothesisService: DiagnosticHypothesisService
  ) {
    this.hypothesisForm = this.fb.group({
      description: ['', [Validators.required, Validators.minLength(5)]],
      icdCode: ['', [Validators.required, Validators.pattern(this.cidPattern)]],
      type: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadExistingHypotheses();
  }

  loadExistingHypotheses() {
    if (!this.medicalRecordId) return;

    this.diagnosticHypothesisService.getByMedicalRecord(this.medicalRecordId).subscribe({
      next: (hypotheses) => {
        this.existingHypotheses.set(hypotheses);
      },
      error: (error) => {
        console.error('Error loading hypotheses:', error);
      }
    });
  }

  onIcdCodeInput(event: Event) {
    const input = event.target as HTMLInputElement;
    // Auto-uppercase
    input.value = input.value.toUpperCase();
    this.hypothesisForm.patchValue({ icdCode: input.value });
  }

  fillCidExample(code: string) {
    this.hypothesisForm.patchValue({ icdCode: code });
  }

  hasPrincipalDiagnosis(): boolean {
    return this.existingHypotheses().some(h => h.type === DiagnosisType.Principal) ||
           this.hypothesisForm.get('type')?.value == DiagnosisType.Principal;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.hypothesisForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }

  onSubmit() {
    if (this.hypothesisForm.invalid) {
      Object.keys(this.hypothesisForm.controls).forEach(key => {
        this.hypothesisForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.hypothesisForm.value;

    if (this.editingHypothesis()) {
      // Update existing
      const updateData: UpdateDiagnosticHypothesis = {
        description: formValue.description.trim(),
        icd10Code: formValue.icdCode.trim().toUpperCase(),
        type: parseInt(formValue.type) as DiagnosisType
      };

      this.diagnosticHypothesisService.update(this.editingHypothesis()!.id, updateData).subscribe({
        next: (hypothesis) => {
          this.successMessage.set('Diagnóstico atualizado com sucesso!');
          this.loadExistingHypotheses();
          this.hypothesisSaved.emit(hypothesis);
          this.cancelEdit();
          this.isSaving.set(false);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao atualizar diagnóstico: ' + (error.error?.message || 'Erro desconhecido'));
          this.isSaving.set(false);
        }
      });
    } else {
      // Create new
      const createData: CreateDiagnosticHypothesis = {
        medicalRecordId: this.medicalRecordId,
        description: formValue.description.trim(),
        icd10Code: formValue.icdCode.trim().toUpperCase(),
        type: parseInt(formValue.type) as DiagnosisType
      };

      this.diagnosticHypothesisService.create(createData).subscribe({
        next: (hypothesis) => {
          this.successMessage.set('Diagnóstico adicionado com sucesso!');
          this.loadExistingHypotheses();
          this.hypothesisSaved.emit(hypothesis);
          this.resetForm();
          this.isSaving.set(false);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao adicionar diagnóstico: ' + (error.error?.message || 'Erro desconhecido'));
          this.isSaving.set(false);
        }
      });
    }
  }

  deleteHypothesis(id: string) {
    if (!confirm('Tem certeza que deseja excluir este diagnóstico?')) return;

    this.isDeletingId.set(id);
    this.errorMessage.set('');

    this.diagnosticHypothesisService.delete(id).subscribe({
      next: () => {
        this.successMessage.set('Diagnóstico excluído com sucesso!');
        this.loadExistingHypotheses();
        this.hypothesisDeleted.emit(id);
        this.isDeletingId.set(null);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao excluir diagnóstico: ' + (error.error?.message || 'Erro desconhecido'));
        this.isDeletingId.set(null);
      }
    });
  }

  cancelEdit() {
    this.editingHypothesis.set(null);
    this.resetForm();
  }

  resetForm() {
    this.hypothesisForm.reset({
      description: '',
      icdCode: '',
      type: ''
    });
  }
}
