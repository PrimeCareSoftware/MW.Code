import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { PatientService } from '../../../services/patient';
import { CepService } from '../../../services/cep.service';
import { Auth } from '../../../services/auth';
import { Patient } from '../../../models/patient.model';
import { PatientCompleteHistory, PatientAppointmentHistory, PatientProcedureHistory } from '../../../models/patient-history.model';
import { debounceTime, Subject } from 'rxjs';
import { CpfMaskDirective } from '../../../directives/cpf-mask.directive';
import { PhoneMaskDirective } from '../../../directives/phone-mask.directive';
import { CepMaskDirective } from '../../../directives/cep-mask.directive';
import { ScreenReaderService } from '../../../shared/accessibility/hooks/screen-reader.service';

@Component({
  selector: 'app-patient-form',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, Navbar, CpfMaskDirective, PhoneMaskDirective, CepMaskDirective],
  templateUrl: './patient-form.html',
  styleUrl: './patient-form.scss'
})
export class PatientForm implements OnInit {
  patientForm: FormGroup;
  isEditMode = signal<boolean>(false);
  patientId = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  isLoadingCep = signal<boolean>(false);
  isLoadingHistory = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  isChildPatient = signal<boolean>(false);
  guardianSearchTerm = '';
  guardianSearchResults = signal<Patient[]>([]);
  selectedGuardian = signal<Patient | null>(null);
  private searchSubject = new Subject<string>();

  // Tab management
  activeTab = signal<'basic' | 'appointments' | 'procedures'>('basic');

  // History data
  appointmentHistory = signal<PatientAppointmentHistory[]>([]);
  procedureHistory = signal<PatientProcedureHistory[]>([]);
  
  // Permission for viewing medical records
  canViewMedicalRecords = signal<boolean>(false);

  constructor(
    private fb: FormBuilder,
    private patientService: PatientService,
    private cepService: CepService,
    private auth: Auth,
    private route: ActivatedRoute,
    private router: Router,
    private screenReader: ScreenReaderService
  ) {
    this.patientForm = this.fb.group({
      name: ['', [Validators.required]],
      document: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      gender: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phoneCountryCode: ['+55', [Validators.required]],
      phoneNumber: ['', [Validators.required]],
      address: this.fb.group({
        street: ['', [Validators.required]],
        number: ['', [Validators.required]],
        complement: [''],
        neighborhood: ['', [Validators.required]],
        city: ['', [Validators.required]],
        state: ['', [Validators.required]],
        zipCode: ['', [Validators.required]],
        country: ['Brasil', [Validators.required]]
      }),
      medicalHistory: [''],
      allergies: ['']
    });

    // Setup debounced search
    this.searchSubject.pipe(debounceTime(300)).subscribe(term => {
      if (term.length >= 3) {
        this.patientService.search(term).subscribe({
          next: (results) => {
            // Filter out children - guardians must be adults
            this.guardianSearchResults.set(results.filter(p => !p.isChild));
          },
          error: (error) => {
            console.error('Error searching patients:', error);
          }
        });
      } else {
        this.guardianSearchResults.set([]);
      }
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.patientId.set(id);
      this.loadPatient(id);
      // Load history data for existing patient
      this.loadPatientHistory(id);
    }

    // TODO: Check user permissions for medical records
    // For now, we assume the user can view medical records if they're a doctor
    // In production, check: this.auth.currentUser()?.permissions.includes('medical-records.view')
    this.canViewMedicalRecords.set(true); // Placeholder
  }

  setActiveTab(tab: 'basic' | 'appointments' | 'procedures'): void {
    this.activeTab.set(tab);
  }

  loadPatientHistory(patientId: string): void {
    this.isLoadingHistory.set(true);

    // Load appointment history
    this.patientService.getAppointmentHistory(patientId, this.canViewMedicalRecords()).subscribe({
      next: (history) => {
        this.appointmentHistory.set(history.appointments);
        this.isLoadingHistory.set(false);
      },
      error: (error) => {
        console.error('Error loading appointment history:', error);
        this.isLoadingHistory.set(false);
      }
    });

    // Load procedure history
    this.patientService.getProcedureHistory(patientId).subscribe({
      next: (procedures) => {
        this.procedureHistory.set(procedures);
      },
      error: (error) => {
        console.error('Error loading procedure history:', error);
      }
    });
  }

  formatPaymentMethod(method: string): string {
    const methodMap: { [key: string]: string } = {
      'Cash': 'Dinheiro',
      'CreditCard': 'Cartão de Crédito',
      'DebitCard': 'Cartão de Débito',
      'Pix': 'PIX',
      'BankTransfer': 'Transferência Bancária',
      'Check': 'Cheque'
    };
    return methodMap[method] || method;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }

  formatTime(timeString: string): string {
    // timeString format: "HH:MM:SS" or TimeSpan format
    return timeString.substring(0, 5); // Returns "HH:MM"
  }

  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('pt-BR', { 
      style: 'currency', 
      currency: 'BRL' 
    }).format(amount);
  }
  
  getStatusBadgeClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Scheduled': 'badge-info',
      'Confirmed': 'badge-success',
      'InProgress': 'badge-warning',
      'Completed': 'badge-success',
      'Cancelled': 'badge-error',
      'NoShow': 'badge-error'
    };
    return statusMap[status] || 'badge-default';
  }

  getPaymentStatusBadgeClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'badge-warning',
      'Processing': 'badge-info',
      'Paid': 'badge-success',
      'Failed': 'badge-error',
      'Refunded': 'badge-info',
      'Cancelled': 'badge-error'
    };
    return statusMap[status] || 'badge-default';
  }

  formatAppointmentStatus(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Scheduled': 'Agendado',
      'Confirmed': 'Confirmado',
      'InProgress': 'Em Andamento',
      'Completed': 'Concluído',
      'Cancelled': 'Cancelado',
      'NoShow': 'Faltou'
    };
    return statusMap[status] || status;
  }

  formatPaymentStatus(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Pending': 'Pendente',
      'Processing': 'Processando',
      'Paid': 'Pago',
      'Failed': 'Falhou',
      'Refunded': 'Reembolsado',
      'Cancelled': 'Cancelado'
    };
    return statusMap[status] || status;
  }

  onDateOfBirthChange(): void {
    const dateOfBirth = this.patientForm.get('dateOfBirth')?.value;
    if (dateOfBirth) {
      const age = this.calculateAge(new Date(dateOfBirth));
      this.isChildPatient.set(age < 18);
      
      // If not a child anymore, clear guardian
      if (age >= 18) {
        this.selectedGuardian.set(null);
      }
    }
  }

  calculateAge(birthDate: Date): number {
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    return age;
  }

  searchGuardian(): void {
    this.searchSubject.next(this.guardianSearchTerm);
  }

  selectGuardian(patient: Patient): void {
    this.selectedGuardian.set(patient);
    this.guardianSearchResults.set([]);
    this.guardianSearchTerm = patient.name;
  }

  clearGuardian(): void {
    this.selectedGuardian.set(null);
    this.guardianSearchTerm = '';
  }

  loadPatient(id: string): void {
    this.isLoading.set(true);
    this.patientService.getById(id).subscribe({
      next: (patient) => {
        this.patientForm.patchValue({
          name: patient.name,
          document: patient.document,
          dateOfBirth: patient.dateOfBirth.split('T')[0],
          gender: patient.gender,
          email: patient.email,
          phoneCountryCode: '+55',
          phoneNumber: patient.phone.replace('+55', ''),
          address: patient.address,
          medicalHistory: patient.medicalHistory,
          allergies: patient.allergies
        });
        
        // Check if child and load guardian if exists
        this.isChildPatient.set(patient.isChild);
        if (patient.guardianId && patient.guardianName) {
          this.selectedGuardian.set({
            id: patient.guardianId,
            name: patient.guardianName,
            document: '',
            dateOfBirth: '',
            gender: '',
            email: '',
            phone: '',
            address: {
              street: '',
              number: '',
              neighborhood: '',
              city: '',
              state: '',
              zipCode: '',
              country: ''
            },
            isActive: true,
            age: 0,
            isChild: false,
            createdAt: ''
          } as Patient);
          this.guardianSearchTerm = patient.guardianName;
        }
        
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar paciente');
        this.isLoading.set(false);
        console.error('Error loading patient:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.patientForm.valid) {
      // Validate guardian for children
      if (this.isChildPatient() && !this.selectedGuardian() && !this.isEditMode()) {
        this.errorMessage.set('Crianças menores de 18 anos devem ter um responsável');
        this.screenReader.announceError('Crianças menores de 18 anos devem ter um responsável');
        return;
      }

      this.isLoading.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');
      this.screenReader.announceLoading(this.isEditMode() ? 'atualização do paciente' : 'cadastro do paciente');

      const formValue = this.patientForm.value;

      if (this.isEditMode()) {
        const updateData = {
          name: formValue.name,
          email: formValue.email,
          phoneCountryCode: formValue.phoneCountryCode,
          phoneNumber: formValue.phoneNumber,
          address: formValue.address,
          medicalHistory: formValue.medicalHistory,
          allergies: formValue.allergies
        };

        this.patientService.update(this.patientId()!, updateData).subscribe({
          next: () => {
            this.successMessage.set('Paciente atualizado com sucesso!');
            this.screenReader.announceSuccess('Paciente atualizado com sucesso!');
            this.isLoading.set(false);
            setTimeout(() => this.router.navigate(['/patients']), 1500);
          },
          error: (error) => {
            this.errorMessage.set('Erro ao atualizar paciente');
            this.screenReader.announceError('Erro ao atualizar paciente');
            this.isLoading.set(false);
            console.error('Error updating patient:', error);
          }
        });
      } else {
        const createData = {
          ...formValue,
          guardianId: this.selectedGuardian()?.id
        };

        this.patientService.create(createData).subscribe({
          next: () => {
            this.successMessage.set('Paciente cadastrado com sucesso!');
            this.screenReader.announceSuccess('Paciente cadastrado com sucesso!');
            this.isLoading.set(false);
            setTimeout(() => this.router.navigate(['/patients']), 1500);
          },
          error: (error) => {
            this.errorMessage.set('Erro ao cadastrar paciente');
            this.screenReader.announceError('Erro ao cadastrar paciente');
            this.isLoading.set(false);
            console.error('Error creating patient:', error);
          }
        });
      }
    }
  }

  /**
   * Look up CEP and auto-fill address fields
   */
  onCepBlur(): void {
    const addressGroup = this.patientForm.get('address');
    const cep = addressGroup?.get('zipCode')?.value;
    
    if (!cep || cep.replace(/\D/g, '').length !== 8) {
      return;
    }

    this.isLoadingCep.set(true);
    this.cepService.lookupCep(cep).subscribe({
      next: (addressData) => {
        this.isLoadingCep.set(false);
        if (addressData && addressGroup) {
          // Auto-fill address fields
          addressGroup.patchValue({
            street: addressData.street,
            neighborhood: addressData.neighborhood,
            city: addressData.city,
            state: addressData.state
          });
          
          if (addressData.complement) {
            addressGroup.patchValue({ complement: addressData.complement });
          }
        }
      },
      error: (error) => {
        this.isLoadingCep.set(false);
        console.error('Error looking up CEP:', error);
      }
    });
  }
}
