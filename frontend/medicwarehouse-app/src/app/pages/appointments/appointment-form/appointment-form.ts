import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { AppointmentService } from '../../../services/appointment';
import { PatientService } from '../../../services/patient';
import { Patient } from '../../../models/patient.model';

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
    private router: Router
  ) {
    this.appointmentForm = this.fb.group({
      patientId: ['', [Validators.required]],
      clinicId: ['00000000-0000-0000-0000-000000000001', [Validators.required]],
      scheduledDate: ['', [Validators.required]],
      scheduledTime: ['', [Validators.required]],
      durationMinutes: [30, [Validators.required, Validators.min(15)]],
      type: ['Regular', [Validators.required]],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadPatients();
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
