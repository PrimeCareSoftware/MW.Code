import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  PatientJourney,
  PatientJourneyMetrics,
  CreatePatientTouchpoint,
  UpdatePatientJourneyMetrics,
  AdvanceJourneyStage
} from '../../models/crm';

@Injectable({
  providedIn: 'root'
})
export class PatientJourneyService {
  private readonly apiUrl = `${environment.apiUrl}/crm/journey`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json'
    });
  }

  private handleError(error: HttpErrorResponse & { userMessage?: string }): Observable<never> {
    // Preserve the original HttpErrorResponse so that any normalized fields
    // (e.g., userMessage, status) added by the global error interceptor are not lost.
    const anyError = error as any;
    
    // Respect userMessage already defined by errorInterceptor
    let fallbackMessage = 'Ocorreu um erro desconhecido';
    
    if (error.error instanceof ErrorEvent) {
      fallbackMessage = `Erro: ${error.error.message}`;
    } else {
      fallbackMessage = error.error?.message || `Erro ${error.status}: ${error.statusText}`;
    }
    
    // If interceptor hasn't set userMessage yet, define a fallback
    if (!anyError.userMessage) {
      anyError.userMessage = fallbackMessage;
    }
    
    console.error('Patient Journey Service Error:', error);
    // Rethrow original HTTP error to preserve status and additional fields
    return throwError(() => error);
  }

  getOrCreateJourney(patientId: string): Observable<PatientJourney> {
    return this.http.get<PatientJourney>(`${this.apiUrl}/${patientId}`, { headers: this.getHeaders() })
      .pipe(
        map(journey => this.parseServerDates(journey)),
        catchError(this.handleError)
      );
  }

  advanceStage(patientId: string, stageData: AdvanceJourneyStage): Observable<PatientJourney> {
    return this.http.post<PatientJourney>(`${this.apiUrl}/${patientId}/advance`, stageData, { headers: this.getHeaders() })
      .pipe(
        map(journey => this.parseServerDates(journey)),
        catchError(this.handleError)
      );
  }

  addTouchpoint(patientId: string, touchpoint: CreatePatientTouchpoint): Observable<PatientJourney> {
    return this.http.post<PatientJourney>(`${this.apiUrl}/${patientId}/touchpoint`, touchpoint, { headers: this.getHeaders() })
      .pipe(
        map(journey => this.parseServerDates(journey)),
        catchError(this.handleError)
      );
  }

  getMetrics(patientId: string): Observable<PatientJourneyMetrics> {
    return this.http.get<PatientJourneyMetrics>(`${this.apiUrl}/${patientId}/metrics`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  updateMetrics(patientId: string, metrics: UpdatePatientJourneyMetrics): Observable<PatientJourney> {
    return this.http.patch<PatientJourney>(`${this.apiUrl}/${patientId}/metrics`, metrics, { headers: this.getHeaders() })
      .pipe(
        map(journey => this.parseServerDates(journey)),
        catchError(this.handleError)
      );
  }

  recalculateMetrics(patientId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${patientId}/metrics/recalculate`, {}, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  private parseServerDates(journey: any): PatientJourney {
    return {
      ...journey,
      createdAt: new Date(journey.createdAt),
      updatedAt: new Date(journey.updatedAt),
      stages: journey.stages.map((stage: any) => ({
        ...stage,
        enteredAt: new Date(stage.enteredAt),
        exitedAt: stage.exitedAt ? new Date(stage.exitedAt) : undefined,
        touchpoints: stage.touchpoints.map((tp: any) => ({
          ...tp,
          timestamp: new Date(tp.timestamp)
        }))
      }))
    };
  }
}
