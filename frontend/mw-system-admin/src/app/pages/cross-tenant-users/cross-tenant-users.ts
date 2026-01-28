import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SystemAdminService } from '../../services/system-admin';
import { CrossTenantUser, CrossTenantUserFilter } from '../../models/system-admin.model';
import { Navbar } from '../../shared/navbar/navbar';

@Component({
  selector: 'app-cross-tenant-users',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, Navbar],
  templateUrl: './cross-tenant-users.html',
  styleUrl: './cross-tenant-users.scss'
})
export class CrossTenantUsers implements OnInit {
  users = signal<CrossTenantUser[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  error = signal<string | null>(null);
  
  // Filters
  searchTerm = signal('');
  selectedRole = signal<string | undefined>(undefined);
  selectedStatus = signal<boolean | undefined>(undefined);
  currentPage = signal(1);
  pageSize = 20;
  
  // Password reset modal
  showPasswordResetModal = false;
  selectedUserId = '';
  newPassword = '';
  resetProcessing = signal(false);
  resetError = signal<string | null>(null);

  constructor(
    private systemAdminService: SystemAdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.error.set(null);

    const filters: CrossTenantUserFilter = {
      searchTerm: this.searchTerm() || undefined,
      role: this.selectedRole(),
      isActive: this.selectedStatus(),
      page: this.currentPage(),
      pageSize: this.pageSize
    };

    this.systemAdminService.getCrossTenantUsers(filters).subscribe({
      next: (response) => {
        this.users.set(response.users);
        this.totalCount.set(response.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set(err.error?.message || 'Erro ao carregar usuários');
        this.loading.set(false);
      }
    });
  }

  applyFilters(): void {
    this.currentPage.set(1);
    this.loadUsers();
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedRole.set(undefined);
    this.selectedStatus.set(undefined);
    this.currentPage.set(1);
    this.loadUsers();
  }

  changePage(page: number): void {
    this.currentPage.set(page);
    this.loadUsers();
  }

  getTotalPages(): number {
    return Math.ceil(this.totalCount() / this.pageSize);
  }

  openPasswordResetModal(userId: string): void {
    this.selectedUserId = userId;
    this.newPassword = '';
    this.resetError.set(null);
    this.showPasswordResetModal = true;
  }

  closePasswordResetModal(): void {
    this.showPasswordResetModal = false;
    this.selectedUserId = '';
    this.newPassword = '';
    this.resetError.set(null);
  }

  resetPassword(): void {
    if (!this.newPassword || this.newPassword.length < 8) {
      this.resetError.set('A senha deve ter no mínimo 8 caracteres');
      return;
    }

    this.resetProcessing.set(true);
    this.resetError.set(null);

    this.systemAdminService.resetCrossTenantUserPassword(this.selectedUserId, this.newPassword).subscribe({
      next: () => {
        alert('Senha resetada com sucesso!');
        this.closePasswordResetModal();
        this.resetProcessing.set(false);
      },
      error: (err) => {
        this.resetError.set(err.error?.message || 'Erro ao resetar senha');
        this.resetProcessing.set(false);
      }
    });
  }

  toggleUserStatus(userId: string, currentStatus: boolean): void {
    const action = currentStatus ? 'desativar' : 'ativar';
    if (!confirm(`Tem certeza que deseja ${action} este usuário?`)) {
      return;
    }

    this.systemAdminService.toggleCrossTenantUserStatus(userId).subscribe({
      next: () => {
        this.loadUsers();
        alert(`Usuário ${action}do com sucesso!`);
      },
      error: (err) => {
        alert(err.error?.message || `Erro ao ${action} usuário`);
      }
    });
  }

  viewClinic(tenantId: string): void {
    this.router.navigate(['/clinics', tenantId]);
  }

  formatDate(date?: string): string {
    if (!date) return 'Nunca';
    return new Date(date).toLocaleDateString('pt-BR');
  }

  getRoleBadgeClass(role: string): string {
    const classes: { [key: string]: string } = {
      'Owner': 'badge-owner',
      'Admin': 'badge-admin',
      'Doctor': 'badge-doctor',
      'Receptionist': 'badge-receptionist',
      'Nurse': 'badge-nurse'
    };
    return classes[role] || 'badge-default';
  }

  getRoleLabel(role: string): string {
    const labels: { [key: string]: string } = {
      'Owner': 'Proprietário',
      'Admin': 'Administrador',
      'Doctor': 'Médico',
      'Receptionist': 'Recepcionista',
      'Nurse': 'Enfermeiro'
    };
    return labels[role] || role;
  }
}
