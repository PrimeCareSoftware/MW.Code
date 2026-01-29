import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatChipsModule } from '@angular/material/chips';
import { firstValueFrom } from 'rxjs';
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

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.loading = true;
    this.error = null;

    Promise.all([
      this.loadPersonalData(),
      this.loadMedicalRecords(),
      this.loadAppointments()
    ])
      .then(() => {
        this.loading = false;
      })
      .catch((error) => {
        this.error = 'Erro ao carregar dados. Tente novamente mais tarde.';
        this.loading = false;
        console.error('Error loading data:', error);
      });
  }

  private async loadPersonalData(): Promise<void> {
    const data = await firstValueFrom(
      this.http.get<PersonalData>(`${environment.apiUrl}/patient-portal/personal-data`)
    );
    this.personalData = data || null;
  }

  private async loadMedicalRecords(): Promise<void> {
    const data = await firstValueFrom(
      this.http.get<MedicalRecord[]>(`${environment.apiUrl}/patient-portal/medical-records`)
    );
    this.medicalRecords = data || [];
  }

  private async loadAppointments(): Promise<void> {
    const data = await firstValueFrom(
      this.http.get<Appointment[]>(`${environment.apiUrl}/patient-portal/appointments`)
    );
    this.appointments = data || [];
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
