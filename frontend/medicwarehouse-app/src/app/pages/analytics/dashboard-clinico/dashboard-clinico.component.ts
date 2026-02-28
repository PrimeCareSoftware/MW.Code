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
import { Loading } from '../../../shared/loading/loading';
import { AnalyticsBIService } from '../../../services/analytics-bi.service';
import { DashboardClinico } from '../../../models/analytics-bi.model';
import { MLPredictionService } from '../../../services/ml-prediction.service';
import { PrevisaoConsultas } from '../../../models/ml-prediction.model';

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
  selector: 'app-dashboard-clinico',
  standalone: true,
  imports: [CommonModule, FormsModule, Loading, NgApexchartsModule],
  templateUrl: './dashboard-clinico.component.html',
  styleUrl: './dashboard-clinico.component.scss'
})
export class DashboardClinicoComponent implements OnInit {
  // Date range
  dateRange: string = 'last30days';
  customStartDate: string = '';
  customEndDate: string = '';
  
  // Loading state
  loading = true;
  error: string | null = null;
  
  // Dashboard data
  dashboard?: DashboardClinico;
  
  // ML Predictions
  previsaoDemanda?: PrevisaoConsultas;
  loadingPrevisao = false;
  previsaoError: string | null = null;
  
  // Chart options
  especialidadeChartOptions?: Partial<ChartOptions>;
  diaSemanaChartOptions?: Partial<ChartOptions>;
  tendenciaChartOptions?: Partial<ChartOptions>;
  previsaoDemandaChartOptions?: Partial<ChartOptions>;

  constructor(
    private analyticsBIService: AnalyticsBIService,
    private mlPredictionService: MLPredictionService
  ) {}

  ngOnInit() {
    this.loadDashboard();
    this.loadPrevisaoDemanda();
  }

  loadDashboard() {
    this.loading = true;
    this.error = null;
    
    const { startDate, endDate } = this.getDateRange();
    
    this.analyticsBIService.getDashboardClinico(
      startDate, 
      endDate, 
      undefined
    ).subscribe({
      next: (data) => {
        this.dashboard = data;
        this.initializeCharts();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error loading clinical dashboard:', err);
        this.error = 'Erro ao carregar dashboard clínico';
        this.loading = false;
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
        startDate = subDays(today, 30);
    }

    return {
      startDate: format(startDate, 'yyyy-MM-dd'),
      endDate: format(endDate, 'yyyy-MM-dd')
    };
  }

  onFilterChange() {
    this.loadDashboard();
  }

  loadPrevisaoDemanda() {
    this.loadingPrevisao = true;
    this.previsaoError = null;
    
    this.mlPredictionService.getPrevisaoProximaSemana().subscribe({
      next: (data) => {
        this.previsaoDemanda = data;
        this.initPrevisaoDemandaChart();
        this.loadingPrevisao = false;
      },
      error: (err) => {
        console.error('Error loading demand forecast:', err);
        this.previsaoError = 'Erro ao carregar previsão de demanda. Modelo ML pode não estar treinado.';
        this.loadingPrevisao = false;
      }
    });
  }

  initializeCharts() {
    if (!this.dashboard) return;
    
    this.initEspecialidadeChart();
    this.initDiaSemanaChart();
    this.initTendenciaChart();
  }

  initPrevisaoDemandaChart() {
    if (!this.previsaoDemanda || !this.previsaoDemanda.previsoes.length) return;

    const categories = this.previsaoDemanda.previsoes.map(p => {
      const date = new Date(p.data);
      return format(date, 'dd/MM');
    });
    const data = this.previsaoDemanda.previsoes.map(p => p.consultasPrevistas);

    this.previsaoDemandaChartOptions = {
      series: [{
        name: 'Previsão de Consultas',
        data: data
      }],
      chart: {
        type: 'area',
        height: 300,
        toolbar: {
          show: false
        }
      },
      stroke: {
        curve: 'smooth',
        width: 2
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
      xaxis: {
        categories: categories
      },
      yaxis: {
        labels: {
          formatter: (value) => Math.round(value).toString()
        }
      },
      colors: ['#10b981'],
      dataLabels: {
        enabled: false
      },
      tooltip: {
        y: {
          formatter: (value) => Math.round(value) + ' consultas'
        }
      }
    };
  }

  initEspecialidadeChart() {
    if (!this.dashboard || !this.dashboard.consultasPorEspecialidade.length) return;

    const labels = this.dashboard.consultasPorEspecialidade.map(c => c.especialidade);
    const data = this.dashboard.consultasPorEspecialidade.map(c => c.total);

    this.especialidadeChartOptions = {
      series: data,
      chart: {
        type: 'donut',
        height: 350
      },
      labels: labels,
      colors: ['#0ea5e9', '#8b5cf6', '#ec4899', '#f59e0b', '#10b981', '#6366f1', '#ef4444', '#14b8a6'],
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

  initDiaSemanaChart() {
    if (!this.dashboard || !this.dashboard.consultasPorDiaSemana.length) return;

    const categories = this.dashboard.consultasPorDiaSemana.map(d => d.diaSemana);
    const data = this.dashboard.consultasPorDiaSemana.map(d => d.total);

    this.diaSemanaChartOptions = {
      series: [{
        name: 'Consultas',
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
          formatter: (value) => Math.round(value).toString()
        }
      },
      fill: {
        opacity: 1
      },
      colors: ['#0ea5e9']
    };
  }

  initTendenciaChart() {
    if (!this.dashboard || !this.dashboard.tendenciaConsultas.length) return;

    const categories = this.dashboard.tendenciaConsultas.map(t => t.mes);
    const realizadas = this.dashboard.tendenciaConsultas.map(t => t.realizadas);
    const agendadas = this.dashboard.tendenciaConsultas.map(t => t.agendadas);

    this.tendenciaChartOptions = {
      series: [
        {
          name: 'Realizadas',
          data: realizadas
        },
        {
          name: 'Agendadas',
          data: agendadas
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

  getMonthName(month: number): string {
    const months = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];
    return months[month - 1];
  }

  formatPercent(value: number): string {
    return value.toFixed(1) + '%';
  }

  exportData() {
    console.warn('Export functionality not yet implemented');
    // TODO: Implement export to CSV/Excel
  }
}
