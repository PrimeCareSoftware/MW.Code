import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NgApexchartsModule } from 'ng-apexcharts';
import { CohortAnalysisService } from '../../services/cohort-analysis.service';
import { CohortRetention, CohortRevenue, RetentionCohort, RevenueCohort } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-cohort-analysis',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    NgApexchartsModule,
    Navbar
  ],
  template: `
    <app-navbar></app-navbar>
    
    <div class="page-container">
      <div class="page-header">
        <h1>Análise de Coortes</h1>
        <p class="subtitle">Análise de retenção, receita e comportamento por coorte de clientes</p>
      </div>

      <mat-tab-group>
        <mat-tab label="Retenção">
          @if (loadingRetention()) {
            <div class="loading-container">
              <mat-spinner></mat-spinner>
            </div>
          } @else {
            <div class="tab-content">
              <div class="cohort-heatmap">
                <h3>Mapa de Retenção</h3>
                <div class="table-scroll">
                  <table class="cohort-table">
                    <thead>
                      <tr>
                        <th>Coorte</th>
                        <th>Tamanho</th>
                        @for (month of monthHeaders; track month) {
                          <th>Mês {{ month }}</th>
                        }
                      </tr>
                    </thead>
                    <tbody>
                      @for (cohort of retentionData()?.cohorts; track cohort.month) {
                        <tr>
                          <td class="cohort-label">{{ cohort.month | date:'MMM yyyy' }}</td>
                          <td class="cohort-size">{{ cohort.size }}</td>
                          @for (rate of cohort.retentionRates; track $index) {
                            <td class="retention-cell" [style.background-color]="getHeatmapColor(rate)">
                              {{ rate.toFixed(1) }}%
                            </td>
                          }
                        </tr>
                      }
                    </tbody>
                  </table>
                </div>
              </div>

              <div class="chart-container">
                <h3>Retenção Média por Mês</h3>
                <apx-chart
                  [series]="averageRetentionSeries()"
                  [chart]="{ type: 'line', height: 350, toolbar: { show: true } }"
                  [xaxis]="{ categories: monthHeaders, title: { text: 'Mês' } }"
                  [yaxis]="{ max: 100, title: { text: 'Retenção %' } }"
                  [stroke]="{ curve: 'smooth', width: 3 }"
                  [colors]="['#10b981']"
                ></apx-chart>
              </div>
            </div>
          }
        </mat-tab>

        <mat-tab label="Receita">
          @if (loadingRevenue()) {
            <div class="loading-container">
              <mat-spinner></mat-spinner>
            </div>
          } @else {
            <div class="tab-content">
              <div class="cohort-cards">
                @for (cohort of revenueData()?.cohorts; track cohort.month) {
                  <mat-card class="cohort-card">
                    <mat-card-header>
                      <mat-card-title>{{ cohort.month | date:'MMM yyyy' }}</mat-card-title>
                      <mat-card-subtitle>{{ cohort.size }} clientes</mat-card-subtitle>
                    </mat-card-header>
                    <mat-card-content>
                      <div class="metric">
                        <span class="label">LTV:</span>
                        <span class="value ltv">{{ cohort.ltv | currency:'BRL':'symbol':'1.0-0' }}</span>
                      </div>
                      <div class="metric">
                        <span class="label">MRR Inicial:</span>
                        <span class="value">{{ cohort.mrrByMonth[0] | currency:'BRL':'symbol':'1.0-0' }}</span>
                      </div>
                      <div class="metric">
                        <span class="label">MRR Atual:</span>
                        <span class="value">
                          {{ cohort.mrrByMonth[cohort.mrrByMonth.length - 1] | currency:'BRL':'symbol':'1.0-0' }}
                        </span>
                      </div>
                    </mat-card-content>
                  </mat-card>
                }
              </div>

              <div class="chart-container">
                <h3>MRR por Coorte ao Longo do Tempo</h3>
                <apx-chart
                  [series]="mrrCohortSeries()"
                  [chart]="{ type: 'line', height: 400, toolbar: { show: true } }"
                  [stroke]="{ width: 2, curve: 'smooth' }"
                  [xaxis]="{ title: { text: 'Mês' } }"
                  [yaxis]="{ title: { text: 'MRR' }, labels: { formatter: formatCurrency } }"
                  [legend]="{ position: 'top' }"
                ></apx-chart>
              </div>

              <div class="chart-container">
                <h3>LTV por Coorte</h3>
                <apx-chart
                  [series]="ltvSeries()"
                  [chart]="{ type: 'bar', height: 350 }"
                  [plotOptions]="{ bar: { horizontal: true } }"
                  [xaxis]="{ labels: { formatter: formatCurrency } }"
                  [colors]="['#3b82f6']"
                ></apx-chart>
              </div>
            </div>
          }
        </mat-tab>

        <mat-tab label="Comparação de Coortes">
          <div class="tab-content">
            <div class="comparison-container">
              <h3>Comparação de Performance</h3>
              <p class="info-text">
                Compare o desempenho de diferentes coortes ao longo do tempo
              </p>
              
              @if (revenueData()?.cohorts && revenueData()!.cohorts.length > 0) {
                <div class="chart-container">
                  <apx-chart
                    [series]="cohortComparisonSeries()"
                    [chart]="{ type: 'line', height: 400 }"
                    [stroke]="{ width: 2, curve: 'smooth' }"
                    [xaxis]="{ title: { text: 'Mês desde início' } }"
                    [yaxis]="{ title: { text: 'MRR' } }"
                    [legend]="{ position: 'top' }"
                  ></apx-chart>
                </div>
              }
            </div>
          </div>
        </mat-tab>
      </mat-tab-group>
    </div>
  `,
  styles: [`
    .page-container {
      padding: 24px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .page-header {
      margin-bottom: 32px;
    }

    .page-header h1 {
      margin: 0 0 8px 0;
      font-size: 32px;
      font-weight: 600;
    }

    .subtitle {
      color: #666;
      margin: 0;
    }

    .loading-container {
      display: flex;
      justify-content: center;
      padding: 80px 24px;
    }

    .tab-content {
      padding: 24px 0;
    }

    .cohort-heatmap {
      margin-bottom: 48px;
    }

    .cohort-heatmap h3 {
      margin: 0 0 16px 0;
    }

    .table-scroll {
      overflow-x: auto;
    }

    .cohort-table {
      width: 100%;
      border-collapse: collapse;
      background: white;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }

    .cohort-table th,
    .cohort-table td {
      padding: 12px;
      border: 1px solid #e5e7eb;
      text-align: center;
      min-width: 80px;
    }

    .cohort-table th {
      background: #f9fafb;
      font-weight: 600;
      position: sticky;
      top: 0;
      z-index: 10;
    }

    .cohort-label {
      font-weight: 500;
      background: #f9fafb;
      position: sticky;
      left: 0;
      z-index: 5;
    }

    .cohort-size {
      font-weight: 500;
      background: #f9fafb;
    }

    .retention-cell {
      color: white;
      font-weight: 600;
      text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
    }

    .chart-container {
      margin: 48px 0;
      background: white;
      padding: 24px;
      border-radius: 8px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
    }

    .chart-container h3 {
      margin: 0 0 16px 0;
    }

    .cohort-cards {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
      gap: 24px;
      margin-bottom: 48px;
    }

    .cohort-card {
      transition: transform 0.2s;
    }

    .cohort-card:hover {
      transform: translateY(-4px);
    }

    .metric {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 8px 0;
      border-bottom: 1px solid #e5e7eb;
    }

    .metric:last-child {
      border-bottom: none;
    }

    .metric .label {
      color: #666;
      font-size: 14px;
    }

    .metric .value {
      font-weight: 600;
      font-size: 16px;
    }

    .metric .value.ltv {
      color: #10b981;
      font-size: 20px;
    }

    .comparison-container h3 {
      margin: 0 0 8px 0;
    }

    .info-text {
      color: #666;
      margin: 0 0 24px 0;
    }
  `]
})
export class CohortAnalysisComponent implements OnInit {
  retentionData = signal<CohortRetention | null>(null);
  revenueData = signal<CohortRevenue | null>(null);
  loadingRetention = signal(true);
  loadingRevenue = signal(true);

