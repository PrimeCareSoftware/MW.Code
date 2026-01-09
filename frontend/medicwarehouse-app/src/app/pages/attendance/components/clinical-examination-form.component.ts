import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ClinicalExaminationService } from '../../../services/clinical-examination.service';
import { CreateClinicalExamination, UpdateClinicalExamination, ClinicalExamination } from '../../../models/medical-record.model';

@Component({
  selector: 'app-clinical-examination-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './clinical-examination-form.component.html',
  styleUrl: './clinical-examination-form.component.scss'})
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
    bloodPressureSystolic: { min: 90, max: 140 },
    bloodPressureDiastolic: { min: 60, max: 90 },
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
      bloodPressureSystolic: [null, [Validators.required, Validators.min(50), Validators.max(300)]],
      bloodPressureDiastolic: [null, [Validators.required, Validators.min(30), Validators.max(200)]],
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
      bloodPressureSystolic: this.existingExamination.bloodPressureSystolic,
      bloodPressureDiastolic: this.existingExamination.bloodPressureDiastolic,
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
