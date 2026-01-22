import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Navbar } from '../../../shared/navbar/navbar';
import { AuthService } from '../../../services/auth.service';

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
    private authService: AuthService
    // TODO: Inject ElectronicInvoiceService when created
    // private electronicInvoiceService: ElectronicInvoiceService
  ) {}

  ngOnInit(): void {
    this.loadClinicId();
    this.loadMockData(); // TODO: Replace with actual API calls
  }

  private loadClinicId(): void {
    const currentUser = this.authService.currentUserValue;
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
    
    // For now, use mock data
    setTimeout(() => {
      this.loadMockData();
      this.isLoading.set(false);
    }, 500);
  }

  private loadMockData(): void {
    // Mock data for demonstration
    this.totalIssuedMonth.set(287450.75);
    this.totalTaxesPaid.set(35789.25);

    this.invoicesByType.set([
      { type: 'NFS-e', quantity: 145, totalValue: 198750.50 },
      { type: 'NF-e', quantity: 78, totalValue: 75320.25 },
      { type: 'NFC-e', quantity: 32, totalValue: 13380.00 }
    ]);

    this.taxBreakdown.set([
      { taxType: 'ISS', amount: 14372.54, percentage: 5.0 },
      { taxType: 'PIS', amount: 1873.43, percentage: 0.65 },
      { taxType: 'COFINS', amount: 8623.52, percentage: 3.0 },
      { taxType: 'CSLL', amount: 2587.05, percentage: 0.9 },
      { taxType: 'INSS', amount: 5748.01, percentage: 2.0 },
      { taxType: 'IR', amount: 2584.70, percentage: 0.9 }
    ]);

    this.invoiceStatus.set({
      authorized: 238,
      cancelled: 12,
      error: 5,
      pending: 0
    });

    this.topClients.set([
      { clientId: '1', clientName: 'Hospital Santa Casa', totalBilled: 45780.90, invoiceCount: 23 },
      { clientId: '2', clientName: 'Clínica Médica Centro', totalBilled: 38940.50, invoiceCount: 19 },
      { clientId: '3', clientName: 'Laboratório Analisa', totalBilled: 32150.00, invoiceCount: 15 },
      { clientId: '4', clientName: 'Unimed Regional', totalBilled: 28470.35, invoiceCount: 12 },
      { clientId: '5', clientName: 'Prefeitura Municipal', totalBilled: 24890.75, invoiceCount: 10 }
    ]);

    this.monthlyTrend.set([
      { month: 'Jan', year: 2024, totalIssued: 245870.50, invoiceCount: 231 },
      { month: 'Fev', year: 2024, totalIssued: 268940.75, invoiceCount: 248 },
      { month: 'Mar', year: 2024, totalIssued: 297850.25, invoiceCount: 267 },
      { month: 'Abr', year: 2024, totalIssued: 283470.80, invoiceCount: 255 },
      { month: 'Mai', year: 2024, totalIssued: 287450.75, invoiceCount: 255 }
    ]);

    this.alerts.set([
      {
        id: '1',
        type: 'warning',
        message: 'Certificado digital vence em 45 dias (30/07/2024)',
        date: new Date()
      },
      {
        id: '2',
        type: 'error',
        message: '5 notas fiscais com erro de transmissão - requer atenção',
        date: new Date()
      },
      {
        id: '3',
        type: 'info',
        message: 'Limite mensal de emissões: 75% utilizado',
        date: new Date()
      }
    ]);
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
