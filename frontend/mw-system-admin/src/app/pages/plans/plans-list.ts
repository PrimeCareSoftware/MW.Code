import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { SubscriptionPlan, CreateSubscriptionPlanRequest, UpdateSubscriptionPlanRequest } from '../../models/system-admin.model';

@Component({
  selector: 'app-plans-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="plans-list">
      <div class="header">
        <div>
          <h1>Gerenciar Planos de Assinatura</h1>
          <p class="subtitle">Configure os planos disponíveis para as clínicas</p>
        </div>
        <button class="btn btn-primary" (click)="openCreateModal()">
          <span>➕ Novo Plano</span>
        </button>
      </div>

      @if (loading()) {
        <div class="loading">Carregando planos...</div>
      } @else if (error()) {
        <div class="error">
          <p>Erro: {{ error() }}</p>
          <button (click)="loadPlans()" class="btn-retry">Tentar novamente</button>
        </div>
      } @else {
        <div class="table-container">
          <table class="plans-table">
            <thead>
              <tr>
                <th>Plano</th>
                <th>Preço Mensal</th>
                <th>Preço Anual</th>
                <th>Trial (dias)</th>
                <th>Max Usuários</th>
                <th>Max Pacientes</th>
                <th>Status</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              @for (plan of plans(); track plan.id) {
                <tr>
                  <td>
                    <div class="plan-info">
                      <div class="plan-name">{{ plan.name }}</div>
                      @if (plan.description) {
                        <div class="plan-desc">{{ plan.description }}</div>
                      }
                    </div>
                  </td>
                  <td>{{ formatCurrency(plan.monthlyPrice) }}</td>
                  <td>{{ formatCurrency(plan.yearlyPrice) }}</td>
                  <td>{{ plan.trialDays }} dias</td>
                  <td>{{ plan.maxUsers }}</td>
                  <td>{{ plan.maxPatients }}</td>
                  <td>
                    <span class="status-badge" [class.active]="plan.isActive" [class.inactive]="!plan.isActive">
                      {{ plan.isActive ? '✅ Ativo' : '🚫 Inativo' }}
                    </span>
                  </td>
                  <td>
                    <div class="actions">
                      <button class="btn-icon" (click)="openEditModal(plan)" title="Editar">
                        ✏️
                      </button>
                      <button 
                        class="btn-icon" 
                        [class.btn-danger]="plan.isActive"
                        [class.btn-success]="!plan.isActive"
                        (click)="toggleStatus(plan.id, plan.isActive)" 
                        [title]="plan.isActive ? 'Desativar' : 'Ativar'"
                      >
                        {{ plan.isActive ? '🚫' : '✅' }}
                      </button>
                      <button class="btn-icon btn-danger" (click)="deletePlan(plan.id)" title="Excluir">
                        🗑️
                      </button>
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>

          @if (plans().length === 0) {
            <div class="empty-state">
              <p>Nenhum plano cadastrado</p>
            </div>
          }
        </div>
      }

      <!-- Create/Edit Modal -->
      @if (showModal) {
        <div class="modal-overlay" (click)="closeModal()">
          <div class="modal" (click)="$event.stopPropagation()">
            <div class="modal-header">
              <h2>{{ editingPlan ? 'Editar Plano' : 'Criar Novo Plano' }}</h2>
              <button class="btn-close" (click)="closeModal()">✕</button>
            </div>
            
            <form (ngSubmit)="onSubmit()" class="modal-body">
              <div class="form-group">
                <label>Nome do Plano *</label>
                <input
                  type="text"
                  [(ngModel)]="formData.name"
                  name="name"
                  required
                  placeholder="Ex: Plano Básico"
                />
              </div>

              <div class="form-group">
                <label>Descrição</label>
                <textarea
                  [(ngModel)]="formData.description"
                  name="description"
                  rows="3"
                  placeholder="Descrição opcional do plano"
                ></textarea>
              </div>

              <div class="form-grid">
                <div class="form-group">
                  <label>Preço Mensal (R$) *</label>
                  <input
                    type="number"
                    [(ngModel)]="formData.monthlyPrice"
                    name="monthlyPrice"
                    required
                    min="0"
                    step="0.01"
                    placeholder="0.00"
                  />
                </div>

                <div class="form-group">
                  <label>Preço Anual (R$) *</label>
                  <input
                    type="number"
                    [(ngModel)]="formData.yearlyPrice"
                    name="yearlyPrice"
                    required
                    min="0"
                    step="0.01"
                    placeholder="0.00"
                  />
                </div>

                <div class="form-group">
                  <label>Dias de Trial *</label>
                  <input
                    type="number"
                    [(ngModel)]="formData.trialDays"
                    name="trialDays"
                    required
                    min="0"
                    placeholder="14"
                  />
                </div>

                <div class="form-group">
                  <label>Max Usuários *</label>
                  <input
                    type="number"
                    [(ngModel)]="formData.maxUsers"
                    name="maxUsers"
                    required
                    min="1"
                    placeholder="10"
                  />
                </div>

                <div class="form-group">
                  <label>Max Pacientes *</label>
                  <input
                    type="number"
                    [(ngModel)]="formData.maxPatients"
                    name="maxPatients"
                    required
                    min="1"
                    placeholder="100"
                  />
                </div>

                @if (editingPlan) {
                  <div class="form-group">
                    <label>Status</label>
                    <select [(ngModel)]="formDataUpdate.isActive" name="isActive">
                      <option [value]="true">Ativo</option>
                      <option [value]="false">Inativo</option>
                    </select>
                  </div>
                }
              </div>

              @if (modalError()) {
                <div class="error-message">{{ modalError() }}</div>
              }
            </form>

            <div class="modal-footer">
              <button type="button" class="btn btn-secondary" (click)="closeModal()">
                Cancelar
              </button>
              <button type="button" class="btn btn-primary" (click)="onSubmit()" [disabled]="submitting()">
                @if (submitting()) {
                  <span>Salvando...</span>
                } @else {
                  <span>{{ editingPlan ? 'Salvar' : 'Criar' }}</span>
                }
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .plans-list {
      padding: 24px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 24px;
      flex-wrap: wrap;
      gap: 16px;
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

    .btn {
      padding: 12px 24px;
      border: none;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
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

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
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

    .table-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow-x: auto;
    }

    .plans-table {
      width: 100%;
      border-collapse: collapse;
    }

    .plans-table thead {
      background: #f7fafc;
    }

    .plans-table th {
      padding: 16px;
      text-align: left;
      font-size: 13px;
      font-weight: 600;
      color: #2d3748;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      border-bottom: 2px solid #e2e8f0;
    }

    .plans-table td {
      padding: 16px;
      border-bottom: 1px solid #e2e8f0;
      font-size: 14px;
      color: #2d3748;
    }

    .plans-table tbody tr:hover {
      background: #f7fafc;
    }

    .plan-info {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .plan-name {
      font-weight: 600;
      color: #1a202c;
    }

    .plan-desc {
      font-size: 12px;
      color: #718096;
    }

    .status-badge {
      padding: 6px 12px;
      border-radius: 6px;
      font-size: 13px;
      font-weight: 600;
    }

    .status-badge.active {
      background: #d1fae5;
      color: #065f46;
    }

    .status-badge.inactive {
      background: #fee2e2;
      color: #991b1b;
    }

    .actions {
      display: flex;
      gap: 8px;
    }

    .btn-icon {
      padding: 8px 12px;
      border: none;
      background: #f7fafc;
      border-radius: 6px;
      cursor: pointer;
      font-size: 16px;
      transition: all 0.2s;
    }

    .btn-icon:hover {
      background: #e2e8f0;
      transform: translateY(-2px);
    }

    .btn-icon.btn-danger:hover {
      background: #fee2e2;
    }

    .btn-icon.btn-success:hover {
      background: #d1fae5;
    }

    .empty-state {
      text-align: center;
      padding: 48px 24px;
      color: #718096;
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
      max-width: 700px;
      max-height: 90vh;
      overflow-y: auto;
      box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 24px;
      border-bottom: 2px solid #e2e8f0;
      position: sticky;
      top: 0;
      background: white;
      z-index: 1;
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

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
      margin-bottom: 16px;
    }

    .form-group label {
      font-size: 14px;
      font-weight: 600;
      color: #2d3748;
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

    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 16px;
    }

    .error-message {
      background: #fee2e2;
      color: #991b1b;
      padding: 12px;
      border-radius: 8px;
      margin-top: 16px;
    }

    .modal-footer {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
      padding: 24px;
      border-top: 2px solid #e2e8f0;
      position: sticky;
      bottom: 0;
      background: white;
    }

    .btn-secondary {
      background: #e2e8f0;
      color: #2d3748;
    }

    .btn-secondary:hover {
      background: #cbd5e0;
    }

    @media (max-width: 1024px) {
      .plans-list {
        padding: 16px;
      }

      h1 {
        font-size: 24px;
      }

      .form-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class PlansList implements OnInit {
  plans = signal<SubscriptionPlan[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = false;
  editingPlan: SubscriptionPlan | null = null;
  submitting = signal(false);
  modalError = signal<string | null>(null);

  formData: CreateSubscriptionPlanRequest = {
    name: '',
    description: '',
    monthlyPrice: 0,
    yearlyPrice: 0,
    trialDays: 14,
    maxUsers: 10,
    maxPatients: 100
  };

  formDataUpdate: UpdateSubscriptionPlanRequest = {
    name: '',
    description: '',
    monthlyPrice: 0,
    yearlyPrice: 0,
    trialDays: 14,
    maxUsers: 10,
    maxPatients: 100,
    isActive: true
  };

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPlans();
  }

  loadPlans(): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getSubscriptionPlans().subscribe({
      next: (data) => {
        this.plans.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar planos');
        this.loading.set(false);
      }
    });
  }

  openCreateModal(): void {
    this.editingPlan = null;
    this.formData = {
      name: '',
      description: '',
      monthlyPrice: 0,
      yearlyPrice: 0,
      trialDays: 14,
      maxUsers: 10,
      maxPatients: 100
    };
    this.showModal = true;
  }

  openEditModal(plan: SubscriptionPlan): void {
    this.editingPlan = plan;
    this.formData = {
      name: plan.name,
      description: plan.description || '',
      monthlyPrice: plan.monthlyPrice,
      yearlyPrice: plan.yearlyPrice,
      trialDays: plan.trialDays,
      maxUsers: plan.maxUsers,
      maxPatients: plan.maxPatients
    };
    this.formDataUpdate = {
      ...this.formData,
      isActive: plan.isActive
    };
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.editingPlan = null;
    this.modalError.set(null);
  }

  onSubmit(): void {
    this.submitting.set(true);
    this.modalError.set(null);

    if (this.editingPlan) {
      // Update
      this.systemAdminService.updateSubscriptionPlan(this.editingPlan.id, this.formDataUpdate).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadPlans();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao atualizar plano');
          this.submitting.set(false);
        }
      });
    } else {
      // Create
      this.systemAdminService.createSubscriptionPlan(this.formData).subscribe({
        next: () => {
          this.submitting.set(false);
          this.closeModal();
          this.loadPlans();
        },
        error: (err) => {
          this.modalError.set(err.error?.message || 'Erro ao criar plano');
          this.submitting.set(false);
        }
      });
    }
  }

  toggleStatus(id: string, currentStatus: boolean): void {
    if (!confirm(`Tem certeza que deseja ${currentStatus ? 'desativar' : 'ativar'} este plano?`)) {
      return;
    }

    this.systemAdminService.toggleSubscriptionPlanStatus(id).subscribe({
      next: () => {
        this.loadPlans();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao alterar status do plano');
      }
    });
  }

  deletePlan(id: string): void {
    if (!confirm('Tem certeza que deseja excluir este plano? Esta ação não pode ser desfeita.')) {
      return;
    }

    this.systemAdminService.deleteSubscriptionPlan(id).subscribe({
      next: () => {
        this.loadPlans();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao excluir plano');
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}
