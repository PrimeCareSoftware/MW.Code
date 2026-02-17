import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ProfessionalSpecialty } from '../models/appointment.model';

export interface ConsultationFormProfileDto {
  id: string;
  name: string;
  description: string;
  specialty: ProfessionalSpecialty;
  isActive: boolean;
  isSystemDefault: boolean;
  
  // Field visibility
  showChiefComplaint: boolean;
  showHistoryOfPresentIllness: boolean;
  showPastMedicalHistory: boolean;
  showFamilyHistory: boolean;
  showLifestyleHabits: boolean;
  showCurrentMedications: boolean;
  
  // Field required controls
  requireChiefComplaint: boolean;
  requireHistoryOfPresentIllness: boolean;
  requirePastMedicalHistory: boolean;
  requireFamilyHistory: boolean;
  requireLifestyleHabits: boolean;
  requireCurrentMedications: boolean;
  requireClinicalExamination: boolean;
  requireDiagnosticHypothesis: boolean;
  requireInformedConsent: boolean;
  requireTherapeuticPlan: boolean;
  
  customFields: CustomFieldDto[];
  createdAt: string;
  updatedAt?: string;
}

export interface CustomFieldDto {
  fieldKey: string;
  label: string;
  fieldType: CustomFieldType;
  isRequired: boolean;
  displayOrder: number;
  placeholder?: string;
  defaultValue?: string;
  helpText?: string;
  options?: string[];
}

export enum CustomFieldType {
  TextoCurto = 0,
  TextoLongo = 1,
  Numero = 2,
  Data = 3,
  SelecaoUnica = 4,
  SelecaoMultipla = 5,
  CheckBox = 6,
  SimNao = 7
}

export interface CreateConsultationFormProfileDto {
  name: string;
  description: string;
  specialty: ProfessionalSpecialty;
  
  // Field visibility
  showChiefComplaint?: boolean;
  showHistoryOfPresentIllness?: boolean;
  showPastMedicalHistory?: boolean;
  showFamilyHistory?: boolean;
  showLifestyleHabits?: boolean;
  showCurrentMedications?: boolean;
  
  // Field required controls
  requireChiefComplaint?: boolean;
  requireHistoryOfPresentIllness?: boolean;
  requirePastMedicalHistory?: boolean;
  requireFamilyHistory?: boolean;
  requireLifestyleHabits?: boolean;
  requireCurrentMedications?: boolean;
  requireClinicalExamination?: boolean;
  requireDiagnosticHypothesis?: boolean;
  requireInformedConsent?: boolean;
  requireTherapeuticPlan?: boolean;
  
  customFields?: CustomFieldDto[];
}

export interface UpdateConsultationFormProfileDto {
  name: string;
  description: string;
  
  // Field visibility
  showChiefComplaint: boolean;
  showHistoryOfPresentIllness: boolean;
  showPastMedicalHistory: boolean;
  showFamilyHistory: boolean;
  showLifestyleHabits: boolean;
  showCurrentMedications: boolean;
  
  // Field required controls
  requireChiefComplaint: boolean;
  requireHistoryOfPresentIllness: boolean;
  requirePastMedicalHistory: boolean;
  requireFamilyHistory: boolean;
  requireLifestyleHabits: boolean;
  requireCurrentMedications: boolean;
  requireClinicalExamination: boolean;
  requireDiagnosticHypothesis: boolean;
  requireInformedConsent: boolean;
  requireTherapeuticPlan: boolean;
  
  customFields?: CustomFieldDto[];
}

@Injectable({
  providedIn: 'root'
})
export class ConsultationFormProfileService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/consultation-form-profiles`;

  /**
   * Get all active consultation form profiles
   */
  getAllProfiles(): Observable<ConsultationFormProfileDto[]> {
    return this.http.get<ConsultationFormProfileDto[]>(this.baseUrl);
  }

  /**
   * Get consultation form profile by ID
   */
  getProfileById(id: string): Observable<ConsultationFormProfileDto> {
    return this.http.get<ConsultationFormProfileDto>(`${this.baseUrl}/${id}`);
  }

  /**
   * Get consultation form profiles by specialty
   */
  getProfilesBySpecialty(specialty: ProfessionalSpecialty): Observable<ConsultationFormProfileDto[]> {
    return this.http.get<ConsultationFormProfileDto[]>(`${this.baseUrl}/specialty/${specialty}`);
  }

  /**
   * Create a new consultation form profile
   */
  createProfile(dto: CreateConsultationFormProfileDto): Observable<ConsultationFormProfileDto> {
    return this.http.post<ConsultationFormProfileDto>(this.baseUrl, dto);
  }

  /**
   * Update a consultation form profile
   */
  updateProfile(id: string, dto: UpdateConsultationFormProfileDto): Observable<ConsultationFormProfileDto> {
    return this.http.put<ConsultationFormProfileDto>(`${this.baseUrl}/${id}`, dto);
  }

  /**
   * Delete a consultation form profile
   * Note: System default profiles cannot be deleted
   */
  deleteProfile(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
