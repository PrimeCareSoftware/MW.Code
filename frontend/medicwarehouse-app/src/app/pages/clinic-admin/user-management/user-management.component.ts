import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { ClinicUserDto, CreateClinicUserRequest, UpdateClinicUserRequest, DoctorFieldsConfigDto } from '../../../models/clinic-admin.model';
import { AccessProfileService } from '../../../services/access-profile.service';
import { AccessProfile } from '../../../models/access-profile.model';
import { PhoneMaskDirective } from '../../../directives/phone-mask.directive';
import { RolePermissionService } from '../../../services/role-permission.service';

@Component({
  selector: 'app-user-management',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, PhoneMaskDirective],
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
  
  // Legacy fallback roles aligned with MVP
  userRoles = ['Doctor', 'Nutritionist', 'Psychologist', 'Receptionist', 'Admin', 'Financial', 'Owner'];

  constructor(
    private clinicAdminService: ClinicAdminService,
    private accessProfileService: AccessProfileService,
    private rolePermissionService: RolePermissionService,
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
      canAttendAppointments: [false],
      showInAppointmentScheduling: [false]
    });

    this.editUserForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      name: ['', Validators.required],
      phone: [''],
      professionalId: [''],
      specialty: [''],
      password: [''],
      canAttendAppointments: [false],
      showInAppointmentScheduling: [false]
    });

    this.passwordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(8)]]
    });

    this.roleForm = this.fb.group({
      newRole: ['', Validators.required]
    });

    // Subscribe to role and attendance changes to update validation
    this.createUserForm.get('role')?.valueChanges.subscribe(() => {
      this.updateDoctorFieldValidation();
    });

    this.createUserForm.get('canAttendAppointments')?.valueChanges.subscribe((canAttend) => {
      this.createUserForm.patchValue({ showInAppointmentScheduling: !!canAttend }, { emitEvent: false });
      this.updateDoctorFieldValidation();
    });

    this.editUserForm.get('canAttendAppointments')?.valueChanges.subscribe((canAttend) => {
      this.editUserForm.patchValue({ showInAppointmentScheduling: !!canAttend }, { emitEvent: false });
      this.updateEditProfessionalFieldValidation();
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
        const filteredProfiles = profiles.filter(profile => this.isSelectableProfile(profile.name));
        const defaultCount = filteredProfiles.filter(p => p.isDefault).length;
        const customCount = filteredProfiles.length - defaultCount;
        
        console.log(`✅ Successfully loaded ${profiles.length} access profiles`);
        this.availableProfiles.set(filteredProfiles);
        this.isLoadingProfiles.set(false);
        
        // Show success message if we loaded profiles
        if (filteredProfiles.length > 0) {
          console.info(`📋 Available profiles for selection: ${filteredProfiles.length} (${defaultCount} default, ${customCount} custom)`);
        } else {
          console.warn('⚠️ No selectable profiles returned from API - this may indicate a configuration issue');
          this.errorMessage.set('Aviso: Nenhum perfil MVP/administrativo foi encontrado. Usando perfis básicos como alternativa.');
        }
      },
      error: (error) => {
        console.error('❌ Error loading access profiles:', {
          status: error.status,
          statusText: error.statusText
        });
        this.isLoadingProfiles.set(false);
        
        // Show user-friendly error message based on error type
        if (error.status === 403) {
          this.errorMessage.set('Erro: Você não tem permissão para visualizar os perfis. Apenas proprietários podem gerenciar perfis.');
        } else if (error.status === 401) {
          this.errorMessage.set('Erro: Sua sessão expirou. Por favor, faça login novamente.');
        } else if (error.status === 0) {
          this.errorMessage.set('Erro: Não foi possível conectar ao servidor. Verifique sua conexão com a internet.');
        } else {
          this.errorMessage.set('Erro ao carregar perfis. Usando perfis básicos como alternativa.');
        }
        
        // Fall back to legacy roles if profile loading fails
        console.warn('⚠️ Falling back to legacy role-based system due to error');
      }
    });
  }


  isSelectableProfile(roleName: string): boolean {
    return this.rolePermissionService.isSelectableUserProfile(roleName);
  }

  updateDoctorFieldValidation(): void {
    const role = this.createUserForm.get('role')?.value;
    const canAttend = !!this.createUserForm.get('canAttendAppointments')?.value;
    const professionalIdControl = this.createUserForm.get('professionalId');
    const specialtyControl = this.createUserForm.get('specialty');

    if (this.isProfessionalRole(role) && canAttend) {
      professionalIdControl?.setValidators([Validators.required]);

      if (this.doctorFieldsConfig().specialtyRequired) {
        specialtyControl?.setValidators([Validators.required]);
      } else {
        specialtyControl?.clearValidators();
      }
    } else {
      // Clear validators for non-professional roles (admin, receptionist, etc.)
      professionalIdControl?.clearValidators();
      specialtyControl?.clearValidators();
    }

    professionalIdControl?.updateValueAndValidity();
    specialtyControl?.updateValueAndValidity();
  }

  isProfessionalRole(role?: string): boolean {
    if (!role) return false;
    
    // Check if it's a professional role (healthcare provider)
    const professionalRoles = [
      'Doctor', 'Médico',
      'Psychologist', 'Psicólogo',
      'Nutritionist', 'Nutricionista'
    ];
    
    return professionalRoles.some(pr => pr.toLowerCase() === role.toLowerCase());
  }

  isCurrentRoleProfessional(): boolean {
    const role = this.createUserForm.get('role')?.value;
    return this.isProfessionalRole(role);
  }

  isEditingProfessional(): boolean {
    const role = this.selectedUser()?.role;
    return this.isProfessionalRole(role);
  }

  updateEditProfessionalFieldValidation(): void {
    const role = this.selectedUser()?.role;
    const canAttend = !!this.editUserForm.get('canAttendAppointments')?.value;
    const professionalIdControl = this.editUserForm.get('professionalId');

    if (this.isProfessionalRole(role) && canAttend) {
      professionalIdControl?.setValidators([Validators.required]);
    } else {
      professionalIdControl?.clearValidators();
    }

    professionalIdControl?.updateValueAndValidity();
  }

  canCurrentRoleAttend(): boolean {
    return this.isCurrentRoleProfessional() && !!this.createUserForm.get('canAttendAppointments')?.value;
  }

  canEditingRoleAttend(): boolean {
    return this.isEditingProfessional() && !!this.editUserForm.get('canAttendAppointments')?.value;
  }

  // Create User
  openCreateDialog(): void {
    this.createUserForm.reset({ role: 'Doctor', canAttendAppointments: false, showInAppointmentScheduling: false });
    this.updateDoctorFieldValidation();
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

    const request: CreateClinicUserRequest = {
      ...this.createUserForm.value,
      showInAppointmentScheduling: this.createUserForm.value.canAttendAppointments
    };

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
      phone: user.phone || '',
      professionalId: user.professionalId || '',
      specialty: user.specialty || '',
      canAttendAppointments: user.showInAppointmentScheduling,
      password: '',
      showInAppointmentScheduling: user.showInAppointmentScheduling
    });
    this.updateEditProfessionalFieldValidation();
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

    const request: UpdateClinicUserRequest = {
      email: this.editUserForm.value.email,
      name: this.editUserForm.value.name,
      phone: this.editUserForm.value.phone,
      professionalId: this.editUserForm.value.professionalId,
      specialty: this.editUserForm.value.specialty,
      showInAppointmentScheduling: this.editUserForm.value.canAttendAppointments,
      password: this.editUserForm.value.password || undefined
    };

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
      'Veterinarian': 'Veterinário',
      'Financial': 'Financeiro',
      'Financeiro': 'Financeiro',
      'Secretary': 'Secretária',
      'Secretaria': 'Secretária'
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

  // Check if the given user has a professional role
  isProfessionalUser(user: ClinicUserDto): boolean {
    return this.isProfessionalRole(user.role);
  }

  // Get the label for professional ID based on role
  getProfessionalIdLabel(role: string): string {
    const labelMap: { [key: string]: string } = {
      'Doctor': 'CRM',
      'Médico': 'CRM',
      'Dentist': 'CRO',
      'Dentista': 'CRO',
      'Psychologist': 'CRP',
      'Psicólogo': 'CRP',
      'Nutritionist': 'CRN',
      'Nutricionista': 'CRN',
      'PhysicalTherapist': 'CREFITO',
      'Fisioterapeuta': 'CREFITO',
      'Veterinarian': 'CRMV',
      'Veterinário': 'CRMV',
      'Nurse': 'COREN',
      'Enfermeiro': 'COREN',
      'Enfermeira': 'COREN',
      'OccupationalTherapist': 'CREFITO',
      'Terapeuta Ocupacional': 'CREFITO',
      'SpeechTherapist': 'CRFa',
      'Fonoaudiólogo': 'CRFa'
    };
    return labelMap[role] || 'Registro';
  }
}
