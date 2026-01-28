import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ReportService } from '../../services/report.service';
import { ReportTemplate, ScheduledReport } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatTabsModule,
    MatTableModule,
    MatProgressSpinnerModule,
    Navbar
  ],
  template: `
    <app-navbar></app-navbar>
    
    <div class="page-container">
      <div class="page-header">
        <div>
          <h1>Relatórios</h1>
          <p class="subtitle">Gere e agende relatórios personalizados</p>
        </div>
        <button mat-raised-button color="primary" (click)="generateReport()">
          <mat-icon>assignment</mat-icon>
          Gerar Relatório
        </button>
      </div>

      <mat-tab-group>
        <mat-tab label="Templates">
          @if (loadingTemplates()) {
            <div class="loading-container">
              <mat-spinner></mat-spinner>
            </div>
          } @else {
            <div class="templates-grid">
              @for (template of templates(); track template.id) {
                <mat-card class="template-card" (click)="selectTemplate(template)">
                  <mat-card-header>
                    <mat-card-title>{{ template.name }}</mat-card-title>
                  </mat-card-header>
                  <mat-card-content>
                    <p>{{ template.description }}</p>
                    <div class="template-meta">
                      <span class="category-badge" [class]="template.category">
                        {{ getCategoryLabel(template.category) }}
                      </span>
                    </div>
                  </mat-card-content>
                </mat-card>
              }
            </div>
          }
        </mat-tab>

        <mat-tab label="Agendados">
          @if (loadingScheduled()) {
            <div class="loading-container">
              <mat-spinner></mat-spinner>
            </div>
          } @else {
            <div class="table-container">
              <table mat-table [dataSource]="scheduledReports()">
                <ng-container matColumnDef="name">
                  <th mat-header-cell *matHeaderCellDef>Nome</th>
                  <td mat-cell *matCellDef="let report">{{ report.templateName }}</td>
                </ng-container>

                <ng-container matColumnDef="frequency">
                  <th mat-header-cell *matHeaderCellDef>Frequência</th>
                  <td mat-cell *matCellDef="let report">{{ getFrequencyLabel(report.frequency) }}</td>
                </ng-container>

                <ng-container matColumnDef="recipients">
                  <th mat-header-cell *matHeaderCellDef>Destinatários</th>
                  <td mat-cell *matCellDef="let report">{{ report.recipients.length }}</td>
                </ng-container>

                <ng-container matColumnDef="nextRun">
                  <th mat-header-cell *matHeaderCellDef>Próxima Execução</th>
                  <td mat-cell *matCellDef="let report">{{ report.nextRunAt | date:'short' }}</td>
                </ng-container>

                <ng-container matColumnDef="status">
                  <th mat-header-cell *matHeaderCellDef>Status</th>
                  <td mat-cell *matCellDef="let report">
                    <span class="status-badge" [class.active]="report.isActive">
                      {{ report.isActive ? 'Ativo' : 'Inativo' }}
                    </span>
                  </td>
                </ng-container>

                <ng-container matColumnDef="actions">
                  <th mat-header-cell *matHeaderCellDef>Ações</th>
                  <td mat-cell *matCellDef="let report">
                    <button mat-icon-button (click)="deleteScheduledReport(report.id)">
                      <mat-icon>delete</mat-icon>
                    </button>
                  </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="scheduledColumns"></tr>
                <tr mat-row *matRowDef="let row; columns: scheduledColumns"></tr>
              </table>

              @if (scheduledReports().length === 0) {
                <div class="empty-state">
                  <mat-icon>schedule</mat-icon>
                  <p>Nenhum relatório agendado</p>
                </div>
              }
            </div>
          }
        </mat-tab>
      </mat-tab-group>
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

    .loading-container {
      display: flex;
      justify-content: center;
      padding: 80px 24px;
    }

    .templates-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
      gap: 24px;
      padding: 24px 0;
    }

    .template-card {
      cursor: pointer;
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .template-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
    }

    .template-meta {
      margin-top: 16px;
    }

    .category-badge {
      padding: 4px 12px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
    }

    .category-badge.financial {
      background: #dcfce7;
      color: #166534;
    }

    .category-badge.customer {
      background: #dbeafe;
      color: #1e40af;
    }

    .category-badge.operational {
      background: #fef3c7;
      color: #92400e;
    }

    .table-container {
      padding: 24px 0;
    }

    table {
      width: 100%;
    }

    .status-badge {
      padding: 4px 12px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
      background: #fee2e2;
      color: #991b1b;
    }

    .status-badge.active {
      background: #dcfce7;
      color: #166534;
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
      font-size: 64px;
      width: 64px;
      height: 64px;
      color: #ccc;
      margin-bottom: 16px;
    }
  `]
})
export class ReportsComponent implements OnInit {
  templates = signal<ReportTemplate[]>([]);
  scheduledReports = signal<ScheduledReport[]>([]);
  loadingTemplates = signal(true);
  loadingScheduled = signal(true);

  scheduledColumns = ['name', 'frequency', 'recipients', 'nextRun', 'status', 'actions'];

  constructor(
    private reportService: ReportService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTemplates();
    this.loadScheduledReports();
  }

  loadTemplates(): void {
    this.reportService.getAvailableReports().subscribe({
      next: (data) => {
        this.templates.set(data);
        this.loadingTemplates.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar templates:', err);
        this.loadingTemplates.set(false);
      }
    });
  }

  loadScheduledReports(): void {
    this.reportService.getScheduledReports().subscribe({
      next: (data) => {
        this.scheduledReports.set(data);
        this.loadingScheduled.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar relatórios agendados:', err);
        this.loadingScheduled.set(false);
      }
    });
  }

  generateReport(): void {
    this.router.navigate(['/reports', 'wizard']);
  }

  selectTemplate(template: ReportTemplate): void {
    this.router.navigate(['/reports', 'wizard'], {
      queryParams: { templateId: template.id }
    });
  }

  deleteScheduledReport(id: number): void {
    if (confirm('Tem certeza que deseja excluir este agendamento?')) {
      this.reportService.deleteScheduledReport(id).subscribe({
        next: () => this.loadScheduledReports(),
        error: (err) => console.error('Erro ao excluir agendamento:', err)
      });
    }
  }

  getCategoryLabel(category: string): string {
    const labels: any = {
      financial: 'Financeiro',
      customer: 'Cliente',
      operational: 'Operacional'
    };
    return labels[category] || category;
  }

  getFrequencyLabel(frequency: string): string {
    const labels: any = {
      daily: 'Diário',
      weekly: 'Semanal',
      monthly: 'Mensal'
    };
    return labels[frequency] || frequency;
  }
}
