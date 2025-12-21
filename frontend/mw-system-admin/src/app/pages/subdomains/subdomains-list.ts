import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { Subdomain, CreateSubdomainRequest, ClinicSummary } from '../../models/system-admin.model';

@Component({
  selector: 'app-subdomains-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  template: `
    <div class="subdomains-list">
      <div class="header">
        <div>
          <h1>Gerenciar Subdomínios</h1>
          <p class="subtitle">Configure subdomínios para acesso direto das clínicas</p>
        </div>
        <button class="btn btn-primary" (click)="openCreateModal()">
          <span>➕ Novo Subdomínio</span>
        </button>
      </div>

      @if (loading()) {
        <div class="loading">Carregando subdomínios...</div>
      } @else if (error()) {
        <div class="error">{{ error() }}</div>
      } @else {
        <div class="table-container">
          <table class="subdomains-table">
            <thead>
              <tr>
                <th>Subdomínio</th>
                <th>Clínica</th>
                <th>Tenant ID</th>
                <th>Status</th>
                <th>Criado em</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              @for (subdomain of subdomains(); track subdomain.id) {
                <tr>
                  <td>
                    <code class="subdomain-code">{{ subdomain.subdomain }}.medicwarehouse.com</code>
                  </td>
                  <td>{{ subdomain.clinicName }}</td>
                  <td><code>{{ subdomain.tenantId }}</code></td>
                  <td>
                    <span class="status-badge" [class.active]="subdomain.isActive">
                      {{ subdomain.isActive ? '✅ Ativo' : '🚫 Inativo' }}
                    </span>
                  </td>
                  <td>{{ formatDate(subdomain.createdAt) }}</td>
                  <td>
                    <button class="btn-icon btn-danger" (click)="deleteSubdomain(subdomain.id)" title="Excluir">
                      🗑️
                    </button>
                  </td>
                </tr>
              }
            </tbody>
          </table>

          @if (subdomains().length === 0) {
            <div class="empty-state">
              <p>Nenhum subdomínio configurado</p>
            </div>
          }
        </div>
      }

      <!-- Create Modal -->
      @if (showModal) {
        <div class="modal-overlay" (click)="closeModal()">
          <div class="modal" (click)="$event.stopPropagation()">
            <div class="modal-header">
              <h2>Criar Subdomínio</h2>
              <button class="btn-close" (click)="closeModal()">✕</button>
            </div>
            
            <div class="modal-body">
              <p>O subdomínio permite que a clínica acesse o sistema sem precisar informar o Tenant ID.</p>
              
              <div class="form-group">
                <label>Subdomínio *</label>
                <div class="subdomain-input">
                  <input
                    type="text"
                    [(ngModel)]="formData.subdomain"
                    placeholder="minhaclinica"
                    pattern="[a-z0-9-]+"
                  />
                  <span class="domain-suffix">.medicwarehouse.com</span>
                </div>
                <small>Apenas letras minúsculas, números e hífens</small>
              </div>

              <div class="form-group">
                <label>Clínica *</label>
                <select [(ngModel)]="formData.clinicId">
                  <option value="">Selecione uma clínica</option>
                  @for (clinic of clinics(); track clinic.id) {
                    <option [value]="clinic.id">{{ clinic.name }}</option>
                  }
                </select>
              </div>

              @if (modalError()) {
                <div class="error-message">{{ modalError() }}</div>
              }
            </div>

            <div class="modal-footer">
              <button class="btn btn-secondary" (click)="closeModal()">
                Cancelar
              </button>
              <button class="btn btn-primary" (click)="createSubdomain()" [disabled]="!isFormValid() || submitting()">
                @if (submitting()) {
                  <span>Criando...</span>
                } @else {
                  <span>Criar</span>
                }
              </button>
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .subdomains-list {
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
    }

    .btn-primary {
      background: #667eea;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #5568d3;
      transform: translateY(-2px);
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

    .table-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow-x: auto;
    }

    .subdomains-table {
      width: 100%;
      border-collapse: collapse;
    }

    .subdomains-table thead {
      background: #f7fafc;
    }

    .subdomains-table th {
      padding: 16px;
      text-align: left;
      font-size: 13px;
      font-weight: 600;
      color: #2d3748;
      text-transform: uppercase;
      border-bottom: 2px solid #e2e8f0;
    }

    .subdomains-table td {
      padding: 16px;
      border-bottom: 1px solid #e2e8f0;
      font-size: 14px;
      color: #2d3748;
    }

    .subdomains-table tbody tr:hover {
      background: #f7fafc;
    }

    .subdomain-code {
      background: #f7fafc;
      padding: 4px 8px;
      border-radius: 4px;
      font-family: monospace;
      font-size: 13px;
      color: #667eea;
    }

    code {
      background: #f7fafc;
      padding: 2px 6px;
      border-radius: 4px;
      font-family: monospace;
      font-size: 12px;
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

    .btn-icon {
      padding: 8px 12px;
      border: none;
      background: #f7fafc;
      border-radius: 6px;
      cursor: pointer;
      font-size: 16px;
    }

    .btn-icon.btn-danger:hover {
      background: #fee2e2;
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

    .modal-body p {
      color: #718096;
      margin-bottom: 20px;
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

    .form-group small {
      font-size: 12px;
      color: #718096;
    }

    .subdomain-input {
      display: flex;
      align-items: center;
      gap: 4px;
    }

    .subdomain-input input {
      flex: 1;
      padding: 12px;
      border: 2px solid #e2e8f0;
      border-radius: 8px 0 0 8px;
      font-size: 14px;
    }

    .domain-suffix {
      padding: 12px;
      background: #f7fafc;
      border: 2px solid #e2e8f0;
      border-left: none;
      border-radius: 0 8px 8px 0;
      font-size: 14px;
      color: #718096;
    }

    input:focus, select:focus {
      outline: none;
      border-color: #667eea;
    }

    select {
      padding: 12px;
      border: 2px solid #e2e8f0;
      border-radius: 8px;
      font-size: 14px;
    }

    .error-message {
      background: #fee2e2;
      color: #991b1b;
      padding: 12px;
      border-radius: 8px;
    }

    .modal-footer {
      display: flex;
      gap: 12px;
      justify-content: flex-end;
      padding: 24px;
      border-top: 2px solid #e2e8f0;
    }

    .btn-secondary {
      background: #e2e8f0;
      color: #2d3748;
    }
  `]
})
export class SubdomainsList implements OnInit {
  subdomains = signal<Subdomain[]>([]);
  clinics = signal<ClinicSummary[]>([]);
  loading = signal(true);
  error = signal<string | null>(null);
  
  showModal = false;
  submitting = signal(false);
  modalError = signal<string | null>(null);

  formData: CreateSubdomainRequest = {
    subdomain: '',
    clinicId: ''
  };

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSubdomains();
    this.loadClinics();
  }

  loadSubdomains(): void {
    this.loading.set(true);
    this.error.set(null);

    this.systemAdminService.getSubdomains().subscribe({
      next: (data) => {
        this.subdomains.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar subdomínios');
        this.loading.set(false);
      }
    });
  }

  loadClinics(): void {
    this.systemAdminService.getClinics('active', 1, 100).subscribe({
      next: (data) => {
        this.clinics.set(data.clinics);
      },
      error: (err) => {
        console.error('Error loading clinics:', err);
      }
    });
  }

  openCreateModal(): void {
    this.formData = {
      subdomain: '',
      clinicId: ''
    };
    this.modalError.set(null);
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
  }

  isFormValid(): boolean {
    return !!(this.formData.subdomain && this.formData.clinicId);
  }

  createSubdomain(): void {
    if (!this.isFormValid()) return;

    this.submitting.set(true);
    this.modalError.set(null);

    this.systemAdminService.createSubdomain(this.formData).subscribe({
      next: () => {
        this.submitting.set(false);
        this.closeModal();
        this.loadSubdomains();
        alert('Subdomínio criado com sucesso!');
      },
      error: (err) => {
        this.modalError.set(err.error?.message || 'Erro ao criar subdomínio');
        this.submitting.set(false);
      }
    });
  }

  deleteSubdomain(id: string): void {
    if (!confirm('Tem certeza que deseja excluir este subdomínio?')) {
      return;
    }

    this.systemAdminService.deleteSubdomain(id).subscribe({
      next: () => {
        this.loadSubdomains();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao excluir subdomínio');
      }
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
  }
}
