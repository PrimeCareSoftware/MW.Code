import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { TissAnalyticsService } from '../../../services/tiss-analytics.service';
import { Auth } from '../../../services/auth';
import {
  GlosasSummary,
  GlosasByOperator,
  GlosasTrend,
  ProcedureGlosas,
  GlosaAlert
} from '../../../models/tiss.model';

@Component({
  selector: 'app-glosas-dashboard',
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './glosas-dashboard.html',
  styleUrl: './glosas-dashboard.scss'
})
export class GlosasDashboard implements OnInit {
  // Data signals
  summary = signal<GlosasSummary | null>(null);
  operatorData = signal<GlosasByOperator[]>([]);
  trendData = signal<GlosasTrend[]>([]);
  procedureData = signal<ProcedureGlosas[]>([]);
  alerts = signal<GlosaAlert[]>([]);

  // UI state signals
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  // Filter signals
  clinicId = signal<string>('');
  startDate = signal<string>('');
  endDate = signal<string>('');
  trendMonths = signal<number>(6);

  // Computed values
  hasData = computed(() => this.summary() !== null);
  highAlerts = computed(() => this.alerts().filter(a => a.severity === 'high'));
  mediumAlerts = computed(() => this.alerts().filter(a => a.severity === 'medium'));

  constructor(
    private analyticsService: TissAnalyticsService,
    private authService: Auth
  ) {
    // Set default date range (last 30 days)
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 30);
    
    this.endDate.set(this.formatDate(endDate));
    this.startDate.set(this.formatDate(startDate));
  }

  ngOnInit(): void {
    this.loadClinicId();
  }

  private loadClinicId(): void {
    const currentUser = this.authService.currentUser();
    if (currentUser?.clinicId) {
      this.clinicId.set(currentUser.clinicId);
      this.loadAllData();
    } else {
      this.errorMessage.set('Clínica não identificada. Por favor, faça login novamente.');
    }
  }

  loadAllData(): void {
    if (!this.clinicId() || !this.startDate() || !this.endDate()) {
      this.errorMessage.set('Por favor, preencha todos os filtros.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    // Load summary
    this.analyticsService.getGlosasSummary(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => this.summary.set(data),
      error: (error) => {
        console.error('Error loading summary:', error);
        this.errorMessage.set('Erro ao carregar resumo de glosas');
      }
    });

    // Load operator data
    this.analyticsService.getGlosasByOperator(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => this.operatorData.set(data),
      error: (error) => console.error('Error loading operator data:', error)
    });

    // Load trend data
    this.analyticsService.getGlosasTrend(
      this.clinicId(),
      this.trendMonths()
    ).subscribe({
      next: (data) => this.trendData.set(data),
      error: (error) => console.error('Error loading trend data:', error)
    });

    // Load procedure data
    this.analyticsService.getProcedureGlosas(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => this.procedureData.set(data),
      error: (error) => console.error('Error loading procedure data:', error)
    });

    // Load alerts
    this.analyticsService.getGlosaAlerts(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => {
        this.alerts.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading alerts:', error);
        this.isLoading.set(false);
      }
    });
  }

  onDateRangeChange(): void {
    this.loadAllData();
  }

  onTrendMonthsChange(): void {
    if (!this.clinicId()) return;

    this.analyticsService.getGlosasTrend(
      this.clinicId(),
      this.trendMonths()
    ).subscribe({
      next: (data) => this.trendData.set(data),
      error: (error) => console.error('Error loading trend data:', error)
    });
  }

  getSeverityClass(severity: string): string {
    switch (severity) {
      case 'high':
        return 'alert-danger';
      case 'medium':
        return 'alert-warning';
      case 'low':
        return 'alert-info';
      default:
        return 'alert-secondary';
    }
  }

  getSeverityIcon(severity: string): string {
    switch (severity) {
      case 'high':
        return '🔴';
      case 'medium':
        return '🟡';
      case 'low':
        return '🔵';
      default:
        return 'ℹ️';
    }
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatPercentage(value: number): string {
    return `${value.toFixed(2)}%`;
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  exportToPDF(): void {
    // TODO: Implement PDF export using jsPDF or similar library
    alert('Funcionalidade de exportação em PDF será implementada em breve.');
  }
}
