import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  MarketingAutomation,
  MarketingAutomationMetrics,
  CreateMarketingAutomation,
  UpdateMarketingAutomation
} from '../../models/crm';

@Injectable({
  providedIn: 'root'
})
export class MarketingAutomationService {
  private readonly apiUrl = `${environment.apiUrl}/crm/automation`;

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
    
    // Use userMessage from error interceptor if available
    let errorMessage = anyError.userMessage || 'Ocorreu um erro desconhecido';
    
    if (!anyError.userMessage) {
      if (error.error instanceof ErrorEvent) {
        errorMessage = `Erro: ${error.error.message}`;
      } else {
        errorMessage = error.error?.message || `Erro ${error.status}: ${error.statusText}`;
      }
      // Set userMessage for consistent consumption
      anyError.userMessage = errorMessage;
    }
    
    console.error('Marketing Automation Service Error:', error);
    // Return original error to preserve context from interceptor
    return throwError(() => error);
  }

  getAll(): Observable<MarketingAutomation[]> {
    return this.http.get<MarketingAutomation[]>(this.apiUrl, { headers: this.getHeaders() })
      .pipe(
        map(automations => automations.map(a => this.parseServerDates(a))),
        catchError(this.handleError)
      );
  }

  getActive(): Observable<MarketingAutomation[]> {
    return this.http.get<MarketingAutomation[]>(`${this.apiUrl}/active`, { headers: this.getHeaders() })
      .pipe(
        map(automations => automations.map(a => this.parseServerDates(a))),
        catchError(this.handleError)
      );
  }

  getById(id: string): Observable<MarketingAutomation> {
    return this.http.get<MarketingAutomation>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() })
      .pipe(
        map(automation => this.parseServerDates(automation)),
        catchError(this.handleError)
      );
  }

  create(automation: CreateMarketingAutomation): Observable<MarketingAutomation> {
    return this.http.post<MarketingAutomation>(this.apiUrl, automation, { headers: this.getHeaders() })
      .pipe(
        map(a => this.parseServerDates(a)),
        catchError(this.handleError)
      );
  }

  update(id: string, automation: UpdateMarketingAutomation): Observable<MarketingAutomation> {
    return this.http.put<MarketingAutomation>(`${this.apiUrl}/${id}`, automation, { headers: this.getHeaders() })
      .pipe(
        map(a => this.parseServerDates(a)),
        catchError(this.handleError)
      );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  activate(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/activate`, {}, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  deactivate(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/deactivate`, {}, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  getMetrics(id: string): Observable<MarketingAutomationMetrics> {
    return this.http.get<MarketingAutomationMetrics>(`${this.apiUrl}/${id}/metrics`, { headers: this.getHeaders() })
      .pipe(
        map(metrics => this.parseMetricsDates(metrics)),
        catchError(this.handleError)
      );
  }

  getAllMetrics(): Observable<MarketingAutomationMetrics[]> {
    return this.http.get<MarketingAutomationMetrics[]>(`${this.apiUrl}/metrics`, { headers: this.getHeaders() })
      .pipe(
        map(metrics => metrics.map(m => this.parseMetricsDates(m))),
        catchError(this.handleError)
      );
  }

  triggerForPatient(id: string, patientId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/trigger/${patientId}`, {}, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  private parseServerDates(automation: any): MarketingAutomation {
    return {
      ...automation,
      lastExecutedAt: automation.lastExecutedAt ? new Date(automation.lastExecutedAt) : undefined,
      createdAt: new Date(automation.createdAt),
      updatedAt: new Date(automation.updatedAt)
    };
  }

  private parseMetricsDates(metrics: any): MarketingAutomationMetrics {
    return {
      ...metrics,
      lastExecutedAt: metrics.lastExecutedAt ? new Date(metrics.lastExecutedAt) : undefined,
      firstExecutedAt: metrics.firstExecutedAt ? new Date(metrics.firstExecutedAt) : undefined
    };
  }
}
