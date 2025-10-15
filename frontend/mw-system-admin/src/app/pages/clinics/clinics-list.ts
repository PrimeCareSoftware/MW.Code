import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { SystemAdminService } from '../../services/system-admin';
import { ClinicSummary, PaginatedClinics } from '../../models/system-admin.model';

@Component({
  selector: 'app-clinics-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="clinics-list">
      <div class="header">
        <div>
          <h1>Gerenciar Clínicas</h1>
          <p class="subtitle">Visualize e gerencie todas as clínicas cadastradas</p>
        </div>
        <button class="btn btn-primary" (click)="navigateToCreate()">
          <span>➕ Nova Clínica</span>
        </button>
      </div>

      <!-- Filters -->
      <div class="filters">
        <button 
          class="filter-btn" 
          [class.active]="selectedFilter() === ''"
          (click)="applyFilter('')"
        >
          Todas ({{ paginatedClinics()?.totalCount || 0 }})
        </button>
        <button 
          class="filter-btn" 
          [class.active]="selectedFilter() === 'active'"
          (click)="applyFilter('active')"
        >
          Ativas
        </button>
        <button 
          class="filter-btn" 
          [class.active]="selectedFilter() === 'inactive'"
          (click)="applyFilter('inactive')"
        >
          Inativas
        </button>
      </div>

      @if (loading()) {
        <div class="loading">Carregando clínicas...</div>
      } @else if (error()) {
        <div class="error">
          <p>Erro: {{ error() }}</p>
          <button (click)="loadClinics()" class="btn-retry">Tentar novamente</button>
        </div>
      } @else {
        <div class="table-container">
          <table class="clinics-table">
            <thead>
              <tr>
                <th>Clínica</th>
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
              @for (clinic of paginatedClinics()?.clinics; track clinic.id) {
                <tr>
                  <td>
                    <div class="clinic-info">
                      <div class="clinic-name">{{ clinic.name }}</div>
                      <div class="clinic-date">Criada em {{ formatDate(clinic.createdAt) }}</div>
                    </div>
                  </td>
                  <td>{{ clinic.document }}</td>
                  <td>{{ clinic.email }}</td>
                  <td>{{ clinic.phone }}</td>
                  <td>
                    <span class="badge badge-plan">{{ clinic.planName }}</span>
                  </td>
                  <td>
                    <span class="badge" [class]="getStatusClass(clinic.subscriptionStatus)">
                      {{ getStatusLabel(clinic.subscriptionStatus) }}
                    </span>
                  </td>
                  <td>
                    <span class="status-badge" [class.active]="clinic.isActive" [class.inactive]="!clinic.isActive">
                      {{ clinic.isActive ? '✅ Ativa' : '🚫 Inativa' }}
                    </span>
                  </td>
                  <td>
                    <div class="actions">
                      <button class="btn-icon" (click)="viewDetails(clinic.id)" title="Ver detalhes">
                        👁️
                      </button>
                      <button 
                        class="btn-icon" 
                        [class.btn-danger]="clinic.isActive"
                        [class.btn-success]="!clinic.isActive"
                        (click)="toggleStatus(clinic.id, clinic.isActive)" 
                        [title]="clinic.isActive ? 'Desativar' : 'Ativar'"
                      >
                        {{ clinic.isActive ? '🚫' : '✅' }}
                      </button>
                    </div>
                  </td>
                </tr>
              }
            </tbody>
          </table>

          @if (paginatedClinics() && paginatedClinics()!.clinics.length === 0) {
            <div class="empty-state">
              <p>Nenhuma clínica encontrada</p>
            </div>
          }
        </div>

        <!-- Pagination -->
        @if (paginatedClinics() && paginatedClinics()!.totalPages > 1) {
          <div class="pagination">
            <button 
              class="btn-page" 
              (click)="changePage(currentPage() - 1)"
              [disabled]="currentPage() === 1"
            >
              ‹ Anterior
            </button>
            <span class="page-info">
              Página {{ currentPage() }} de {{ paginatedClinics()!.totalPages }}
            </span>
            <button 
              class="btn-page" 
              (click)="changePage(currentPage() + 1)"
              [disabled]="currentPage() === paginatedClinics()!.totalPages"
            >
              Próxima ›
            </button>
          </div>
        }
      }
    </div>
  `,
  styles: [`
    .clinics-list {
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

    .btn-primary:hover {
      background: #5568d3;
      transform: translateY(-2px);
      box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
    }

    .filters {
      display: flex;
      gap: 12px;
      margin-bottom: 24px;
      flex-wrap: wrap;
    }

    .filter-btn {
      padding: 10px 20px;
      border: 2px solid #e2e8f0;
      background: white;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 600;
      color: #2d3748;
      cursor: pointer;
      transition: all 0.2s;
    }

    .filter-btn:hover {
      border-color: #667eea;
      color: #667eea;
    }

    .filter-btn.active {
      background: #667eea;
      border-color: #667eea;
      color: white;
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

    .clinics-table {
      width: 100%;
      border-collapse: collapse;
    }

    .clinics-table thead {
      background: #f7fafc;
    }

    .clinics-table th {
      padding: 16px;
      text-align: left;
      font-size: 13px;
      font-weight: 600;
      color: #2d3748;
      text-transform: uppercase;
      letter-spacing: 0.5px;
      border-bottom: 2px solid #e2e8f0;
    }

    .clinics-table td {
      padding: 16px;
      border-bottom: 1px solid #e2e8f0;
      font-size: 14px;
      color: #2d3748;
    }

    .clinics-table tbody tr:hover {
      background: #f7fafc;
    }

    .clinic-info {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .clinic-name {
      font-weight: 600;
      color: #1a202c;
    }

    .clinic-date {
      font-size: 12px;
      color: #718096;
    }

    .badge {
      padding: 4px 12px;
      border-radius: 6px;
      font-size: 12px;
      font-weight: 600;
      display: inline-block;
    }

    .badge-plan {
      background: #e6fffa;
      color: #047857;
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

    .badge.badge-suspended {
      background: #e0e7ff;
      color: #3730a3;
    }

    .badge.badge-cancelled {
      background: #f3f4f6;
      color: #374151;
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

    .pagination {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 16px;
      margin-top: 24px;
    }

    .btn-page {
      padding: 10px 16px;
      border: 2px solid #e2e8f0;
      background: white;
      border-radius: 8px;
      font-size: 14px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
    }

    .btn-page:hover:not(:disabled) {
      border-color: #667eea;
      color: #667eea;
    }

    .btn-page:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }

    .page-info {
      font-size: 14px;
      color: #2d3748;
      font-weight: 600;
    }

    @media (max-width: 1024px) {
      .clinics-list {
        padding: 16px;
      }

      h1 {
        font-size: 24px;
      }

      .table-container {
        overflow-x: scroll;
      }

      .clinics-table {
        min-width: 800px;
      }
    }
  `]
})
export class ClinicsList implements OnInit {
  paginatedClinics = signal<PaginatedClinics | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  selectedFilter = signal('');
  currentPage = signal(1);

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.selectedFilter.set(params['status'] || '');
      this.loadClinics();
    });
  }

  loadClinics(): void {
    this.loading.set(true);
    this.error.set(null);

    const status = this.selectedFilter() || undefined;
    this.systemAdminService.getClinics(status, this.currentPage(), 20).subscribe({
      next: (data) => {
        this.paginatedClinics.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar clínicas');
        this.loading.set(false);
      }
    });
  }

  applyFilter(status: string): void {
    this.selectedFilter.set(status);
    this.currentPage.set(1);
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: status ? { status } : {},
      queryParamsHandling: 'merge'
    });
  }

  changePage(page: number): void {
    this.currentPage.set(page);
    this.loadClinics();
  }

  navigateToCreate(): void {
    this.router.navigate(['/clinics/create']);
  }

  viewDetails(id: string): void {
    this.router.navigate(['/clinics', id]);
  }

  toggleStatus(id: string, currentStatus: boolean): void {
    if (!confirm(`Tem certeza que deseja ${currentStatus ? 'desativar' : 'ativar'} esta clínica?`)) {
      return;
    }

    this.systemAdminService.toggleClinicStatus(id).subscribe({
      next: () => {
        this.loadClinics();
      },
      error: (err) => {
        alert(err.error?.message || 'Erro ao alterar status da clínica');
      }
    });
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('pt-BR');
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
