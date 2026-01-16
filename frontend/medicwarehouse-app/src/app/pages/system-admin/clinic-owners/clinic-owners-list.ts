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
  templateUrl: './clinic-owners-list.html',
  styleUrl: './clinic-owners-list.scss'})
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
