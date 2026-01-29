import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ImpersonateRequest,
  ImpersonationResult,
  GrantCreditRequest,
  ApplyDiscountRequest,
  SuspendRequest,
  ExportDataRequest,
  MigratePlanRequest,
  SendCustomEmailRequest,
  SmartActionResponse
} from '../models/smart-action.model';

@Injectable({
  providedIn: 'root'
})
export class SmartActionService {
  private apiUrl = `${environment.apiUrl}/system-admin/smart-actions`;

  constructor(private http: HttpClient) {}

  /**
   * Impersonate a clinic
   */
  impersonate(request: ImpersonateRequest): Observable<ImpersonationResult> {
    return this.http.post<ImpersonationResult>(`${this.apiUrl}/impersonate`, request);
  }

  /**
   * Grant credit days to a clinic
   */
  grantCredit(request: GrantCreditRequest): Observable<SmartActionResponse> {
    return this.http.post<SmartActionResponse>(`${this.apiUrl}/grant-credit`, request);
  }

  /**
   * Apply discount to a clinic
   */
  applyDiscount(request: ApplyDiscountRequest): Observable<SmartActionResponse> {
    return this.http.post<SmartActionResponse>(`${this.apiUrl}/apply-discount`, request);
  }

  /**
   * Suspend clinic temporarily
   */
  suspend(request: SuspendRequest): Observable<SmartActionResponse> {
    return this.http.post<SmartActionResponse>(`${this.apiUrl}/suspend`, request);
  }

  /**
   * Export clinic data
   */
  exportData(request: ExportDataRequest): Observable<Blob> {
    return this.http.post(`${this.apiUrl}/export-data`, request, {
      responseType: 'blob'
    });
  }

  /**
   * Migrate clinic to new plan
   */
  migratePlan(request: MigratePlanRequest): Observable<SmartActionResponse> {
    return this.http.post<SmartActionResponse>(`${this.apiUrl}/migrate-plan`, request);
  }

  /**
   * Send custom email to clinic
   */
  sendEmail(request: SendCustomEmailRequest): Observable<SmartActionResponse> {
    return this.http.post<SmartActionResponse>(`${this.apiUrl}/send-email`, request);
  }
}
