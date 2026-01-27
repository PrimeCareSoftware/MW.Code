import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConsultationFormConfigurationDto } from '../models/consultation-form-configuration.model';
import { ApiConfigService } from './api-config.service';

@Injectable({
  providedIn: 'root'
})
export class ConsultationFormConfigurationService {
  private readonly http = inject(HttpClient);
  private readonly apiConfig = inject(ApiConfigService);
  private readonly baseUrl = `${this.apiConfig.getApiUrl()}/api/consultation-form-configurations`;

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
}
