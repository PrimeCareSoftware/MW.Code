import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Appointment } from '../models/appointment.model';

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
}
