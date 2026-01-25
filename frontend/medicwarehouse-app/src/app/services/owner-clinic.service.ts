import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface ClinicDto {
  id: string;
  name: string;
  tradeName: string;
  document: string;
  phone: string;
  email: string;
  address: string;
  openingTime: string;
  closingTime: string;
  appointmentDurationMinutes: number;
  allowEmergencySlots: boolean;
  isActive: boolean;
  defaultPaymentReceiverType: string;
  numberOfRooms: number;
  notifyPrimaryDoctorOnOtherDoctorAppointment: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateClinicDto {
  name: string;
  tradeName: string;
  document: string;
  phone: string;
  email: string;
  address: string;
  openingTime: string;
  closingTime: string;
  appointmentDurationMinutes: number;
}

export interface UpdateClinicDto {
  name: string;
  tradeName: string;
  phone: string;
  email: string;
  address: string;
  openingTime: string;
  closingTime: string;
  appointmentDurationMinutes: number;
  allowEmergencySlots: boolean;
  defaultPaymentReceiverType?: string;
  numberOfRooms?: number;
  notifyPrimaryDoctorOnOtherDoctorAppointment?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class OwnerClinicService {
  private apiUrl = `${environment.apiUrl}/owner-clinics`;

  constructor(private http: HttpClient) {}

  getMyClinics(): Observable<ClinicDto[]> {
    return this.http.get<ClinicDto[]>(this.apiUrl);
  }

  getClinicById(id: string): Observable<ClinicDto> {
    return this.http.get<ClinicDto>(`${this.apiUrl}/${id}`);
  }

  createClinic(clinic: CreateClinicDto): Observable<ClinicDto> {
    return this.http.post<ClinicDto>(this.apiUrl, clinic);
  }

  updateClinic(id: string, clinic: UpdateClinicDto): Observable<ClinicDto> {
    return this.http.put<ClinicDto>(`${this.apiUrl}/${id}`, clinic);
  }
}
