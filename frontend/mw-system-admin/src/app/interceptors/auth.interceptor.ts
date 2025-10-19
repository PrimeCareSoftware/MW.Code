import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Auth } from '../services/auth';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(Auth);
  const token = authService.getToken();
  const currentUser = authService.currentUser();

  if (token) {
    let headers = req.headers.set('Authorization', `Bearer ${token}`);
    
    // Add X-Tenant-Id header if user has tenantId
    if (currentUser?.tenantId) {
      headers = headers.set('X-Tenant-Id', currentUser.tenantId);
    }
    
    const cloned = req.clone({ headers });
    return next(cloned);
  }

  return next(req);
};
