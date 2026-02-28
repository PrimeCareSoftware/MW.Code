import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthorizationRequestService } from '../../../services/authorization-request.service';
import { PatientHealthInsuranceService } from '../../../services/patient-health-insurance.service';
import { TussProcedureService } from '../../../services/tuss-procedure.service';
import { PatientHealthInsurance, TussProcedure } from '../../../models/tiss.model';

@Component({
  selector: 'app-authorization-request-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './authorization-request-form.html',
  styleUrl: './authorization-request-form.scss'
})
export class AuthorizationRequestFormComponent implements OnInit {
  requestForm: FormGroup;
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  patientInsurances = signal<PatientHealthInsurance[]>([]);
  procedureSearchResults = signal<TussProcedure[]>([]);
  selectedProcedure = signal<TussProcedure | null>(null);
  isSearchingProcedures = signal<boolean>(false);

  constructor(
    private fb: FormBuilder,
    private authorizationService: AuthorizationRequestService,
    private patientInsuranceService: PatientHealthInsuranceService,
    private tussService: TussProcedureService,
    private router: Router
  ) {
    this.requestForm = this.fb.group({
      patientHealthInsuranceId: ['', [Validators.required]],
      procedureCode: ['', [Validators.required]],
      procedureName: [''],
      quantity: [1, [Validators.required, Validators.min(1)]],
      justification: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  ngOnInit(): void {
    // Patient insurances will be loaded based on search/filter in a future enhancement
    // For now, users can manually select from available patient insurances
  }

  onProcedureSearch(searchTerm: string): void {
    if (!searchTerm || searchTerm.length < 2) {
      this.procedureSearchResults.set([]);
      return;
    }

    this.isSearchingProcedures.set(true);
    this.tussService.search(searchTerm).subscribe({
      next: (procedures) => {
        this.procedureSearchResults.set(procedures);
        this.isSearchingProcedures.set(false);
      },
      error: (error) => {
        this.isSearchingProcedures.set(false);
        console.error('Error searching procedures:', error);
      }
    });
  }

  selectProcedure(procedure: TussProcedure): void {
    this.selectedProcedure.set(procedure);
    this.requestForm.patchValue({
      procedureCode: procedure.code,
      procedureName: procedure.name
    });
    this.procedureSearchResults.set([]);
  }

  onSubmit(): void {
    if (this.requestForm.invalid) {
      this.requestForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const formValue = this.requestForm.value;
    const requestData = {
      patientHealthInsuranceId: formValue.patientHealthInsuranceId,
      procedureCode: formValue.procedureCode,
      quantity: formValue.quantity,
      justification: formValue.justification
    };

    this.authorizationService.create(requestData).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.successMessage.set('Solicitação de autorização criada com sucesso');
        setTimeout(() => this.router.navigate(['/tiss/authorizations']), 1500);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao criar solicitação de autorização');
        this.isLoading.set(false);
        console.error('Error creating authorization request:', error);
      }
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.requestForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
