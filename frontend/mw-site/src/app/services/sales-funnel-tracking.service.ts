import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

/**
 * Service for tracking sales funnel metrics
 * Tracks customer journey through registration process to identify drop-off points
 */
@Injectable({
  providedIn: 'root'
})
export class SalesFunnelTrackingService {
  private readonly API_URL = `${environment.apiUrl}/api/salesfunnel`;
  private sessionId: string = '';
  private stepStartTimes: Map<number, number> = new Map();

  constructor(private http: HttpClient) {
    this.initializeSession();
  }

  /**
   * Initialize or retrieve session ID
   */
  private initializeSession(): void {
    // Check if session ID exists in sessionStorage
    const existingSessionId = sessionStorage.getItem('salesFunnelSessionId');
    
    if (existingSessionId) {
      this.sessionId = existingSessionId;
    } else {
      // Generate new session ID
      this.sessionId = this.generateSessionId();
      sessionStorage.setItem('salesFunnelSessionId', this.sessionId);
    }
  }

  /**
   * Generate unique session ID
   */
  private generateSessionId(): string {
    return `sf_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Get current session ID
   */
  getSessionId(): string {
    return this.sessionId;
  }

  /**
   * Track entering a step
   */
  trackStepEntered(step: number, capturedData?: any, planId?: string): void {
    this.stepStartTimes.set(step, Date.now());
    
    this.trackEvent({
      sessionId: this.sessionId,
      step: step,
      action: 'entered',
      capturedData: capturedData ? this.sanitizeData(capturedData) : undefined,
      planId: planId,
      referrer: document.referrer,
      metadata: this.collectMetadata()
    }).subscribe();
  }

  /**
   * Track completing a step
   */
  trackStepCompleted(step: number, capturedData?: any, planId?: string): void {
    const durationMs = this.calculateStepDuration(step);
    
    this.trackEvent({
      sessionId: this.sessionId,
      step: step,
      action: 'completed',
      capturedData: capturedData ? this.sanitizeData(capturedData) : undefined,
      planId: planId,
      durationMs: durationMs,
      referrer: document.referrer,
      metadata: this.collectMetadata()
    }).subscribe();
  }

  /**
   * Track abandoning a step
   */
  trackStepAbandoned(step: number, capturedData?: any, planId?: string): void {
    const durationMs = this.calculateStepDuration(step);
    
    this.trackEvent({
      sessionId: this.sessionId,
      step: step,
      action: 'abandoned',
      capturedData: capturedData ? this.sanitizeData(capturedData) : undefined,
      planId: planId,
      durationMs: durationMs,
      referrer: document.referrer,
      metadata: this.collectMetadata()
    }).subscribe();
  }

  /**
   * Calculate duration for a step
   */
  private calculateStepDuration(step: number): number | undefined {
    const startTime = this.stepStartTimes.get(step);
    if (startTime) {
      return Date.now() - startTime;
    }
    return undefined;
  }

  /**
   * Sanitize captured data (remove sensitive information like passwords)
   */
  private sanitizeData(data: any): string {
    const sanitized = { ...data };
    
    // Remove sensitive fields
    const sensitiveFields = ['password', 'passwordConfirm', 'creditCard', 'cvv', 'securityCode'];
    sensitiveFields.forEach(field => {
      if (sanitized[field]) {
        sanitized[field] = '[REDACTED]';
      }
    });
    
    return JSON.stringify(sanitized);
  }

  /**
   * Collect metadata (UTM parameters, screen size, etc.)
   */
  private collectMetadata(): string {
    const metadata: any = {
      screenWidth: window.innerWidth,
      screenHeight: window.innerHeight,
      timestamp: new Date().toISOString()
    };

    // Collect UTM parameters from URL
    const urlParams = new URLSearchParams(window.location.search);
    const utmParams = ['utm_source', 'utm_medium', 'utm_campaign', 'utm_term', 'utm_content'];
    
    utmParams.forEach(param => {
      const value = urlParams.get(param);
      if (value) {
        metadata[param] = value;
      }
    });

    return JSON.stringify(metadata);
  }

  /**
   * Track an event (internal method)
   */
  private trackEvent(eventData: TrackEventDto): Observable<any> {
    return this.http.post(`${this.API_URL}/track`, eventData).pipe(
      catchError(error => {
        // Log error but don't fail the application
        console.error('Failed to track sales funnel event:', error);
        return of(null);
      })
    );
  }

  /**
   * Clear session data (called after successful registration)
   */
  clearSession(): void {
    sessionStorage.removeItem('salesFunnelSessionId');
    this.stepStartTimes.clear();
    this.sessionId = this.generateSessionId();
  }
}

/**
 * DTO for tracking events
 */
interface TrackEventDto {
  sessionId: string;
  step: number;
  action: string;
  capturedData?: string;
  planId?: string;
  referrer?: string;
  durationMs?: number;
  metadata?: string;
}
