import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { ClinicUserDto, CreateClinicUserRequest, UpdateClinicUserRequest, DoctorFieldsConfigDto } from '../../../models/clinic-admin.model';
import { Navbar } from '../../../shared/navbar/navbar';
import { AccessProfileService } from '../../../services/access-profile.service';
import { AccessProfile } from '../../../models/access-profile.model';

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
  
  // Doctor fields configuration
  doctorFieldsConfig = signal<DoctorFieldsConfigDto>({ professionalIdRequired: false, specialtyRequired: false });
  
  // Access profiles loaded dynamically
  availableProfiles = signal<AccessProfile[]>([]);
  isLoadingProfiles = signal<boolean>(false);
  
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
  
  // Legacy roles kept for backward compatibility
  userRoles = ['Doctor', 'Nurse', 'Receptionist', 'Admin', 'Owner'];

  constructor(
    private clinicAdminService: ClinicAdminService,
    private accessProfileService: AccessProfileService,
    private fb: FormBuilder
  ) {
    this.initializeForms();
  }

  ngOnInit(): void {
    this.loadUsers();
    this.loadDoctorFieldsConfig();
    this.loadAccessProfiles();
  }

  private initializeForms(): void {
    this.createUserForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      name: ['', Validators.required],
      phone: [''],
      role: ['Doctor', Validators.required],
      professionalId: [''],
      specialty: [''],
      showInAppointmentScheduling: [true]
    });

    this.editUserForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      name: ['', Validators.required],
      phone: [''],
      professionalId: [''],
      specialty: [''],
      showInAppointmentScheduling: [true]
    });

    this.passwordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]]
    });

    this.roleForm = this.fb.group({
      newRole: ['', Validators.required]
    });

    // Subscribe to role changes to update validation
    this.createUserForm.get('role')?.valueChanges.subscribe(() => {
      this.updateDoctorFieldValidation();
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

  loadDoctorFieldsConfig(): void {
    this.clinicAdminService.getDoctorFieldsConfiguration().subscribe({
      next: (config) => {
        this.doctorFieldsConfig.set(config);
        this.updateDoctorFieldValidation();
      },
      error: (error) => {
        console.error('Error loading doctor fields configuration:', error);
        // Show user-friendly error message
        this.errorMessage.set('Aviso: Não foi possível carregar as configurações de campos para médicos. Usando configuração padrão (campos opcionais).');
        // Use default configuration (both fields optional)
        this.doctorFieldsConfig.set({ professionalIdRequired: false, specialtyRequired: false });
      }
    });
  }

  loadAccessProfiles(): void {
    this.isLoadingProfiles.set(true);
    this.accessProfileService.getProfiles().subscribe({
      next: (profiles) => {
        this.availableProfiles.set(profiles);
        this.isLoadingProfiles.set(false);
        console.log('Loaded profiles:', profiles.length);
      },
      error: (error) => {
        console.error('Error loading access profiles:', error);
        this.isLoadingProfiles.set(false);
        // Fall back to legacy roles if profile loading fails
        console.warn('Falling back to legacy role-based system');
      }
    });
  }

  updateDoctorFieldValidation(): void {
    const role = this.createUserForm.get('role')?.value;
    const professionalIdControl = this.createUserForm.get('professionalId');
    const specialtyControl = this.createUserForm.get('specialty');

    if (role === 'Doctor') {
      // Apply validation based on configuration
      if (this.doctorFieldsConfig().professionalIdRequired) {
        professionalIdControl?.setValidators([Validators.required]);
      } else {
        professionalIdControl?.clearValidators();
      }

      if (this.doctorFieldsConfig().specialtyRequired) {
        specialtyControl?.setValidators([Validators.required]);
      } else {
        specialtyControl?.clearValidators();
      }
    } else {
      // Clear validators for non-doctor roles
      professionalIdControl?.clearValidators();
      specialtyControl?.clearValidators();
    }

    professionalIdControl?.updateValueAndValidity();
    specialtyControl?.updateValueAndValidity();
  }

  isDoctorRole(): boolean {
    return this.createUserForm.get('role')?.value === 'Doctor';
  }

  isEditingDoctor(): boolean {
    return this.selectedUser()?.role === 'Doctor';
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
      phone: '', // Phone is not returned from backend, so keep empty
      professionalId: user.professionalId || '',
      specialty: user.specialty || '',
      showInAppointmentScheduling: user.showInAppointmentScheduling
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
      'Owner': 'Proprietário',
      'Dentist': 'Dentista',
      'Nutritionist': 'Nutricionista',
      'Psychologist': 'Psicólogo',
      'PhysicalTherapist': 'Fisioterapeuta',
      'Veterinarian': 'Veterinário'
    };
    return roleMap[role] || role;
  }

  // Check if we have profiles loaded
  hasProfiles(): boolean {
    return this.availableProfiles().length > 0;
  }

  // Get profile name by ID
  getProfileName(profileId: string): string {
    const profile = this.availableProfiles().find(p => p.id === profileId);
    return profile?.name || 'Perfil não encontrado';
  }
}
