import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Appointment, CreateAppointment, UpdateAppointment, DailyAgenda, AvailableSlot, Professional,
  BlockedTimeSlot, CreateBlockedTimeSlot, UpdateBlockedTimeSlot, 
  RecurringAppointmentPattern, CreateRecurringBlockedSlots
} from '../models/appointment.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl = `${environment.apiUrl}/appointments`;
  private usersApiUrl = `${environment.apiUrl}/users`;
  private blockedSlotsApiUrl = `${environment.apiUrl}/blocked-time-slots`;
  private recurringApiUrl = `${environment.apiUrl}/recurring-appointments`;

  constructor(private http: HttpClient) { }

  create(appointment: CreateAppointment): Observable<Appointment> {
    return this.http.post<Appointment>(this.apiUrl, appointment);
  }

  update(id: string, appointment: UpdateAppointment): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.apiUrl}/${id}`, appointment);
  }

  cancel(id: string, cancellationReason: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/cancel`, { cancellationReason });
  }

  getDailyAgenda(clinicId: string, date: string, professionalId?: string): Observable<DailyAgenda> {
    let params = new HttpParams()
      .set('clinicId', clinicId)
      .set('date', date);
    
    if (professionalId) {
      params = params.set('professionalId', professionalId);
    }
    
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

  markAsPaid(id: string, paymentReceiverType: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/mark-as-paid`, { paymentReceiverType });
  }

  complete(id: string, notes?: string, registerPayment: boolean = false): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/complete`, { notes, registerPayment });
  }

  getProfessionals(): Observable<Professional[]> {
    return this.http.get<Professional[]>(`${this.usersApiUrl}/professionals`);
  }

  // Blocked Time Slots Methods
  createBlockedTimeSlot(blockedSlot: CreateBlockedTimeSlot): Observable<BlockedTimeSlot> {
    return this.http.post<BlockedTimeSlot>(this.blockedSlotsApiUrl, blockedSlot);
  }

  updateBlockedTimeSlot(id: string, blockedSlot: UpdateBlockedTimeSlot): Observable<BlockedTimeSlot> {
    return this.http.put<BlockedTimeSlot>(`${this.blockedSlotsApiUrl}/${id}`, blockedSlot);
  }

  deleteBlockedTimeSlot(id: string): Observable<void> {
    return this.http.delete<void>(`${this.blockedSlotsApiUrl}/${id}`);
  }

  getBlockedTimeSlotsByDate(date: string, clinicId: string, professionalId?: string): Observable<BlockedTimeSlot[]> {
    let params = new HttpParams()
      .set('date', date)
      .set('clinicId', clinicId);
    
    if (professionalId) {
      params = params.set('professionalId', professionalId);
    }
    
    return this.http.get<BlockedTimeSlot[]>(this.blockedSlotsApiUrl, { params });
  }

  getBlockedTimeSlotsByDateRange(startDate: string, endDate: string, clinicId: string): Observable<BlockedTimeSlot[]> {
    const params = new HttpParams()
      .set('startDate', startDate)
      .set('endDate', endDate)
      .set('clinicId', clinicId);
    
    return this.http.get<BlockedTimeSlot[]>(`${this.blockedSlotsApiUrl}/range`, { params });
  }

  getBlockedTimeSlotById(id: string): Observable<BlockedTimeSlot> {
    return this.http.get<BlockedTimeSlot>(`${this.blockedSlotsApiUrl}/${id}`);
  }

  // Recurring Blocked Slots Methods
  createRecurringBlockedSlots(pattern: CreateRecurringBlockedSlots): Observable<RecurringAppointmentPattern> {
    return this.http.post<RecurringAppointmentPattern>(`${this.recurringApiUrl}/blocked-slots`, pattern);
  }

  getRecurringPatternsByClinic(clinicId: string): Observable<RecurringAppointmentPattern[]> {
    return this.http.get<RecurringAppointmentPattern[]>(`${this.recurringApiUrl}/clinic/${clinicId}`);
  }

  getRecurringPatternsByProfessional(professionalId: string): Observable<RecurringAppointmentPattern[]> {
    return this.http.get<RecurringAppointmentPattern[]>(`${this.recurringApiUrl}/professional/${professionalId}`);
  }
}
