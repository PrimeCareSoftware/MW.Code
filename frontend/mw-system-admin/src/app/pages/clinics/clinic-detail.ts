import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { ClinicDetail as ClinicDetailModel, UpdateClinicRequest, EnableManualOverrideRequest, SubscriptionPlan, UpdateSubscriptionRequest } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinic-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="clinic-detail">
      <div class="header">
        <div>
          <button class="btn-back" (click)="goBack()">← Voltar</button>
          <h1>Detalhes da Clínica</h1>
        </div>
      </div>

      @if (loading()) {
        <div class="loading">Carregando dados...</div>
      } @else if (error()) {
        <div class="error-message">{{ error() }}</div>
      } @else if (clinic()) {
        <div class="content">
          <!-- Clinic Info Card -->
          <div class="card">
            <div class="card-header">
              <h2>Informações da Clínica</h2>
              @if (!editMode()) {
                <button class="btn btn-secondary" (click)="toggleEditMode()">
                  ✏️ Editar
                </button>
              }
            </div>

            @if (editMode()) {
              <form (ngSubmit)="saveClinicChanges()" class="edit-form">
                <div class="form-grid">
                  <div class="form-group">
                    <label>Nome</label>
                    <input type="text" [(ngModel)]="editData.name" name="name" required />
                  </div>
                  <div class="form-group">
                    <label>CNPJ</label>
                    <input type="text" [(ngModel)]="editData.document" name="document" required />
                  </div>
                  <div class="form-group">
                    <label>Email</label>
                    <input type="email" [(ngModel)]="editData.email" name="email" required />
                  </div>
                  <div class="form-group">
                    <label>Telefone</label>
                    <input type="tel" [(ngModel)]="editData.phone" name="phone" required />
                  </div>
                  <div class="form-group full-width">
                    <label>Endereço</label>
                    <input type="text" [(ngModel)]="editData.address" name="address" required />
                  </div>
                </div>

                @if (saveError()) {
                  <div class="error-message">{{ saveError() }}</div>
                }

                <div class="form-actions">
                  <button type="button" class="btn btn-secondary" (click)="cancelEdit()">
                    Cancelar
                  </button>
                  <button type="submit" class="btn btn-primary" [disabled]="saving()">
                    @if (saving()) {
                      <span>Salvando...</span>
                    } @else {
                      <span>Salvar Alterações</span>
                    }
                  </button>
                </div>
              </form>
            } @else {
              <div class="info-grid">
                <div class="info-item">
                  <span class="label">Nome:</span>
                  <span class="value">{{ clinic()!.name }}</span>
                </div>
                <div class="info-item">
                  <span class="label">CNPJ:</span>
                  <span class="value">{{ clinic()!.document }}</span>
                </div>
                <div class="info-item">
                  <span class="label">Email:</span>
                  <span class="value">{{ clinic()!.email }}</span>
                </div>
                <div class="info-item">
                  <span class="label">Telefone:</span>
                  <span class="value">{{ clinic()!.phone }}</span>
                </div>
                <div class="info-item">
                  <span class="label">Endereço:</span>
                  <span class="value">{{ clinic()!.address }}</span>
                </div>
                <div class="info-item">
                  <span class="label">Tenant ID:</span>
                  <span class="value">{{ clinic()!.tenantId }}</span>
                </div>
                <div class="info-item">
                  <span class="label">Status:</span>
                  <span class="badge" [class.active]="clinic()!.isActive" [class.inactive]="!clinic()!.isActive">
                    {{ clinic()!.isActive ? 'Ativa' : 'Inativa' }}
                  </span>
                </div>
                <div class="info-item">
                  <span class="label">Criada em:</span>
                  <span class="value">{{ formatDate(clinic()!.createdAt) }}</span>
                </div>
              </div>
            }
          </div>

          <!-- Subscription Card -->
          <div class="card">
            <div class="card-header">
              <h2>Assinatura e Plano</h2>
            </div>

            <div class="info-grid">
              <div class="info-item">
                <span class="label">Plano Atual:</span>
                <span class="value">{{ clinic()!.planName }}</span>
              </div>
              <div class="info-item">
                <span class="label">Valor:</span>
                <span class="value">{{ formatCurrency(clinic()!.planPrice) }}/mês</span>
              </div>
              <div class="info-item">
                <span class="label">Status:</span>
                <span class="badge" [class]="getStatusClass(clinic()!.subscriptionStatus)">
                  {{ getStatusLabel(clinic()!.subscriptionStatus) }}
                </span>
              </div>
              @if (clinic()!.trialEndsAt) {
                <div class="info-item">
                  <span class="label">Trial termina em:</span>
                  <span class="value">{{ formatDate(clinic()!.trialEndsAt) }}</span>
                </div>
              }
              @if (clinic()!.nextBillingDate) {
                <div class="info-item">
                  <span class="label">Próximo pagamento:</span>
                  <span class="value">{{ formatDate(clinic()!.nextBillingDate) }}</span>
                </div>
              }
            </div>

            <div class="subscription-actions">
              <button class="btn btn-secondary" (click)="showChangePlanModal = true">
                Alterar Plano
              </button>
              <button class="btn btn-primary" (click)="showManualOverrideModal = true">
                🔓 Override Manual
              </button>
            </div>
          </div>

          <!-- Users Card -->
          <div class="card">
            <div class="card-header">
              <h2>Usuários</h2>
            </div>

            <div class="info-grid">
              <div class="info-item">
                <span class="label">Total de Usuários:</span>
                <span class="value">{{ clinic()!.totalUsers }}</span>
              </div>
              <div class="info-item">
                <span class="label">Usuários Ativos:</span>
                <span class="value success">{{ clinic()!.activeUsers }}</span>
              </div>
            </div>

            <div class="subscription-actions">
              <button class="btn btn-secondary" (click)="navigateToOwners()">
                Gerenciar Proprietários
              </button>
            </div>
          </div>
        </div>

        <!-- Manual Override Modal -->
        @if (showManualOverrideModal) {
          <div class="modal-overlay" (click)="showManualOverrideModal = false">
            <div class="modal" (click)="$event.stopPropagation()">
              <div class="modal-header">
                <h2>Ativar Override Manual</h2>
                <button class="btn-close" (click)="showManualOverrideModal = false">✕</button>
              </div>
              
              <div class="modal-body">
                <p>O override manual permite estender o acesso da clínica mesmo após o vencimento do trial ou falta de pagamento.</p>
                
                <div class="form-group">
                  <label>Motivo *</label>
                  <textarea
                    [(ngModel)]="overrideReason"
                    placeholder="Ex: Cliente solicitou extensão para teste..."
                    rows="3"
                  ></textarea>
                </div>

                <div class="form-group">
                  <label>Estender até (opcional)</label>
                  <input
                    type="date"
                    [(ngModel)]="overrideExtendUntil"
                  />
                  <small>Deixe em branco para acesso indefinido</small>
                </div>

                @if (overrideError()) {
                  <div class="error-message">{{ overrideError() }}</div>
                }
              </div>

              <div class="modal-footer">
                <button class="btn btn-secondary" (click)="showManualOverrideModal = false">
                  Cancelar
                </button>
                <button class="btn btn-primary" (click)="activateManualOverride()" [disabled]="!overrideReason || overrideProcessing()">
                  @if (overrideProcessing()) {
                    <span>Ativando...</span>
                  } @else {
                    <span>Ativar Override</span>
                  }
                </button>
              </div>
            </div>
          </div>
        }

        <!-- Change Plan Modal -->
        @if (showChangePlanModal) {
          <div class="modal-overlay" (click)="showChangePlanModal = false">
            <div class="modal" (click)="$event.stopPropagation()">
              <div class="modal-header">
                <h2>Alterar Plano</h2>
                <button class="btn-close" (click)="showChangePlanModal = false">✕</button>
              </div>
              
              <div class="modal-body">
                <div class="form-group">
                  <label>Novo Plano *</label>
                  <select [(ngModel)]="newPlanId">
                    <option value="">Selecione um plano</option>
                    @for (plan of availablePlans(); track plan.id) {
                      <option [value]="plan.id">
                        {{ plan.name }} - R$ {{ plan.monthlyPrice.toFixed(2) }}/mês
                      </option>
                    }
                  </select>
                </div>

                @if (changePlanError()) {
                  <div class="error-message">{{ changePlanError() }}</div>
                }
              </div>

              <div class="modal-footer">
                <button class="btn btn-secondary" (click)="showChangePlanModal = false">
                  Cancelar
                </button>
                <button class="btn btn-primary" (click)="changePlan()" [disabled]="!newPlanId || changePlanProcessing()">
                  @if (changePlanProcessing()) {
                    <span>Alterando...</span>
                  } @else {
                    <span>Alterar Plano</span>
                  }
                </button>
              </div>
            </div>
          </div>
        }
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
      background: none;
      border: none;
      color: #667eea;
      font-size: 16px;
      cursor: pointer;
      padding: 8px 0;
      margin-bottom: 8px;
      font-weight: 600;
    }

    .btn-back:hover {
      text-decoration: underline;
    }

    h1 {
      font-size: 32px;
      font-weight: 700;
      color: #1a202c;
      margin: 0;
    }

    .loading, .error-message {
      text-align: center;
      padding: 48px 24px;
      color: #718096;
    }

    .error-message {
      background: #fee2e2;
      color: #991b1b;
      border-radius: 8px;
    }

    .content {
      display: flex;
      flex-direction: column;
      gap: 24px;
    }

    .card {
      background: white;
      border-radius: 12px;
      padding: 24px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
      padding-bottom: 12px;
      border-bottom: 2px solid #e2e8f0;
    }

    .card-header h2 {
      font-size: 20px;
      font-weight: 600;
      color: #1a202c;
      margin: 0;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 16px;
    }

    .info-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .label {
      font-size: 13px;
      font-weight: 600;
      color: #718096;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .value {
      font-size: 15px;
      color: #1a202c;
    }

    .value.success {
      color: #10b981;
      font-weight: 600;
    }

    .badge {
      padding: 4px 12px;
      border-radius: 6px;
      font-size: 12px;
      font-weight: 600;
      display: inline-block;
      width: fit-content;
    }

    .badge.active {
      background: #d1fae5;
      color: #065f46;
    }

    .badge.inactive {
      background: #fee2e2;
      color: #991b1b;
    }

    .badge.badge-active {
      background: #d1fae5;
      color: #065f46;
    }

    .badge.badge-trial {
      background: #fef3c7;
      color: #92400e;
    }

    .badge.badge-expired {
      background: #fee2e2;
      color: #991b1b;
    }

    .subscription-actions {
      margin-top: 20px;
      padding-top: 20px;
      border-top: 2px solid #e2e8f0;
      display: flex;
      gap: 12px;
    }

    .btn {
      padding: 12px 24px;
      border: none;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #5568d3;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .btn-secondary {
      background: #e2e8f0;
      color: #2d3748;
    }

    .btn-secondary:hover:not(:disabled) {
      background: #cbd5e0;
    }

    .edit-form {
      margin-top: 20px;
    }

    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 20px;
      margin-bottom: 20px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .form-group.full-width {
      grid-column: 1 / -1;
    }

    .form-group label {
      font-size: 14px;
      font-weight: 600;
      color: #2d3748;
    }

    .form-group small {
      font-size: 12px;
      color: #718096;
    }

    input, select, textarea {
      padding: 12px;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 14px;
      transition: border-color 0.2s;
    }

    input:focus, select:focus, textarea:focus {
      outline: none;
      border-color: #667eea;
    }

    .form-actions {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
    }

    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }

    .modal {
      background: white;
      border-radius: 12px;
      width: 90%;
      max-width: 500px;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px;
      border-bottom: 2px solid #e2e8f0;
    }

    .modal-header h2 {
      font-size: 20px;
      font-weight: 600;
      color: #1a202c;
      margin: 0;
    }

    .btn-close {
      background: none;
      border: none;
      font-size: 24px;
      color: #718096;
      cursor: pointer;
      padding: 0;
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 6px;
    }

    .btn-close:hover {
      background: #f7fafc;
      color: #1a202c;
    }

    .modal-body {
      padding: 24px;
    }

    .modal-body p {
      color: #718096;
      margin-bottom: 20px;
    }

    .modal-footer {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
      padding: 24px;
      border-top: 2px solid #e2e8f0;
    }

    @media (max-width: 768px) {
      .clinic-detail {
        padding: 16px;
      }

      h1 {
        font-size: 24px;
      }

      .info-grid {
        grid-template-columns: 1fr;
      }

      .form-grid {
        grid-template-columns: 1fr;
      }

      .subscription-actions {
        flex-direction: column;
      }
    }
  `]
})
export class ClinicDetail implements OnInit {
  clinic = signal<ClinicDetailModel | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  editMode = signal(false);
  saving = signal(false);
  saveError = signal<string | null>(null);

  editData: UpdateClinicRequest = {
    name: '',
    document: '',
    email: '',
    phone: '',
    address: ''
  };

  // Manual Override
  showManualOverrideModal = false;
  overrideReason = '';
  overrideExtendUntil = '';
  overrideProcessing = signal(false);
  overrideError = signal<string | null>(null);

  // Change Plan
  showChangePlanModal = false;
  newPlanId = '';
  availablePlans = signal<SubscriptionPlan[]>([]);
  changePlanProcessing = signal(false);
  changePlanError = signal<string | null>(null);

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadClinic(id);
      this.loadPlans();
    }
  }

  loadClinic(id: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getClinic(id).subscribe({
      next: (data) => {
        this.clinic.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar clínica');
        this.loading.set(false);
      }
    });
  }

  loadPlans(): void {
    this.systemAdminService.getSubscriptionPlans(true).subscribe({
      next: (data) => {
        this.availablePlans.set(data);
      },
      error: (err) => {
        console.error('Error loading plans:', err);
      }
    });
  }

  toggleEditMode(): void {
    const c = this.clinic();
    if (c) {
      this.editData = {
        name: c.name,
        document: c.document,
        email: c.email,
        phone: c.phone,
        address: c.address
      };
    }
    this.editMode.set(true);
  }

  cancelEdit(): void {
    this.editMode.set(false);
    this.saveError.set(null);
  }

  saveClinicChanges(): void {
    const c = this.clinic();
    if (!c) return;

    this.saving.set(true);
    this.saveError.set(null);

    this.systemAdminService.updateClinic(c.id, this.editData).subscribe({
      next: () => {
        this.editMode.set(false);
        this.saving.set(false);
        this.loadClinic(c.id);
      },
      error: (err) => {
        this.saveError.set(err.error?.message || 'Erro ao salvar alterações');
        this.saving.set(false);
      }
    });
  }

  activateManualOverride(): void {
    const c = this.clinic();
    if (!c || !this.overrideReason) return;

    this.overrideProcessing.set(true);
    this.overrideError.set(null);

    const request: EnableManualOverrideRequest = {
      reason: this.overrideReason,
      extendUntil: this.overrideExtendUntil || undefined
    };

    this.systemAdminService.enableManualOverrideExtended(c.id, request).subscribe({
      next: () => {
        this.showManualOverrideModal = false;
        this.overrideProcessing.set(false);
        this.overrideReason = '';
        this.overrideExtendUntil = '';
        this.loadClinic(c.id);
        alert('Override manual ativado com sucesso!');
      },
      error: (err) => {
        this.overrideError.set(err.error?.message || 'Erro ao ativar override');
        this.overrideProcessing.set(false);
      }
    });
  }

  changePlan(): void {
    const c = this.clinic();
    if (!c || !this.newPlanId) return;

    this.changePlanProcessing.set(true);
    this.changePlanError.set(null);

    const request: UpdateSubscriptionRequest = {
      newPlanId: this.newPlanId
    };

    this.systemAdminService.updateSubscription(c.id, request).subscribe({
      next: () => {
        this.showChangePlanModal = false;
        this.changePlanProcessing.set(false);
        this.newPlanId = '';
        this.loadClinic(c.id);
        alert('Plano alterado com sucesso!');
      },
      error: (err) => {
        this.changePlanError.set(err.error?.message || 'Erro ao alterar plano');
        this.changePlanProcessing.set(false);
      }
    });
  }

  navigateToOwners(): void {
    const c = this.clinic();
    if (c) {
      this.router.navigate(['/clinic-owners'], { queryParams: { clinicId: c.id } });
    }
  }

  goBack(): void {
    this.router.navigate(['/clinics']);
  }

  formatDate(date?: string): string {
    if (!date) return '-';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getStatusLabel(status: string): string {
    const labels: { [key: string]: string } = {
      'Active': 'Ativo',
      'Trial': 'Trial',
      'Expired': 'Expirado',
      'Suspended': 'Suspenso',
      'PaymentOverdue': 'Pagamento Atrasado',
      'Cancelled': 'Cancelado'
    };
    return labels[status] || status;
  }

  getStatusClass(status: string): string {
    const classes: { [key: string]: string } = {
      'Active': 'badge-active',
      'Trial': 'badge-trial',
      'Expired': 'badge-expired',
      'Suspended': 'badge-suspended',
      'PaymentOverdue': 'badge-expired',
      'Cancelled': 'badge-cancelled'
    };
    return classes[status] || '';
  }
}
