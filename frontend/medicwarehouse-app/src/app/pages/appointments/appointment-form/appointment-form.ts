import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AppointmentService } from '../../../services/appointment';
import { PatientService } from '../../../services/patient';
import { Patient } from '../../../models/patient.model';
import { Professional } from '../../../models/appointment.model';
import { Auth } from '../../../services/auth';
import { ScreenReaderService } from '../../../shared/accessibility/hooks/screen-reader.service';
import { AccessibleBreadcrumbsComponent, BreadcrumbItem } from '../../../shared/accessibility/components/accessible-breadcrumbs.component';
import { Subject, of } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-appointment-form',
  imports: [CommonModule, ReactiveFormsModule, FormsModule, RouterLink, AccessibleBreadcrumbsComponent],
  templateUrl: './appointment-form.html',
  styleUrl: './appointment-form.scss'
})
export class AppointmentForm implements OnInit, OnDestroy {
  appointmentForm: FormGroup;
  patientSearchTerm = '';
  patientSearchResults = signal<Patient[]>([]);
  selectedPatient = signal<Patient | null>(null);
  isSearchingPatients = signal<boolean>(false);
  patientSearchMessage = signal<string>('Digite pelo menos 3 caracteres para buscar por nome, CPF ou telefone.');
  professionals = signal<Professional[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  isEditMode = signal<boolean>(false);
  appointmentId: string | null = null;
  breadcrumbs: BreadcrumbItem[] = [];
  private patientSearchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private patientService: PatientService,
    private router: Router,
    private route: ActivatedRoute,
    private auth: Auth,
    private screenReader: ScreenReaderService
  ) {
    this.appointmentForm = this.fb.group({
      patientId: ['', [Validators.required]],
      clinicId: ['', [Validators.required]],
      professionalId: [''], // Optional
      scheduledDate: ['', [Validators.required]],
      scheduledTime: ['', [Validators.required]],
      durationMinutes: [30, [Validators.required, Validators.min(15)]],
      type: ['Regular', [Validators.required]],
      notes: ['']
    });

    this.setupPatientSearch();
  }

