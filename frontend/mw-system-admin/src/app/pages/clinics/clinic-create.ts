import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { CreateClinicRequest, SubscriptionPlan } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinic-create',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="clinic-create">
      <div class="header">
        <div>
          <button class="btn-back" (click)="goBack()">← Voltar</button>
          <h1>Criar Nova Clínica</h1>
          <p class="subtitle">Preencha os dados da clínica e do proprietário</p>
        </div>
      </div>

      @if (loading()) {
        <div class="loading">Carregando planos...</div>
      } @else if (error()) {
        <div class="error-message">{{ error() }}</div>
      } @else {
        <form (ngSubmit)="onSubmit()" class="form">
          <div class="form-section">
            <h2>Dados da Clínica</h2>
            <div class="form-grid">
              <div class="form-group">
                <label for="name">Nome da Clínica *</label>
                <input
                  type="text"
                  id="name"
                  [(ngModel)]="formData.name"
                  name="name"
                  required
                  placeholder="Ex: Clínica Saúde Total"
                />
              </div>

              <div class="form-group">
                <label for="document">CNPJ *</label>
                <input
                  type="text"
                  id="document"
                  [(ngModel)]="formData.document"
                  name="document"
                  required
                  placeholder="00.000.000/0000-00"
                />
              </div>

              <div class="form-group">
                <label for="email">Email *</label>
                <input
                  type="email"
                  id="email"
                  [(ngModel)]="formData.email"
                  name="email"
                  required
                  placeholder="contato@clinica.com"
                />
              </div>

              <div class="form-group">
                <label for="phone">Telefone *</label>
                <input
                  type="tel"
                  id="phone"
                  [(ngModel)]="formData.phone"
                  name="phone"
                  required
                  placeholder="(00) 0000-0000"
                />
              </div>

              <div class="form-group full-width">
                <label for="address">Endereço *</label>
                <input
                  type="text"
                  id="address"
                  [(ngModel)]="formData.address"
                  name="address"
                  required
                  placeholder="Rua, número, bairro, cidade - UF"
                />
              </div>

              <div class="form-group">
                <label for="planId">Plano de Assinatura *</label>
                <select
                  id="planId"
                  [(ngModel)]="formData.planId"
                  name="planId"
                  required
                >
                  <option value="">Selecione um plano</option>
                  @for (plan of plans(); track plan.id) {
                    <option [value]="plan.id">
                      {{ plan.name }} - R$ {{ plan.monthlyPrice.toFixed(2) }}/mês
                      ({{ plan.trialDays }} dias de trial)
                    </option>
                  }
                </select>
              </div>
            </div>
          </div>

          <div class="form-section">
            <h2>Dados do Proprietário (Owner)</h2>
            <p class="section-description">
              O proprietário terá acesso total à clínica e poderá gerenciar usuários.
              Os dados de acesso serão enviados para você repassar ao cliente.
            </p>
            <div class="form-grid">
              <div class="form-group">
                <label for="ownerFullName">Nome Completo *</label>
                <input
                  type="text"
                  id="ownerFullName"
                  [(ngModel)]="formData.ownerFullName"
                  name="ownerFullName"
                  required
                  placeholder="Nome completo do proprietário"
                />
              </div>

              <div class="form-group">
                <label for="ownerUsername">Nome de Usuário *</label>
                <input
                  type="text"
                  id="ownerUsername"
                  [(ngModel)]="formData.ownerUsername"
                  name="ownerUsername"
                  required
                  placeholder="usuario.owner"
                />
              </div>

              <div class="form-group">
                <label for="ownerPassword">Senha *</label>
                <input
                  type="password"
                  id="ownerPassword"
                  [(ngModel)]="formData.ownerPassword"
                  name="ownerPassword"
                  required
                  placeholder="Senha forte"
                  minlength="8"
                />
              </div>

              <div class="form-group">
                <label for="confirmPassword">Confirmar Senha *</label>
                <input
                  type="password"
                  id="confirmPassword"
                  [(ngModel)]="confirmPassword"
                  name="confirmPassword"
                  required
                  placeholder="Repita a senha"
                  minlength="8"
                />
              </div>
            </div>
          </div>

          @if (submitError()) {
            <div class="error-message">{{ submitError() }}</div>
          }

          @if (successMessage()) {
            <div class="success-message">
              <p>{{ successMessage() }}</p>
              <div class="credentials">
                <h3>Dados de Acesso (guarde estas informações):</h3>
                <p><strong>Username:</strong> {{ formData.ownerUsername }}</p>
                <p><strong>Senha:</strong> {{ formData.ownerPassword }}</p>
                <p><strong>Tenant ID:</strong> {{ createdTenantId() }}</p>
              </div>
            </div>
          }

          <div class="form-actions">
            <button type="button" class="btn btn-secondary" (click)="goBack()" [disabled]="submitting()">
              Cancelar
            </button>
            <button type="submit" class="btn btn-primary" [disabled]="submitting() || !isFormValid()">
              @if (submitting()) {
                <span>Criando...</span>
              } @else {
                <span>Criar Clínica</span>
              }
            </button>
          </div>
        </form>
      }
    </div>
  `,
  styles: [`
    .clinic-create {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
    }

    .header {
      margin-bottom: 32px;
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
      margin: 0 0 8px 0;
    }

    .subtitle {
      color: #718096;
      font-size: 16px;
      margin: 0;
    }

    .loading {
      text-align: center;
      padding: 48px 24px;
      color: #718096;
    }

    .error-message {
      background: #fee2e2;
      color: #991b1b;
      padding: 16px;
      border-radius: 8px;
      margin-bottom: 24px;
    }

    .success-message {
      background: #d1fae5;
      color: #065f46;
      padding: 24px;
      border-radius: 8px;
      margin-bottom: 24px;
    }

    .credentials {
      margin-top: 16px;
      padding: 16px;
      background: white;
      border-radius: 8px;
    }

    .credentials h3 {
      font-size: 16px;
      margin: 0 0 12px 0;
      color: #1a202c;
    }

    .credentials p {
      margin: 8px 0;
      font-family: monospace;
    }

    .form {
      background: white;
      border-radius: 12px;
      padding: 32px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .form-section {
      margin-bottom: 32px;
    }

    .form-section:last-of-type {
      margin-bottom: 24px;
    }

    .form-section h2 {
      font-size: 20px;
      font-weight: 600;
      color: #1a202c;
      margin: 0 0 8px 0;
    }

    .section-description {
      color: #718096;
      font-size: 14px;
      margin: 0 0 20px 0;
    }

    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 20px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
    }

    .form-group.full-width {
      grid-column: 1 / -1;
    }

    label {
      font-size: 14px;
      font-weight: 600;
      color: #2d3748;
      margin-bottom: 8px;
    }

    input, select {
      padding: 12px;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 14px;
      transition: border-color 0.2s;
    }

    input:focus, select:focus {
      outline: none;
      border-color: #667eea;
    }

    input::placeholder {
      color: #a0aec0;
    }

    .form-actions {
      display: flex;
      gap: 16px;
      justify-content: flex-end;
      margin-top: 24px;
      padding-top: 24px;
      border-top: 2px solid #e2e8f0;
    }

    .btn {
      padding: 12px 32px;
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

    @media (max-width: 768px) {
      .clinic-create {
        padding: 16px;
      }

      h1 {
        font-size: 24px;
      }

      .form {
        padding: 20px;
      }

      .form-grid {
        grid-template-columns: 1fr;
      }

      .form-actions {
        flex-direction: column;
      }

      .btn {
        width: 100%;
      }
    }
  `]
})
export class ClinicCreate {
  plans = signal<SubscriptionPlan[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  submitting = signal(false);
  submitError = signal<string | null>(null);
  successMessage = signal<string | null>(null);
  createdTenantId = signal<string | null>(null);
  confirmPassword = '';

  formData: CreateClinicRequest = {
    name: '',
    document: '',
    email: '',
    phone: '',
    address: '',
    ownerUsername: '',
    ownerPassword: '',
    ownerFullName: '',
    planId: ''
  };

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {
    this.loadPlans();
  }

  loadPlans(): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getSubscriptionPlans(true).subscribe({
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

  isFormValid(): boolean {
    return !!(
      this.formData.name &&
      this.formData.document &&
      this.formData.email &&
      this.formData.phone &&
      this.formData.address &&
      this.formData.ownerUsername &&
      this.formData.ownerPassword &&
      this.formData.ownerFullName &&
      this.formData.planId &&
      this.formData.ownerPassword === this.confirmPassword &&
      this.formData.ownerPassword.length >= 8
    );
  }

  onSubmit(): void {
    if (!this.isFormValid() || this.submitting()) {
      return;
    }

    this.submitting.set(true);
    this.submitError.set(null);

    this.systemAdminService.createClinic(this.formData).subscribe({
      next: (response) => {
        this.successMessage.set('Clínica criada com sucesso!');
        this.createdTenantId.set(response.clinicId);
        this.submitting.set(false);

        // Navigate back after 5 seconds
        setTimeout(() => {
          this.router.navigate(['/clinics']);
        }, 5000);
      },
      error: (err) => {
        this.submitError.set(err.error?.message || 'Erro ao criar clínica');
        this.submitting.set(false);
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/clinics']);
  }
}
