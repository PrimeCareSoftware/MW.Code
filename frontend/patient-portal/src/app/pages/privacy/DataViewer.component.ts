import { Component, OnInit, DestroyRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatChipsModule } from '@angular/material/chips';
import { forkJoin } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { environment } from '../../../environments/environment';

interface PersonalData {
  nome: string;
  cpf: string;
  dataNascimento: string;
  email: string;
  telefone: string;
  endereco: string;
}

interface MedicalRecord {
  id: number;
  data: string;
  tipo: string;
  descricao: string;
  profissional: string;
}

interface Appointment {
  id: number;
  data: string;
  hora: string;
  tipo: string;
  profissional: string;
  status: string;
}

@Component({
  selector: 'app-data-viewer',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatExpansionModule,
    MatChipsModule
  ],
  templateUrl: './DataViewer.component.html',
  styleUrls: ['./privacy.scss']
})
export class DataViewerComponent implements OnInit {
  loading = true;
  error: string | null = null;
  personalData: PersonalData | null = null;
  medicalRecords: MedicalRecord[] = [];
  appointments: Appointment[] = [];
  
  private destroyRef = inject(DestroyRef);

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.loading = true;
    this.error = null;

    forkJoin({
      personalData: this.http.get<PersonalData>(`${environment.apiUrl}/patient-portal/personal-data`),
      medicalRecords: this.http.get<MedicalRecord[]>(`${environment.apiUrl}/patient-portal/medical-records`),
      appointments: this.http.get<Appointment[]>(`${environment.apiUrl}/patient-portal/appointments`)
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (results) => {
          this.personalData = results.personalData || null;
          this.medicalRecords = results.medicalRecords || [];
          this.appointments = results.appointments || [];
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Erro ao carregar dados. Tente novamente mais tarde.';
          this.loading = false;
          console.error('Error loading data:', error);
        }
      });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }

  getStatusColor(status: string): string {
    const colors: { [key: string]: string } = {
      'Confirmado': 'primary',
      'Pendente': 'accent',
      'Cancelado': 'warn',
      'Concluído': 'primary'
    };
    return colors[status] || 'primary';
  }
}
