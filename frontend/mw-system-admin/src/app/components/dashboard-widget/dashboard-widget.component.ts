import { Component, Input, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgApexchartsModule } from 'ng-apexcharts';
import { DashboardService } from '../../services/dashboard.service';
import { DashboardWidget, WidgetConfig } from '../../models/system-admin.model';
import { ApexOptions } from 'apexcharts';

@Component({
  selector: 'app-dashboard-widget',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule,
    NgApexchartsModule
  ],
  template: `
    <mat-card class="widget-card">
      <mat-card-header>
        <mat-card-title>{{ widget.title }}</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        @if (loading()) {
          <div class="loading-container">
            <mat-spinner diameter="40"></mat-spinner>
          </div>
        } @else if (error()) {
          <div class="error-container">
            <mat-icon color="warn">error</mat-icon>
            <p>{{ error() }}</p>
          </div>
        } @else {
          @switch (widget.type) {
            @case ('line') {
              <apx-chart
                [series]="chartOptions().series"
                [chart]="chartOptions().chart"
                [xaxis]="chartOptions().xaxis"
                [yaxis]="chartOptions().yaxis"
                [stroke]="chartOptions().stroke"
              ></apx-chart>
            }
            @case ('bar') {
              <apx-chart
                [series]="chartOptions().series"
                [chart]="chartOptions().chart"
                [xaxis]="chartOptions().xaxis"
                [yaxis]="chartOptions().yaxis"
                [plotOptions]="chartOptions().plotOptions"
              ></apx-chart>
            }
            @case ('pie') {
              <apx-chart
                [series]="chartOptions().series"
                [chart]="chartOptions().chart"
                [labels]="chartOptions().labels"
              ></apx-chart>
            }
            @case ('metric') {
              <div class="metric-widget">
                @if (config().icon) {
                  <mat-icon [style.color]="config().color">{{ config().icon }}</mat-icon>
                }
                <div class="metric-value" [style.color]="getMetricColor()">
                  {{ formatMetricValue() }}
                </div>
              </div>
            }
            @case ('table') {
              <div class="table-container">
                <table mat-table [dataSource]="data()">
                  @for (column of getTableColumns(); track column) {
                    <ng-container [matColumnDef]="column">
                      <th mat-header-cell *matHeaderCellDef>{{ column }}</th>
                      <td mat-cell *matCellDef="let element">{{ element[column] }}</td>
                    </ng-container>
                  }
                  <tr mat-header-row *matHeaderRowDef="getTableColumns()"></tr>
                  <tr mat-row *matRowDef="let row; columns: getTableColumns()"></tr>
                </table>
              </div>
            }
            @case ('markdown') {
              <div class="markdown-content" [innerHTML]="data()"></div>
            }
            @default {
              <div class="unsupported-type">
                Tipo de widget não suportado: {{ widget.type }}
              </div>
            }
          }
        }
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .widget-card {
      height: 100%;
      display: flex;
      flex-direction: column;
    }

    mat-card-content {
      flex: 1;
      display: flex;
      flex-direction: column;
      padding: 16px;
    }

    .loading-container,
    .error-container {
      display: flex;
      align-items: center;
      justify-content: center;
      min-height: 200px;
      flex-direction: column;
      gap: 16px;
    }

    .metric-widget {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 16px;
      padding: 32px;
    }

    .metric-widget mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
    }

    .metric-value {
      font-size: 48px;
      font-weight: 700;
    }

    .table-container {
      overflow: auto;
      max-height: 400px;
    }

    .markdown-content {
      padding: 16px;
    }

    .unsupported-type {
      padding: 32px;
      text-align: center;
      color: #999;
    }
  `]
})
export class DashboardWidgetComponent implements OnInit, OnDestroy {
  @Input() widget!: DashboardWidget;

  loading = signal(false);
  error = signal<string | null>(null);
  data = signal<any>(null);
  config = signal<WidgetConfig>({});
  chartOptions = signal<Partial<ApexOptions>>({});
  
  private refreshInterval?: number;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.parseConfig();
    this.loadData();
    this.setupAutoRefresh();
  }

  ngOnDestroy(): void {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  private parseConfig(): void {
    try {
      this.config.set(JSON.parse(this.widget.config || '{}'));
    } catch (e) {
      console.error('Error parsing widget config:', e);
      this.config.set({});
    }
  }

  private setupAutoRefresh(): void {
    if (this.widget.refreshInterval > 0) {
      this.refreshInterval = window.setInterval(() => {
        this.loadData();
      }, this.widget.refreshInterval * 1000);
    }
  }

  loadData(): void {
    this.loading.set(true);
    this.error.set(null);

    this.dashboardService.getWidgetData(this.widget.id).subscribe({
      next: (response) => {
        this.data.set(response);
        this.prepareChartData(response);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar dados');
        this.loading.set(false);
      }
    });
  }

  private prepareChartData(data: any): void {
    const cfg = this.config();
    
    if (this.widget.type === 'pie') {
      this.chartOptions.set({
        series: data.map((d: any) => d[cfg.valueField || 'value']),
        labels: data.map((d: any) => d[cfg.labelField || 'label']),
        chart: {
          type: 'pie',
          height: 350
        }
      });
    } else if (this.widget.type === 'line' || this.widget.type === 'bar') {
      this.chartOptions.set({
        series: [{
          name: cfg.yAxis || 'Value',
          data: data.map((d: any) => d[cfg.yAxis || 'value'])
        }],
        chart: {
          type: this.widget.type,
          height: 350
        },
        xaxis: {
          categories: data.map((d: any) => d[cfg.xAxis || 'label'])
        },
        yaxis: {
          title: {
            text: cfg.yAxis || 'Value'
          }
        },
        stroke: this.widget.type === 'line' ? { curve: 'smooth', width: 2 } : undefined,
        plotOptions: this.widget.type === 'bar' ? {
          bar: {
            horizontal: false,
            columnWidth: '55%'
          }
        } : undefined
      });
    }
  }

  formatMetricValue(): string {
    const value = this.data();
    const format = this.config().format;

    if (value === null || value === undefined) return '-';

    if (format === 'currency') {
      return new Intl.NumberFormat('pt-BR', {
        style: 'currency',
        currency: 'BRL'
      }).format(value);
    }

    if (format === 'percent') {
      return `${value}%`;
    }

    return value.toString();
  }

  getMetricColor(): string {
    const value = this.data();
    const threshold = this.config().threshold;

    if (!threshold || value === null || value === undefined) {
      return this.config().color || '#1a1a1a';
    }

    if (threshold.critical && value >= threshold.critical) {
      return '#ef4444';
    }

    if (threshold.warning && value >= threshold.warning) {
      return '#f59e0b';
    }

    return this.config().color || '#10b981';
  }

  getTableColumns(): string[] {
    const data = this.data();
    if (!data || !Array.isArray(data) || data.length === 0) {
      return [];
    }
    return Object.keys(data[0]);
  }
}
