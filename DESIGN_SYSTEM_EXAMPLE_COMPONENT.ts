// Example: Patient List Component with Design System Features
// Location: frontend/medicwarehouse-app/src/app/pages/patients/patient-list/

import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';

interface Patient {
  id: string;
  name: string;
  email: string;
  phone: string;
  status: 'active' | 'inactive';
}

@Component({
  selector: 'app-patient-list',
  template: `
    <!-- LOADING STATE: Skeleton -->
    <div class="skeleton-patient-list" *ngIf="loading">
      <div class="skeleton-patient-item" *ngFor="let i of [1,2,3,4,5]">
        <div class="skeleton skeleton-avatar"></div>
        <div class="skeleton-info">
          <div class="skeleton skeleton-name"></div>
          <div class="skeleton skeleton-details"></div>
        </div>
        <div class="skeleton skeleton-status"></div>
      </div>
    </div>

    <!-- ERROR STATE: Humanized Error Message -->
    <div class="error-message" *ngIf="error && !loading">
      <div class="error-icon">⚠️</div>
      <div class="error-content">
        <div class="error-title">Ops! Não foi possível carregar os pacientes</div>
        <div class="error-description">
          {{ getHumanizedError(error) }}
        </div>
        <div class="error-actions">
          <button class="btn-retry" (click)="loadPatients()">
            Tentar Novamente
          </button>
          <button class="btn-dismiss" (click)="error = null">
            Fechar
          </button>
        </div>
      </div>
      <button class="error-close" (click)="error = null">✕</button>
    </div>

    <!-- EMPTY STATE: No Patients -->
    <div class="empty-state" *ngIf="!loading && !error && patients.length === 0">
      <div class="empty-icon">👥</div>
      <h3>Nenhum paciente cadastrado</h3>
      <p>Adicione seu primeiro paciente para começar a usar o sistema. É rápido e fácil!</p>
      <div class="empty-action">
        <button mat-raised-button color="primary" (click)="openAddPatientDialog()">
          <mat-icon>add</mat-icon>
          Adicionar Primeiro Paciente
        </button>
        <a routerLink="/help/patients" class="link-secondary">
          Como adicionar pacientes?
        </a>
      </div>
    </div>

    <!-- DATA LOADED: Patient List -->
    <div class="patient-list" *ngIf="!loading && !error && patients.length > 0">
      <!-- Search bar -->
      <mat-form-field appearance="outline" class="search-field">
        <mat-icon matPrefix>search</mat-icon>
        <input matInput 
               placeholder="Buscar paciente..." 
               [(ngModel)]="searchQuery"
               (ngModelChange)="filterPatients()">
      </mat-form-field>

      <!-- EMPTY STATE: Search Results -->
      <div class="empty-state" *ngIf="filteredPatients.length === 0 && searchQuery">
        <div class="empty-icon">🔍</div>
        <h3>Nenhum resultado encontrado</h3>
        <p>Tente ajustar os termos de busca.</p>
        <div class="empty-action">
          <button mat-raised-button (click)="clearSearch()">
            <mat-icon>clear</mat-icon>
            Limpar Busca
          </button>
        </div>
      </div>

      <!-- Patient Cards (with automatic hover micro-interaction) -->
      <mat-card *ngFor="let patient of filteredPatients" class="patient-card">
        <mat-card-content>
          <div class="patient-info">
            <div class="patient-avatar">
              {{ patient.name.charAt(0) }}
            </div>
            <div class="patient-details">
              <h3>{{ patient.name }}</h3>
              <p>{{ patient.email }}</p>
              <p>{{ patient.phone }}</p>
            </div>
            <div class="patient-status" [class.active]="patient.status === 'active'">
              {{ patient.status === 'active' ? 'Ativo' : 'Inativo' }}
            </div>
          </div>
          <div class="patient-actions">
            <button mat-icon-button (click)="editPatient(patient)">
              <mat-icon>edit</mat-icon>
            </button>
            <button mat-icon-button (click)="deletePatient(patient)">
              <mat-icon>delete</mat-icon>
            </button>
          </div>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .patient-list {
      padding: 1rem;
    }

    .search-field {
      width: 100%;
      margin-bottom: 1rem;
    }

    .patient-card {
      margin-bottom: 1rem;
      cursor: pointer;
      /* Hover elevation is automatic from Design System */
    }

    .patient-info {
      display: flex;
      align-items: center;
      gap: 1rem;
    }

    .patient-avatar {
      width: 48px;
      height: 48px;
      border-radius: 50%;
      background: var(--primary-600);
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: bold;
      font-size: 1.25rem;
    }

    .patient-details {
      flex: 1;

      h3 {
        margin: 0;
        font-size: 1rem;
        font-weight: 600;
      }

      p {
        margin: 0.25rem 0 0 0;
        font-size: 0.875rem;
        color: var(--gray-600);
      }
    }

    .patient-status {
      padding: 0.5rem 1rem;
      border-radius: var(--radius-lg);
      font-size: 0.875rem;
      font-weight: 500;
      background-color: var(--gray-200);
      color: var(--gray-700);

      &.active {
        background-color: var(--success-100);
        color: var(--success-700);
      }
    }

    .patient-actions {
      margin-top: 1rem;
      display: flex;
      gap: 0.5rem;
      justify-content: flex-end;
    }
  `]
})
export class PatientListComponent implements OnInit {
  patients: Patient[] = [];
  filteredPatients: Patient[] = [];
  loading = false;
  error: any = null;
  searchQuery = '';

