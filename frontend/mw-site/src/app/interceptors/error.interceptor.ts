import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

/**
 * HTTP Error Interceptor for mw-site (public website)
 * Handles API errors by redirecting to custom error pages instead of login
 * Login should only be accessible when URL is explicitly invoked
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle different HTTP status codes
      if (error.status) {
        // For client errors (4xx) and server errors (5xx), redirect to error page
        // with the specific status code
        const statusCode = error.status.toString();
        
        // List of status codes that should show error pages
        const errorCodes = ['400', '401', '403', '404', '408', '429', '500', '502', '503', '504'];
        
        if (errorCodes.includes(statusCode)) {
          // Navigate to error page with the status code
          router.navigate(['/error'], { 
            queryParams: { code: statusCode },
            skipLocationChange: false 
          });
        }
      } else if (error.error instanceof ErrorEvent) {
        // Client-side or network error
        console.error('Network error:', error.error.message);
      } else {
        // Backend returned an unsuccessful response code
        console.error(`Backend returned code ${error.status}, body was:`, error.error);
      }

      // Return the error for further handling if needed
      return throwError(() => error);
    })
  );
};
