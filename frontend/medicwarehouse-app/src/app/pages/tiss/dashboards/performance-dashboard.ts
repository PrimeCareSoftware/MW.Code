import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { TissAnalyticsService } from '../../../services/tiss-analytics.service';
import { Auth } from '../../../services/auth';
import {
  AuthorizationRate,
  ApprovalTime,
  MonthlyPerformance
} from '../../../models/tiss.model';

@Component({
  selector: 'app-performance-dashboard',
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './performance-dashboard.html',
  styleUrl: './performance-dashboard.scss'
})
export class PerformanceDashboard implements OnInit {
  // Data signals
  authorizationData = signal<AuthorizationRate[]>([]);
  approvalTimeData = signal<ApprovalTime[]>([]);
  monthlyPerformance = signal<MonthlyPerformance[]>([]);

  // UI state signals
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  // Filter signals
  clinicId = signal<string>('');
  startDate = signal<string>('');
  endDate = signal<string>('');
  performanceMonths = signal<number>(12);

  // Computed values
  hasData = computed(() => 
    this.authorizationData().length > 0 || 
    this.approvalTimeData().length > 0 ||
    this.monthlyPerformance().length > 0
  );

  averageAuthorizationRate = computed(() => {
    const data = this.authorizationData();
    if (data.length === 0) return 0;
    const sum = data.reduce((acc, item) => acc + item.approvalRate, 0);
    return sum / data.length;
  });

  averageApprovalDays = computed(() => {
    const data = this.approvalTimeData();
    if (data.length === 0) return 0;
    const sum = data.reduce((acc, item) => acc + item.averageApprovalDays, 0);
    return sum / data.length;
  });

  constructor(
    private analyticsService: TissAnalyticsService,
    private authService: Auth
  ) {
    // Set default date range (last 90 days)
    const endDate = new Date();
    const startDate = new Date();
    startDate.setDate(startDate.getDate() - 90);
    
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

    // Load authorization rate
    this.analyticsService.getAuthorizationRate(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => this.authorizationData.set(data),
      error: (error) => {
        console.error('Error loading authorization rate:', error);
        this.errorMessage.set('Erro ao carregar taxa de autorização');
      }
    });

    // Load approval time
    this.analyticsService.getApprovalTime(
      this.clinicId(),
      this.startDate(),
      this.endDate()
    ).subscribe({
      next: (data) => this.approvalTimeData.set(data),
      error: (error) => console.error('Error loading approval time:', error)
    });

    // Load monthly performance
    this.analyticsService.getMonthlyPerformance(
      this.clinicId(),
      this.performanceMonths()
    ).subscribe({
      next: (data) => {
        this.monthlyPerformance.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading monthly performance:', error);
        this.isLoading.set(false);
      }
    });
  }

  onDateRangeChange(): void {
    this.loadAllData();
  }

  onPerformanceMonthsChange(): void {
    if (!this.clinicId()) return;

    this.analyticsService.getMonthlyPerformance(
      this.clinicId(),
      this.performanceMonths()
    ).subscribe({
      next: (data) => this.monthlyPerformance.set(data),
      error: (error) => console.error('Error loading monthly performance:', error)
    });
  }

  getAuthRateClass(rate: number): string {
    if (rate >= 90) return 'text-success';
    if (rate >= 70) return 'text-warning';
    return 'text-danger';
  }

  getApprovalDaysClass(days: number): string {
    if (days <= 5) return 'text-success';
    if (days <= 10) return 'text-warning';
    return 'text-danger';
  }

  formatPercentage(value: number): string {
    return `${value.toFixed(2)}%`;
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatDays(days: number): string {
    return `${days.toFixed(1)} dias`;
  }

  private formatDate(date: Date): string {
    return date.toISOString().split('T')[0];
  }

  exportToPDF(): void {
    // TODO: Implement PDF export
    alert('Funcionalidade de exportação em PDF será implementada em breve.');
  }
}
