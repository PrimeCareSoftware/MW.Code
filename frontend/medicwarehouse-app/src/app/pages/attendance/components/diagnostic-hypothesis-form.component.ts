import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormArray } from '@angular/forms';
import { DiagnosticHypothesisService } from '../../../services/diagnostic-hypothesis.service';
import { CreateDiagnosticHypothesis, UpdateDiagnosticHypothesis, DiagnosticHypothesis, DiagnosisType } from '../../../models/medical-record.model';

@Component({
  selector: 'app-diagnostic-hypothesis-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './diagnostic-hypothesis-form.component.html',
  styleUrl: './diagnostic-hypothesis-form.component.scss'})
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
