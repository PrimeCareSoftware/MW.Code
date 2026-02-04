import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import {
  Complaint,
  ComplaintInteraction,
  ComplaintDashboard,
  CreateComplaint,
  UpdateComplaint,
  AddComplaintInteraction,
  UpdateComplaintStatus,
  AssignComplaint,
  ComplaintCategory,
  ComplaintStatus,
  ComplaintPriority
} from '../../models/crm';

@Injectable({
  providedIn: 'root'
})
export class ComplaintService {
  private readonly apiUrl = `${environment.apiUrl}/crm/complaint`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'Content-Type': 'application/json'
    });
  }

  private handleError(error: HttpErrorResponse & { userMessage?: string }): Observable<never> {
    // Preserve the original HttpErrorResponse so that any normalized fields
    // (e.g., userMessage, status) added by the global error interceptor are not lost.
    
    // Use userMessage from error interceptor if available
    let errorMessage = error.userMessage || 'Ocorreu um erro desconhecido';
    
    if (!error.userMessage) {
      if (error.error instanceof ErrorEvent) {
        errorMessage = `Erro: ${error.error.message}`;
      } else {
        errorMessage = error.error?.message || `Erro ${error.status}: ${error.statusText}`;
      }
      // Set userMessage for consistent consumption
      error.userMessage = errorMessage;
    }
    
    console.error('Complaint Service Error:', error);
    return throwError(() => error);
  }

  create(complaint: CreateComplaint): Observable<Complaint> {
    return this.http.post<Complaint>(this.apiUrl, complaint, { headers: this.getHeaders() })
      .pipe(
        map(c => this.parseServerDates(c)),
        catchError(this.handleError)
      );
  }

  getAll(): Observable<Complaint[]> {
    return this.http.get<Complaint[]>(this.apiUrl, { headers: this.getHeaders() })
      .pipe(
        map(complaints => complaints.map(c => this.parseServerDates(c))),
        catchError(this.handleError)
      );
  }

  getById(id: string): Observable<Complaint> {
    return this.http.get<Complaint>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() })
      .pipe(
        map(c => this.parseServerDates(c)),
        catchError(this.handleError)
      );
  }

  getByProtocolNumber(protocolNumber: string): Observable<Complaint> {
    return this.http.get<Complaint>(`${this.apiUrl}/protocol/${protocolNumber}`, { headers: this.getHeaders() })
      .pipe(
        map(c => this.parseServerDates(c)),
        catchError(this.handleError)
      );
  }

  update(id: string, complaint: UpdateComplaint): Observable<Complaint> {
    return this.http.put<Complaint>(`${this.apiUrl}/${id}`, complaint, { headers: this.getHeaders() })
      .pipe(
        map(c => this.parseServerDates(c)),
        catchError(this.handleError)
      );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() })
      .pipe(catchError(this.handleError));
  }

  addInteraction(id: string, interaction: AddComplaintInteraction): Observable<ComplaintInteraction> {
    return this.http.post<ComplaintInteraction>(`${this.apiUrl}/${id}/interact`, interaction, { headers: this.getHeaders() })
      .pipe(
        map(i => ({
          ...i,
          interactionDate: new Date(i.interactionDate)
        })),
        catchError(this.handleError)
      );
  }

  updateStatus(id: string, status: UpdateComplaintStatus): Observable<Complaint> {
    return this.http.put<Complaint>(`${this.apiUrl}/${id}/status`, status, { headers: this.getHeaders() })
      .pipe(
        map(c => this.parseServerDates(c)),
        catchError(this.handleError)
      );
  }

  assign(id: string, assignment: AssignComplaint): Observable<Complaint> {
    return this.http.put<Complaint>(`${this.apiUrl}/${id}/assign`, assignment, { headers: this.getHeaders() })
      .pipe(
        map(c => this.parseServerDates(c)),
        catchError(this.handleError)
      );
  }

  getDashboard(): Observable<ComplaintDashboard> {
    return this.http.get<ComplaintDashboard>(`${this.apiUrl}/dashboard`, { headers: this.getHeaders() })
      .pipe(
        map(dashboard => ({
          ...dashboard,
          recentComplaints: dashboard.recentComplaints.map(c => this.parseServerDates(c)),
          urgentComplaints: dashboard.urgentComplaints.map(c => this.parseServerDates(c))
        })),
        catchError(this.handleError)
      );
  }

  getByCategory(category: ComplaintCategory): Observable<Complaint[]> {
    return this.http.get<Complaint[]>(`${this.apiUrl}/category/${category}`, { headers: this.getHeaders() })
      .pipe(
        map(complaints => complaints.map(c => this.parseServerDates(c))),
        catchError(this.handleError)
      );
  }

  getByStatus(status: ComplaintStatus): Observable<Complaint[]> {
    return this.http.get<Complaint[]>(`${this.apiUrl}/status/${status}`, { headers: this.getHeaders() })
      .pipe(
        map(complaints => complaints.map(c => this.parseServerDates(c))),
        catchError(this.handleError)
      );
  }

  getByPriority(priority: ComplaintPriority): Observable<Complaint[]> {
    return this.http.get<Complaint[]>(`${this.apiUrl}/priority/${priority}`, { headers: this.getHeaders() })
      .pipe(
        map(complaints => complaints.map(c => this.parseServerDates(c))),
        catchError(this.handleError)
      );
  }

  private parseServerDates(complaint: any): Complaint {
    return {
      ...complaint,
      receivedAt: new Date(complaint.receivedAt),
      firstResponseAt: complaint.firstResponseAt ? new Date(complaint.firstResponseAt) : undefined,
      resolvedAt: complaint.resolvedAt ? new Date(complaint.resolvedAt) : undefined,
      closedAt: complaint.closedAt ? new Date(complaint.closedAt) : undefined,
      createdAt: new Date(complaint.createdAt),
      updatedAt: new Date(complaint.updatedAt),
      interactions: complaint.interactions.map((i: any) => ({
        ...i,
        interactionDate: new Date(i.interactionDate)
      }))
    };
  }
}