  ngOnInit(): void {
    // Check if we are in edit mode by looking at route parameters
    this.appointmentId = this.route.snapshot.paramMap.get('id');
    if (this.appointmentId) {
      this.isEditMode.set(true);
      this.loadAppointmentData(this.appointmentId);
      // Set breadcrumbs for edit mode
      this.breadcrumbs = [
        { label: 'Início', url: '/' },
        { label: 'Agendamentos', url: '/appointments' },
        { label: 'Editar Agendamento' }
      ];
    } else {
      // Set breadcrumbs for create mode
      this.breadcrumbs = [
        { label: 'Início', url: '/' },
        { label: 'Agendamentos', url: '/appointments' },
        { label: 'Novo Agendamento' }
      ];
    }

    // Get clinicId from authenticated user
    const clinicId = this.auth.getClinicId();
    
    if (!clinicId) {
      this.errorMessage.set('Para criar uma consulta, você precisa estar associado a uma clínica. Entre em contato com o administrador do sistema para configurar sua associação com uma clínica.');
      return;
    }
    
    // Set the clinicId in the form
    this.appointmentForm.patchValue({ clinicId });
    
    this.loadProfessionals();
    
    // Pre-fill date and time from query parameters if provided (only in create mode)
    if (!this.isEditMode()) {
      this.route.queryParams.subscribe(params => {
        if (params['date']) {
          this.appointmentForm.patchValue({ scheduledDate: params['date'] });
        }
        if (params['time']) {
          this.appointmentForm.patchValue({ scheduledTime: params['time'] });
        }
      });
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadAppointmentData(id: string): void {
    this.isLoading.set(true);
    this.appointmentService.getById(id).subscribe({
      next: (appointment) => {
        this.appointmentForm.patchValue({
          patientId: appointment.patientId,
          clinicId: appointment.clinicId,
          professionalId: appointment.professionalId || '',
          scheduledDate: appointment.scheduledDate,
          scheduledTime: appointment.scheduledTime,
          durationMinutes: appointment.durationMinutes,
          type: appointment.type,
          notes: appointment.notes || ''
        });

        this.loadSelectedPatient(appointment.patientId);
        
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar agendamento');
        this.isLoading.set(false);
        console.error('Error loading appointment:', error);
      }
    });
  }

  setupPatientSearch(): void {
    this.patientSearchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap((term) => {
        const normalizedTerm = term.trim();

        if (normalizedTerm.length < 3) {
          this.isSearchingPatients.set(false);
          this.patientSearchResults.set([]);
          this.patientSearchMessage.set('Digite pelo menos 3 caracteres para buscar por nome, CPF ou telefone.');
          return of([]);
        }

        this.isSearchingPatients.set(true);
        this.patientSearchMessage.set('Buscando pacientes...');

        return this.patientService.search(normalizedTerm).pipe(
          catchError((error) => {
            this.errorMessage.set('Erro ao buscar pacientes. Tente novamente.');
            this.patientSearchMessage.set('Erro ao buscar pacientes.');
            console.error('Error searching patients:', error);
            return of([]);
          })
        );
      }),
      takeUntil(this.destroy$)
    ).subscribe((patients) => {
      this.isSearchingPatients.set(false);
      this.patientSearchResults.set(patients);

      if (this.patientSearchTerm.trim().length < 3) {
        this.patientSearchMessage.set('Digite pelo menos 3 caracteres para buscar por nome, CPF ou telefone.');
        return;
      }

      this.patientSearchMessage.set(
        patients.length > 0
          ? `${patients.length} paciente(s) encontrado(s).`
          : 'Nenhum paciente encontrado para o termo informado.'
      );
    });
  }

  searchPatients(): void {
    this.patientSearchSubject.next(this.patientSearchTerm);
  }

  selectPatient(patient: Patient): void {
    this.selectedPatient.set(patient);
    this.appointmentForm.patchValue({ patientId: patient.id });
    this.patientSearchTerm = patient.name;
    this.patientSearchResults.set([]);
    this.patientSearchMessage.set(`Paciente selecionado: ${patient.name}.`);
  }

  clearSelectedPatient(): void {
    if (this.isEditMode()) {
      return;
    }

    this.selectedPatient.set(null);
    this.patientSearchTerm = '';
    this.patientSearchResults.set([]);
    this.patientSearchMessage.set('Digite pelo menos 3 caracteres para buscar por nome, CPF ou telefone.');
    this.appointmentForm.patchValue({ patientId: '' });
    this.appointmentForm.get('patientId')?.markAsTouched();
  }

  private loadSelectedPatient(patientId: string): void {
    this.patientService.getById(patientId).subscribe({
      next: (patient) => {
        this.selectedPatient.set(patient);
        this.patientSearchTerm = patient.name;
      },
      error: (error) => {
        console.error('Error loading selected patient:', error);
      }
    });
  }

  loadProfessionals(): void {
    this.appointmentService.getProfessionals().subscribe({
      next: (professionals) => {
        this.professionals.set(professionals);
        // Auto-select professional if only one is available
        if (professionals.length === 1 && !this.isEditMode()) {
          this.appointmentForm.patchValue({ professionalId: professionals[0].id });
        }
      },
      error: (error) => {
        console.error('Error loading professionals:', error);
        // Don't show error to user, professionals are optional
      }
    });
  }

  onSubmit(): void {
    if (this.appointmentForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');
      this.screenReader.announceLoading(this.isEditMode() ? 'atualização do agendamento' : 'criação do agendamento');

      if (this.isEditMode() && this.appointmentId) {
        // Update existing appointment - only send editable fields
        const updateData = {
          professionalId: this.appointmentForm.value.professionalId || null,
          scheduledDate: this.appointmentForm.value.scheduledDate,
          scheduledTime: this.appointmentForm.value.scheduledTime,
          durationMinutes: this.appointmentForm.value.durationMinutes,
          type: this.appointmentForm.value.type,
          notes: this.appointmentForm.value.notes
        };
        
        this.appointmentService.update(this.appointmentId, updateData).subscribe({
          next: () => {
            this.successMessage.set('Agendamento atualizado com sucesso!');
            this.screenReader.announceSuccess('Agendamento atualizado com sucesso!');
            this.isLoading.set(false);
            setTimeout(() => this.router.navigate(['/appointments']), 1500);
          },
          error: (error) => {
            this.errorMessage.set('Erro ao atualizar agendamento');
            this.screenReader.announceError('Erro ao atualizar agendamento');
            this.isLoading.set(false);
            console.error('Error updating appointment:', error);
          }
        });
      } else {
        // Create new appointment - send all fields
        const createData = {
          ...this.appointmentForm.value,
          professionalId: this.appointmentForm.value.professionalId || null
        };
        
        this.appointmentService.create(createData).subscribe({
          next: () => {
            this.successMessage.set('Agendamento criado com sucesso!');
            this.screenReader.announceSuccess('Agendamento criado com sucesso!');
            this.isLoading.set(false);
            setTimeout(() => this.router.navigate(['/appointments']), 1500);
          },
          error: (error) => {
            this.errorMessage.set('Erro ao criar agendamento');
            this.screenReader.announceError('Erro ao criar agendamento');
            this.isLoading.set(false);
            console.error('Error creating appointment:', error);
          }
        });
      }
    }
  }
}
