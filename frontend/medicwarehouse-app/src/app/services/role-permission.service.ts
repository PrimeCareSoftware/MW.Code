import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class RolePermissionService {
  private readonly clinicalMvpRoles = new Set([
    'doctor',
    'medico',
    'médico',
    'nutritionist',
    'nutricionista',
    'psychologist',
    'psicologo',
    'psicólogo'
  ]);

  private readonly adminRoleKeywords = [
    'owner',
    'proprietario',
    'proprietário',
    'admin',
    'administrador',
    'financial',
    'financeiro',
    'finance',
    'secretary',
    'secretaria',
    'secretário',
    'reception',
    'receptionist',
    'recepcionista',
    'manager',
    'coordinator',
    'coordenador'
  ];

  isClinicalMvpRole(role?: string | null): boolean {
    const normalizedRole = this.normalizeRole(role);
    return this.clinicalMvpRoles.has(normalizedRole);
  }

  isAdministrativeRole(role?: string | null): boolean {
    const normalizedRole = this.normalizeRole(role);
    return this.adminRoleKeywords.some(keyword => normalizedRole.includes(this.normalizeRole(keyword)));
  }

  canAccessCareFeatures(role?: string | null, isSystemOwner: boolean = false): boolean {
    if (isSystemOwner) {
      return true;
    }

    return this.isClinicalMvpRole(role) || this.isOwnerRole(role);
  }

  isOwnerRole(role?: string | null): boolean {
    const normalizedRole = this.normalizeRole(role);
    return normalizedRole === 'owner' || normalizedRole === 'clinicowner' || normalizedRole === 'proprietario' || normalizedRole === 'proprietário';
  }

  isSelectableUserProfile(role?: string | null): boolean {
    return this.isClinicalMvpRole(role) || this.isAdministrativeRole(role);
  }

  private normalizeRole(role?: string | null): string {
    return (role ?? '')
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .trim();
  }
}
