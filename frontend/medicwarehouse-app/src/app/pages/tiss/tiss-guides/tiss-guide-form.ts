import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { TissGuideService } from '../../../services/tiss-guide.service';
import { PatientHealthInsuranceService } from '../../../services/patient-health-insurance.service';
import { AuthorizationRequestService } from '../../../services/authorization-request.service';
import { TussProcedureService } from '../../../services/tuss-procedure.service';
import { TissGuideType, PatientHealthInsurance, AuthorizationRequest, TussProcedure, CreateTissGuideProcedure } from '../../../models/tiss.model';
import { ScreenReaderService } from '../../../shared/accessibility/hooks/screen-reader.service';

@Component({
  selector: 'app-tiss-guide-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar, AccessibleBreadcrumbsComponent],
  templateUrl: './tiss-guide-form.html',
  styleUrl: './tiss-guide-form.scss'
})
export class TissGuideFormComponent implements OnInit {
  guideForm: FormGroup;
  isEditMode = signal<boolean>(false);
  guideId = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  breadcrumbs: BreadcrumbItem[] = [];
  
  patientInsurances = signal<PatientHealthInsurance[]>([]);
  authorizationRequests = signal<AuthorizationRequest[]>([]);
  procedureSearchResults = signal<TussProcedure[]>([]);
  isSearchingProcedures = signal<boolean>(false);
  
  guideTypes = Object.values(TissGuideType);
  totalAmount = signal<number>(0);

