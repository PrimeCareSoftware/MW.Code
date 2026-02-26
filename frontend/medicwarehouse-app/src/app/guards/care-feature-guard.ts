import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';
import { RolePermissionService } from '../services/role-permission.service';

export const careFeatureGuard: CanActivateFn = () => {
  const authService = inject(Auth);
  const router = inject(Router);
  const rolePermissionService = inject(RolePermissionService);

  const user = authService.currentUser();
  const canAccess = rolePermissionService.canAccessCareFeatures(user?.role, !!user?.isSystemOwner);

  if (canAccess) {
    return true;
  }

  router.navigate(['/403']);
  return false;
};
