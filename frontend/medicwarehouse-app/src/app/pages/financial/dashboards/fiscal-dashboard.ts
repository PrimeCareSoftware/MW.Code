import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { Auth } from '../../../services/auth';
import { environment } from '../../../../environments/environment';
import { MOCK_FISCAL_DASHBOARD_DATA } from '../../../mocks/fiscal-dashboard.mock';

// TODO: Create ElectronicInvoiceService similar to TissAnalyticsService
// TODO: Backend analytics endpoints need to be created similar to TissAnalyticsController pattern
// Example endpoint structure needed:
// - GET /api/ElectronicInvoice/analytics/monthly-summary?clinicId=X&month=Y&year=Z
// - GET /api/ElectronicInvoice/analytics/tax-breakdown?clinicId=X&month=Y&year=Z
// - GET /api/ElectronicInvoice/analytics/status-summary?clinicId=X&month=Y&year=Z
// - GET /api/ElectronicInvoice/analytics/top-clients?clinicId=X&month=Y&year=Z&limit=5
// - GET /api/ElectronicInvoice/analytics/monthly-trend?clinicId=X&months=12

interface InvoiceTypeSummary {
  type: string; // NFSe, NFe, NFCe
  quantity: number;
  totalValue: number;
}

interface TaxBreakdown {
  taxType: string; // ISS, PIS, COFINS, CSLL, INSS, IR
  amount: number;
  percentage: number;
}

interface InvoiceStatusSummary {
  authorized: number;
  cancelled: number;
  error: number;
  pending: number;
}

interface TopClient {
  clientId: string;
  clientName: string;
  totalBilled: number;
  invoiceCount: number;
}

interface MonthlyTrend {
  month: string;
  year: number;
  totalIssued: number;
  invoiceCount: number;
}

interface Alert {
  id: string;
  type: 'warning' | 'error' | 'info';
  message: string;
  date: Date;
}

@Component({
  selector: 'app-fiscal-dashboard',
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './fiscal-dashboard.html',
  styleUrl: './fiscal-dashboard.scss'
})
export class FiscalDashboard implements OnInit {
  // Data signals
  totalIssuedMonth = signal<number>(0);
  totalTaxesPaid = signal<number>(0);
  invoicesByType = signal<InvoiceTypeSummary[]>([]);
  taxBreakdown = signal<TaxBreakdown[]>([]);
  invoiceStatus = signal<InvoiceStatusSummary>({
    authorized: 0,
    cancelled: 0,
    error: 0,
    pending: 0
  });
  topClients = signal<TopClient[]>([]);
  monthlyTrend = signal<MonthlyTrend[]>([]);
  alerts = signal<Alert[]>([]);

  // UI state signals
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  // Filter signals
  clinicId = signal<string>('');
  selectedMonth = signal<number>(new Date().getMonth() + 1);
  selectedYear = signal<number>(new Date().getFullYear());
  invoiceTypeFilter = signal<string>('all'); // all, NFSe, NFe, NFCe
  trendMonths = signal<number>(12);

  // Computed values
  hasData = computed(() => 
    this.totalIssuedMonth() > 0 || 
    this.invoicesByType().length > 0 ||
    this.topClients().length > 0
  );

  totalInvoices = computed(() => {
    return this.invoicesByType().reduce((sum, item) => sum + item.quantity, 0);
  });

  averageInvoiceValue = computed(() => {
    const total = this.totalIssuedMonth();
    const count = this.totalInvoices();
    return count > 0 ? total / count : 0;
  });

  taxEffectiveRate = computed(() => {
    const total = this.totalIssuedMonth();
    const taxes = this.totalTaxesPaid();
    return total > 0 ? (taxes / total) * 100 : 0;
  });

  constructor(
    private authService: Auth
    // TODO: Inject ElectronicInvoiceService when created
    // private electronicInvoiceService: ElectronicInvoiceService
  ) {}

  ngOnInit(): void {
    this.loadClinicId();
    if (environment.useMockData) {
      this.loadMockData();
    }
    // TODO: Replace with actual API calls when backend is ready
  }

  private loadClinicId(): void {
    const currentUser = this.authService.currentUser();
    if (currentUser?.clinicId) {
      this.clinicId.set(currentUser.clinicId);
    } else {
      this.errorMessage.set('Clínica não identificada. Por favor, faça login novamente.');
    }
  }

  loadAllData(): void {
    if (!this.clinicId() || !this.selectedMonth() || !this.selectedYear()) {
      this.errorMessage.set('Por favor, preencha todos os filtros.');
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set('');

    // TODO: Replace with actual API calls
    // this.loadMonthlySummary();
    // this.loadTaxBreakdown();
    // this.loadStatusSummary();
    // this.loadTopClients();
    // this.loadMonthlyTrend();
    // this.loadAlerts();
    
    // For now, use mock data only if enabled
    if (environment.useMockData) {
      setTimeout(() => {
        this.loadMockData();
        this.isLoading.set(false);
      }, 500);
    } else {
      this.isLoading.set(false);
      this.errorMessage.set('Funcionalidade em desenvolvimento. Por favor, habilite o modo de dados mock ou aguarde a implementação do backend.');
    }
  }

  private loadMockData(): void {
    // Use centralized mock data
    const mockData = MOCK_FISCAL_DASHBOARD_DATA;
    
    this.totalIssuedMonth.set(mockData.totalIssuedMonth);
    this.totalTaxesPaid.set(mockData.totalTaxesPaid);
    this.invoicesByType.set(mockData.invoicesByType);
    this.taxBreakdown.set(mockData.taxBreakdown);
    this.invoiceStatus.set(mockData.invoiceStatus);
    this.topClients.set(mockData.topClients);
    this.monthlyTrend.set(mockData.monthlyTrend);
    this.alerts.set(mockData.alerts);
  }

  onFilterChange(): void {
    this.loadAllData();
  }

  onMonthChange(): void {
    this.loadAllData();
  }

  onYearChange(): void {
    this.loadAllData();
  }

  onInvoiceTypeFilterChange(): void {
    // Filter invoices by type
    // TODO: Implement actual filtering with API call
    console.log('Filter changed to:', this.invoiceTypeFilter());
  }

  onTrendMonthsChange(): void {
    // TODO: Load monthly trend data with new months parameter
    console.log('Trend months changed to:', this.trendMonths());
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'authorized':
        return 'status-success';
      case 'cancelled':
        return 'status-warning';
      case 'error':
        return 'status-danger';
      case 'pending':
        return 'status-info';
      default:
        return '';
    }
  }

  getAlertClass(type: 'warning' | 'error' | 'info'): string {
    return `alert-${type}`;
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

  formatNumber(value: number): string {
    return new Intl.NumberFormat('pt-BR').format(value);
  }

  formatDate(date: Date): string {
    return date.toLocaleDateString('pt-BR');
  }

  getMonthName(month: number): string {
    const months = [
      'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
      'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
    ];
    return months[month - 1] || '';
  }

  generateYears(): number[] {
    const currentYear = new Date().getFullYear();
    const years: number[] = [];
    for (let i = currentYear; i >= currentYear - 5; i--) {
      years.push(i);
    }
    return years;
  }

  exportToPDF(): void {
    // TODO: Implement PDF export
    alert('Funcionalidade de exportação em PDF será implementada em breve.');
  }

  exportToExcel(): void {
    // TODO: Implement Excel export
    alert('Funcionalidade de exportação em Excel será implementada em breve.');
  }

  dismissAlert(alertId: string): void {
    const currentAlerts = this.alerts();
    this.alerts.set(currentAlerts.filter(a => a.id !== alertId));
  }
}
