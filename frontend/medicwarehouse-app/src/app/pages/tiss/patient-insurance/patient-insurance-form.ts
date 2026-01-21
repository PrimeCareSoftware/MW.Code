import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { PatientHealthInsuranceService } from '../../../services/patient-health-insurance.service';
import { PatientService } from '../../../services/patient';
import { HealthInsuranceOperatorService } from '../../../services/health-insurance-operator.service';
import { HealthInsurancePlanService, HealthInsurancePlan } from '../../../services/health-insurance-plan.service';
import { Patient } from '../../../models/patient.model';
import { HealthInsuranceOperator } from '../../../models/tiss.model';

@Component({
  selector: 'app-patient-insurance-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar],
  templateUrl: './patient-insurance-form.html',
  styleUrl: './patient-insurance-form.scss'
})
export class PatientInsuranceFormComponent implements OnInit {
  insuranceForm: FormGroup;
  isEditMode = signal<boolean>(false);
  insuranceId = signal<string | null>(null);
  patientId = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  patients = signal<Patient[]>([]);
  operators = signal<HealthInsuranceOperator[]>([]);
  plans = signal<HealthInsurancePlan[]>([]);
  isLoadingPlans = signal<boolean>(false);

  constructor(
    private fb: FormBuilder,
    private insuranceService: PatientHealthInsuranceService,
    private patientService: PatientService,
    private operatorService: HealthInsuranceOperatorService,
    private planService: HealthInsurancePlanService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.insuranceForm = this.fb.group({
      patientId: ['', [Validators.required]],
      operatorId: ['', [Validators.required]],
      healthInsurancePlanId: ['', [Validators.required]],
      cardNumber: ['', [Validators.required]],
      validityDate: ['', [Validators.required]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadPatients();
    this.loadOperators();
    
    const patientId = this.route.snapshot.paramMap.get('patientId');
    if (patientId) {
      this.patientId.set(patientId);
      this.insuranceForm.patchValue({ patientId });
    }

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.insuranceId.set(id);
      this.loadInsurance(id);
    }
  }

  loadPatients(): void {
    this.patientService.getAll().subscribe({
      next: (patients) => {
        this.patients.set(patients);
      },
      error: (error) => {
        console.error('Error loading patients:', error);
      }
    });
  }

  loadOperators(): void {
    this.operatorService.getAll().subscribe({
      next: (operators) => {
        this.operators.set(operators.filter(op => op.isActive));
      },
      error: (error) => {
        console.error('Error loading operators:', error);
      }
    });
  }

  onOperatorChange(): void {
    const operatorId = this.insuranceForm.get('operatorId')?.value;
    if (operatorId) {
      this.loadPlansByOperator(operatorId);
      this.insuranceForm.patchValue({ healthInsurancePlanId: '' });
    } else {
      this.plans.set([]);
    }
  }

  loadPlansByOperator(operatorId: string): void {
    this.isLoadingPlans.set(true);
    this.planService.getByOperatorId(operatorId).subscribe({
      next: (plans) => {
        this.plans.set(plans.filter(p => p.isActive));
        this.isLoadingPlans.set(false);
      },
      error: (error) => {
        console.error('Error loading plans:', error);
        this.isLoadingPlans.set(false);
      }
    });
  }

  loadInsurance(id: string): void {
    this.isLoading.set(true);
    this.insuranceService.getById(id).subscribe({
      next: (insurance) => {
        this.insuranceForm.patchValue({
          patientId: insurance.patientId,
          healthInsurancePlanId: insurance.healthInsurancePlanId,
          cardNumber: insurance.cardNumber,
          validityDate: insurance.validityDate,
          isActive: insurance.isActive
        });
        
        // Load operator and plans for editing
        // You'd need to get operator from plan or store it
        
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar convênio');
        this.isLoading.set(false);
        console.error('Error loading insurance:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.insuranceForm.invalid) {
      this.insuranceForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.insuranceForm.value;

    if (this.isEditMode()) {
      const id = this.insuranceId();
      if (!id) return;

      const updateData = {
        cardNumber: formValue.cardNumber,
        validityDate: formValue.validityDate,
        isActive: formValue.isActive
      };

      this.insuranceService.update(id, updateData).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.successMessage.set('Convênio atualizado com sucesso');
          const patientId = this.patientId() || formValue.patientId;
          setTimeout(() => this.router.navigate(['/tiss/patient-insurance', patientId]), 1500);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao atualizar convênio');
          this.isLoading.set(false);
          console.error('Error updating insurance:', error);
        }
      });
    } else {
      const createData = {
        patientId: formValue.patientId,
        healthInsurancePlanId: formValue.healthInsurancePlanId,
        cardNumber: formValue.cardNumber,
        validityDate: formValue.validityDate
      };

      this.insuranceService.create(createData).subscribe({
        next: () => {
          this.isLoading.set(false);
          this.successMessage.set('Convênio cadastrado com sucesso');
          setTimeout(() => this.router.navigate(['/tiss/patient-insurance', formValue.patientId]), 1500);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao cadastrar convênio');
          this.isLoading.set(false);
          console.error('Error creating insurance:', error);
        }
      });
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.insuranceForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
