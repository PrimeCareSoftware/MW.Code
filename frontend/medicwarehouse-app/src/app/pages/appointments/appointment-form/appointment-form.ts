import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { AppointmentService } from '../../../services/appointment';
import { PatientService } from '../../../services/patient';
import { Patient } from '../../../models/patient.model';
import { Auth } from '../../../services/auth';

@Component({
  selector: 'app-appointment-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar],
  templateUrl: './appointment-form.html',
  styleUrl: './appointment-form.scss'
})
export class AppointmentForm implements OnInit {
  appointmentForm: FormGroup;
  patients = signal<Patient[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private patientService: PatientService,
    private router: Router,
    private route: ActivatedRoute,
    private auth: Auth
  ) {
    this.appointmentForm = this.fb.group({
      patientId: ['', [Validators.required]],
      clinicId: ['', [Validators.required]],
      scheduledDate: ['', [Validators.required]],
      scheduledTime: ['', [Validators.required]],
      durationMinutes: [30, [Validators.required, Validators.min(15)]],
      type: ['Regular', [Validators.required]],
      notes: ['']
    });
  }

  ngOnInit(): void {
    // Get clinicId from authenticated user
    const clinicId = this.auth.getClinicId();
    
    if (!clinicId) {
      this.errorMessage.set('Para criar uma consulta, você precisa estar associado a uma clínica. Entre em contato com o administrador do sistema para configurar sua associação com uma clínica.');
      return;
    }
    
    // Set the clinicId in the form
    this.appointmentForm.patchValue({ clinicId });
    
    this.loadPatients();
    
    // Pre-fill date and time from query parameters if provided
    this.route.queryParams.subscribe(params => {
      if (params['date']) {
        this.appointmentForm.patchValue({ scheduledDate: params['date'] });
      }
      if (params['time']) {
        this.appointmentForm.patchValue({ scheduledTime: params['time'] });
      }
    });
  }

  loadPatients(): void {
    this.patientService.getAll().subscribe({
      next: (data) => {
        this.patients.set(data);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar pacientes');
        console.error('Error loading patients:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.appointmentForm.valid) {
      this.isLoading.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

      this.appointmentService.create(this.appointmentForm.value).subscribe({
        next: () => {
          this.successMessage.set('Agendamento criado com sucesso!');
          this.isLoading.set(false);
          setTimeout(() => this.router.navigate(['/appointments']), 1500);
        },
        error: (error) => {
          this.errorMessage.set('Erro ao criar agendamento');
          this.isLoading.set(false);
          console.error('Error creating appointment:', error);
        }
      });
    }
  }
}
