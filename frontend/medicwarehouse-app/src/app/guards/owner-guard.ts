import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';

export const ownerGuard: CanActivateFn = (route, state) => {
  const authService = inject(Auth);
  const router = inject(Router);

  const user = authService.currentUser();
  // Check if user is an owner - either has role "Owner" or is a system owner
  if (user && (user.role === 'Owner' || user.isSystemOwner)) {
    return true;
  }

  router.navigate(['/dashboard']);
  return false;
};
