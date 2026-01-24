import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { timeout, catchError, throwError } from 'rxjs';
import { Auth } from '../services/auth';

/**
 * HTTP Interceptor for adding security headers and JWT token to requests
 * Also adds timeout to prevent hanging requests
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(Auth);
  const token = authService.getToken();

  // Clone the request and add authorization header if token exists
  if (token) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        // Add security headers
        'X-Requested-With': 'XMLHttpRequest',
        'Content-Type': 'application/json'
      }
    });
  } else {
    // Even without token, add security headers
    req = req.clone({
      setHeaders: {
        'X-Requested-With': 'XMLHttpRequest',
        'Content-Type': 'application/json'
      }
    });
  }

  // Add timeout to prevent hanging requests (30 seconds)
  return next(req).pipe(
    timeout(30000),
    catchError(error => {
      // If timeout error, convert to a more user-friendly error
      if (error.name === 'TimeoutError') {
        return throwError(() => ({
          status: 408,
          statusText: 'Request Timeout',
          message: 'A operação demorou muito tempo. Por favor, tente novamente.'
        }));
      }
      return throwError(() => error);
    })
  );
};
