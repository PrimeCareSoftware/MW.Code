import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';

export const ownerGuard: CanActivateFn = (route, state) => {
  const authService = inject(Auth);
  const router = inject(Router);

  const user = authService.currentUser();
  if (user && user.isOwner) {
    return true;
  }

  router.navigate(['/dashboard']);
  return false;
};
