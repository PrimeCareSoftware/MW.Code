import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
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

  constructor(private patientService: PatientService) {}

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
}
