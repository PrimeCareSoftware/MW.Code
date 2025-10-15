import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { SystemAdminService } from '../../services/system-admin';
import { ClinicDetail } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinic-detail',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="clinic-detail">
      <div class="header">
        <button (click)="goBack()" class="btn-back">← Voltar à Lista</button>
      </div>

      @if (loading()) {
        <div class="loading">Carregando detalhes da clínica...</div>
      } @else if (error()) {
        <div class="error">
          <p>Erro ao carregar detalhes: {{ error() }}</p>
          <button (click)="loadClinic()" class="btn-retry">Tentar novamente</button>
        </div>
      } @else if (clinic()) {
        <div class="detail-container">
          <!-- Main Info Card -->
          <div class="card main-info">
            <div class="card-header">
              <div>
                <h1>{{ clinic()!.name }}</h1>
                <span class="badge" [class.badge-success]="clinic()!.isActive" [class.badge-error]="!clinic()!.isActive">
                  {{ clinic()!.isActive ? 'Ativa' : 'Inativa' }}
                </span>
              </div>
              <div class="actions">
                <button 
                  (click)="toggleStatus()" 
                  [class.btn-danger]="clinic()!.isActive"
                  [class.btn-success]="!clinic()!.isActive"
                  class="btn-action">
                  {{ clinic()!.isActive ? '🚫 Desativar Clínica' : '✅ Ativar Clínica' }}
                </button>
              </div>
            </div>

            <div class="info-grid">
              <div class="info-item">
                <label>CNPJ:</label>
                <span>{{ clinic()!.document }}</span>
              </div>
              <div class="info-item">
                <label>Email:</label>
                <span>{{ clinic()!.email }}</span>
              </div>
              <div class="info-item">
                <label>Telefone:</label>
                <span>{{ clinic()!.phone }}</span>
              </div>
              <div class="info-item">
                <label>Endereço:</label>
                <span>{{ clinic()!.address }}</span>
              </div>
              <div class="info-item">
                <label>Tenant ID:</label>
                <span class="monospace">{{ clinic()!.tenantId }}</span>
              </div>
              <div class="info-item">
                <label>Data de Criação:</label>
                <span>{{ formatDate(clinic()!.createdAt) }}</span>
              </div>
            </div>
          </div>

          <!-- Subscription Info -->
          <div class="card">
            <h2>Informações de Assinatura</h2>
            <div class="subscription-info">
              <div class="info-row">
                <div class="info-item">
                  <label>Plano:</label>
                  <span class="highlight">{{ clinic()!.planName }}</span>
                </div>
                <div class="info-item">
                  <label>Valor Mensal:</label>
                  <span class="highlight price">{{ formatCurrency(clinic()!.planPrice) }}</span>
                </div>
              </div>
              <div class="info-row">
                <div class="info-item">
                  <label>Status:</label>
                  <span class="badge" [class]="'badge-' + getStatusClass(clinic()!.subscriptionStatus)">
                    {{ clinic()!.subscriptionStatus }}
                  </span>
                </div>
                @if (clinic()!.nextBillingDate) {
                  <div class="info-item">
                    <label>Próxima Cobrança:</label>
                    <span>{{ formatDate(clinic()!.nextBillingDate) }}</span>
                  </div>
                }
              </div>
              @if (clinic()!.trialEndsAt) {
                <div class="info-row">
                  <div class="info-item">
                    <label>Período de Teste Termina:</label>
                    <span class="trial-date">{{ formatDate(clinic()!.trialEndsAt) }}</span>
                  </div>
                </div>
              }
            </div>

            <div class="subscription-actions">
              <button (click)="enableManualOverride()" class="btn-secondary">
                🔓 Ativar Override Manual
              </button>
              <button (click)="disableManualOverride()" class="btn-secondary">
                🔒 Desativar Override Manual
              </button>
            </div>
          </div>

          <!-- Users Stats -->
          <div class="card">
            <h2>Usuários da Clínica</h2>
            <div class="stats-grid">
              <div class="stat-card">
                <div class="stat-value">{{ clinic()!.totalUsers }}</div>
                <div class="stat-label">Total de Usuários</div>
              </div>
              <div class="stat-card">
                <div class="stat-value success">{{ clinic()!.activeUsers }}</div>
                <div class="stat-label">Usuários Ativos</div>
              </div>
              <div class="stat-card">
                <div class="stat-value error">{{ clinic()!.totalUsers - clinic()!.activeUsers }}</div>
                <div class="stat-label">Usuários Inativos</div>
              </div>
            </div>
          </div>

          <!-- System Info -->
          <div class="card system-info">
            <h2>Informações do Sistema</h2>
            <div class="info-grid">
              <div class="info-item">
                <label>ID da Clínica:</label>
                <span class="monospace">{{ clinic()!.id }}</span>
              </div>
              <div class="info-item">
                <label>Tenant ID:</label>
                <span class="monospace">{{ clinic()!.tenantId }}</span>
              </div>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .clinic-detail {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .header {
      margin-bottom: 24px;
    }

    .btn-back {
      padding: 12px 24px;
      background: #f3f4f6;
      color: #1a1a1a;
      border: none;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      transition: background 0.2s;
    }

    .btn-back:hover {
      background: #e5e7eb;
    }

    .detail-container {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }

    .card {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      padding: 24px;
    }

    .card h1 {
      font-size: 32px;
      font-weight: 700;
      color: #1a1a1a;
      margin: 0 16px 0 0;
    }

    .card h2 {
      font-size: 20px;
      font-weight: 600;
      color: #1a1a1a;
      margin: 0 0 20px 0;
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 24px;
    }

    .card-header > div:first-child {
      display: flex;
      align-items: center;
      flex-wrap: wrap;
      gap: 12px;
    }

    .badge {
      display: inline-block;
      padding: 6px 16px;
      border-radius: 16px;
      font-size: 14px;
      font-weight: 500;
    }

    .badge-success {
      background: #d1fae5;
      color: #065f46;
    }

    .badge-error {
      background: #fee2e2;
      color: #991b1b;
    }

    .badge-warning {
      background: #fef3c7;
      color: #92400e;
    }

    .badge-info {
      background: #e0e7ff;
      color: #3730a3;
    }

    .actions {
      display: flex;
      gap: 12px;
    }

    .btn-action, .btn-secondary {
      padding: 12px 24px;
      border: none;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-action.btn-danger {
      background: #fee2e2;
      color: #991b1b;
    }

    .btn-action.btn-danger:hover {
      background: #fecaca;
    }

    .btn-action.btn-success {
      background: #d1fae5;
      color: #065f46;
    }

    .btn-action.btn-success:hover {
      background: #a7f3d0;
    }

    .btn-secondary {
      background: #e0e7ff;
      color: #3730a3;
    }

    .btn-secondary:hover {
      background: #c7d2fe;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
      gap: 20px;
    }

    .info-item {
      display: flex;
      flex-direction: column;
      gap: 6px;
    }

    .info-item label {
      font-size: 12px;
      font-weight: 600;
      text-transform: uppercase;
      color: #666;
      letter-spacing: 0.5px;
    }

    .info-item span {
      font-size: 16px;
      color: #1a1a1a;
    }

    .info-item span.monospace {
      font-family: 'Courier New', monospace;
      font-size: 14px;
      background: #f9fafb;
      padding: 4px 8px;
      border-radius: 4px;
    }

    .info-item span.highlight {
      font-weight: 600;
      color: #667eea;
    }

    .info-item span.price {
      font-size: 24px;
    }

    .subscription-info {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin-bottom: 24px;
    }

    .info-row {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
    }

    .subscription-actions {
      display: flex;
      gap: 12px;
      flex-wrap: wrap;
    }

    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 16px;
    }

    .stat-card {
      background: #f9fafb;
      padding: 20px;
      border-radius: 8px;
      text-align: center;
    }

    .stat-value {
      font-size: 36px;
      font-weight: 700;
      color: #1a1a1a;
      margin-bottom: 8px;
    }

    .stat-value.success {
      color: #10b981;
    }

    .stat-value.error {
      color: #ef4444;
    }

    .stat-label {
      font-size: 14px;
      color: #666;
    }

    .trial-date {
      color: #f59e0b;
      font-weight: 600;
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
export class ClinicDetailComponent implements OnInit {
  clinic = signal<ClinicDetail | null>(null);
  loading = signal<boolean>(true);
  error = signal<string | null>(null);
  clinicId: string = '';

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.clinicId = params['id'];
      this.loadClinic();
    });
  }

  loadClinic() {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getClinic(this.clinicId).subscribe({
      next: (data: ClinicDetail) => {
        this.clinic.set(data);
        this.loading.set(false);
      },
      error: (err: any) => {
        this.error.set(err.message || 'Erro ao carregar detalhes da clínica');
        this.loading.set(false);
      }
    });
  }

  toggleStatus() {
    const clinic = this.clinic();
    if (!clinic) return;

    const action = clinic.isActive ? 'desativar' : 'ativar';
    if (!confirm(`Tem certeza que deseja ${action} a clínica "${clinic.name}"?`)) {
      return;
    }

    this.systemAdminService.toggleClinicStatus(this.clinicId).subscribe({
      next: (response: { message: string; isActive: boolean }) => {
        alert(response.message);
        this.loadClinic(); // Reload clinic
      },
      error: (err: any) => {
        alert('Erro ao alterar status: ' + (err.message || 'Erro desconhecido'));
      }
    });
  }

  enableManualOverride() {
    const reason = prompt('Informe o motivo para ativar o override manual:');
    if (!reason) return;

    this.systemAdminService.enableManualOverride(this.clinicId, { reason }).subscribe({
      next: (response: any) => {
        alert('Override manual ativado com sucesso!');
        this.loadClinic();
      },
      error: (err: any) => {
        alert('Erro ao ativar override: ' + (err.message || 'Erro desconhecido'));
      }
    });
  }

  disableManualOverride() {
    if (!confirm('Tem certeza que deseja desativar o override manual?')) {
      return;
    }

    this.systemAdminService.disableManualOverride(this.clinicId).subscribe({
      next: (response: { message: string }) => {
        alert(response.message);
        this.loadClinic();
      },
      error: (err: any) => {
        alert('Erro ao desativar override: ' + (err.message || 'Erro desconhecido'));
      }
    });
  }

  goBack() {
    this.router.navigate(['/system-admin/clinics']);
  }

  formatDate(dateStr: string | undefined): string {
    if (!dateStr) return 'N/A';
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getStatusClass(status: string): string {
    const statusMap: { [key: string]: string } = {
      'Active': 'success',
      'Ativa': 'success',
      'Trial': 'warning',
      'Teste': 'warning',
      'Expired': 'error',
      'Expirada': 'error',
      'Canceled': 'error',
      'Cancelada': 'error',
      'Pending': 'info',
      'Pendente': 'info'
    };
    return statusMap[status] || 'info';
  }
}
