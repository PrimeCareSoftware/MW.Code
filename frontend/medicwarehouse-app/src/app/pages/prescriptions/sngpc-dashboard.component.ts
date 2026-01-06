import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DigitalPrescriptionService } from '../../../services/prescriptions/digital-prescription.service';
import { SNGPCReport, SNGPCReportStatus } from '../../../models/prescriptions/digital-prescription.model';

/**
 * Dashboard for SNGPC (Sistema Nacional de Gerenciamento de Produtos Controlados) reporting
 * Shows report status, deadlines, and allows report generation and transmission
 */
@Component({
  selector: 'app-sngpc-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="sngpc-dashboard">
      <h1>Dashboard SNGPC</h1>
      <p class="subtitle">Sistema Nacional de Gerenciamento de Produtos Controlados - ANVISA</p>

      <!-- Quick Stats -->
      <div class="stats-grid">
        <mat-card class="stat-card">
          <mat-card-content>
            <mat-icon class="stat-icon" color="primary">assignment</mat-icon>
            <div class="stat-value">{{ unreportedCount }}</div>
            <div class="stat-label">Prescrições Não Reportadas</div>
          </mat-card-content>
        </mat-card>

        <mat-card class="stat-card">
          <mat-card-content>
            <mat-icon class="stat-icon" color="accent">schedule</mat-icon>
            <div class="stat-value">{{ overdueReports.length }}</div>
            <div class="stat-label">Relatórios Atrasados</div>
          </mat-card-content>
        </mat-card>

        <mat-card class="stat-card">
          <mat-card-content>
            <mat-icon class="stat-icon" color="primary">check_circle</mat-icon>
            <div class="stat-value">{{ transmittedThisYear }}</div>
            <div class="stat-label">Transmitidos Este Ano</div>
          </mat-card-content>
        </mat-card>

        <mat-card class="stat-card" *ngIf="latestReport">
          <mat-card-content>
            <mat-icon class="stat-icon" [color]="latestReport.isOverdue ? 'warn' : 'primary'">timer</mat-icon>
            <div class="stat-value">{{ latestReport.daysUntilDeadline }}</div>
            <div class="stat-label">Dias até Prazo (Último)</div>
          </mat-card-content>
        </mat-card>
      </div>

      <!-- Overdue Reports Alert -->
      <mat-card class="alert-card" *ngIf="overdueReports.length > 0">
        <mat-card-content>
          <div class="alert-content">
            <mat-icon class="alert-icon">warning</mat-icon>
            <div>
              <h3>Atenção: Relatórios Vencidos!</h3>
              <p>Você tem {{ overdueReports.length }} relatório(s) SNGPC com prazo vencido. 
                 Transmita imediatamente para evitar multas da ANVISA.</p>
            </div>
          </div>
        </mat-card-content>
      </mat-card>

      <!-- Reports Table -->
      <mat-card>
        <mat-card-header>
          <mat-card-title>Histórico de Relatórios SNGPC</mat-card-title>
          <button mat-raised-button color="primary" (click)="createNewReport()">
            <mat-icon>add</mat-icon>
            Gerar Novo Relatório
          </button>
        </mat-card-header>

        <mat-card-content>
          <table mat-table [dataSource]="reports" class="reports-table">
            <!-- Period Column -->
            <ng-container matColumnDef="period">
              <th mat-header-cell *matHeaderCellDef>Período</th>
              <td mat-cell *matCellDef="let report">
                {{ report.month | number:'2.0' }}/{{ report.year }}
              </td>
            </ng-container>

            <!-- Status Column -->
            <ng-container matColumnDef="status">
              <th mat-header-cell *matHeaderCellDef>Status</th>
              <td mat-cell *matCellDef="let report">
                <mat-chip [class]="'status-' + report.status.toLowerCase()">
                  {{ getStatusLabel(report.status) }}
                </mat-chip>
              </td>
            </ng-container>

            <!-- Prescriptions Column -->
            <ng-container matColumnDef="prescriptions">
              <th mat-header-cell *matHeaderCellDef>Prescrições</th>
              <td mat-cell *matCellDef="let report">{{ report.totalPrescriptions }}</td>
            </ng-container>

            <!-- Items Column -->
            <ng-container matColumnDef="items">
              <th mat-header-cell *matHeaderCellDef>Itens</th>
              <td mat-cell *matCellDef="let report">{{ report.totalItems }}</td>
            </ng-container>

            <!-- Deadline Column -->
            <ng-container matColumnDef="deadline">
              <th mat-header-cell *matHeaderCellDef>Prazo</th>
              <td mat-cell *matCellDef="let report">
                <span [class.overdue]="report.isOverdue">
                  {{ report.daysUntilDeadline }} dias
                </span>
              </td>
            </ng-container>

            <!-- Actions Column -->
            <ng-container matColumnDef="actions">
              <th mat-header-cell *matHeaderCellDef>Ações</th>
              <td mat-cell *matCellDef="let report">
                <button mat-icon-button [matMenuTriggerFor]="menu">
                  <mat-icon>more_vert</mat-icon>
                </button>
                <mat-menu #menu="matMenu">
                  <button mat-menu-item (click)="generateXML(report.id)">
                    <mat-icon>code</mat-icon>
                    <span>Gerar XML</span>
                  </button>
                  <button mat-menu-item (click)="downloadXML(report.id)">
                    <mat-icon>download</mat-icon>
                    <span>Baixar XML</span>
                  </button>
                  <button mat-menu-item (click)="transmitReport(report.id)">
                    <mat-icon>send</mat-icon>
                    <span>Transmitir</span>
                  </button>
                </mat-menu>
              </td>
            </ng-container>

            <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
            <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
          </table>
        </mat-card-content>
      </mat-card>

      <!-- Info Panel -->
      <mat-card class="info-card">
        <mat-card-content>
          <h3><mat-icon>info</mat-icon> Informações Importantes</h3>
          <ul>
            <li>Relatórios devem ser transmitidos até o dia 10 do mês seguinte</li>
            <li>Inclui todas as prescrições de medicamentos controlados (Listas A, B e C1)</li>
            <li>Multas por atraso: de R$ 5.000 a R$ 100.000 (Lei 6.360/76)</li>
            <li>Formato XML deve seguir o schema ANVISA v2.1</li>
            <li>Assinatura digital ICP-Brasil é obrigatória</li>
          </ul>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .sngpc-dashboard {
      padding: 20px;
      max-width: 1400px;
      margin: 0 auto;
    }

    h1 {
      color: #333;
      margin-bottom: 8px;
    }

    .subtitle {
      color: #666;
      font-size: 14px;
      margin-bottom: 24px;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
      gap: 16px;
      margin-bottom: 24px;
    }

    .stat-card {
      text-align: center;
    }

    .stat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      margin-bottom: 12px;
    }

    .stat-value {
      font-size: 32px;
      font-weight: bold;
      color: #333;
      margin-bottom: 8px;
    }

    .stat-label {
      font-size: 14px;
      color: #666;
    }

    .alert-card {
      background-color: #fff3cd;
      border-left: 4px solid #ff9800;
      margin-bottom: 24px;
    }

    .alert-content {
      display: flex;
      align-items: flex-start;
      gap: 16px;
    }

    .alert-icon {
      color: #ff9800;
      font-size: 32px;
      width: 32px;
      height: 32px;
    }

    .alert-content h3 {
      margin: 0 0 8px 0;
      color: #856404;
    }

    .alert-content p {
      margin: 0;
      color: #856404;
    }

    mat-card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }

    .reports-table {
      width: 100%;
    }

    .status-draft { background-color: #e0e0e0; }
    .status-generated { background-color: #bbdefb; }
    .status-transmitted { background-color: #c8e6c9; }
    .status-transmissionfailed { background-color: #ffcdd2; }
    .status-validated { background-color: #a5d6a7; }

    .overdue {
      color: #f44336;
      font-weight: bold;
    }

    .info-card {
      margin-top: 24px;
      background-color: #e3f2fd;
    }

    .info-card h3 {
      display: flex;
      align-items: center;
      gap: 8px;
      color: #1976d2;
      margin-bottom: 16px;
    }

    .info-card ul {
      margin: 0;
      padding-left: 20px;
      color: #1565c0;
    }

    .info-card li {
      margin-bottom: 8px;
    }
  `]
})
export class SNGPCDashboardComponent implements OnInit {
  private prescriptionService = inject(DigitalPrescriptionService);

  reports: SNGPCReport[] = [];
  overdueReports: SNGPCReport[] = [];
  latestReport: SNGPCReport | null = null;
  unreportedCount = 0;
  transmittedThisYear = 0;

  displayedColumns = ['period', 'status', 'prescriptions', 'items', 'deadline', 'actions'];

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    const currentYear = new Date().getFullYear();

    // Load report history
    this.prescriptionService.getSNGPCReportHistory(12).subscribe(reports => {
      this.reports = reports;
      this.transmittedThisYear = reports.filter(r => 
        r.status === 'Transmitted' && r.year === currentYear
      ).length;
    });

    // Load overdue reports
    this.prescriptionService.getOverdueSNGPCReports().subscribe(reports => {
      this.overdueReports = reports;
    });

    // Load latest report
    this.prescriptionService.getLatestSNGPCReport().subscribe(report => {
      this.latestReport = report;
    });

    // Load unreported prescriptions count
    this.prescriptionService.getUnreportedToSNGPC().subscribe(prescriptions => {
      this.unreportedCount = prescriptions.length;
    });
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      'Draft': 'Rascunho',
      'Generated': 'XML Gerado',
      'Transmitted': 'Transmitido',
      'TransmissionFailed': 'Falha na Transmissão',
      'Validated': 'Validado'
    };
    return labels[status] || status;
  }

  createNewReport(): void {
    const now = new Date();
    const lastMonth = new Date(now.getFullYear(), now.getMonth() - 1, 1);
    
    this.prescriptionService.createSNGPCReport({
      month: lastMonth.getMonth() + 1,
      year: lastMonth.getFullYear()
    }).subscribe(() => {
      this.loadDashboardData();
    });
  }

  generateXML(reportId: string): void {
    this.prescriptionService.generateSNGPCXML(reportId).subscribe(() => {
      this.loadDashboardData();
    });
  }

  downloadXML(reportId: string): void {
    this.prescriptionService.downloadSNGPCXML(reportId).subscribe(blob => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `SNGPC_${reportId}.xml`;
      a.click();
      window.URL.revokeObjectURL(url);
    });
  }

  transmitReport(reportId: string): void {
    const protocol = `ANVISA-${Date.now()}`;
    this.prescriptionService.markSNGPCAsTransmitted(reportId, protocol).subscribe(() => {
      this.loadDashboardData();
    });
  }
}