  constructor(
    private snackBar: MatSnackBar,
    private dialog: MatDialog,
    // private patientService: PatientService // Inject your service
  ) {}

  ngOnInit() {
    this.loadPatients();
  }

  loadPatients() {
    this.loading = true;
    this.error = null;

    // Simulate API call
    setTimeout(() => {
      // Mock data - Replace with actual service call
      // this.patientService.getPatients().subscribe({...})
      
      // SUCCESS CASE:
      this.patients = [
        { id: '1', name: 'João Silva', email: 'joao@email.com', phone: '(11) 99999-9999', status: 'active' },
        { id: '2', name: 'Maria Santos', email: 'maria@email.com', phone: '(11) 88888-8888', status: 'active' },
      ];
      this.filteredPatients = [...this.patients];
      this.loading = false;
      
      // Show success toast
      this.showSuccess('Pacientes carregados com sucesso!');

      // ERROR CASE (uncomment to test):
      // this.error = { message: 'Network error' };
      // this.loading = false;
    }, 1500);
  }

  filterPatients() {
    if (!this.searchQuery) {
      this.filteredPatients = [...this.patients];
      return;
    }

    const query = this.searchQuery.toLowerCase();
    this.filteredPatients = this.patients.filter(patient =>
      patient.name.toLowerCase().includes(query) ||
      patient.email.toLowerCase().includes(query) ||
      patient.phone.includes(query)
    );
  }

  clearSearch() {
    this.searchQuery = '';
    this.filterPatients();
  }

  openAddPatientDialog() {
    // Open dialog to add new patient
    // const dialogRef = this.dialog.open(AddPatientDialogComponent);
    // dialogRef.afterClosed().subscribe(result => {...});
    
    this.showInfo('Funcionalidade em desenvolvimento');
  }

  editPatient(patient: Patient) {
    this.showInfo(`Editando ${patient.name}`);
  }

  deletePatient(patient: Patient) {
    // Confirm and delete
    if (confirm(\`Deseja realmente excluir \${patient.name}?\`)) {
      this.showSuccess(\`\${patient.name} foi removido\`);
      // Actually delete from array/API
    }
  }

  // Error Handling - Humanized Messages
  getHumanizedError(error: any): string {
    const errorMap: { [key: string]: string } = {
      'Network error': 'Sem conexão com a internet. Verifique sua rede e tente novamente.',
      'Timeout': 'A operação está demorando mais que o esperado. Tente novamente.',
      'Forbidden': 'Você não tem permissão para visualizar pacientes.',
      'Not Found': 'Nenhum paciente encontrado.',
      'Internal Server Error': 'Algo deu errado no servidor. Tente novamente em alguns instantes.'
    };

    const errorType = error?.message || 'Unknown error';
    return errorMap[errorType] || 'Ocorreu um erro inesperado. Tente novamente.';
  }

  // Toast Notifications (using Design System styles)
  showSuccess(message: string) {
    this.snackBar.open(message, 'OK', {
      duration: 3000,
      panelClass: ['toast-success']
    });
  }

  showError(message: string) {
    this.snackBar.open(message, 'Fechar', {
      duration: 5000,
      panelClass: ['toast-error']
    });
  }

  showInfo(message: string) {
    this.snackBar.open(message, 'OK', {
      duration: 3000,
      panelClass: ['toast-info']
    });
  }
}
