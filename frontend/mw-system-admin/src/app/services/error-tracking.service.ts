import { Injectable, ErrorHandler } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface ErrorReport {
  message: string;
  stack?: string;
  url: string;
  timestamp: number;
  userAgent: string;
  componentName?: string;
  severity: 'low' | 'medium' | 'high' | 'critical';
  context?: any;
}

@Injectable({
  providedIn: 'root'
})
export class ErrorTrackingService implements ErrorHandler {
  private errorQueue: ErrorReport[] = [];
  private readonly MAX_QUEUE_SIZE = 10;
  
  constructor(private http: HttpClient) {}
  
  handleError(error: Error | HttpErrorResponse): void {
    console.error('Error caught by ErrorTrackingService:', error);
    
    if (error instanceof HttpErrorResponse) {
      this.trackHttpError(error);
    } else {
      this.trackError(error);
    }
  }
  
  trackError(error: Error, context?: any, severity: ErrorReport['severity'] = 'medium'): void {
    const errorReport: ErrorReport = {
      message: error.message,
      stack: error.stack,
      url: window.location.href,
      timestamp: Date.now(),
      userAgent: navigator.userAgent,
      severity,
      context
    };
    
    this.queueError(errorReport);
    this.sendErrors();
  }
  
  trackHttpError(error: HttpErrorResponse): void {
    const errorReport: ErrorReport = {
      message: `HTTP ${error.status}: ${error.statusText}`,
      stack: error.error?.stack,
      url: error.url || window.location.href,
      timestamp: Date.now(),
      userAgent: navigator.userAgent,
      severity: this.getHttpErrorSeverity(error.status),
      context: {
        status: error.status,
        statusText: error.statusText,
        error: error.error
      }
    };
    
    this.queueError(errorReport);
    this.sendErrors();
  }
  
  trackCustomError(
    message: string, 
    severity: ErrorReport['severity'] = 'medium',
    context?: any
  ): void {
    const errorReport: ErrorReport = {
      message,
      url: window.location.href,
      timestamp: Date.now(),
      userAgent: navigator.userAgent,
      severity,
      context
    };
    
    this.queueError(errorReport);
    this.sendErrors();
  }
  
  private queueError(error: ErrorReport): void {
    this.errorQueue.push(error);
    
    if (this.errorQueue.length > this.MAX_QUEUE_SIZE) {
      this.errorQueue.shift();
    }
  }
  
  private sendErrors(): void {
    if (this.errorQueue.length === 0) {
      return;
    }
    
    const errors = [...this.errorQueue];
    this.errorQueue = [];
    
    if (environment.production) {
      this.http.post(`${environment.apiUrl}/api/system-admin/errors`, { errors })
        .subscribe({
          error: (err) => {
            console.error('Failed to send error reports:', err);
            // Re-queue errors if send failed
            this.errorQueue.push(...errors);
          }
        });
    } else {
      console.log('Error Reports:', errors);
    }
  }
  
  private getHttpErrorSeverity(status: number): ErrorReport['severity'] {
    if (status >= 500) return 'critical';
    if (status >= 400) return 'high';
    if (status >= 300) return 'medium';
    return 'low';
  }
  
  setUserContext(userId: string, email: string): void {
    // Store user context for error reports
    sessionStorage.setItem('error-tracking-user', JSON.stringify({ userId, email }));
  }
  
  clearUserContext(): void {
    sessionStorage.removeItem('error-tracking-user');
  }
}
