import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConsultationFormConfigurationDto, CreateConsultationFormConfigurationDto, UpdateConsultationFormConfigurationDto } from '../models/consultation-form-configuration.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ConsultationFormConfigurationService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/consultation-form-configurations`;

  /**
   * Get active consultation form configuration for a clinic
   */
  getActiveConfigurationByClinicId(clinicId: string): Observable<ConsultationFormConfigurationDto> {
    return this.http.get<ConsultationFormConfigurationDto>(`${this.baseUrl}/clinic/${clinicId}`);
  }

  /**
   * Get consultation form configuration by ID
   */
  getConfigurationById(id: string): Observable<ConsultationFormConfigurationDto> {
    return this.http.get<ConsultationFormConfigurationDto>(`${this.baseUrl}/${id}`);
  }

  /**
   * Create a new consultation form configuration
   */
  createConfiguration(dto: CreateConsultationFormConfigurationDto): Observable<ConsultationFormConfigurationDto> {
    return this.http.post<ConsultationFormConfigurationDto>(this.baseUrl, dto);
  }

  /**
   * Create a configuration from an existing profile
   */
  createConfigurationFromProfile(clinicId: string, profileId: string): Observable<ConsultationFormConfigurationDto> {
    return this.http.post<ConsultationFormConfigurationDto>(`${this.baseUrl}/from-profile`, {
      clinicId,
      profileId
    });
  }

  /**
   * Update a consultation form configuration
   */
  updateConfiguration(id: string, dto: UpdateConsultationFormConfigurationDto): Observable<ConsultationFormConfigurationDto> {
    return this.http.put<ConsultationFormConfigurationDto>(`${this.baseUrl}/${id}`, dto);
  }

  /**
   * Delete a consultation form configuration
   */
  deleteConfiguration(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  /**
   * Get terminology for a specific professional specialty
   */
  getTerminology(specialty: string): Observable<{ [key: string]: string }> {
    return this.http.get<{ [key: string]: string }>(`${this.baseUrl}/terminology/${specialty}`);
  }
}
