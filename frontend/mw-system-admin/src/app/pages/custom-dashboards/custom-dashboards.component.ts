import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DashboardService } from '../../services/dashboard.service';
import { CustomDashboard } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-custom-dashboards',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    Navbar
  ],
  template: `
    <app-navbar></app-navbar>
    
    <div class="page-container">
      <div class="page-header">
        <div>
          <h1>Dashboards Customizados</h1>
          <p class="subtitle">Crie e gerencie seus dashboards personalizados</p>
        </div>
        <button mat-raised-button color="primary" (click)="createDashboard()">
          <mat-icon>add</mat-icon>
          Novo Dashboard
        </button>
      </div>

      @if (loading()) {
        <div class="loading-container">
          <mat-spinner></mat-spinner>
        </div>
      } @else if (error()) {
        <div class="error-container">
          <mat-icon color="warn">error</mat-icon>
          <p>{{ error() }}</p>
        </div>
      } @else {
        <div class="dashboards-grid">
          @for (dashboard of dashboards(); track dashboard.id) {
            <mat-card class="dashboard-card" (click)="openDashboard(dashboard.id)">
              <mat-card-header>
                <mat-card-title>{{ dashboard.name }}</mat-card-title>
                <button mat-icon-button [matMenuTriggerFor]="menu" (click)="$event.stopPropagation()">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="editDashboard(dashboard.id)">
                    <mat-icon>edit</mat-icon>
                    <span>Editar</span>
                  </button>
                  <button mat-menu-item (click)="duplicateDashboard(dashboard.id)">
                    <mat-icon>content_copy</mat-icon>
                    <span>Duplicar</span>
                  </button>
                  <button mat-menu-item (click)="exportDashboard(dashboard.id)">
                    <mat-icon>download</mat-icon>
                    <span>Exportar</span>
                  </button>
                  <button mat-menu-item (click)="deleteDashboard(dashboard.id)" class="delete-button">
                    <mat-icon>delete</mat-icon>
                    <span>Excluir</span>
                  </button>
                </mat-menu>
              </mat-card-header>
              <mat-card-content>
                <p class="description">{{ dashboard.description }}</p>
                <div class="dashboard-meta">
                  <span class="meta-item">
                    <mat-icon>widgets</mat-icon>
                    {{ dashboard.widgets.length }} widgets
                  </span>
                  @if (dashboard.isDefault) {
                    <span class="badge default">Padrão</span>
                  }
                  @if (dashboard.isPublic) {
                    <span class="badge public">Público</span>
                  }
                </div>
              </mat-card-content>
            </mat-card>
          }
        </div>

        @if (dashboards().length === 0) {
          <div class="empty-state">
            <mat-icon>dashboard</mat-icon>
            <h2>Nenhum dashboard criado</h2>
            <p>Crie seu primeiro dashboard customizado para visualizar métricas importantes</p>
            <button mat-raised-button color="primary" (click)="createDashboard()">
              <mat-icon>add</mat-icon>
              Criar Dashboard
            </button>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .page-container {
      padding: 24px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 32px;
    }

    .page-header h1 {
      margin: 0 0 8px 0;
      font-size: 32px;
      font-weight: 600;
    }

    .subtitle {
      color: #666;
      margin: 0;
    }

    .loading-container,
    .error-container {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 400px;
      flex-direction: column;
      gap: 16px;
    }

    .dashboards-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(320px, 1fr));
      gap: 24px;
    }

    .dashboard-card {
      cursor: pointer;
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .dashboard-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
    }

    mat-card-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
    }

    .description {
      color: #666;
      margin: 16px 0;
      min-height: 40px;
    }

    .dashboard-meta {
      display: flex;
      align-items: center;
      gap: 12px;
      font-size: 14px;
      color: #999;
    }

    .meta-item {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .meta-item mat-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
    }

    .badge {
      padding: 4px 8px;
      border-radius: 4px;
      font-size: 12px;
      font-weight: 500;
    }

    .badge.default {
      background: #e3f2fd;
      color: #1976d2;
    }

    .badge.public {
      background: #f3e5f5;
      color: #7b1fa2;
    }

    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 80px 24px;
      text-align: center;
    }

    .empty-state mat-icon {
      font-size: 80px;
      width: 80px;
      height: 80px;
      color: #ccc;
      margin-bottom: 16px;
    }

    .empty-state h2 {
      margin: 0 0 8px 0;
      font-size: 24px;
      font-weight: 600;
    }

    .empty-state p {
      color: #666;
      margin: 0 0 24px 0;
    }

    .delete-button {
      color: #ef4444;
    }
  `]
})
export class CustomDashboardsComponent implements OnInit {
  dashboards = signal<CustomDashboard[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private dashboardService: DashboardService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadDashboards();
  }

  loadDashboards(): void {
    this.loading.set(true);
    this.error.set(null);

    this.dashboardService.getAllDashboards().subscribe({
      next: (data) => {
        this.dashboards.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar dashboards');
        this.loading.set(false);
      }
    });
  }

  createDashboard(): void {
    this.router.navigate(['/custom-dashboards', 'new', 'edit']);
  }

  openDashboard(id: number): void {
    this.router.navigate(['/custom-dashboards', id]);
  }

  editDashboard(id: number): void {
    this.router.navigate(['/custom-dashboards', id, 'edit']);
  }

  duplicateDashboard(id: number): void {
    const dashboard = this.dashboards().find(d => d.id === id);
    if (dashboard) {
      this.dashboardService.createDashboard({
        name: `${dashboard.name} (Cópia)`,
        description: dashboard.description,
        layout: dashboard.layout
      }).subscribe({
        next: () => this.loadDashboards(),
        error: (err) => console.error('Erro ao duplicar dashboard:', err)
      });
    }
  }

  exportDashboard(id: number): void {
    this.dashboardService.exportDashboard(id, 'pdf').subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `dashboard-${id}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Erro ao exportar dashboard:', err)
    });
  }

  deleteDashboard(id: number): void {
    if (confirm('Tem certeza que deseja excluir este dashboard?')) {
      this.dashboardService.deleteDashboard(id).subscribe({
        next: () => this.loadDashboards(),
        error: (err) => console.error('Erro ao excluir dashboard:', err)
      });
    }
  }
}
