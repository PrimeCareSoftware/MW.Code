import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(Auth);
  const router = inject(Router);

  const ADMIN_ROLES = ['Admin', 'SystemAdmin', 'Owner', 'ClinicOwner'];
  const user = authService.currentUser();
  if (user && (ADMIN_ROLES.includes(user.role ?? '') || user.isSystemOwner)) {
    return true;
  }

  router.navigate(['/403']);
  return false;
};