  constructor(
    private fb: FormBuilder,
    private guideService: TissGuideService,
    private patientInsuranceService: PatientHealthInsuranceService,
    private authorizationService: AuthorizationRequestService,
    private tussService: TussProcedureService,
    private route: ActivatedRoute,
    private router: Router,
    private screenReader: ScreenReaderService
  ) {
    this.guideForm = this.fb.group({
      guideType: ['', [Validators.required]],
      patientHealthInsuranceId: ['', [Validators.required]],
      authorizationRequestId: [''],
      serviceDate: ['', [Validators.required]],
      procedures: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.loadPatientInsurances();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.guideId.set(id);
      this.loadGuide(id);
      // Set breadcrumbs for edit mode
      this.breadcrumbs = [
        { label: 'Início', url: '/' },
        { label: 'TISS', url: '/tiss' },
        { label: 'Guias', url: '/tiss/guides' },
        { label: 'Editar Guia' }
      ];
    } else {
      // Set breadcrumbs for create mode
      this.breadcrumbs = [
        { label: 'Início', url: '/' },
        { label: 'TISS', url: '/tiss' },
        { label: 'Guias', url: '/tiss/guides' },
        { label: 'Nova Guia' }
      ];
    }

    // Add first procedure row by default
    if (!this.isEditMode()) {
      this.addProcedure();
    }
  }

  get procedures(): FormArray {
    return this.guideForm.get('procedures') as FormArray;
  }

  loadPatientInsurances(): void {
    // Patient insurances will be loaded based on search/filter in a future enhancement
    // For now, users can manually select from available patient insurances via dropdown
  }

  onPatientInsuranceChange(): void {
    const insuranceId = this.guideForm.get('patientHealthInsuranceId')?.value;
    if (insuranceId) {
      this.loadAuthorizationRequests(insuranceId);
    }
  }

  loadAuthorizationRequests(patientInsuranceId: string): void {
    this.authorizationService.getByPatientInsurance(patientInsuranceId).subscribe({
      next: (requests) => {
        this.authorizationRequests.set(requests.filter(r => r.status === 'Approved'));
      },
      error: (error) => {
        console.error('Error loading authorization requests:', error);
      }
    });
  }

  loadGuide(id: string): void {
    this.isLoading.set(true);
    this.guideService.getById(id).subscribe({
      next: (guide) => {
        this.guideForm.patchValue({
          guideType: guide.guideType,
          patientHealthInsuranceId: guide.patientHealthInsuranceId,
          authorizationRequestId: guide.authorizationRequestId,
          serviceDate: guide.serviceDate
        });
        
        // Load procedures
        guide.procedures.forEach(proc => {
          this.procedures.push(this.fb.group({
            procedureCode: [proc.procedureCode, [Validators.required]],
            procedureName: [proc.procedureName],
            quantity: [proc.quantity, [Validators.required, Validators.min(1)]],
            unitPrice: [proc.unitPrice, [Validators.required, Validators.min(0)]],
            totalPrice: [proc.totalPrice]
          }));
        });
        
        this.calculateTotal();
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar guia');
        this.isLoading.set(false);
        console.error('Error loading guide:', error);
      }
    });
  }

  addProcedure(): void {
    const procedureGroup = this.fb.group({
      procedureCode: ['', [Validators.required]],
      procedureName: [''],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      totalPrice: [0]
    });

    // Listen to quantity and unitPrice changes to update totalPrice
    // Use a lambda to get the current index dynamically
    procedureGroup.get('quantity')?.valueChanges.subscribe(() => {
      const index = this.procedures.controls.indexOf(procedureGroup);
      if (index !== -1) this.updateProcedureTotal(index);
    });
    
    procedureGroup.get('unitPrice')?.valueChanges.subscribe(() => {
      const index = this.procedures.controls.indexOf(procedureGroup);
      if (index !== -1) this.updateProcedureTotal(index);
    });

    this.procedures.push(procedureGroup);
  }

  removeProcedure(index: number): void {
    this.procedures.removeAt(index);
    this.calculateTotal();
  }

  onProcedureSearch(index: number, searchTerm: string): void {
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

  selectProcedure(index: number, procedure: TussProcedure): void {
    const procedureGroup = this.procedures.at(index);
    procedureGroup.patchValue({
      procedureCode: procedure.code,
      procedureName: procedure.name,
      unitPrice: 0 // Unit price must be manually entered for now
    });
    this.procedureSearchResults.set([]);
    this.updateProcedureTotal(index);
  }

  updateProcedureTotal(index: number): void {
    const procedureGroup = this.procedures.at(index);
    const quantity = procedureGroup.get('quantity')?.value || 0;
    const unitPrice = procedureGroup.get('unitPrice')?.value || 0;
    const total = quantity * unitPrice;
    
    procedureGroup.patchValue({ totalPrice: total }, { emitEvent: false });
    this.calculateTotal();
  }

  calculateTotal(): void {
    let total = 0;
    this.procedures.controls.forEach(control => {
      total += control.get('totalPrice')?.value || 0;
    });
    this.totalAmount.set(total);
  }

  onSubmit(): void {
    if (this.guideForm.invalid) {
      this.guideForm.markAllAsTouched();
      this.screenReader.announceError('Por favor, preencha todos os campos obrigatórios');
      return;
    }

    if (this.procedures.length === 0) {
      this.errorMessage.set('Adicione pelo menos um procedimento');
      this.screenReader.announceError('Adicione pelo menos um procedimento');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');
    this.screenReader.announceLoading('criação da guia TISS');

    const formValue = this.guideForm.value;
    const procedures: CreateTissGuideProcedure[] = formValue.procedures.map((proc: any) => ({
      procedureCode: proc.procedureCode,
      quantity: proc.quantity,
      unitPrice: proc.unitPrice
    }));

    const guideData = {
      guideType: formValue.guideType,
      patientHealthInsuranceId: formValue.patientHealthInsuranceId,
      authorizationRequestId: formValue.authorizationRequestId || undefined,
      serviceDate: formValue.serviceDate,
      procedures
    };

    this.guideService.create(guideData).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.successMessage.set('Guia criada com sucesso');
        this.screenReader.announceSuccess('Guia criada com sucesso');
        setTimeout(() => this.router.navigate(['/tiss/guides']), 1500);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao criar guia');
        this.screenReader.announceError('Erro ao criar guia');
        this.isLoading.set(false);
        console.error('Error creating guide:', error);
      }
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.guideForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }
}
