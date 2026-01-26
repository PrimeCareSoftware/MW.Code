import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  Appointment, 
  Specialty, 
  Doctor, 
  AvailableSlotsResponse, 
  BookAppointmentRequest, 
  BookAppointmentResponse,
  CancelAppointmentRequest,
  RescheduleAppointmentRequest
} from '../models/appointment.model';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl = `${environment.apiUrl}/appointments`;

  constructor(private http: HttpClient) {}

  getMyAppointments(skip: number = 0, take: number = 50): Observable<Appointment[]> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());
    return this.http.get<Appointment[]>(this.apiUrl, { params });
  }

  getUpcomingAppointments(take: number = 10): Observable<Appointment[]> {
    const params = new HttpParams().set('take', take.toString());
    return this.http.get<Appointment[]>(`${this.apiUrl}/upcoming`, { params });
  }

  getAppointmentById(id: string): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.apiUrl}/${id}`);
  }

  getAppointmentsByStatus(status: string, skip: number = 0, take: number = 50): Observable<Appointment[]> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());
    return this.http.get<Appointment[]>(`${this.apiUrl}/status/${status}`, { params });
  }

  getAppointmentsCount(): Observable<{ count: number }> {
    return this.http.get<{ count: number }>(`${this.apiUrl}/count`);
  }

  // Booking-related methods
  getSpecialties(): Observable<Specialty[]> {
    return this.http.get<Specialty[]>(`${this.apiUrl}/specialties`);
  }

  getDoctors(specialty?: string): Observable<Doctor[]> {
    let params = new HttpParams();
    if (specialty) {
      params = params.set('specialty', specialty);
    }
    return this.http.get<Doctor[]>(`${this.apiUrl}/doctors`, { params });
  }

  getAvailableSlots(doctorId: string, date: string): Observable<AvailableSlotsResponse> {
    const params = new HttpParams()
      .set('doctorId', doctorId)
      .set('date', date);
    return this.http.get<AvailableSlotsResponse>(`${this.apiUrl}/available-slots`, { params });
  }

  bookAppointment(request: BookAppointmentRequest): Observable<BookAppointmentResponse> {
    return this.http.post<BookAppointmentResponse>(`${this.apiUrl}/book`, request);
  }

  confirmAppointment(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/confirm`, {});
  }

  cancelAppointment(id: string, request: CancelAppointmentRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/cancel`, request);
  }

  rescheduleAppointment(id: string, request: RescheduleAppointmentRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/reschedule`, request);
  }
}
