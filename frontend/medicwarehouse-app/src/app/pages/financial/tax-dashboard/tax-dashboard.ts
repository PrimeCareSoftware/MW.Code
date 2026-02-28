import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { 
  FiscalService, 
  ApuracaoImpostos, 
  ConfiguracaoFiscal,
  StatusApuracao,
  RegimeTributarioEnum 
} from '../../../services/fiscal.service';
import { NgApexchartsModule } from 'ng-apexcharts';
import { 
  ApexChart, 
  ApexAxisChartSeries, 
  ApexXAxis, 
  ApexDataLabels,
  ApexPlotOptions,
  ApexYAxis,
  ApexLegend,
  ApexTooltip,
  ApexFill
} from 'ng-apexcharts';

export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: ApexChart;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  plotOptions: ApexPlotOptions;
  yaxis: ApexYAxis;
  legend: ApexLegend;
  tooltip: ApexTooltip;
  fill: ApexFill;
  colors: string[];
};

@Component({
  selector: 'app-tax-dashboard',
  imports: [CommonModule, FormsModule, NgApexchartsModule],
  templateUrl: './tax-dashboard.html',
  styleUrl: './tax-dashboard.scss'
})
export class TaxDashboard implements OnInit {
  // Data signals
  apuracao = signal<ApuracaoImpostos | null>(null);
  configuracao = signal<ConfiguracaoFiscal | null>(null);
  evolucaoMensal = signal<ApuracaoImpostos[]>([]);
  
  // UI state signals
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  
  // Filter signals (writable)
  selectedMonth = signal<number>(new Date().getMonth() + 1);
  selectedYear = signal<number>(new Date().getFullYear());
  trendMonths = signal<number>(12);
  
  // For template binding
  get month() { return this.selectedMonth(); }
  set month(value: number) { this.selectedMonth.set(value); }
  
  get year() { return this.selectedYear(); }
  set year(value: number) { this.selectedYear.set(value); }
  
  get months() { return this.trendMonths(); }
  set months(value: number) { this.trendMonths.set(value); }
  
  // Chart options
  chartOptions = signal<Partial<ChartOptions> | null>(null);
  evolutionChartOptions = signal<Partial<ChartOptions> | null>(null);

  // Computed values
  hasData = computed(() => this.apuracao() !== null);
  
  totalImpostos = computed(() => {
    const ap = this.apuracao();
    if (!ap) return 0;
    return (ap.totalPIS || 0) + (ap.totalCOFINS || 0) + (ap.totalIR || 0) + 
           (ap.totalCSLL || 0) + (ap.totalISS || 0) + (ap.totalINSS || 0);
  });

  cargaTributaria = computed(() => {
    const ap = this.apuracao();
    if (!ap) return 0;
    return this.fiscalService.calcularCargaTributaria(ap);
  });

  optanteSimplesNacional = computed(() => {
    const config = this.configuracao();
    return config?.regime === RegimeTributarioEnum.SimplesNacional;
  });

  constructor(private fiscalService: FiscalService) {}

  ngOnInit(): void {
    this.loadAllData();
  }

  loadAllData(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    // Load data in parallel
    Promise.all([
      this.loadApuracao(),
      this.loadConfiguracao(),
      this.loadEvolucao()
    ])
    .then(() => {
      this.isLoading.set(false);
      this.updateCharts();
    })
    .catch((error) => {
      console.error('Erro ao carregar dados fiscais:', error);
      this.errorMessage.set('Erro ao carregar dados. Tente novamente.');
      this.isLoading.set(false);
    });
  }

  private async loadApuracao(): Promise<void> {
    try {
      const data = await this.fiscalService.getApuracaoMensal(
        this.selectedMonth(), 
        this.selectedYear()
      ).toPromise();
      
      this.apuracao.set(data || null);
    } catch (error: any) {
      // If apuracao doesn't exist, try to generate it
      if (error.status === 404) {
        console.log('Apuração não encontrada, tentando gerar...');
        try {
          const data = await this.fiscalService.gerarApuracao(
            this.selectedMonth(), 
            this.selectedYear()
          ).toPromise();
          this.apuracao.set(data || null);
        } catch (genError) {
          console.warn('Não foi possível gerar apuração automática');
          this.errorMessage.set('Não há dados fiscais para este período.');
          this.apuracao.set(null);
        }
      } else {
        throw error;
      }
    }
  }

