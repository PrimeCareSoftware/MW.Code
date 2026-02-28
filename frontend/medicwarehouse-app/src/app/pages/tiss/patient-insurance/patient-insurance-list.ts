import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PatientHealthInsuranceService } from '../../../services/patient-health-insurance.service';
import { PatientHealthInsurance } from '../../../models/tiss.model';

@Component({
  selector: 'app-patient-insurance-list',
  imports: [CommonModule, RouterLink],
  templateUrl: './patient-insurance-list.html',
  styleUrl: './patient-insurance-list.scss'
})
export class PatientInsuranceList implements OnInit {
  insurances = signal<PatientHealthInsurance[]>([]);
  patientId = signal<string>('');
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(
    private insuranceService: PatientHealthInsuranceService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const patientId = this.route.snapshot.paramMap.get('patientId');
    if (patientId) {
      this.patientId.set(patientId);
      this.loadInsurances(patientId);
    }
  }

  loadInsurances(patientId: string): void {
    this.isLoading.set(true);
    this.insuranceService.getByPatientId(patientId).subscribe({
      next: (data) => {
        this.insurances.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar convênios');
        this.isLoading.set(false);
        console.error('Error loading insurances:', error);
      }
    });
  }

  deleteInsurance(id: string): void {
    if (confirm('Tem certeza que deseja remover este convênio?')) {
      this.insuranceService.delete(id).subscribe({
        next: () => {
          this.loadInsurances(this.patientId());
        },
        error: (error) => {
          this.errorMessage.set('Erro ao remover convênio');
          console.error('Error deleting insurance:', error);
        }
      });
    }
  }
}
