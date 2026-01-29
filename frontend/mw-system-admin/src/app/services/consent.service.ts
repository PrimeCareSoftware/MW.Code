import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ConsentLog {
  id: string;
  patientId: string;
  patientName: string;
  consentType: string;
  purpose: string;
  description: string;
  status: 'Active' | 'Revoked' | 'Expired';
  consentDate: string;
  expirationDate: string | null;
  revokedDate: string | null;
  revokedBy: string | null;
  consentText: string;
  version: string;
  obtainmentMethod: 'WEB' | 'MOBILE' | 'PAPER';
  ipAddress: string;
  userAgent: string;
}

export interface ConsentFilter {
  patientId?: string;
  consentType?: string;
  purpose?: string;
  status?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface ConsentResponse {
  data: ConsentLog[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface RevokeConsentRequest {
  consentId: string;
  revokedBy: string;
  reason: string;
}

@Injectable({
  providedIn: 'root'
})
export class ConsentService {
  private apiUrl = `${environment.apiUrl}/consent`;
  
  loading = signal(false);
  
  constructor(private http: HttpClient) {}

  /**
   * Get all consents for a patient
   */
  getPatientConsents(patientId: string): Observable<ConsentLog[]> {
    this.loading.set(true);
    return this.http.get<ConsentLog[]>(`${this.apiUrl}/patient/${patientId}`)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Get active consents for a patient
   */
  getActiveConsents(patientId: string): Observable<ConsentLog[]> {
    this.loading.set(true);
    return this.http.get<ConsentLog[]>(`${this.apiUrl}/patient/${patientId}/active`)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Check if patient has active consent for a specific purpose
   */
  hasActiveConsent(patientId: string, purpose: string): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/patient/${patientId}/has-consent`, {
      params: new HttpParams().set('purpose', purpose)
    });
  }

  /**
   * Record new consent
   */
  recordConsent(consent: Partial<ConsentLog>): Observable<ConsentLog> {
    this.loading.set(true);
    return this.http.post<ConsentLog>(this.apiUrl, consent)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Revoke consent
   */
  revokeConsent(request: RevokeConsentRequest): Observable<void> {
    this.loading.set(true);
    return this.http.post<void>(`${this.apiUrl}/${request.consentId}/revoke`, request)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Get available consent types
   */
  getAvailableConsentTypes(): string[] {
    return [
      'TREATMENT',
      'DATA_PROCESSING',
      'MARKETING',
      'RESEARCH',
      'SHARING',
      'TELEHEALTH',
      'BILLING',
      'OTHER'
    ];
  }

  /**
   * Get available purposes
   */
  getAvailablePurposes(): string[] {
    return [
      'HEALTHCARE',
      'BILLING',
      'MARKETING',
      'RESEARCH',
      'LEGAL_OBLIGATION',
      'LEGITIMATE_INTEREST',
      'OTHER'
    ];
  }

  /**
   * Get available statuses
   */
  getAvailableStatuses(): string[] {
    return ['Active', 'Revoked', 'Expired'];
  }
}
