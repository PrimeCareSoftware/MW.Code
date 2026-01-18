import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { SystemAdminService } from '../../../services/system-admin';
import { SystemAnalytics } from '../../../models/system-admin.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'})
export class Dashboard implements OnInit {
  analytics = signal<SystemAnalytics | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadAnalytics();
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
    this.router.navigate(['/system-admin/clinics'], { queryParams: status ? { status } : {} });
  }

  navigateToPlans(): void {
    this.router.navigate(['/system-admin/plans']);
  }

  navigateToOwners(): void {
    this.router.navigate(['/system-admin/clinic-owners']);
  }

  navigateToSubdomains(): void {
    this.router.navigate(['/system-admin/subdomains']);
  }
}
