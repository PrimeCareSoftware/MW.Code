import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  Survey,
  SurveyResponse,
  SurveyAnalytics,
  CreateSurvey,
  UpdateSurvey,
  SubmitSurveyResponse
} from '../../models/crm';

@Injectable({
  providedIn: 'root'
})
export class SurveyService {
  private readonly apiUrl = `${environment.apiUrl}/crm/survey`;

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
    
    console.error('Survey Service Error:', error);
    return throwError(() => error);
  }

  getAll(): Observable<Survey[]> {
    return this.http.get<Survey[]>(this.apiUrl, { headers: this.getHeaders() })
      .pipe(
        map(surveys => surveys.map(s => this.parseServerDates(s))),
        catchError(this.handleError)
      );
  }

  getActive(): Observable<Survey[]> {
    return this.http.get<Survey[]>(`${this.apiUrl}/active`, { headers: this.getHeaders() })
      .pipe(
        map(surveys => surveys.map(s => this.parseServerDates(s))),
        catchError(this.handleError)
      );
  }

  getById(id: string): Observable<Survey> {
    return this.http.get<Survey>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() })
      .pipe(
        map(survey => this.parseServerDates(survey)),
        catchError(this.handleError)
      );
  }

  create(survey: CreateSurvey): Observable<Survey> {
    return this.http.post<Survey>(this.apiUrl, survey, { headers: this.getHeaders() })
      .pipe(
        map(s => this.parseServerDates(s)),
        catchError(this.handleError)
      );
  }

  update(id: string, survey: UpdateSurvey): Observable<Survey> {
    return this.http.put<Survey>(`${this.apiUrl}/${id}`, survey, { headers: this.getHeaders() })
      .pipe(
        map(s => this.parseServerDates(s)),
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

  submitResponse(response: SubmitSurveyResponse): Observable<SurveyResponse> {
    return this.http.post<SurveyResponse>(`${this.apiUrl}/response`, response, { headers: this.getHeaders() })
      .pipe(
        map(r => this.parseResponseDates(r)),
        catchError(this.handleError)
      );
  }

  getResponses(surveyId: string): Observable<SurveyResponse[]> {
    return this.http.get<SurveyResponse[]>(`${this.apiUrl}/${surveyId}/responses`, { headers: this.getHeaders() })
      .pipe(
        map(responses => responses.map(r => this.parseResponseDates(r))),
        catchError(this.handleError)
      );
  }

  getAnalytics(surveyId: string): Observable<SurveyAnalytics> {
    return this.http.get<SurveyAnalytics>(`${this.apiUrl}/${surveyId}/analytics`, { headers: this.getHeaders() })
      .pipe(
        map(analytics => ({
          ...analytics,
          recentResponses: analytics.recentResponses.map(r => this.parseResponseDates(r))
        })),
        catchError(this.handleError)
      );
  }

  sendToPatient(surveyId: string, patientId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${surveyId}/send/${patientId}`, {}, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  private parseServerDates(survey: any): Survey {
    return {
      ...survey,
      createdAt: new Date(survey.createdAt),
      updatedAt: new Date(survey.updatedAt)
    };
  }

  private parseResponseDates(response: any): SurveyResponse {
    return {
      ...response,
      startedAt: new Date(response.startedAt),
      completedAt: response.completedAt ? new Date(response.completedAt) : undefined
    };
  }
}
