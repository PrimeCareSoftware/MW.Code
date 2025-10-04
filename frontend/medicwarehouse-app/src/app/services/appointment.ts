import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Appointment, CreateAppointment, UpdateAppointment, DailyAgenda, AvailableSlot } from '../models/appointment.model';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl = 'http://localhost:5000/api/appointments';

  constructor(private http: HttpClient) { }

  create(appointment: CreateAppointment): Observable<Appointment> {
    return this.http.post<Appointment>(this.apiUrl, appointment);
  }

  update(id: string, appointment: UpdateAppointment): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.apiUrl}/${id}`, appointment);
  }

  cancel(id: string, reason: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/cancel`, { reason });
  }

  getDailyAgenda(clinicId: string, date: string): Observable<DailyAgenda> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('date', date);
    return this.http.get<DailyAgenda>(`${this.apiUrl}/agenda`, { params });
  }

  getAvailableSlots(clinicId: string, date: string, duration: number): Observable<AvailableSlot[]> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('date', date)
      .set('durationMinutes', duration.toString());
    return this.http.get<AvailableSlot[]>(`${this.apiUrl}/available-slots`, { params });
  }

  getById(id: string): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.apiUrl}/${id}`);
  }
}
