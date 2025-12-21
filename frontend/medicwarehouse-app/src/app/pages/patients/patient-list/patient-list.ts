import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Navbar } from '../../../shared/navbar/navbar';
import { PatientService } from '../../../services/patient';
import { Patient } from '../../../models/patient.model';

@Component({
  selector: 'app-patient-list',
  imports: [CommonModule, RouterLink, Navbar],
  templateUrl: './patient-list.html',
  styleUrl: './patient-list.scss'
})
export class PatientList implements OnInit {
  patients = signal<Patient[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(
    private patientService: PatientService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this.isLoading.set(true);
    this.patientService.getAll().subscribe({
      next: (data) => {
        this.patients.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar pacientes');
        this.isLoading.set(false);
        console.error('Error loading patients:', error);
      }
    });
  }

  deletePatient(id: string): void {
    if (confirm('Tem certeza que deseja excluir este paciente?')) {
      this.patientService.delete(id).subscribe({
        next: () => {
          this.loadPatients();
        },
        error: (error) => {
          this.errorMessage.set('Erro ao excluir paciente');
          console.error('Error deleting patient:', error);
        }
      });
    }
  }

  startAttendance(patientId: string): void {
    // Navigate to waiting queue to add patient or directly to attendance
    // For now, we'll navigate to create an appointment with the patient pre-selected
    this.router.navigate(['/appointments/new'], { queryParams: { patientId } });
  }
}
