import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { AnamnesisService } from '../../../services/anamnesis.service';
import { PatientService } from '../../../services/patient';
import { AnamnesisResponse, SPECIALTY_NAMES } from '../../../models/anamnesis.model';
import { Patient } from '../../../models/patient.model';

@Component({
  selector: 'app-anamnesis-history',
  imports: [CommonModule, RouterLink],
  templateUrl: './history.html',
  styleUrl: './history.scss'
})
export class AnamnesisHistoryComponent implements OnInit {
  responses = signal<AnamnesisResponse[]>([]);
  patient = signal<Patient | null>(null);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  selectedResponse = signal<AnamnesisResponse | null>(null);
  
  patientId: string = '';
  specialtyNames = SPECIALTY_NAMES;

  constructor(
    private anamnesisService: AnamnesisService,
    private patientService: PatientService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.patientId = params['patientId'];
    });

    if (!this.patientId) {
      this.errorMessage.set('ID do paciente não fornecido');
      return;
    }

    this.loadPatient();
    this.loadHistory();
  }

  loadPatient(): void {
    this.patientService.getById(this.patientId).subscribe({
      next: (patient) => {
        this.patient.set(patient);
      },
      error: (error) => {
        console.error('Error loading patient:', error);
      }
    });
  }

  loadHistory(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.anamnesisService.getPatientHistory(this.patientId).subscribe({
      next: (data) => {
        // Sort by date descending (most recent first)
        const sorted = data.sort((a, b) => 
          new Date(b.responseDate).getTime() - new Date(a.responseDate).getTime()
        );
        this.responses.set(sorted);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar histórico de anamneses');
        this.isLoading.set(false);
        console.error('Error loading history:', error);
      }
    });
  }

  viewDetails(response: AnamnesisResponse): void {
    this.selectedResponse.set(response);
  }

  closeDetails(): void {
    this.selectedResponse.set(null);
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  formatDateTime(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getAnswerCount(response: AnamnesisResponse): number {
    return response.answers?.length || 0;
  }

  getCompletionBadgeClass(response: AnamnesisResponse): string {
    return response.isComplete ? 'badge-success' : 'badge-warning';
  }

  getCompletionText(response: AnamnesisResponse): string {
    return response.isComplete ? 'Completa' : 'Rascunho';
  }
}
