import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { Auth } from '../services/auth';

/**
 * Guard to protect routes that should only be accessible to system administrators
 * Checks if the user is authenticated AND is a system owner
 */
export const systemAdminGuard: CanActivateFn = () => {
  const authService = inject(Auth);
  const router = inject(Router);

  const userInfo = authService.getUserInfo();
  
  // User must be authenticated and be a system owner
  if (authService.hasToken() && userInfo?.isSystemOwner) {
    return true;
  }

  // Redirect to login if not authenticated or not a system owner
  router.navigate(['/login']);
  return false;
};
