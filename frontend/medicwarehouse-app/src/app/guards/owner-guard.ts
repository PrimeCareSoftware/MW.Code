import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';

export const ownerGuard: CanActivateFn = (route, state) => {
  const authService = inject(Auth);
  const router = inject(Router);

  const user = authService.currentUser();
  // Check if user is an owner or system admin - either has role "Owner", "ClinicOwner", "SystemAdmin", or is a system owner
  if (user && (user.role === 'Owner' || user.role === 'ClinicOwner' || user.role === 'SystemAdmin' || user.isSystemOwner)) {
    return true;
  }

  // Redirect to 403 forbidden page for permission denied
  router.navigate(['/403']);
  return false;
};
