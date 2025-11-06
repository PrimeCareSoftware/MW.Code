import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  WaitingQueueEntry,
  CreateWaitingQueueEntry,
  UpdateQueueTriage,
  WaitingQueueSummary,
  PublicQueueDisplay,
  WaitingQueueConfiguration,
  UpdateWaitingQueueConfiguration
} from '../models/waiting-queue.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class WaitingQueueService {
  private apiUrl = `${environment.apiUrl}/waiting-queue`;

  constructor(private http: HttpClient) { }

  getQueueSummary(clinicId: string): Observable<WaitingQueueSummary> {
    const params = new HttpParams().set('clinicId', clinicId);
    return this.http.get<WaitingQueueSummary>(`${this.apiUrl}/summary`, { params });
  }

  getPublicQueueDisplay(clinicId: string, tenantId: string): Observable<PublicQueueDisplay[]> {
    const params = new HttpParams().set('tenantId', tenantId);
    return this.http.get<PublicQueueDisplay[]>(`${this.apiUrl}/public/${clinicId}`, { params });
  }

  addToQueue(entry: CreateWaitingQueueEntry): Observable<WaitingQueueEntry> {
    return this.http.post<WaitingQueueEntry>(this.apiUrl, entry);
  }

  updateTriage(entryId: string, triage: UpdateQueueTriage): Observable<WaitingQueueEntry> {
    return this.http.put<WaitingQueueEntry>(`${this.apiUrl}/${entryId}/triage`, triage);
  }

  callPatient(entryId: string): Observable<WaitingQueueEntry> {
    return this.http.put<WaitingQueueEntry>(`${this.apiUrl}/${entryId}/call`, {});
  }

  startService(entryId: string): Observable<WaitingQueueEntry> {
    return this.http.put<WaitingQueueEntry>(`${this.apiUrl}/${entryId}/start`, {});
  }

  completeService(entryId: string): Observable<WaitingQueueEntry> {
    return this.http.put<WaitingQueueEntry>(`${this.apiUrl}/${entryId}/complete`, {});
  }

  cancelEntry(entryId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${entryId}`);
  }

  getConfiguration(clinicId: string): Observable<WaitingQueueConfiguration> {
    const params = new HttpParams().set('clinicId', clinicId);
    return this.http.get<WaitingQueueConfiguration>(`${this.apiUrl}/configuration`, { params });
  }

  updateConfiguration(clinicId: string, config: UpdateWaitingQueueConfiguration): Observable<WaitingQueueConfiguration> {
    const params = new HttpParams().set('clinicId', clinicId);
    return this.http.put<WaitingQueueConfiguration>(`${this.apiUrl}/configuration`, config, { params });
  }
}
