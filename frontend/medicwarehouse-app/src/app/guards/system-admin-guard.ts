import { inject } from '@angular/core';
import { Router, CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Auth } from '../services/auth';

/**
 * Guard to protect routes that should only be accessible to system administrators
 * Checks if the user is authenticated AND is a system owner
 */
export const systemAdminGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(Auth);
  const router = inject(Router);

  const userInfo = authService.getUserInfo();
  
  // User must be authenticated and be a system owner
  if (authService.hasToken() && userInfo?.isSystemOwner) {
    return true;
  }

  // Check if user is authenticated
  if (!authService.hasToken()) {
    // Not authenticated - redirect to system admin login with return URL
    router.navigate(['/system-admin/login'], { queryParams: { returnUrl: state.url } });
  } else {
    // Authenticated but not a system owner - redirect to 403
    router.navigate(['/403']);
  }
  return false;
};
