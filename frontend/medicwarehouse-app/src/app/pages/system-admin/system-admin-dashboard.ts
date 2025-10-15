import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { SystemAdminService } from '../../services/system-admin';
import { SystemAnalytics } from '../../models/system-admin.model';

@Component({
  selector: 'app-system-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="system-admin-dashboard">
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

        <!-- Quick Actions -->
        <div class="quick-actions">
          <h2>Ações Rápidas</h2>
          <div class="actions-grid">
            <button (click)="navigateToClinics()" class="action-btn">
              <i class="icon">📋</i>
              <span>Gerenciar Clínicas</span>
            </button>
            <button (click)="navigateToClinics('active')" class="action-btn">
              <i class="icon">✅</i>
              <span>Ver Clínicas Ativas</span>
            </button>
            <button (click)="navigateToClinics('inactive')" class="action-btn">
              <i class="icon">❌</i>
              <span>Ver Clínicas Inativas</span>
            </button>
          </div>
        </div>

        <!-- Subscription Details -->
        <div class="subscription-details">
          <div class="detail-card">
            <h3>Assinaturas por Status</h3>
            <div class="status-list">
              @for (item of getSubscriptionsByStatus(); track item.status) {
                <div class="status-item">
                  <span class="status-name">{{ item.status }}</span>
                  <span class="status-count">{{ item.count }}</span>
                </div>
              }
            </div>
          </div>

          <div class="detail-card">
            <h3>Assinaturas por Plano</h3>
            <div class="plan-list">
              @for (item of getSubscriptionsByPlan(); track item.plan) {
                <div class="plan-item">
                  <span class="plan-name">{{ item.plan }}</span>
                  <span class="plan-count">{{ item.count }}</span>
                </div>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .system-admin-dashboard {
      padding: 24px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .header {
      margin-bottom: 32px;
    }

    .header h1 {
      font-size: 32px;
      font-weight: 700;
      color: #1a1a1a;
      margin: 0 0 8px 0;
    }

    .subtitle {
      font-size: 16px;
      color: #666;
      margin: 0;
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
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      padding: 24px;
      transition: transform 0.2s, box-shadow 0.2s;
    }

    .card:hover {
      transform: translateY(-4px);
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
    }

    .card.highlight {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }

    .card-header h3 {
      font-size: 18px;
      font-weight: 600;
      margin: 0;
    }

    .card.highlight .card-header h3 {
      color: white;
    }

    .icon {
      font-size: 32px;
    }

    .card-body {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .stat-large {
      font-size: 48px;
      font-weight: 700;
      line-height: 1;
    }

    .stat-details {
      display: flex;
      flex-direction: column;
      gap: 8px;
      font-size: 14px;
    }

    .stat-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .stat-label {
      color: #666;
    }

    .card.highlight .stat-label {
      color: rgba(255, 255, 255, 0.9);
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

    .quick-actions {
      margin-bottom: 32px;
    }

    .quick-actions h2 {
      font-size: 24px;
      font-weight: 600;
      margin-bottom: 16px;
    }

    .actions-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
    }

    .action-btn {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 12px;
      padding: 24px;
      background: white;
      border: 2px solid #e5e7eb;
      border-radius: 12px;
      cursor: pointer;
      transition: all 0.2s;
      font-size: 16px;
      font-weight: 500;
      color: #1a1a1a;
    }

    .action-btn:hover {
      border-color: #667eea;
      background: #f9fafb;
      transform: translateY(-2px);
    }

    .action-btn .icon {
      font-size: 48px;
    }

    .subscription-details {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 24px;
    }

    .detail-card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      padding: 24px;
    }

    .detail-card h3 {
      font-size: 20px;
      font-weight: 600;
      margin: 0 0 16px 0;
    }

    .status-list, .plan-list {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }

    .status-item, .plan-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px;
      background: #f9fafb;
      border-radius: 8px;
    }

    .status-name, .plan-name {
      font-weight: 500;
      color: #1a1a1a;
    }

    .status-count, .plan-count {
      font-weight: 600;
      color: #667eea;
      font-size: 18px;
    }

    .loading, .error {
      text-align: center;
      padding: 48px;
      font-size: 18px;
      color: #666;
    }

    .error {
      color: #ef4444;
    }

    .btn-retry {
      margin-top: 16px;
      padding: 12px 24px;
      background: #667eea;
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 16px;
      font-weight: 500;
      cursor: pointer;
      transition: background 0.2s;
    }

    .btn-retry:hover {
      background: #5568d3;
    }
  `]
})
export class SystemAdminDashboard implements OnInit {
  analytics = signal<SystemAnalytics | null>(null);
  loading = signal<boolean>(true);
  error = signal<string | null>(null);

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadAnalytics();
  }

  loadAnalytics() {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getAnalytics().subscribe({
      next: (data: SystemAnalytics) => {
        this.analytics.set(data);
        this.loading.set(false);
      },
      error: (err: any) => {
        this.error.set(err.message || 'Erro ao carregar dados');
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

  navigateToClinics(status?: string) {
    if (status) {
      this.router.navigate(['/system-admin/clinics'], { queryParams: { status } });
    } else {
      this.router.navigate(['/system-admin/clinics']);
    }
  }

  getSubscriptionsByStatus(): Array<{ status: string; count: number }> {
    const data = this.analytics()?.subscriptionsByStatus;
    if (!data || typeof data !== 'object') return [];
    
    return Object.entries(data).map(([status, count]) => ({
      status,
      count: count as number
    }));
  }

  getSubscriptionsByPlan(): Array<{ plan: string; count: number }> {
    const data = this.analytics()?.subscriptionsByPlan;
    if (!data || typeof data !== 'object') return [];
    
    return Object.entries(data).map(([plan, count]) => ({
      plan,
      count: count as number
    }));
  }
}
