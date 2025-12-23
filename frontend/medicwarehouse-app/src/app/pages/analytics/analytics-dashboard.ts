import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgApexchartsModule } from 'ng-apexcharts';
import { 
  ApexAxisChartSeries,
  ApexChart,
  ApexDataLabels,
  ApexPlotOptions,
  ApexYAxis,
  ApexLegend,
  ApexStroke,
  ApexXAxis,
  ApexFill,
  ApexTooltip,
  ApexNonAxisChartSeries,
  ApexResponsive,
  ChartComponent
} from 'ng-apexcharts';
import { format, subDays, subMonths, startOfMonth, endOfMonth } from 'date-fns';
import { Navbar } from '../../shared/navbar/navbar';
import { Loading } from '../../shared/loading/loading';
import { Auth } from '../../services/auth';
import { Analytics, FinancialSummary, RevenueReport, AppointmentsReport, PatientsReport } from '../../services/analytics/analytics';

export type ChartOptions = {
  series: ApexAxisChartSeries | ApexNonAxisChartSeries;
  chart: ApexChart;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  yaxis: ApexYAxis;
  xaxis: ApexXAxis;
  fill: ApexFill;
  tooltip: ApexTooltip;
  stroke: ApexStroke;
  legend: ApexLegend;
  colors: string[];
  labels: string[];
  responsive: ApexResponsive[];
};

@Component({
  selector: 'app-analytics-dashboard',
  imports: [CommonModule, FormsModule, Navbar, Loading, NgApexchartsModule],
  templateUrl: './analytics-dashboard.html',
  styleUrl: './analytics-dashboard.scss'
})
export class AnalyticsDashboard implements OnInit {
  @ViewChild('revenueChart') revenueChart!: ChartComponent;
  @ViewChild('expensesChart') expensesChart!: ChartComponent;
  @ViewChild('appointmentsChart') appointmentsChart!: ChartComponent;
  @ViewChild('patientsChart') patientsChart!: ChartComponent;

  // Date range
  dateRange: string = 'last30days';
  customStartDate: string = '';
  customEndDate: string = '';

  // Loading states
  loading = true;
  
  // Data
  financialSummary?: FinancialSummary;
  revenueReport?: RevenueReport;
  appointmentsReport?: AppointmentsReport;
  patientsReport?: PatientsReport;

  // Chart options
  revenueChartOptions?: Partial<ChartOptions>;
  expensesChartOptions?: Partial<ChartOptions>;
  appointmentsStatusChartOptions?: Partial<ChartOptions>;
  appointmentsTypeChartOptions?: Partial<ChartOptions>;
  patientsGrowthChartOptions?: Partial<ChartOptions>;
  revenueByMethodChartOptions?: Partial<ChartOptions>;

  constructor(
    public authService: Auth,
    private analyticsService: Analytics
  ) {}

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loading = true;
    const { startDate, endDate } = this.getDateRange();
    const clinicId = this.authService.getClinicId();
    
    if (!clinicId) {
      console.error('No clinic ID available');
      this.loading = false;
      return;
    }

