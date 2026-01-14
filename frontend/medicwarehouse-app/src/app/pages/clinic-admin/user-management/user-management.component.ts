import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { ClinicUserDto, CreateClinicUserRequest, UpdateClinicUserRequest } from '../../../models/clinic-admin.model';
import { Navbar } from '../../../shared/navbar/navbar';

@Component({
  selector: 'app-user-management',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, Navbar],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.scss'
})
export class UserManagementComponent implements OnInit {
  users = signal<ClinicUserDto[]>([]);
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  
  // Dialogs
  showCreateDialog = signal<boolean>(false);
  showEditDialog = signal<boolean>(false);
  showPasswordDialog = signal<boolean>(false);
  showRoleDialog = signal<boolean>(false);
  showDeactivateDialog = signal<boolean>(false);
  
  // Forms
  createUserForm!: FormGroup;
  editUserForm!: FormGroup;
  passwordForm!: FormGroup;
  roleForm!: FormGroup;
  
  selectedUser = signal<ClinicUserDto | null>(null);
  isProcessing = signal<boolean>(false);
  
  userRoles = ['Doctor', 'Nurse', 'Receptionist', 'Admin', 'Owner'];

  constructor(
    private clinicAdminService: ClinicAdminService,
    private fb: FormBuilder
  ) {
    this.initializeForms();
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  private initializeForms(): void {
    this.createUserForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      name: ['', Validators.required],
      phone: [''],
      role: ['Doctor', Validators.required]
    });

    this.editUserForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      name: ['', Validators.required],
      phone: ['']
    });

    this.passwordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]]
    });

    this.roleForm = this.fb.group({
      newRole: ['', Validators.required]
    });
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

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

  // Create User
  openCreateDialog(): void {
    this.createUserForm.reset({ role: 'Doctor' });
    this.showCreateDialog.set(true);
  }

  closeCreateDialog(): void {
    this.showCreateDialog.set(false);
    this.createUserForm.reset();
  }

  createUser(): void {
    if (this.createUserForm.invalid) {
      return;
    }

    this.isProcessing.set(true);
    this.errorMessage.set('');

    const request: CreateClinicUserRequest = this.createUserForm.value;

    this.clinicAdminService.createUser(request).subscribe({
      next: () => {
        this.successMessage.set('Usuário criado com sucesso!');
        this.closeCreateDialog();
        this.loadUsers();
        this.isProcessing.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao criar usuário: ' + (error.error?.message || error.message));
        this.isProcessing.set(false);
      }
    });
  }

  // Edit User
  openEditDialog(user: ClinicUserDto): void {
    this.selectedUser.set(user);
    this.editUserForm.patchValue({
      email: user.email,
      name: user.name,
      phone: '' // Phone is not returned from backend, so keep empty
    });
    this.showEditDialog.set(true);
  }

  closeEditDialog(): void {
    this.showEditDialog.set(false);
    this.selectedUser.set(null);
    this.editUserForm.reset();
  }

  updateUser(): void {
    if (this.editUserForm.invalid || !this.selectedUser()) {
      return;
    }

    this.isProcessing.set(true);
    this.errorMessage.set('');

    const request: UpdateClinicUserRequest = this.editUserForm.value;

    this.clinicAdminService.updateUser(this.selectedUser()!.id, request).subscribe({
      next: () => {
        this.successMessage.set('Usuário atualizado com sucesso!');
        this.closeEditDialog();
        this.loadUsers();
        this.isProcessing.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao atualizar usuário: ' + (error.error?.message || error.message));
        this.isProcessing.set(false);
      }
    });
  }

  // Change Password
  openPasswordDialog(user: ClinicUserDto): void {
    this.selectedUser.set(user);
    this.passwordForm.reset();
    this.showPasswordDialog.set(true);
  }

  closePasswordDialog(): void {
    this.showPasswordDialog.set(false);
    this.selectedUser.set(null);
    this.passwordForm.reset();
  }

  changePassword(): void {
    if (this.passwordForm.invalid || !this.selectedUser()) {
      return;
    }

    this.isProcessing.set(true);
    this.errorMessage.set('');

    this.clinicAdminService.changeUserPassword(
      this.selectedUser()!.id, 
      this.passwordForm.value
    ).subscribe({
      next: () => {
        this.successMessage.set('Senha alterada com sucesso!');
        this.closePasswordDialog();
        this.isProcessing.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao alterar senha: ' + (error.error?.message || error.message));
        this.isProcessing.set(false);
      }
    });
  }

  // Change Role
  openRoleDialog(user: ClinicUserDto): void {
    this.selectedUser.set(user);
    this.roleForm.patchValue({ newRole: user.role });
    this.showRoleDialog.set(true);
  }

  closeRoleDialog(): void {
    this.showRoleDialog.set(false);
    this.selectedUser.set(null);
    this.roleForm.reset();
  }

  changeRole(): void {
    if (this.roleForm.invalid || !this.selectedUser()) {
      return;
    }

    this.isProcessing.set(true);
    this.errorMessage.set('');

    this.clinicAdminService.changeUserRole(
      this.selectedUser()!.id, 
      this.roleForm.value
    ).subscribe({
      next: () => {
        this.successMessage.set('Perfil alterado com sucesso!');
        this.closeRoleDialog();
        this.loadUsers();
        this.isProcessing.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao alterar perfil: ' + (error.error?.message || error.message));
        this.isProcessing.set(false);
      }
    });
  }

  // Activate/Deactivate User
  openDeactivateDialog(user: ClinicUserDto): void {
    this.selectedUser.set(user);
    this.showDeactivateDialog.set(true);
  }

  closeDeactivateDialog(): void {
    this.showDeactivateDialog.set(false);
    this.selectedUser.set(null);
  }

  toggleUserStatus(): void {
    if (!this.selectedUser()) {
      return;
    }

    this.isProcessing.set(true);
    this.errorMessage.set('');

    const user = this.selectedUser()!;
    const action = user.isActive 
      ? this.clinicAdminService.deactivateUser(user.id)
      : this.clinicAdminService.activateUser(user.id);

    action.subscribe({
      next: () => {
        this.successMessage.set(
          user.isActive ? 'Usuário desativado com sucesso!' : 'Usuário ativado com sucesso!'
        );
        this.closeDeactivateDialog();
        this.loadUsers();
        this.isProcessing.set(false);
      },
      error: (error) => {
        this.errorMessage.set('Erro ao alterar status: ' + (error.error?.message || error.message));
        this.isProcessing.set(false);
      }
    });
  }

  getStatusBadgeClass(isActive: boolean): string {
    return isActive ? 'badge-success' : 'badge-inactive';
  }

  getStatusText(isActive: boolean): string {
    return isActive ? 'Ativo' : 'Inativo';
  }

  getRoleText(role: string): string {
    const roleMap: { [key: string]: string } = {
      'Doctor': 'Médico',
      'Nurse': 'Enfermeiro',
      'Receptionist': 'Recepcionista',
      'Admin': 'Administrador',
      'Owner': 'Proprietário'
    };
    return roleMap[role] || role;
  }
}