  monthHeaders = Array.from({ length: 12 }, (_, i) => i);

  constructor(private cohortAnalysisService: CohortAnalysisService) {}

  ngOnInit(): void {
    this.loadRetentionData();
    this.loadRevenueData();
  }

  loadRetentionData(): void {
    this.cohortAnalysisService.getRetentionAnalysis(12).subscribe({
      next: (data) => {
        this.retentionData.set(data);
        this.loadingRetention.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar dados de retenção:', err);
        this.loadingRetention.set(false);
      }
    });
  }

  loadRevenueData(): void {
    this.cohortAnalysisService.getRevenueAnalysis(12).subscribe({
      next: (data) => {
        this.revenueData.set(data);
        this.loadingRevenue.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar dados de receita:', err);
        this.loadingRevenue.set(false);
      }
    });
  }

  getHeatmapColor(rate: number): string {
    if (rate >= 80) return '#10b981';
    if (rate >= 60) return '#fbbf24';
    if (rate >= 40) return '#f59e0b';
    return '#ef4444';
  }

  averageRetentionSeries() {
    const cohorts = this.retentionData()?.cohorts || [];
    if (cohorts.length === 0) return [];

    const averages = this.monthHeaders.map(month => {
      const rates = cohorts
        .filter(c => c.retentionRates.length > month)
        .map(c => c.retentionRates[month]);
      return rates.length > 0 ? rates.reduce((a, b) => a + b, 0) / rates.length : 0;
    });

    return [{
      name: 'Retenção Média',
      data: averages
    }];
  }

  mrrCohortSeries() {
    const cohorts = this.revenueData()?.cohorts || [];
    return cohorts.map(cohort => ({
      name: new Date(cohort.month).toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' }),
      data: cohort.mrrByMonth
    }));
  }

  ltvSeries() {
    const cohorts = this.revenueData()?.cohorts || [];
    return [{
      name: 'LTV',
      data: cohorts.map(c => c.ltv)
    }];
  }

  cohortComparisonSeries() {
    const cohorts = this.revenueData()?.cohorts || [];
    return cohorts.map(cohort => ({
      name: new Date(cohort.month).toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' }),
      data: cohort.mrrByMonth
    }));
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(value);
  }
}
