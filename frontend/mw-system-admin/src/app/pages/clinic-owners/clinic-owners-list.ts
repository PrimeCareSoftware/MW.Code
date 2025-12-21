import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { ClinicOwner, ResetPasswordRequest } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinic-owners-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="owners-list">
      <div class="header">
        <div>
          <h1>Gerenciar Proprietários de Clínicas</h1>
          <p class="subtitle">Gerencie acesso e senhas dos proprietários</p>
        </div>
      </div>

      @if (loading()) {
        <div class="loading">Carregando proprietários...</div>
      } @else if (error()) {
        <div class="error">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="owners-table">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Username</th>
                <th>Email</th>
                <th>Clínica</th>
                <th>Último Login</th>
                <th>Status</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              @for (owner of owners(); track owner.id) {
                <tr>
                  <td>{{ owner.fullName }}</td>
                  <td>{{ owner.username }}</td>
                  <td>{{ owner.email }}</td>
                  <td>{{ owner.clinicName || 'System Owner' }}</td>
                  <td>{{ owner.lastLoginAt ? formatDate(owner.lastLoginAt) : 'Nunca' }}</td>
                  <td>
                    <span class="status-badge" [class.active]="owner.isActive" [class.inactive]="!owner.isActive">
                      {{ owner.isActive ? '✅ Ativo' : '🚫 Inativo' }}
                    </span>
                  </td>
                  <td>
                    <div class="actions">
                      <button class="btn-icon" (click)="openResetPasswordModal(owner)" title="Resetar Senha">
                        🔑
                      </button>
                      <button 
                        class="btn-icon" 
                        [class.btn-danger]="owner.isActive"
                        [class.btn-success]="!owner.isActive"
                        (click)="toggleStatus(owner.id, owner.isActive)" 
                        [title]="owner.isActive ? 'Suspender' : 'Ativar'"
                      >
                        {{ owner.isActive ? '🚫' : '✅' }}
                      </button>
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>

          @if (owners().length === 0) {
            <div class="empty-state">
              <p>Nenhum proprietário encontrado</p>
            </div>
          }
        </div>
      }

      <!-- Reset Password Modal -->
      @if (showResetPasswordModal && selectedOwner) {
        <div class="modal-overlay" (click)="closeModal()">
          <div class="modal" (click)="$event.stopPropagation()">
            <div class="modal-header">
              <h2>Resetar Senha</h2>
              <button class="btn-close" (click)="closeModal()">✕</button>
            </div>
            
            <div class="modal-body">
              <p>Resetar senha para: <strong>{{ selectedOwner.fullName }}</strong></p>
              
              <div class="form-group">
                <label>Nova Senha *</label>
                <input
                  type="password"
                  [(ngModel)]="newPassword"
                  placeholder="Digite a nova senha"
                  minlength="8"
                />
              </div>

              @if (modalError()) {
                <div class="error-message">{{ modalError() }}</div>
              }
            </div>

            <div class="modal-footer">
              <button class="btn btn-secondary" (click)="closeModal()">
                Cancelar
              </button>
              <button class="btn btn-primary" (click)="resetPassword()" [disabled]="!newPassword || submitting()">
                @if (submitting()) {
                  <span>Resetando...</span>
                } @else {
                  <span>Resetar Senha</span>
                }
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .owners-list {
      padding: 24px;
      max-width: 1400px;
      margin: 0 auto;
    }

    .header {
      margin-bottom: 24px;
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

    .table-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow-x: auto;
    }

    .owners-table {
      width: 100%;
      border-collapse: collapse;
    }

    .owners-table thead {
      background: #f7fafc;
    }

    .owners-table th {
      padding: 16px;
      text-align: left;
      font-size: 13px;
      font-weight: 600;
      color: #2d3748;
      text-transform: uppercase;
      border-bottom: 2px solid #e2e8f0;
    }

    .owners-table td {
      padding: 16px;
      border-bottom: 1px solid #e2e8f0;
      font-size: 14px;
      color: #2d3748;
    }

    .owners-table tbody tr:hover {
      background: #f7fafc;
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
    }

    .modal-body {
      padding: 24px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
      margin-top: 16px;
    }

    .form-group label {
      font-size: 14px;
      font-weight: 600;
      color: #2d3748;
    }

    input {
      padding: 12px;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 14px;
    }

    input:focus {
      outline: none;
      border-color: #667eea;
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
    }

    .btn {
      padding: 12px 24px;
      border: none;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 600;
      cursor: pointer;
    }

    .btn:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-secondary {
      background: #e2e8f0;
      color: #2d3748;
    }
  `]
})
export class ClinicOwnersList implements OnInit {
  owners = signal<ClinicOwner[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showResetPasswordModal = false;
  selectedOwner: ClinicOwner | null = null;
  newPassword = '';
  submitting = signal(false);
  modalError = signal<string | null>(null);

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const clinicId = params['clinicId'];
      this.loadOwners(clinicId);
    });
  }

  loadOwners(clinicId?: string): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getClinicOwners(clinicId).subscribe({
      next: (data) => {
        this.owners.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar proprietários');
        this.loading.set(false);
      }
    });
  }

  openResetPasswordModal(owner: ClinicOwner): void {
    this.selectedOwner = owner;
    this.newPassword = '';
    this.modalError.set(null);
    this.showResetPasswordModal = true;
  }

  closeModal(): void {
    this.showResetPasswordModal = false;
    this.selectedOwner = null;
    this.newPassword = '';
    this.modalError.set(null);
  }

  resetPassword(): void {
    if (!this.selectedOwner || !this.newPassword) return;

    this.submitting.set(true);
    this.modalError.set(null);

    const request: ResetPasswordRequest = {
      newPassword: this.newPassword
    };

    this.systemAdminService.resetOwnerPassword(this.selectedOwner.id, request).subscribe({
      next: () => {
        alert('Senha resetada com sucesso!');
        this.closeModal();
        this.submitting.set(false);
      },
      error: (err) => {
        this.modalError.set(err.error?.message || 'Erro ao resetar senha');
        this.submitting.set(false);
      }
    });
  }

  toggleStatus(id: string, currentStatus: boolean): void {
    if (!confirm(`Tem certeza que deseja ${currentStatus ? 'suspender' : 'ativar'} este proprietário?`)) {
      return;
    }

    this.systemAdminService.toggleClinicOwnerStatus(id).subscribe({
      next: () => {
        this.loadOwners();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao alterar status');
      }
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }
}
