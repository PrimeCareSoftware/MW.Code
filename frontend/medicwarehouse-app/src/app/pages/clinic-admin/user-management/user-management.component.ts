import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { ClinicUserDto } from '../../../models/clinic-admin.model';

@Component({
  selector: 'app-user-management',
  imports: [CommonModule],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  users = signal<ClinicUserDto[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private clinicAdminService: ClinicAdminService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.clinicAdminService.getClinicUsers().subscribe({
      next: (data) => {
        this.users.set(data);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao carregar usuários: ' + (error.error?.message || error.message));
        this.isLoading.set(false);
      }
    });
  }

  getStatusBadgeClass(isActive: boolean): string {
    return isActive ? 'badge-success' : 'badge-inactive';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Ativo' : 'Inativo';
  }
}
