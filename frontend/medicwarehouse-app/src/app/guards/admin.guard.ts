import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';

export const adminGuard: CanActivateFn = () => {
  const authService = inject(Auth);
  const router = inject(Router);

  const user = authService.currentUser();
  if (user && (
    user.role === 'Admin' ||
    user.role === 'SystemAdmin' ||
    user.role === 'Owner' ||
    user.role === 'ClinicOwner' ||
    user.isSystemOwner
  )) {
    return true;
  }

  router.navigate(['/403']);
  return false;
};
