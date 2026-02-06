import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ExternalServiceConfigurationDto,
  CreateExternalServiceConfigurationDto,
  UpdateExternalServiceConfigurationDto,
  ExternalServiceType
} from '../models/external-service.model';

@Injectable({
  providedIn: 'root'
})
export class ExternalServiceService {
  private apiUrl = `${environment.apiUrl}/ExternalServices`;

  constructor(private http: HttpClient) {}

  /**
   * Get all external service configurations
   */
  getAll(): Observable<ExternalServiceConfigurationDto[]> {
    return this.http.get<ExternalServiceConfigurationDto[]>(this.apiUrl);
  }

  /**
   * Get external service configuration by ID
   */
  getById(id: string): Observable<ExternalServiceConfigurationDto> {
    return this.http.get<ExternalServiceConfigurationDto>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get external service configuration by service type
   */
  getByServiceType(serviceType: ExternalServiceType, clinicId?: string): Observable<ExternalServiceConfigurationDto> {
    let url = `${this.apiUrl}/by-type/${serviceType}`;
    if (clinicId) {
      url += `?clinicId=${clinicId}`;
    }
    return this.http.get<ExternalServiceConfigurationDto>(url);
  }

  /**
   * Get external service configurations for a clinic
   */
  getByClinicId(clinicId: string): Observable<ExternalServiceConfigurationDto[]> {
    return this.http.get<ExternalServiceConfigurationDto[]>(`${this.apiUrl}/clinic/${clinicId}`);
  }

  /**
   * Get all active external service configurations
   */
  getActive(): Observable<ExternalServiceConfigurationDto[]> {
    return this.http.get<ExternalServiceConfigurationDto[]>(`${this.apiUrl}/active`);
  }

  /**
   * Create external service configuration
   */
  create(dto: CreateExternalServiceConfigurationDto): Observable<ExternalServiceConfigurationDto> {
    return this.http.post<ExternalServiceConfigurationDto>(this.apiUrl, dto);
  }

  /**
   * Update external service configuration
   */
  update(id: string, dto: UpdateExternalServiceConfigurationDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * Delete external service configuration
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Record successful sync for a service
   */
  recordSync(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/sync`, {});
  }

  /**
   * Record error for a service
   */
  recordError(id: string, errorMessage: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/error`, JSON.stringify(errorMessage), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
