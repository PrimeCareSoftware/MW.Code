import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ClinicSummary,
  ClinicDetail,
  PaginatedClinics,
  SystemAnalytics,
  UpdateSubscriptionRequest,
  ManualOverrideRequest,
  CreateClinicRequest,
  SystemOwner,
  CreateSystemOwnerRequest
} from '../models/system-admin.model';

@Injectable({
  providedIn: 'root'
})
export class SystemAdminService {
  private apiUrl = `${environment.apiUrl}/system-admin`;

  constructor(private http: HttpClient) {}

  /**
   * Get all clinics with pagination and filtering
   */
  getClinics(status?: string, page: number = 1, pageSize: number = 20): Observable<PaginatedClinics> {
    const params: any = { page, pageSize };
    if (status) {
      params.status = status;
    }
    return this.http.get<PaginatedClinics>(`${this.apiUrl}/clinics`, { params });
  }

  /**
   * Get detailed information about a specific clinic
   */
  getClinic(id: string): Observable<ClinicDetail> {
    return this.http.get<ClinicDetail>(`${this.apiUrl}/clinics/${id}`);
  }

  /**
   * Create a new clinic
   */
  createClinic(request: CreateClinicRequest): Observable<{ message: string; clinicId: string }> {
    return this.http.post<{ message: string; clinicId: string }>(
      `${this.apiUrl}/clinics`,
      request
    );
  }

  /**
   * Toggle clinic active status
   */
  toggleClinicStatus(id: string): Observable<{ message: string; isActive: boolean }> {
    return this.http.post<{ message: string; isActive: boolean }>(
      `${this.apiUrl}/clinics/${id}/toggle-status`,
      {}
    );
  }

  /**
   * Update clinic subscription
   */
  updateSubscription(id: string, request: UpdateSubscriptionRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
      `${this.apiUrl}/clinics/${id}/subscription`,
      request
    );
  }

  /**
   * Get system-wide analytics
   */
  getAnalytics(): Observable<SystemAnalytics> {
    return this.http.get<SystemAnalytics>(`${this.apiUrl}/analytics`);
  }

  /**
   * Enable manual override for a clinic subscription
   */
  enableManualOverride(id: string, request: ManualOverrideRequest): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/clinics/${id}/subscription/manual-override/enable`,
      request
    );
  }

  /**
   * Disable manual override for a clinic subscription
   */
  disableManualOverride(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/clinics/${id}/subscription/manual-override/disable`,
      {}
    );
  }

  /**
   * Get all system owners
   */
  getSystemOwners(): Observable<SystemOwner[]> {
    return this.http.get<SystemOwner[]>(`${this.apiUrl}/system-owners`);
  }

  /**
   * Create a new system owner
   */
  createSystemOwner(request: CreateSystemOwnerRequest): Observable<{ message: string; ownerId: string }> {
    return this.http.post<{ message: string; ownerId: string }>(
      `${this.apiUrl}/system-owners`,
      request
    );
  }

  /**
   * Toggle system owner active status
   */
  toggleSystemOwnerStatus(id: string): Observable<{ message: string; isActive: boolean }> {
    return this.http.post<{ message: string; isActive: boolean }>(
      `${this.apiUrl}/system-owners/${id}/toggle-status`,
      {}
    );
  }
}
