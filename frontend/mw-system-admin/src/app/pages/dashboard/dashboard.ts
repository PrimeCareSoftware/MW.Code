import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { SystemAdminService } from '../../services/system-admin';
import { SystemAnalytics } from '../../models/system-admin.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="dashboard">
      <div class="header">
        <h1>Painel de Administração do Sistema</h1>
        <p class="subtitle">Gerencie todas as clínicas e visualize métricas do sistema</p>
      </div>

      @if (loading()) {
        <div class="loading">Carregando dados...</div>
      } @else if (error()) {
        <div class="error">
          <p>Erro ao carregar dados: {{ error() }}</p>
          <button (click)="loadAnalytics()" class="btn-retry">Tentar novamente</button>
        </div>
      } @else if (analytics()) {
        <div class="analytics-grid">
          <!-- Clinics Card -->
          <div class="card">
            <div class="card-header">
              <h3>Clínicas</h3>
              <i class="icon">🏥</i>
            </div>
            <div class="card-body">
              <div class="stat-large">{{ analytics()!.totalClinics }}</div>
              <div class="stat-details">
                <span class="stat-item">
                  <span class="stat-label">Ativas:</span>
                  <span class="stat-value success">{{ analytics()!.activeClinics }}</span>
                </span>
                <span class="stat-item">
                  <span class="stat-label">Inativas:</span>
                  <span class="stat-value error">{{ analytics()!.inactiveClinics }}</span>
                </span>
              </div>
            </div>
          </div>

          <!-- Users Card -->
          <div class="card">
            <div class="card-header">
              <h3>Usuários</h3>
              <i class="icon">👥</i>
            </div>
            <div class="card-body">
              <div class="stat-large">{{ analytics()!.totalUsers }}</div>
              <div class="stat-details">
                <span class="stat-item">
                  <span class="stat-label">Ativos:</span>
                  <span class="stat-value success">{{ analytics()!.activeUsers }}</span>
                </span>
                <span class="stat-item">
                  <span class="stat-label">Inativos:</span>
                  <span class="stat-value error">{{ analytics()!.totalUsers - analytics()!.activeUsers }}</span>
                </span>
              </div>
            </div>
          </div>

          <!-- Patients Card -->
          <div class="card">
            <div class="card-header">
              <h3>Pacientes</h3>
              <i class="icon">🩺</i>
            </div>
            <div class="card-body">
              <div class="stat-large">{{ analytics()!.totalPatients }}</div>
              <div class="stat-details">
                <span class="stat-item">Total de pacientes cadastrados no sistema</span>
              </div>
            </div>
          </div>

          <!-- Revenue Card -->
          <div class="card highlight">
            <div class="card-header">
              <h3>Receita Mensal Recorrente</h3>
              <i class="icon">💰</i>
            </div>
            <div class="card-body">
              <div class="stat-large">{{ formatCurrency(analytics()!.monthlyRecurringRevenue) }}</div>
              <div class="stat-details">
                <span class="stat-item">MRR total de todas as clínicas ativas</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Subscription Status Distribution -->
        <div class="section">
          <h2>Distribuição de Assinaturas por Status</h2>
          <div class="distribution-grid">
            @for (item of getSubscriptionStatusItems(); track item.key) {
              <div class="distribution-card">
                <div class="distribution-value">{{ item.value }}</div>
                <div class="distribution-label">{{ item.label }}</div>
              </div>
            }
          </div>
        </div>

        <!-- Subscription Plan Distribution -->
        <div class="section">
          <h2>Distribuição de Assinaturas por Plano</h2>
          <div class="distribution-grid">
            @for (item of getSubscriptionPlanItems(); track item.key) {
              <div class="distribution-card">
                <div class="distribution-value">{{ item.value }}</div>
                <div class="distribution-label">{{ item.label }}</div>
              </div>
            }
          </div>
        </div>

        <!-- Quick Actions -->
        <div class="quick-actions">
          <h2>Ações Rápidas</h2>
          <div class="actions-grid">
            <button class="action-btn" (click)="navigateToClinics('')">
              <span class="action-icon">📋</span>
              <span class="action-text">Gerenciar Clínicas</span>
            </button>
            <button class="action-btn" (click)="navigateToPlans()">
              <span class="action-icon">💎</span>
              <span class="action-text">Planos de Assinatura</span>
            </button>
            <button class="action-btn" (click)="navigateToOwners()">
              <span class="action-icon">👤</span>
              <span class="action-text">Gerenciar Proprietários</span>
            </button>
            <button class="action-btn" (click)="navigateToSubdomains()">
              <span class="action-icon">🌐</span>
              <span class="action-text">Configurar Subdomínios</span>
            </button>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .dashboard {
      padding: 24px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .header {
      margin-bottom: 32px;
    }

    h1 {
      font-size: 32px;
      font-weight: 700;
      color: #1a202c;
      margin: 0 0 8px 0;
    }

    .subtitle {
      color: #718096;
      font-size: 16px;
      margin: 0;
    }

    .loading, .error {
      text-align: center;
      padding: 48px 24px;
      color: #718096;
    }

    .error {
      color: #c53030;
    }

    .btn-retry {
      margin-top: 16px;
      padding: 12px 24px;
      background: #667eea;
      color: white;
      border: none;
      border-radius: 8px;
      cursor: pointer;
      font-size: 14px;
      font-weight: 600;
    }

    .btn-retry:hover {
      background: #5568d3;
    }

    .analytics-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 24px;
      margin-bottom: 32px;
    }

    .card {
      background: white;
      border-radius: 12px;
      padding: 24px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 16px rgba(0, 0, 0, 0.15);
    }

    .card.highlight {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .card.highlight .stat-large,
    .card.highlight .stat-label,
    .card.highlight .stat-item {
      color: white;
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }

    .card-header h3 {
      font-size: 16px;
      font-weight: 600;
      color: #2d3748;
      margin: 0;
    }

    .card.highlight .card-header h3 {
      color: white;
    }

    .icon {
      font-size: 32px;
    }

    .card-body {
      text-align: left;
    }

    .stat-large {
      font-size: 48px;
      font-weight: 700;
      color: #1a202c;
      margin-bottom: 12px;
    }

    .stat-details {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .stat-item {
      display: flex;
      justify-content: space-between;
      font-size: 14px;
    }

    .stat-label {
      color: #718096;
      font-weight: 500;
    }

    .stat-value {
      font-weight: 600;
    }

    .stat-value.success {
      color: #10b981;
    }

    .stat-value.error {
      color: #ef4444;
    }

    .section {
      margin-bottom: 32px;
      background: white;
      border-radius: 12px;
      padding: 24px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .section h2 {
      font-size: 20px;
      font-weight: 600;
      color: #1a202c;
      margin: 0 0 20px 0;
    }

    .distribution-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
      gap: 16px;
    }

    .distribution-card {
      text-align: center;
      padding: 20px;
      background: #f7fafc;
      border-radius: 8px;
      border: 2px solid #e2e8f0;
    }

    .distribution-value {
      font-size: 32px;
      font-weight: 700;
      color: #667eea;
      margin-bottom: 8px;
    }

    .distribution-label {
      font-size: 13px;
      color: #718096;
      font-weight: 500;
    }

    .quick-actions {
      background: white;
      border-radius: 12px;
      padding: 24px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .quick-actions h2 {
      font-size: 20px;
      font-weight: 600;
      color: #1a202c;
      margin: 0 0 20px 0;
    }

    .actions-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
      gap: 16px;
    }

    .action-btn {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 24px;
      background: #f7fafc;
      border: 2px solid #e2e8f0;
      border-radius: 12px;
      cursor: pointer;
      transition: all 0.2s;
    }

    .action-btn:hover {
      background: #667eea;
      border-color: #667eea;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .action-btn:hover .action-icon,
    .action-btn:hover .action-text {
      color: white;
    }

    .action-icon {
      font-size: 32px;
    }

    .action-text {
      font-size: 14px;
      font-weight: 600;
      color: #2d3748;
      text-align: center;
    }

    @media (max-width: 768px) {
      .dashboard {
        padding: 16px;
      }

      h1 {
        font-size: 24px;
      }

      .analytics-grid,
      .distribution-grid,
      .actions-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
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
