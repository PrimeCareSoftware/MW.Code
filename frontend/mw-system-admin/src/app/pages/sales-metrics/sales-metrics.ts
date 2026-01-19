import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { Navbar } from '../../shared/navbar/navbar';

interface FunnelStats {
  totalSessions: number;
  conversions: number;
  conversionRate: number;
  stepStats: { [key: number]: StepStats };
}

interface StepStats {
  stepNumber: number;
  stepName: string;
  entered: number;
  completed: number;
  abandoned: number;
  completionRate: number;
  abandonmentRate: number;
}

interface IncompleteSession {
  sessionId: string;
  lastStep: number;
  lastStepName: string;
  capturedData?: string;
  planId?: string;
  lastActivity: string;
  hoursSinceLastActivity: number;
}

interface SalesFunnelMetric {
  id: string;
  sessionId: string;
  step: number;
  stepName: string;
  action: string;
  capturedData?: string;
  planId?: string;
  ipAddress?: string;
  userAgent?: string;
  referrer?: string;
  clinicId?: string;
  ownerId?: string;
  isConverted: boolean;
  durationMs?: number;
  metadata?: string;
  createdAt: string;
}

@Component({
  selector: 'app-sales-metrics',
  imports: [CommonModule, FormsModule, Navbar],
  templateUrl: './sales-metrics.html',
  styleUrl: './sales-metrics.scss',
  standalone: true
})
export class SalesMetrics implements OnInit {
  private http = inject(HttpClient);
  private readonly API_URL = `${environment.apiUrl}/salesfunnel`;

  // Reactive state
  stats = signal<FunnelStats | null>(null);
  incompleteSessions = signal<IncompleteSession[]>([]);
  selectedSession = signal<SalesFunnelMetric[] | null>(null);
  isLoading = signal(false);
  error = signal<string | null>(null);

  // Filter state
  dateRange = signal({
    startDate: this.getDefaultStartDate(),
    endDate: this.getDefaultEndDate()
  });
  hoursOld = signal(24);

  ngOnInit(): void {
    this.loadStats();
    this.loadIncompleteSessions();
  }

  private getDefaultStartDate(): string {
    const date = new Date();
    date.setDate(date.getDate() - 30); // Last 30 days
    return date.toISOString().split('T')[0];
  }

  private getDefaultEndDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  loadStats(): void {
    this.isLoading.set(true);
    this.error.set(null);

    const params: any = {};
    if (this.dateRange().startDate) {
      params.startDate = new Date(this.dateRange().startDate).toISOString();
    }
    if (this.dateRange().endDate) {
      const endDate = new Date(this.dateRange().endDate);
      endDate.setHours(23, 59, 59, 999); // End of day
      params.endDate = endDate.toISOString();
    }

    this.http.get<FunnelStats>(`${this.API_URL}/stats`, { params }).subscribe({
      next: (stats) => {
        this.stats.set(stats);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Failed to load statistics');
        this.isLoading.set(false);
        console.error('Error loading stats:', err);
      }
    });
  }

  loadIncompleteSessions(): void {
    const params = {
      hoursOld: this.hoursOld().toString(),
      limit: '100'
    };

    this.http.get<IncompleteSession[]>(`${this.API_URL}/incomplete`, { params }).subscribe({
      next: (sessions) => {
        this.incompleteSessions.set(sessions);
      },
      error: (err) => {
        console.error('Error loading incomplete sessions:', err);
      }
    });
  }

  viewSessionDetails(sessionId: string): void {
    this.http.get<SalesFunnelMetric[]>(`${this.API_URL}/session/${sessionId}`).subscribe({
      next: (metrics) => {
        this.selectedSession.set(metrics);
      },
      error: (err) => {
        console.error('Error loading session details:', err);
      }
    });
  }

  closeSessionDetails(): void {
    this.selectedSession.set(null);
  }

  updateStartDate(value: string): void {
    this.dateRange.set({
      ...this.dateRange(),
      startDate: value
    });
  }

  updateEndDate(value: string): void {
    this.dateRange.set({
      ...this.dateRange(),
      endDate: value
    });
  }

  updateHoursOld(value: number | string): void {
    this.hoursOld.set(typeof value === 'string' ? parseInt(value, 10) : value);
  }

  applyFilters(): void {
    this.loadStats();
    this.loadIncompleteSessions();
  }

  resetFilters(): void {
    this.dateRange.set({
      startDate: this.getDefaultStartDate(),
      endDate: this.getDefaultEndDate()
    });
    this.hoursOld.set(24);
    this.applyFilters();
  }

  getStepArray(): number[] {
    return [1, 2, 3, 4, 5, 6];
  }

  getStepStats(step: number): StepStats | undefined {
    return this.stats()?.stepStats[step];
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleString('pt-BR');
  }

  parseCapturedData(data: string | undefined): any {
    if (!data) return null;
    try {
      return JSON.parse(data);
    } catch {
      return null;
    }
  }

  exportData(): void {
    const stats = this.stats();
    if (!stats) return;

    const data = {
      stats: stats,
      incompleteSessions: this.incompleteSessions(),
      exportedAt: new Date().toISOString()
    };

    const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `sales-funnel-metrics-${new Date().toISOString().split('T')[0]}.json`;
    a.click();
    window.URL.revokeObjectURL(url);
  }
}