  private async loadConfiguracao(): Promise<void> {
    try {
      const data = await this.fiscalService.getConfiguracao().toPromise();
      this.configuracao.set(data || null);
    } catch (error) {
      console.error('Erro ao carregar configuração fiscal:', error);
      this.configuracao.set(null);
    }
  }

  private async loadEvolucao(): Promise<void> {
    try {
      const data = await this.fiscalService.getEvolucaoMensal(this.trendMonths()).toPromise();
      this.evolucaoMensal.set(data || []);
    } catch (error) {
      console.error('Erro ao carregar evolução mensal:', error);
      this.evolucaoMensal.set([]);
    }
  }
  
  calculateTaxPercentage(taxValue: number, faturamento: number): string {
    if (faturamento === 0 || taxValue === 0) return '0.00%';
    return this.formatPercentage((taxValue / faturamento) * 100);
  }

  private updateCharts(): void {
    this.updateTaxBreakdownChart();
    this.updateEvolutionChart();
  }

  private updateTaxBreakdownChart(): void {
    const ap = this.apuracao();
    if (!ap) return;

    this.chartOptions.set({
      series: [{
        name: 'Valor',
        data: [ap.totalISS, ap.totalPIS, ap.totalCOFINS, ap.totalIR, ap.totalCSLL, ap.totalINSS]
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
          dataLabels: {
            position: 'top'
          }
        }
      },
      dataLabels: {
        enabled: true,
        formatter: (val: number) => this.formatCurrency(val),
        offsetY: -20,
        style: {
          fontSize: '12px',
          colors: ['#304758']
        }
      },
      xaxis: {
        categories: ['ISS', 'PIS', 'COFINS', 'IR', 'CSLL', 'INSS']
      },
      yaxis: {
        labels: {
          formatter: (val: number) => this.formatCurrency(val)
        }
      },
      colors: ['#008FFB'],
      tooltip: {
        y: {
          formatter: (val: number) => this.formatCurrency(val)
        }
      }
    });
  }

  private updateEvolutionChart(): void {
    const evolucao = this.evolucaoMensal();
    if (evolucao.length === 0) return;

    const categories = evolucao.map(e => this.getMonthName(e.mes) + '/' + e.ano);
    const faturamentos = evolucao.map(e => e.faturamentoBruto);
    const impostos = evolucao.map(e => 
      e.totalPIS + e.totalCOFINS + e.totalIR + e.totalCSLL + e.totalISS + e.totalINSS
    );

    this.evolutionChartOptions.set({
      series: [
        {
          name: 'Faturamento Bruto',
          data: faturamentos
        },
        {
          name: 'Total Impostos',
          data: impostos
        }
      ],
      chart: {
        type: 'line',
        height: 350,
        toolbar: {
          show: false
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
          formatter: (val: number) => this.formatCurrency(val)
        }
      },
      colors: ['#00E396', '#FF4560'],
      tooltip: {
        y: {
          formatter: (val: number) => this.formatCurrency(val)
        }
      }
    });
  }

  onMonthChange(): void {
    this.loadAllData();
  }

  onYearChange(): void {
    this.loadAllData();
  }

  onTrendMonthsChange(): void {
    this.loadEvolucao().then(() => this.updateEvolutionChart());
  }

  getStatusClass(status: StatusApuracao): string {
    switch (status) {
      case StatusApuracao.Pago:
        return 'status-success';
      case StatusApuracao.Atrasado:
        return 'status-danger';
      case StatusApuracao.Apurado:
        return 'status-info';
      case StatusApuracao.Parcelado:
        return 'status-warning';
      default:
        return 'status-secondary';
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

  formatNumber(value: number): string {
    return new Intl.NumberFormat('pt-BR').format(value);
  }

  getMonthName(month: number): string {
    const months = [
      'Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun',
      'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'
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
    // TODO: Implement PDF export functionality
    console.warn('Funcionalidade de exportação em PDF será implementada em breve.');
  }

  exportToExcel(): void {
    // TODO: Implement Excel export functionality
    console.warn('Funcionalidade de exportação em Excel será implementada em breve.');
  }
}
