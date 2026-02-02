import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export enum BusinessType {
  SoloPractitioner = 1,
  SmallClinic = 2,
  MediumClinic = 3,
  LargeClinic = 4
}

export enum ProfessionalSpecialty {
  Medico = 1,
  Psicologo = 2,
  Nutricionista = 3,
  Fisioterapeuta = 4,
  Dentista = 5,
  Enfermeiro = 6,
  TerapeutaOcupacional = 7,
  Fonoaudiologo = 8,
  Outro = 99
}

export interface BusinessConfiguration {
  id: string;
  clinicId: string;
  businessType: BusinessType;
  primarySpecialty: ProfessionalSpecialty;
  
  // Clinical Features
  electronicPrescription: boolean;
  labIntegration: boolean;
  vaccineControl: boolean;
  inventoryManagement: boolean;
  
  // Administrative Features
  multiRoom: boolean;
  receptionQueue: boolean;
  financialModule: boolean;
  healthInsurance: boolean;
  
  // Consultation Features
  telemedicine: boolean;
  homeVisit: boolean;
  groupSessions: boolean;
  
  // Marketing Features
  publicProfile: boolean;
  onlineBooking: boolean;
  patientReviews: boolean;
  
  // Advanced Features
  biReports: boolean;
  apiAccess: boolean;
  whiteLabel: boolean;
  
  createdAt: string;
  updatedAt: string;
}

export interface CreateBusinessConfigurationDto {
  clinicId: string;
  businessType: BusinessType;
  primarySpecialty: ProfessionalSpecialty;
}

export interface UpdateBusinessTypeDto {
  businessType: BusinessType;
}

export interface UpdatePrimarySpecialtyDto {
  primarySpecialty: ProfessionalSpecialty;
}

export interface UpdateFeatureDto {
  featureName: string;
  enabled: boolean;
}

export interface FeatureCheckResponse {
  featureName: string;
  enabled: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class BusinessConfigurationService {
  private readonly baseUrl = '/api/BusinessConfiguration';

  constructor(private http: HttpClient) {}

  /**
   * Get business configuration for a clinic
   */
  getByClinicId(clinicId: string): Observable<BusinessConfiguration> {
    return this.http.get<BusinessConfiguration>(`${this.baseUrl}/clinic/${clinicId}`);
  }

  /**
   * Create a new business configuration
   */
  create(dto: CreateBusinessConfigurationDto): Observable<BusinessConfiguration> {
    return this.http.post<BusinessConfiguration>(this.baseUrl, dto);
  }

  /**
   * Update the business type
   */
  updateBusinessType(configId: string, dto: UpdateBusinessTypeDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${configId}/business-type`, dto);
  }

  /**
   * Update the primary specialty
   */
  updatePrimarySpecialty(configId: string, dto: UpdatePrimarySpecialtyDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${configId}/primary-specialty`, dto);
  }

  /**
   * Update a feature flag
   */
  updateFeature(configId: string, dto: UpdateFeatureDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${configId}/feature`, dto);
  }

  /**
   * Check if a feature is enabled for a clinic
   */
  isFeatureEnabled(clinicId: string, featureName: string): Observable<FeatureCheckResponse> {
    return this.http.get<FeatureCheckResponse>(
      `${this.baseUrl}/clinic/${clinicId}/feature/${featureName}`
    );
  }

  /**
   * Get terminology map for a clinic
   */
  getTerminology(clinicId: string): Observable<{ [key: string]: string }> {
    return this.http.get<{ [key: string]: string }>(
      `${this.baseUrl}/clinic/${clinicId}/terminology`
    );
  }
}
