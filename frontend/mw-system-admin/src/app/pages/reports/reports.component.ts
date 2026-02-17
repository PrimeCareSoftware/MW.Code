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
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
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
