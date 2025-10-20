import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  Procedure, 
  CreateProcedure, 
  UpdateProcedure, 
  AppointmentProcedure, 
  AddProcedureToAppointment,
  AppointmentBillingSummary
} from '../models/procedure.model';

@Injectable({
  providedIn: 'root'
})
export class ProcedureService {
  private apiUrl = `${environment.apiUrl}/procedures`;

  constructor(private http: HttpClient) {}

  getAll(activeOnly: boolean = true): Observable<Procedure[]> {
    const params = new HttpParams().set('activeOnly', activeOnly.toString());
    return this.http.get<Procedure[]>(this.apiUrl, { params });
  }

  getById(id: string): Observable<Procedure> {
    return this.http.get<Procedure>(`${this.apiUrl}/${id}`);
  }

  create(procedure: CreateProcedure): Observable<Procedure> {
    return this.http.post<Procedure>(this.apiUrl, procedure);
  }

  update(id: string, procedure: UpdateProcedure): Observable<Procedure> {
    return this.http.put<Procedure>(`${this.apiUrl}/${id}`, procedure);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addProcedureToAppointment(appointmentId: string, data: AddProcedureToAppointment): Observable<AppointmentProcedure> {
    return this.http.post<AppointmentProcedure>(
      `${this.apiUrl}/appointments/${appointmentId}/procedures`, 
      data
    );
  }

  getAppointmentProcedures(appointmentId: string): Observable<AppointmentProcedure[]> {
    return this.http.get<AppointmentProcedure[]>(
      `${this.apiUrl}/appointments/${appointmentId}/procedures`
    );
  }

  getAppointmentBillingSummary(appointmentId: string): Observable<AppointmentBillingSummary> {
    return this.http.get<AppointmentBillingSummary>(
      `${this.apiUrl}/appointments/${appointmentId}/billing-summary`
    );
  }
}