    Promise.all([
      import('rxjs').then(rxjs => rxjs.firstValueFrom(this.analyticsService.getFinancialSummary(clinicId, startDate, endDate))),
      import('rxjs').then(rxjs => rxjs.firstValueFrom(this.analyticsService.getRevenueReport(clinicId, startDate, endDate))),
      import('rxjs').then(rxjs => rxjs.firstValueFrom(this.analyticsService.getAppointmentsReport(clinicId, startDate, endDate))),
      import('rxjs').then(rxjs => rxjs.firstValueFrom(this.analyticsService.getPatientsReport(clinicId, startDate, endDate)))
    ]).then(([financial, revenue, appointments, patients]) => {
      this.financialSummary = financial;
      this.revenueReport = revenue;
      this.appointmentsReport = appointments;
      this.patientsReport = patients;
      
      this.initializeCharts();
      this.loading = false;
    }).catch(error => {
      console.error('Error loading analytics data:', error);
      this.loading = false;
    });
  }

  getDateRange(): { startDate: string; endDate: string } {
    const today = new Date();
    let startDate: Date;
    let endDate: Date = today;

    switch (this.dateRange) {
      case 'today':
        startDate = today;
        break;
      case 'last7days':
        startDate = subDays(today, 7);
        break;
      case 'last30days':
        startDate = subDays(today, 30);
        break;
      case 'last3months':
        startDate = subMonths(today, 3);
        break;
      case 'last6months':
        startDate = subMonths(today, 6);
        break;
      case 'thisMonth':
        startDate = startOfMonth(today);
        endDate = endOfMonth(today);
        break;
      case 'lastMonth':
        const lastMonth = subMonths(today, 1);
        startDate = startOfMonth(lastMonth);
        endDate = endOfMonth(lastMonth);
        break;
      case 'custom':
        startDate = new Date(this.customStartDate);
        endDate = new Date(this.customEndDate);
        break;
      default:
        startDate = subDays(today, 30);
    }

    return {
      startDate: format(startDate, 'yyyy-MM-dd'),
      endDate: format(endDate, 'yyyy-MM-dd')
    };
  }

  onDateRangeChange() {
    this.loadData();
  }

  initializeCharts() {
    this.initRevenueChart();
    this.initExpensesChart();
    this.initAppointmentsCharts();
    this.initPatientsChart();
    this.initRevenueByMethodChart();
  }

  initRevenueChart() {
    if (!this.revenueReport) return;

    const categories = this.revenueReport.dailyBreakdown.map(d => 
      format(new Date(d.date), 'dd/MM')
    );
    const data = this.revenueReport.dailyBreakdown.map(d => d.revenue);

    this.revenueChartOptions = {
      series: [{
        name: 'Receita',
        data: data
      }],
      chart: {
        type: 'area',
        height: 350,
        toolbar: {
          show: true
        },
        zoom: {
          enabled: true
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        curve: 'smooth',
        width: 2
      },
      xaxis: {
        categories: categories,
        labels: {
          rotate: -45
        }
      },
      yaxis: {
        labels: {
          formatter: (value) => 'R$ ' + value.toFixed(2)
        }
      },
      fill: {
        type: 'gradient',
        gradient: {
          shadeIntensity: 1,
          opacityFrom: 0.7,
          opacityTo: 0.3,
          stops: [0, 90, 100]
        }
      },
      colors: ['#0ea5e9'],
      tooltip: {
        y: {
          formatter: (value) => 'R$ ' + value.toFixed(2)
        }
      }
    };
  }

  initExpensesChart() {
    if (!this.financialSummary) return;

    const categories = this.financialSummary.expensesByCategory.map(e => e.category);
    const data = this.financialSummary.expensesByCategory.map(e => e.amount);

    this.expensesChartOptions = {
      series: data,
      chart: {
        type: 'donut',
        height: 350
      },
      labels: categories,
      colors: ['#0ea5e9', '#8b5cf6', '#ec4899', '#f59e0b', '#10b981', '#6366f1'],
      dataLabels: {
        enabled: true,
        formatter: (val: number) => val.toFixed(1) + '%'
      },
      legend: {
        position: 'bottom'
      },
      tooltip: {
        y: {
          formatter: (value) => 'R$ ' + value.toFixed(2)
        }
      },
      responsive: [{
        breakpoint: 480,
        options: {
          chart: {
            width: 300
          },
          legend: {
            position: 'bottom'
          }
        }
      }]
    };
  }

  initAppointmentsCharts() {
    if (!this.appointmentsReport) return;

    // Status chart
    const statusLabels = this.appointmentsReport.appointmentsByStatus.map(a => this.translateStatus(a.status));
    const statusData = this.appointmentsReport.appointmentsByStatus.map(a => a.count);

    this.appointmentsStatusChartOptions = {
      series: statusData,
      chart: {
        type: 'pie',
        height: 350
      },
      labels: statusLabels,
      colors: ['#22c55e', '#f59e0b', '#ef4444', '#6b7280'],
      legend: {
        position: 'bottom'
      },
      responsive: [{
        breakpoint: 480,
        options: {
          chart: {
            width: 300
          },
          legend: {
            position: 'bottom'
          }
        }
      }]
    };

    // Type chart
    const typeLabels = this.appointmentsReport.appointmentsByType.map(a => this.translateType(a.type));
    const typeData = this.appointmentsReport.appointmentsByType.map(a => a.count);

    this.appointmentsTypeChartOptions = {
      series: [{
        name: 'Consultas',
        data: typeData
      }],
      chart: {
        type: 'bar',
        height: 350,
        toolbar: {
          show: false
        }
      },
      plotOptions: {
        bar: {
          horizontal: true,
          distributed: true
        }
      },
      dataLabels: {
        enabled: true
      },
      xaxis: {
        categories: typeLabels
      },
      colors: ['#0ea5e9', '#8b5cf6', '#ec4899', '#f59e0b']
    };
  }

  initPatientsChart() {
    if (!this.patientsReport) return;

    const categories = this.patientsReport.monthlyBreakdown.map(m => 
      `${this.getMonthName(m.month)}/${m.year}`
    );
    const newPatients = this.patientsReport.monthlyBreakdown.map(m => m.newPatients);
    const totalPatients = this.patientsReport.monthlyBreakdown.map(m => m.totalPatients);

    this.patientsGrowthChartOptions = {
      series: [
        {
          name: 'Novos Pacientes',
          data: newPatients
        },
        {
          name: 'Total de Pacientes',
          data: totalPatients
        }
      ],
      chart: {
        type: 'line',
        height: 350,
        toolbar: {
          show: true
        }
      },
      stroke: {
        curve: 'smooth',
        width: [2, 2]
      },
      xaxis: {
        categories: categories
      },
      yaxis: {
        labels: {
          formatter: (value) => Math.round(value).toString()
        }
      },
      colors: ['#0ea5e9', '#8b5cf6'],
      legend: {
        position: 'top'
      }
    };
  }

  initRevenueByMethodChart() {
    if (!this.financialSummary) return;

    const categories = this.financialSummary.revenueByPaymentMethod.map(r => 
      this.translatePaymentMethod(r.paymentMethod)
    );
    const data = this.financialSummary.revenueByPaymentMethod.map(r => r.amount);

    this.revenueByMethodChartOptions = {
      series: [{
        name: 'Receita',
        data: data
      }],
      chart: {
        type: 'bar',
        height: 350,
        toolbar: {
          show: false
        }
      },
      plotOptions: {
        bar: {
          horizontal: false,
          columnWidth: '55%',
          borderRadius: 8
        }
      },
      dataLabels: {
        enabled: false
      },
      xaxis: {
        categories: categories
      },
      yaxis: {
        labels: {
          formatter: (value) => 'R$ ' + value.toFixed(0)
        }
      },
      fill: {
        opacity: 1
      },
      colors: ['#0ea5e9'],
      tooltip: {
        y: {
          formatter: (value) => 'R$ ' + value.toFixed(2)
        }
      }
    };
  }

  translateStatus(status: string): string {
    const translations: { [key: string]: string } = {
      'Scheduled': 'Agendado',
      'Completed': 'Concluído',
      'Cancelled': 'Cancelado',
      'NoShow': 'Não Compareceu'
    };
    return translations[status] || status;
  }

  translateType(type: string): string {
    const translations: { [key: string]: string } = {
      'Regular': 'Regular',
      'FirstVisit': 'Primeira Consulta',
      'Return': 'Retorno',
      'Emergency': 'Emergência'
    };
    return translations[type] || type;
  }

  translatePaymentMethod(method: string): string {
    const translations: { [key: string]: string } = {
      'Cash': 'Dinheiro',
      'CreditCard': 'Cartão Crédito',
      'DebitCard': 'Cartão Débito',
      'Pix': 'PIX',
      'BankTransfer': 'Transferência',
      'Check': 'Cheque'
    };
    return translations[method] || method;
  }

  getMonthName(month: number): string {
    const months = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];
    return months[month - 1];
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatPercent(value: number): string {
    return value.toFixed(1) + '%';
  }

  exportData() {
    // TODO: Implement export functionality
    console.warn('Export functionality not yet implemented');
  }
}
