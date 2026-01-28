import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { SystemAdminService } from '../../services/system-admin';
import { SaasMetricsService } from '../../services/saas-metrics.service';
import { SystemAnalytics, SaasDashboard } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';
import { KpiCardComponent } from '../../components/kpi-card/kpi-card.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, Navbar, KpiCardComponent],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'})
export class Dashboard implements OnInit {
  analytics = signal<SystemAnalytics | null>(null);
  saasDashboard = signal<SaasDashboard | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private systemAdminService: SystemAdminService,
    private saasMetricsService: SaasMetricsService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAnalytics();
    this.loadSaasMetrics();
    
    // Auto-refresh every 30 seconds
    setInterval(() => {
      this.loadSaasMetrics();
    }, 30000);
  }

  loadAnalytics(): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getAnalytics().subscribe({
      next: (data) => {
        this.analytics.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar dados');
        this.loading.set(false);
      }
    });
  }

  loadSaasMetrics(): void {
    this.saasMetricsService.getDashboard().subscribe({
      next: (data) => {
        this.saasDashboard.set(data);
      },
      error: (err) => {
        console.error('Erro ao carregar métricas SaaS:', err);
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getSubscriptionStatusItems(): Array<{ key: string; label: string; value: number }> {
    const statusLabels: { [key: string]: string } = {
      'Active': 'Ativo',
      'Trial': 'Trial',
      'Expired': 'Expirado',
      'Suspended': 'Suspenso',
      'PaymentOverdue': 'Pagamento Atrasado',
      'Cancelled': 'Cancelado'
    };

    return Object.entries(this.analytics()?.subscriptionsByStatus || {}).map(([key, value]) => ({
      key,
      label: statusLabels[key] || key,
      value: value as number
    }));
  }

  getSubscriptionPlanItems(): Array<{ key: string; label: string; value: number }> {
    return Object.entries(this.analytics()?.subscriptionsByPlan || {}).map(([key, value]) => ({
      key,
      label: key,
      value: value as number
    }));
  }

  navigateToClinics(status: string): void {
    this.router.navigate(['/clinics'], { queryParams: status ? { status } : {} });
  }

  navigateToPlans(): void {
    this.router.navigate(['/plans']);
  }

  navigateToOwners(): void {
    this.router.navigate(['/clinic-owners']);
  }

  navigateToSubdomains(): void {
    this.router.navigate(['/subdomains']);
  }
}
