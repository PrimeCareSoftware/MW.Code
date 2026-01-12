import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { UserManagementComponent } from './user-management.component';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { ClinicUserDto } from '../../../models/clinic-admin.model';

describe('UserManagementComponent', () => {
  let component: UserManagementComponent;
  let fixture: ComponentFixture<UserManagementComponent>;
  let clinicAdminService: jasmine.SpyObj<ClinicAdminService>;

  const mockUsers: ClinicUserDto[] = [
    {
      id: '1',
      username: 'test.user',
      name: 'Test User',
      email: 'test@test.com',
      role: 'Doctor',
      isActive: true,
      createdAt: '2024-01-01T00:00:00Z'
    },
    {
      id: '2',
      username: 'inactive.user',
      name: 'Inactive User',
      email: 'inactive@test.com',
      role: 'Nurse',
      isActive: false,
      createdAt: '2024-01-02T00:00:00Z'
    }
  ];

  beforeEach(async () => {
    const clinicAdminServiceSpy = jasmine.createSpyObj('ClinicAdminService', [
      'getClinicUsers',
      'createUser',
      'updateUser',
      'changeUserPassword',
      'changeUserRole',
      'activateUser',
      'deactivateUser'
    ]);

    await TestBed.configureTestingModule({
      imports: [
        UserManagementComponent,
        HttpClientTestingModule,
        ReactiveFormsModule
      ],
      providers: [
        { provide: ClinicAdminService, useValue: clinicAdminServiceSpy }
      ]
    }).compileComponents();

    clinicAdminService = TestBed.inject(ClinicAdminService) as jasmine.SpyObj<ClinicAdminService>;
    fixture = TestBed.createComponent(UserManagementComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Component Initialization', () => {
    it('should load users on init', () => {
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));
      
      component.ngOnInit();
      
      expect(clinicAdminService.getClinicUsers).toHaveBeenCalled();
      expect(component.users()).toEqual(mockUsers);
      expect(component.isLoading()).toBe(false);
    });

    it('should handle error when loading users', () => {
      const error = { error: { message: 'Error loading users' } };
      clinicAdminService.getClinicUsers.and.returnValue(throwError(() => error));
      
      component.ngOnInit();
      
      expect(component.errorMessage()).toContain('Error loading users');
      expect(component.isLoading()).toBe(false);
    });

    it('should initialize all forms', () => {
      expect(component.createUserForm).toBeDefined();
      expect(component.editUserForm).toBeDefined();
      expect(component.passwordForm).toBeDefined();
      expect(component.roleForm).toBeDefined();
    });
  });

  describe('User Creation', () => {
    beforeEach(() => {
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));
    });

    it('should open create dialog', () => {
      component.openCreateDialog();
      
      expect(component.showCreateDialog()).toBe(true);
      expect(component.createUserForm.value.role).toBe('Doctor');
    });

    it('should close create dialog', () => {
      component.openCreateDialog();
      component.closeCreateDialog();
      
      expect(component.showCreateDialog()).toBe(false);
    });

    it('should create user successfully', () => {
      const newUser: ClinicUserDto = {
        id: '3',
        username: 'new.user',
        name: 'New User',
        email: 'new@test.com',
        role: 'Receptionist',
        isActive: true,
        createdAt: '2024-01-03T00:00:00Z'
      };

      clinicAdminService.createUser.and.returnValue(of(newUser));
      clinicAdminService.getClinicUsers.and.returnValue(of([...mockUsers, newUser]));

      component.openCreateDialog();
      component.createUserForm.patchValue({
        username: 'new.user',
        email: 'new@test.com',
        password: 'Password123!',
        name: 'New User',
        phone: '1234567890',
        role: 'Receptionist'
      });

      component.createUser();

      expect(clinicAdminService.createUser).toHaveBeenCalled();
      expect(component.showCreateDialog()).toBe(false);
      expect(component.successMessage()).toContain('criado com sucesso');
    });

    it('should not create user with invalid form', () => {
      component.openCreateDialog();
      component.createUserForm.patchValue({
        username: 'a', // Too short
        email: 'invalid-email',
        password: 'short',
        name: ''
      });

      component.createUser();

      expect(clinicAdminService.createUser).not.toHaveBeenCalled();
    });

    it('should handle error when creating user', () => {
      const error = { error: { message: 'User limit reached' } };
      clinicAdminService.createUser.and.returnValue(throwError(() => error));

      component.openCreateDialog();
      component.createUserForm.patchValue({
        username: 'test',
        email: 'test@test.com',
        password: 'Password123!',
        name: 'Test',
        role: 'Doctor'
      });

      component.createUser();

      expect(component.errorMessage()).toContain('User limit reached');
    });
  });

  describe('User Update', () => {
    beforeEach(() => {
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));
    });

    it('should open edit dialog with user data', () => {
      const user = mockUsers[0];
      component.openEditDialog(user);

      expect(component.showEditDialog()).toBe(true);
      expect(component.selectedUser()).toEqual(user);
      expect(component.editUserForm.value.email).toBe(user.email);
    });

    it('should update user successfully', () => {
      clinicAdminService.updateUser.and.returnValue(of({}));
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));

      const user = mockUsers[0];
      component.openEditDialog(user);
      component.editUserForm.patchValue({
        email: 'updated@test.com',
        name: 'Updated Name'
      });

      component.updateUser();

      expect(clinicAdminService.updateUser).toHaveBeenCalledWith(
        user.id,
        jasmine.objectContaining({ email: 'updated@test.com' })
      );
      expect(component.successMessage()).toContain('atualizado com sucesso');
    });
  });

  describe('Password Change', () => {
    beforeEach(() => {
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));
    });

    it('should open password dialog', () => {
      const user = mockUsers[0];
      component.openPasswordDialog(user);

      expect(component.showPasswordDialog()).toBe(true);
      expect(component.selectedUser()).toEqual(user);
    });

    it('should change password successfully', () => {
      clinicAdminService.changeUserPassword.and.returnValue(of({}));

      const user = mockUsers[0];
      component.openPasswordDialog(user);
      component.passwordForm.patchValue({
        newPassword: 'NewPassword123!'
      });

      component.changePassword();

      expect(clinicAdminService.changeUserPassword).toHaveBeenCalledWith(
        user.id,
        jasmine.objectContaining({ newPassword: 'NewPassword123!' })
      );
      expect(component.successMessage()).toContain('Senha alterada');
    });

    it('should not change password if form is invalid', () => {
      const user = mockUsers[0];
      component.openPasswordDialog(user);
      component.passwordForm.patchValue({
        newPassword: 'short' // Too short
      });

      component.changePassword();

      expect(clinicAdminService.changeUserPassword).not.toHaveBeenCalled();
    });
  });

  describe('Role Change', () => {
    beforeEach(() => {
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));
    });

    it('should open role dialog', () => {
      const user = mockUsers[0];
      component.openRoleDialog(user);

      expect(component.showRoleDialog()).toBe(true);
      expect(component.selectedUser()).toEqual(user);
      expect(component.roleForm.value.newRole).toBe(user.role);
    });

    it('should change role successfully', () => {
      clinicAdminService.changeUserRole.and.returnValue(of({}));
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));

      const user = mockUsers[0];
      component.openRoleDialog(user);
      component.roleForm.patchValue({
        newRole: 'Admin'
      });

      component.changeRole();

      expect(clinicAdminService.changeUserRole).toHaveBeenCalledWith(
        user.id,
        jasmine.objectContaining({ newRole: 'Admin' })
      );
      expect(component.successMessage()).toContain('Perfil alterado');
    });
  });

  describe('User Activation/Deactivation', () => {
    beforeEach(() => {
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));
    });

    it('should deactivate active user', () => {
      clinicAdminService.deactivateUser.and.returnValue(of({}));
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));

      const activeUser = mockUsers[0];
      component.selectedUser.set(activeUser);
      
      component.toggleUserStatus();

      expect(clinicAdminService.deactivateUser).toHaveBeenCalledWith(activeUser.id);
      expect(component.successMessage()).toContain('desativado com sucesso');
    });

    it('should activate inactive user', () => {
      clinicAdminService.activateUser.and.returnValue(of({}));
      clinicAdminService.getClinicUsers.and.returnValue(of(mockUsers));

      const inactiveUser = mockUsers[1];
      component.selectedUser.set(inactiveUser);
      
      component.toggleUserStatus();

      expect(clinicAdminService.activateUser).toHaveBeenCalledWith(inactiveUser.id);
      expect(component.successMessage()).toContain('ativado com sucesso');
    });
  });

  describe('Utility Functions', () => {
    it('should return correct status badge class', () => {
      expect(component.getStatusBadgeClass(true)).toBe('badge-success');
      expect(component.getStatusBadgeClass(false)).toBe('badge-inactive');
    });

    it('should return correct status text', () => {
      expect(component.getStatusText(true)).toBe('Ativo');
      expect(component.getStatusText(false)).toBe('Inativo');
    });

    it('should return correct role text', () => {
      expect(component.getRoleText('Doctor')).toBe('Médico');
      expect(component.getRoleText('Nurse')).toBe('Enfermeiro');
      expect(component.getRoleText('Receptionist')).toBe('Recepcionista');
      expect(component.getRoleText('Admin')).toBe('Administrador');
      expect(component.getRoleText('Owner')).toBe('Proprietário');
    });

    it('should return original role for unknown role', () => {
      expect(component.getRoleText('Unknown')).toBe('Unknown');
    });
  });
});
