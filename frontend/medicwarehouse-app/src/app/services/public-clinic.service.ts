import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PublicClinicDto {
  id: string;
  name: string;
  tradeName: string;
  phone: string;
  email: string;
  address: string;
  city: string;
  state: string;
  openingTime: string;
  closingTime: string;
  appointmentDurationMinutes: number;
  isAcceptingNewPatients: boolean;
  clinicType: string; // Medical, Dental, Nutritionist, etc.
  whatsAppNumber?: string;
  rating?: number;
  reviewCount?: number;
  professionalCount?: number;
}

export interface SearchClinicsRequest {
  name?: string;
  city?: string;
  state?: string;
  clinicType?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface SearchClinicsResult {
  clinics: PublicClinicDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface AvailableSlotDto {
  date: string;
  time: string;
  durationMinutes: number;
  isAvailable: boolean;
}

export interface PublicAppointmentRequest {
  clinicId: string;
  scheduledDate: string;
  scheduledTime: string;
  durationMinutes: number;
  patientName: string;
  patientCpf: string;
  patientBirthDate: string;
  patientPhone: string;
  patientEmail: string;
  notes?: string;
}

export interface PublicAppointmentResponse {
  appointmentId: string;
  clinicId: string;
  clinicName: string;
  scheduledDate: string;
  scheduledTime: string;
  status: string;
  message: string;
}

/**
 * Serviço para API pública de clínicas e agendamentos.
 * Não requer autenticação - usado no site público.
 */
@Injectable({
  providedIn: 'root'
})
export class PublicClinicService {
  private readonly apiUrl = `${environment.apiUrl}/public/clinics`;

  constructor(private http: HttpClient) {}

  /**
   * Busca clínicas disponíveis publicamente
   */
  searchClinics(request: SearchClinicsRequest): Observable<SearchClinicsResult> {
    let params = new HttpParams();
    
    if (request.name !== undefined && request.name !== null) {
      params = params.set('name', request.name);
    }
    if (request.city !== undefined && request.city !== null) {
      params = params.set('city', request.city);
    }
    if (request.state !== undefined && request.state !== null) {
      params = params.set('state', request.state);
    }
    if (request.clinicType !== undefined && request.clinicType !== null) {
      params = params.set('clinicType', request.clinicType);
    }
    if (request.pageNumber !== undefined && request.pageNumber !== null) {
      params = params.set('pageNumber', request.pageNumber.toString());
    }
    if (request.pageSize !== undefined && request.pageSize !== null) {
      params = params.set('pageSize', request.pageSize.toString());
    }

    return this.http.get<SearchClinicsResult>(`${this.apiUrl}/search`, { params });
  }

  /**
   * Obtém detalhes de uma clínica específica
   */
  getClinicById(clinicId: string): Observable<PublicClinicDto> {
    return this.http.get<PublicClinicDto>(`${this.apiUrl}/${clinicId}`);
  }

  /**
   * Obtém horários disponíveis de uma clínica
   */
  getAvailableSlots(clinicId: string, date: string, durationMinutes: number = 30): Observable<AvailableSlotDto[]> {
    let params = new HttpParams()
      .set('date', date)
      .set('durationMinutes', durationMinutes.toString());

    return this.http.get<AvailableSlotDto[]>(`${this.apiUrl}/${clinicId}/available-slots`, { params });
  }

  /**
   * Cria um agendamento público (sem autenticação)
   */
  createPublicAppointment(request: PublicAppointmentRequest): Observable<PublicAppointmentResponse> {
    return this.http.post<PublicAppointmentResponse>(`${this.apiUrl}/appointments`, request);
  }
}
