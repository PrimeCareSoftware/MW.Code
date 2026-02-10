import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap, shareReplay } from 'rxjs/operators';
import { 
  Appointment, CreateAppointment, UpdateAppointment, DailyAgenda, WeekAgenda, AvailableSlot, Professional,
  BlockedTimeSlot, CreateBlockedTimeSlot, UpdateBlockedTimeSlot, 
  RecurringAppointmentPattern, CreateRecurringBlockedSlots,
  RecurringDeleteScope
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
  
  // Cache for API requests
  private cache = new Map<string, Observable<any>>();

  constructor(private http: HttpClient) { }

  create(appointment: CreateAppointment): Observable<Appointment> {
    return this.http.post<Appointment>(this.apiUrl, appointment)
      .pipe(
        tap(() => {
          const dateStr = new Date(appointment.scheduledDate).toISOString().split('T')[0];
          this.invalidateCache(appointment.clinicId, dateStr);
        })
      );
  }

  update(id: string, appointment: UpdateAppointment): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.apiUrl}/${id}`, appointment)
      .pipe(
        tap(() => this.invalidateCache())
      );
  }

  cancel(id: string, cancellationReason: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/cancel`, { cancellationReason })
      .pipe(
        tap(() => this.invalidateCache())
      );
  }

  getDailyAgenda(clinicId: string, date: string, professionalId?: string): Observable<DailyAgenda> {
    const cacheKey = `agenda_${clinicId}_${date}_${professionalId || 'all'}`;
    
    // Return from cache if exists
    if (this.cache.has(cacheKey)) {
      return this.cache.get(cacheKey)!;
    }

    let params = new HttpParams()
      .set('clinicId', clinicId)
      .set('date', date);
    
    if (professionalId) {
      params = params.set('professionalId', professionalId);
    }
    
    // ShareReplay to avoid multiple simultaneous requests
    const request$ = this.http.get<DailyAgenda>(`${this.apiUrl}/agenda`, { params })
      .pipe(
        shareReplay({ bufferSize: 1, refCount: true })
      );

    this.cache.set(cacheKey, request$);
    
    // Clear cache after 5 minutes
    setTimeout(() => this.cache.delete(cacheKey), 5 * 60 * 1000);
    
    return request$;
  }

  getWeekAgenda(clinicId: string, startDate: string, endDate: string, professionalId?: string): Observable<WeekAgenda> {
    const cacheKey = `week_agenda_${clinicId}_${startDate}_${endDate}_${professionalId || 'all'}`;
    
    // Return from cache if exists
    if (this.cache.has(cacheKey)) {
      return this.cache.get(cacheKey)!;
    }

    let params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    
    if (professionalId) {
      params = params.set('professionalId', professionalId);
    }
    
    // ShareReplay to avoid multiple simultaneous requests
    const request$ = this.http.get<WeekAgenda>(`${this.apiUrl}/week-agenda`, { params })
      .pipe(
        shareReplay({ bufferSize: 1, refCount: true })
      );

    this.cache.set(cacheKey, request$);
    
    // Clear cache after 5 minutes
    setTimeout(() => this.cache.delete(cacheKey), 5 * 60 * 1000);
    
    return request$;
  }

  // Invalidate cache when creating/updating appointments
  invalidateCache(clinicId?: string, date?: string): void {
    if (clinicId && date) {
      // Invalidate specific cache
      const prefix = `agenda_${clinicId}_${date}`;
      for (const key of this.cache.keys()) {
        if (key.startsWith(prefix)) {
          this.cache.delete(key);
        }
      }
    } else {
      // Clear all cache
      this.cache.clear();
    }
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
    return this.http.post<void>(`${this.apiUrl}/${id}/mark-as-paid`, { paymentReceiverType })
      .pipe(
        tap(() => this.invalidateCache())
      );
  }

  complete(id: string, notes?: string, registerPayment: boolean = false): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/complete`, { notes, registerPayment })
      .pipe(
        tap(() => this.invalidateCache())
      );
  }

  getProfessionals(): Observable<Professional[]> {
    return this.http.get<Professional[]>(`${this.usersApiUrl}/professionals`);
  }

  // Blocked Time Slots Methods
  createBlockedTimeSlot(blockedSlot: CreateBlockedTimeSlot): Observable<BlockedTimeSlot> {
    return this.http.post<BlockedTimeSlot>(this.blockedSlotsApiUrl, blockedSlot)
      .pipe(
        tap(() => this.invalidateCache())
      );
  }

  updateBlockedTimeSlot(id: string, blockedSlot: UpdateBlockedTimeSlot, updateSeries: boolean = false): Observable<BlockedTimeSlot> {
    const params = new HttpParams().set('updateSeries', updateSeries.toString());
    return this.http.put<BlockedTimeSlot>(`${this.blockedSlotsApiUrl}/${id}`, blockedSlot, { params })
      .pipe(
        tap(() => this.invalidateCache())
      );
  }

  deleteBlockedTimeSlot(
    id: string, 
    scope: RecurringDeleteScope = RecurringDeleteScope.ThisOccurrence,
    reason?: string
  ): Observable<void> {
    let params = new HttpParams().set('scope', scope.toString());
    
    if (reason) {
      params = params.set('reason', reason);
    }
    
    return this.http.delete<void>(`${this.blockedSlotsApiUrl}/${id}`, { params })
      .pipe(
        tap(() => this.invalidateCache())
      );
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
