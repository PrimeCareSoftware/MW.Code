import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { ClinicSummary, PaginatedClinics } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinic-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="clinic-list">
      <div class="header">
        <div>
          <h1>Gerenciar Clínicas</h1>
          <p class="subtitle">Visualize e gerencie todas as clínicas do sistema</p>
        </div>
        <button (click)="goBack()" class="btn-back">← Voltar ao Dashboard</button>
      </div>

      <!-- Filters -->
      <div class="filters">
        <div class="filter-group">
          <label>Status:</label>
          <select [(ngModel)]="statusFilter" (change)="onFilterChange()" class="filter-select">
            <option value="">Todas</option>
            <option value="active">Ativas</option>
            <option value="inactive">Inativas</option>
          </select>
        </div>
      </div>

      @if (loading()) {
        <div class="loading">Carregando clínicas...</div>
      } @else if (error()) {
        <div class="error">
          <p>Erro ao carregar clínicas: {{ error() }}</p>
          <button (click)="loadClinics()" class="btn-retry">Tentar novamente</button>
        </div>
      } @else if (clinics()) {
        <!-- Clinics Table -->
        <div class="table-container">
          <table class="clinics-table">
            <thead>
              <tr>
                <th>Nome</th>
                <th>CNPJ</th>
                <th>Email</th>
                <th>Telefone</th>
                <th>Plano</th>
                <th>Status Assinatura</th>
                <th>Status</th>
                <th>Ações</th>
              </tr>
            </thead>
            <tbody>
              @for (clinic of clinics()!.clinics; track clinic.id) {
                <tr [class.inactive]="!clinic.isActive">
                  <td>
                    <div class="clinic-name">
                      <strong>{{ clinic.name }}</strong>
                      <small>Criada em {{ formatDate(clinic.createdAt) }}</small>
                    </div>
                  </td>
                  <td>{{ clinic.document }}</td>
                  <td>{{ clinic.email }}</td>
                  <td>{{ clinic.phone }}</td>
                  <td>
                    <span class="badge badge-plan">{{ clinic.planName }}</span>
                  </td>
                  <td>
                    <span class="badge" [class]="'badge-' + getStatusClass(clinic.subscriptionStatus)">
                      {{ clinic.subscriptionStatus }}
                    </span>
                  </td>
                  <td>
                    <span class="badge" [class.badge-success]="clinic.isActive" [class.badge-error]="!clinic.isActive">
                      {{ clinic.isActive ? 'Ativa' : 'Inativa' }}
                    </span>
                  </td>
                  <td>
                    <div class="actions">
                      <button (click)="viewDetails(clinic.id)" class="btn-icon" title="Ver detalhes">
                        👁️
                      </button>
                      <button 
                        (click)="toggleStatus(clinic)" 
                        class="btn-icon" 
                        [title]="clinic.isActive ? 'Desativar' : 'Ativar'"
                        [class.btn-danger]="clinic.isActive"
                        [class.btn-success]="!clinic.isActive">
                        {{ clinic.isActive ? '🚫' : '✅' }}
                      </button>
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>
        </div>

        <!-- Pagination -->
        @if (clinics()!.totalPages > 1) {
          <div class="pagination">
            <button 
              (click)="changePage(currentPage() - 1)" 
              [disabled]="currentPage() === 1"
              class="btn-page">
              ← Anterior
            </button>
            <span class="page-info">
              Página {{ currentPage() }} de {{ clinics()!.totalPages }}
              ({{ clinics()!.totalCount }} clínicas no total)
            </span>
            <button 
              (click)="changePage(currentPage() + 1)" 
              [disabled]="currentPage() === clinics()!.totalPages"
              class="btn-page">
              Próxima →
            </button>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .clinic-list {
      padding: 24px;
      max-width: 1600px;
      margin: 0 auto;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
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

    .filters {
      background: white;
      padding: 20px;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      margin-bottom: 24px;
    }

    .filter-group {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .filter-group label {
      font-weight: 500;
      color: #1a1a1a;
    }

    .filter-select {
      padding: 8px 16px;
      border: 2px solid #e5e7eb;
      border-radius: 8px;
      font-size: 14px;
      cursor: pointer;
      transition: border-color 0.2s;
    }

    .filter-select:focus {
      outline: none;
      border-color: #667eea;
    }

    .table-container {
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
      overflow-x: auto;
      margin-bottom: 24px;
    }

    .clinics-table {
      width: 100%;
      border-collapse: collapse;
    }

    .clinics-table th {
      background: #f9fafb;
      padding: 16px;
      text-align: left;
      font-weight: 600;
      color: #1a1a1a;
      border-bottom: 2px solid #e5e7eb;
    }

    .clinics-table td {
      padding: 16px;
      border-bottom: 1px solid #e5e7eb;
    }

    .clinics-table tr:hover {
      background: #f9fafb;
    }

    .clinics-table tr.inactive {
      opacity: 0.6;
    }

    .clinic-name {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .clinic-name strong {
      color: #1a1a1a;
    }

    .clinic-name small {
      color: #666;
      font-size: 12px;
    }

    .badge {
      display: inline-block;
      padding: 4px 12px;
      border-radius: 12px;
      font-size: 12px;
      font-weight: 500;
      white-space: nowrap;
    }

    .badge-plan {
      background: #dbeafe;
      color: #1e40af;
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
      gap: 8px;
    }

    .btn-icon {
      padding: 8px 12px;
      background: #f3f4f6;
      border: none;
      border-radius: 6px;
      cursor: pointer;
      transition: all 0.2s;
      font-size: 16px;
    }

    .btn-icon:hover {
      background: #e5e7eb;
      transform: scale(1.1);
    }

    .btn-icon.btn-danger:hover {
      background: #fee2e2;
    }

    .btn-icon.btn-success:hover {
      background: #d1fae5;
    }

    .pagination {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 16px;
      padding: 20px;
      background: white;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .btn-page {
      padding: 10px 20px;
      background: #667eea;
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      transition: background 0.2s;
    }

    .btn-page:hover:not(:disabled) {
      background: #5568d3;
    }

    .btn-page:disabled {
      background: #e5e7eb;
      color: #9ca3af;
      cursor: not-allowed;
    }

    .page-info {
      font-size: 14px;
      color: #666;
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
export class ClinicList implements OnInit {
  clinics = signal<PaginatedClinics | null>(null);
  loading = signal<boolean>(true);
  error = signal<string | null>(null);
  currentPage = signal<number>(1);
  statusFilter = '';

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // Check for status filter in query params
    this.route.queryParams.subscribe(params => {
      if (params['status']) {
        this.statusFilter = params['status'];
      }
      this.loadClinics();
    });
  }

  loadClinics() {
    this.loading.set(true);
    this.error.set(null);

    const status = this.statusFilter || undefined;
    this.systemAdminService.getClinics(status, this.currentPage(), 20).subscribe({
      next: (data: PaginatedClinics) => {
        this.clinics.set(data);
        this.loading.set(false);
      },
      error: (err: any) => {
        this.error.set(err.message || 'Erro ao carregar clínicas');
        this.loading.set(false);
      }
    });
  }

  onFilterChange() {
    this.currentPage.set(1);
    this.loadClinics();
  }

  changePage(page: number) {
    this.currentPage.set(page);
    this.loadClinics();
  }

  toggleStatus(clinic: ClinicSummary) {
    const action = clinic.isActive ? 'desativar' : 'ativar';
    if (!confirm(`Tem certeza que deseja ${action} a clínica "${clinic.name}"?`)) {
      return;
    }

    this.systemAdminService.toggleClinicStatus(clinic.id).subscribe({
      next: (response: { message: string; isActive: boolean }) => {
        alert(response.message);
        this.loadClinics(); // Reload list
      },
      error: (err: any) => {
        alert('Erro ao alterar status: ' + (err.message || 'Erro desconhecido'));
      }
    });
  }

  viewDetails(clinicId: string) {
    this.router.navigate(['/system-admin/clinics', clinicId]);
  }

  goBack() {
    this.router.navigate(['/system-admin']);
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('pt-BR');
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
