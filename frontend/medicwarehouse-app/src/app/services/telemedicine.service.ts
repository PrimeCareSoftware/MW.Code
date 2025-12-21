import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiConfigService } from './api-config.service';
import {
  CreateSessionRequest,
  JoinSessionRequest,
  JoinSessionResponse,
  CompleteSessionRequest,
  TelemedicineSession
} from '../models/telemedicine.model';

/**
 * Service for managing telemedicine video call sessions
 * Integrates with MedicSoft.Telemedicine.Api microservice
 */
@Injectable({
  providedIn: 'root'
})
export class TelemedicineService {
  private get apiUrl(): string {
    return `${this.apiConfig.telemedicineUrl}/sessions`;
  }

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService
  ) { }

  /**
   * Gets headers including tenant ID
   */
  private getHeaders(): HttpHeaders {
    const tenantId = localStorage.getItem('tenantId') || '';
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'X-Tenant-Id': tenantId
    });
  }

  /**
   * Creates a new telemedicine session
   */
  createSession(request: CreateSessionRequest): Observable<TelemedicineSession> {
    return this.http.post<TelemedicineSession>(
      this.apiUrl,
      request,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Joins an existing session and gets access credentials
   */
  joinSession(sessionId: string, request: JoinSessionRequest): Observable<JoinSessionResponse> {
    return this.http.post<JoinSessionResponse>(
      `${this.apiUrl}/${sessionId}/join`,
      request,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Starts a scheduled session
   */
  startSession(sessionId: string): Observable<TelemedicineSession> {
    return this.http.post<TelemedicineSession>(
      `${this.apiUrl}/${sessionId}/start`,
      null,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Completes an in-progress session
   */
  completeSession(sessionId: string, request?: CompleteSessionRequest): Observable<TelemedicineSession> {
    return this.http.post<TelemedicineSession>(
      `${this.apiUrl}/${sessionId}/complete`,
      request || {},
      { headers: this.getHeaders() }
    );
  }

  /**
   * Cancels a session
   */
  cancelSession(sessionId: string, reason: string): Observable<TelemedicineSession> {
    return this.http.post<TelemedicineSession>(
      `${this.apiUrl}/${sessionId}/cancel`,
      JSON.stringify(reason),
      { headers: this.getHeaders() }
    );
  }

  /**
   * Gets a session by its ID
   */
  getSessionById(sessionId: string): Observable<TelemedicineSession> {
    return this.http.get<TelemedicineSession>(
      `${this.apiUrl}/${sessionId}`,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Gets a session by appointment ID
   */
  getSessionByAppointmentId(appointmentId: string): Observable<TelemedicineSession> {
    return this.http.get<TelemedicineSession>(
      `${this.apiUrl}/appointment/${appointmentId}`,
      { headers: this.getHeaders() }
    );
  }

  /**
   * Gets sessions for a specific clinic
   */
  getClinicSessions(clinicId: string, skip: number = 0, take: number = 50): Observable<TelemedicineSession[]> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());
    
    return this.http.get<TelemedicineSession[]>(
      `${this.apiUrl}/clinic/${clinicId}`,
      { headers: this.getHeaders(), params }
    );
  }

  /**
   * Gets sessions for a specific provider (doctor)
   */
  getProviderSessions(providerId: string, skip: number = 0, take: number = 50): Observable<TelemedicineSession[]> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());
    
    return this.http.get<TelemedicineSession[]>(
      `${this.apiUrl}/provider/${providerId}`,
      { headers: this.getHeaders(), params }
    );
  }

  /**
   * Gets sessions for a specific patient
   */
  getPatientSessions(patientId: string, skip: number = 0, take: number = 50): Observable<TelemedicineSession[]> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());
    
    return this.http.get<TelemedicineSession[]>(
      `${this.apiUrl}/patient/${patientId}`,
      { headers: this.getHeaders(), params }
    );
  }
}
