import { Component, Input, Output, EventEmitter, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TherapeuticPlanService } from '../../../services/therapeutic-plan.service';
import { CreateTherapeuticPlan, UpdateTherapeuticPlan, TherapeuticPlan } from '../../../models/medical-record.model';

@Component({
  selector: 'app-therapeutic-plan-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './therapeutic-plan-form.component.html',
  styleUrl: './therapeutic-plan-form.component.scss'})
export class TherapeuticPlanFormComponent implements OnInit {
  @Input() medicalRecordId!: string;
  @Input() set existingPlanInput(value: TherapeuticPlan | undefined) {
    if (value) {
      this.existingPlan.set(value);
    }
  }
  @Output() planSaved = new EventEmitter<TherapeuticPlan>();
  @Output() cancelled = new EventEmitter<void>();

  planForm: FormGroup;
  existingPlan = signal<TherapeuticPlan | undefined>(undefined);
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
      examRequests: [''],
      referrals: [''],
      patientGuidance: [''],
      returnDate: ['']
    });
  }

  ngOnInit() {
    if (this.existingPlan()) {
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
          this.existingPlan.set(latestPlan);
          this.loadExistingData();
        }
      },
      error: (error) => {
        console.error('Error loading therapeutic plan:', error);
      }
    });
  }

  loadExistingData() {
    const plan = this.existingPlan();
    if (!plan) return;

    this.planForm.patchValue({
      treatment: plan.treatment,
      medicationPrescription: plan.medicationPrescription || '',
      examRequests: plan.examRequests || '',
      referrals: plan.referrals || '',
      patientGuidance: plan.patientGuidance || '',
      returnDate: plan.returnDate ? 
        new Date(plan.returnDate).toISOString().split('T')[0] : ''
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
      examRequests: formValue.examRequests?.trim() || null,
      referrals: formValue.referrals?.trim() || null,
      patientGuidance: formValue.patientGuidance?.trim() || null,
      returnDate: formValue.returnDate || null
    };

    if (this.existingPlan()) {
      // Update existing
      const updateData: UpdateTherapeuticPlan = planData;

      this.therapeuticPlanService.update(this.existingPlan()!.id, updateData).subscribe({
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
