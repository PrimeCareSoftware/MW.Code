import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { SystemAdminService } from '../../../services/system-admin';
import { ClinicSummary, PaginatedClinics } from '../../../models/system-admin.model';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-clinics-list',
  standalone: true,
  imports: [CommonModule, RouterModule, Navbar],
  templateUrl: './clinics-list.html',
  styleUrl: './clinics-list.scss'})
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
    this.router.navigate(['/system-admin/clinics/create']);
  }

  viewDetails(id: string): void {
    this.router.navigate(['/system-admin/clinics', id]);
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
