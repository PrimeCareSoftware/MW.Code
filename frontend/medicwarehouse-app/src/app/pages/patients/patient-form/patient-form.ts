import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { PatientService } from '../../../services/patient';
import { Patient } from '../../../models/patient.model';

@Component({
  selector: 'app-patient-form',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, Navbar],
  templateUrl: './patient-form.html',
  styleUrl: './patient-form.scss'
})
export class PatientForm implements OnInit {
  patientForm: FormGroup;
  isEditMode = signal<boolean>(false);
  patientId = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');

  constructor(
    private fb: FormBuilder,
    private patientService: PatientService,
    private route: ActivatedRoute,
    private router: Router
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
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.patientId.set(id);
      this.loadPatient(id);
    }
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
      this.isLoading.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

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
            this.isLoading.set(false);
            setTimeout(() => this.router.navigate(['/patients']), 1500);
          },
          error: (error) => {
            this.errorMessage.set('Erro ao atualizar paciente');
            this.isLoading.set(false);
            console.error('Error updating patient:', error);
          }
        });
      } else {
        this.patientService.create(formValue).subscribe({
          next: () => {
            this.successMessage.set('Paciente cadastrado com sucesso!');
            this.isLoading.set(false);
            setTimeout(() => this.router.navigate(['/patients']), 1500);
          },
          error: (error) => {
            this.errorMessage.set('Erro ao cadastrar paciente');
            this.isLoading.set(false);
            console.error('Error creating patient:', error);
          }
        });
      }
    }
  }
}
