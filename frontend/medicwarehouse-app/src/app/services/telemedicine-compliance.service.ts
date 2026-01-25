import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ConsentRequest {
  patientId: string;
  appointmentId?: string;
  acceptsRecording: boolean;
  acceptsDataSharing: boolean;
  digitalSignature?: string;
}

export interface ConsentResponse {
  id: string;
  patientId: string;
  appointmentId?: string;
  consentDate: string;
  acceptsRecording: boolean;
  acceptsDataSharing: boolean;
  isActive: boolean;
  revokedAt?: string;
  revocationReason?: string;
}

export interface SessionComplianceValidation {
  sessionId: string;
  isCompliant: boolean;
  compliance: {
    patientConsent: {
      isValid: boolean;
      required: boolean;
      message: string;
    };
    providerIdentity: {
      isVerified: boolean;
      required: boolean;
      message: string;
    };
    patientIdentity: {
      isVerified: boolean;
      required: boolean;
      message: string;
    };
  };
  canStart: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class TelemedicineComplianceService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiUrl || 'http://localhost:5000/api';
  private telemedicineUrl = `${this.baseUrl}/telemedicine`;

  /**
   * Records patient consent for telemedicine (CFM 2.314 requirement)
   */
  recordConsent(request: ConsentRequest, tenantId: string): Observable<ConsentResponse> {
    const headers = new HttpHeaders({
      'X-Tenant-Id': tenantId,
      'Content-Type': 'application/json'
    });

    return this.http.post<ConsentResponse>(
      `${this.telemedicineUrl}/consent`,
      request,
      { headers }
    );
  }

  /**
   * Gets consent by ID
   */
  getConsentById(consentId: string, tenantId: string): Observable<ConsentResponse> {
    const headers = new HttpHeaders({
      'X-Tenant-Id': tenantId
    });

    return this.http.get<ConsentResponse>(
      `${this.telemedicineUrl}/consent/${consentId}`,
      { headers }
    );
  }

  /**
   * Gets most recent consent for a patient
   */
  getMostRecentConsent(patientId: string, tenantId: string): Observable<ConsentResponse> {
    const headers = new HttpHeaders({
      'X-Tenant-Id': tenantId
    });

    return this.http.get<ConsentResponse>(
      `${this.telemedicineUrl}/consent/patient/${patientId}/most-recent`,
      { headers }
    );
  }

  /**
   * Checks if patient has valid active consent
   */
  hasValidConsent(patientId: string, tenantId: string): Observable<boolean> {
    const headers = new HttpHeaders({
      'X-Tenant-Id': tenantId
    });

    return this.http.get<boolean>(
      `${this.telemedicineUrl}/consent/patient/${patientId}/has-valid-consent`,
      { headers }
    );
  }

  /**
   * Gets the CFM 2.314 consent text
   */
  getConsentText(includeRecording: boolean = false): Observable<{ consentText: string }> {
    return this.http.get<{ consentText: string }>(
      `${this.telemedicineUrl}/consent/consent-text?includeRecording=${includeRecording}`
    );
  }

  /**
   * Validates CFM 2.314 compliance for a session before starting
   * Checks consent and identity verification for both participants
   */
  validateSessionCompliance(sessionId: string, tenantId: string): Observable<SessionComplianceValidation> {
    const headers = new HttpHeaders({
      'X-Tenant-Id': tenantId
    });

    return this.http.get<SessionComplianceValidation>(
      `${this.baseUrl}/sessions/${sessionId}/validate-compliance`,
      { headers }
    );
  }

  /**
   * Validates first appointment rule (CFM 2.314 requirement)
   */
  validateFirstAppointment(
    patientId: string,
    providerId: string,
    justification: string | null,
    tenantId: string
  ): Observable<any> {
    const headers = new HttpHeaders({
      'X-Tenant-Id': tenantId,
      'Content-Type': 'application/json'
    });

    return this.http.post<any>(
      `${this.telemedicineUrl}/consent/validate-first-appointment`,
      { patientId, providerId, justification },
      { headers }
    );
  }
}
