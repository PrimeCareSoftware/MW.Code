import { Component, OnInit } from '@angular/core';
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
  ApexResponsive
} from 'ng-apexcharts';
import { format, subDays, subMonths, startOfMonth, endOfMonth } from 'date-fns';
import { Navbar } from '../../../shared/navbar/navbar';
import { Loading } from '../../../shared/loading/loading';
import { AnalyticsBIService } from '../../../services/analytics-bi.service';
import { DashboardFinanceiro, ProjecaoReceita } from '../../../models/analytics-bi.model';

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
  selector: 'app-dashboard-financeiro',
  standalone: true,
  imports: [CommonModule, FormsModule, Navbar, Loading, NgApexchartsModule],
  templateUrl: './dashboard-financeiro.component.html',
  styleUrl: './dashboard-financeiro.component.scss'
})
export class DashboardFinanceiroComponent implements OnInit {
  // Date range
  dateRange: string = 'thisMonth';
  customStartDate: string = '';
  customEndDate: string = '';
  
  // Loading state
  loading = true;
  error: string | null = null;
  
  // Dashboard data
  dashboard?: DashboardFinanceiro;
  projecao?: ProjecaoReceita;
  
  // Chart options
  formaPagamentoChartOptions?: Partial<ChartOptions>;
  convenioChartOptions?: Partial<ChartOptions>;
  fluxoCaixaChartOptions?: Partial<ChartOptions>;
  despesasChartOptions?: Partial<ChartOptions>;

  constructor(
    private analyticsBIService: AnalyticsBIService
  ) {}

  ngOnInit() {
    this.loadDashboard();
    this.loadProjecao();
  }

  loadDashboard() {
    this.loading = true;
    this.error = null;
    
    const { startDate, endDate } = this.getDateRange();
    
    this.analyticsBIService.getDashboardFinanceiro(startDate, endDate).subscribe({
      next: (data) => {
        this.dashboard = data;
        this.initializeCharts();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading financial dashboard:', err);
        this.error = 'Erro ao carregar dashboard financeiro';
        this.loading = false;
      }
    });
  }

  loadProjecao() {
    this.analyticsBIService.getProjecaoReceitaMes().subscribe({
      next: (data) => {
        this.projecao = data;
      },
      error: (err) => {
        console.warn('Could not load revenue projection:', err);
      }
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
        startDate = startOfMonth(today);
        endDate = endOfMonth(today);
    }

    return {
      startDate: format(startDate, 'yyyy-MM-dd'),
      endDate: format(endDate, 'yyyy-MM-dd')
    };
  }

  onFilterChange() {
    this.loadDashboard();
  }

  initializeCharts() {
    if (!this.dashboard) return;
    
    this.initFormaPagamentoChart();
    this.initConvenioChart();
    this.initFluxoCaixaChart();
    this.initDespesasChart();
  }

  initFormaPagamentoChart() {
    if (!this.dashboard || !this.dashboard.receitaPorFormaPagamento.length) return;

    const labels = this.dashboard.receitaPorFormaPagamento.map(r => this.translatePaymentMethod(r.formaPagamento));
    const data = this.dashboard.receitaPorFormaPagamento.map(r => r.total);

    this.formaPagamentoChartOptions = {
      series: data,
      chart: {
        type: 'pie',
        height: 350
      },
      labels: labels,
      colors: ['#0ea5e9', '#8b5cf6', '#ec4899', '#f59e0b', '#10b981', '#6366f1'],
      dataLabels: {
        enabled: true,
        formatter: (val: number) => val.toFixed(1) + '%'
      },
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
  }

  initConvenioChart() {
    if (!this.dashboard || !this.dashboard.receitaPorConvenio.length) return;

    const categories = this.dashboard.receitaPorConvenio.map(r => r.nomeConvenio);
    const data = this.dashboard.receitaPorConvenio.map(r => r.total);

    this.convenioChartOptions = {
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
        categories: categories,
        labels: {
          rotate: -45
        }
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
          formatter: (value) => this.formatCurrency(value)
        }
      }
    };
  }

  initFluxoCaixaChart() {
    if (!this.dashboard || !this.dashboard.fluxoCaixaDiario.length) return;

    const categories = this.dashboard.fluxoCaixaDiario.map(f => format(new Date(f.data), 'dd/MM'));
    const entradas = this.dashboard.fluxoCaixaDiario.map(f => f.entradas);
    const saidas = this.dashboard.fluxoCaixaDiario.map(f => f.saidas);

    this.fluxoCaixaChartOptions = {
      series: [
        {
          name: 'Entradas',
          data: entradas
        },
        {
          name: 'Saídas',
          data: saidas
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
        width: [3, 3]
      },
      xaxis: {
        categories: categories,
        labels: {
          rotate: -45
        }
      },
      yaxis: {
        labels: {
          formatter: (value) => 'R$ ' + value.toFixed(0)
        }
      },
      colors: ['#10b981', '#ef4444'],
      legend: {
        position: 'top'
      },
      tooltip: {
        y: {
          formatter: (value) => this.formatCurrency(value)
        }
      }
    };
  }

  initDespesasChart() {
    if (!this.dashboard || !this.dashboard.despesaPorCategoria.length) return;

    const categories = this.dashboard.despesaPorCategoria.map(d => d.categoria);
    const data = this.dashboard.despesaPorCategoria.map(d => d.total);

    this.despesasChartOptions = {
      series: [{
        name: 'Despesas',
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
          horizontal: true,
          borderRadius: 8
        }
      },
      dataLabels: {
        enabled: false
      },
      xaxis: {
        categories: categories,
        labels: {
          formatter: (value) => 'R$ ' + Number(value).toFixed(0)
        }
      },
      colors: ['#ef4444'],
      tooltip: {
        y: {
          formatter: (value) => this.formatCurrency(value)
        }
      }
    };
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

  translatePaymentMethod(method: string): string {
    const translations: { [key: string]: string } = {
      'Dinheiro': 'Dinheiro',
      'CartaoCredito': 'Cartão Crédito',
      'CartaoDebito': 'Cartão Débito',
      'Pix': 'PIX',
      'TransferenciaBancaria': 'Transferência',
      'Cheque': 'Cheque',
      'Cash': 'Dinheiro',
      'CreditCard': 'Cartão Crédito',
      'DebitCard': 'Cartão Débito',
      'BankTransfer': 'Transferência',
      'Check': 'Cheque'
    };
    return translations[method] || method;
  }

  exportData() {
    console.warn('Export functionality not yet implemented');
  }
}
