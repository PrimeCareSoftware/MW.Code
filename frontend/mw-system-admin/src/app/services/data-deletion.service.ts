import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface DataDeletionRequest {
  id: string;
  patientId: string;
  patientName: string;
  patientEmail: string;
  requestType: 'Complete' | 'Anonymization' | 'Partial';
  reason: string;
  status: 'Pending' | 'Processing' | 'Completed' | 'Rejected';
  requestDate: string;
  processedDate: string | null;
  processedBy: string | null;
  completedDate: string | null;
  rejectedDate: string | null;
  rejectedBy: string | null;
  rejectionReason: string | null;
  legalApprovalRequired: boolean;
  legalApprovalDate: string | null;
  legalApprovedBy: string | null;
  processingNotes: string | null;
  affectedDataTypes: string[] | null;
}

export interface ProcessDeletionRequest {
  requestId: string;
  processedBy: string;
  notes?: string;
}

export interface CompleteDeletionRequest {
  requestId: string;
  completedBy: string;
  affectedDataTypes: string[];
  notes?: string;
}

export interface RejectDeletionRequest {
  requestId: string;
  rejectedBy: string;
  reason: string;
}

export interface LegalApprovalRequest {
  requestId: string;
  approvedBy: string;
  notes?: string;
}

export interface DeletionFilter {
  status?: string;
  requestType?: string;
  startDate?: string;
  endDate?: string;
  patientId?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface DeletionResponse {
  data: DataDeletionRequest[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class DataDeletionService {
  private apiUrl = `${environment.apiUrl}/datadeletion`;
  
  loading = signal(false);
  
  constructor(private http: HttpClient) {}

  /**
   * Get pending deletion requests (Admin)
   */
  getPendingRequests(): Observable<DataDeletionRequest[]> {
    this.loading.set(true);
    return this.http.get<DataDeletionRequest[]>(`${this.apiUrl}/pending`)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Get deletion requests for a specific patient
   */
  getPatientRequests(patientId: string): Observable<DataDeletionRequest[]> {
    this.loading.set(true);
    return this.http.get<DataDeletionRequest[]>(`${this.apiUrl}/patient/${patientId}`)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Create a new deletion request
   */
  requestDeletion(request: {
    patientId: string;
    requestType: string;
    reason: string;
  }): Observable<DataDeletionRequest> {
    this.loading.set(true);
    return this.http.post<DataDeletionRequest>(`${this.apiUrl}/request`, request)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Process a deletion request (Admin)
   */
  processRequest(request: ProcessDeletionRequest): Observable<void> {
    this.loading.set(true);
    return this.http.post<void>(`${this.apiUrl}/${request.requestId}/process`, request)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Complete a deletion request (execute anonymization)
   */
  completeRequest(request: CompleteDeletionRequest): Observable<void> {
    this.loading.set(true);
    return this.http.post<void>(`${this.apiUrl}/${request.requestId}/complete`, request)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Reject a deletion request
   */
  rejectRequest(request: RejectDeletionRequest): Observable<void> {
    this.loading.set(true);
    return this.http.post<void>(`${this.apiUrl}/${request.requestId}/reject`, request)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Provide legal approval for a deletion request
   */
  legalApproval(request: LegalApprovalRequest): Observable<void> {
    this.loading.set(true);
    return this.http.post<void>(`${this.apiUrl}/${request.requestId}/legal-approval`, request)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Get available request types
   */
  getAvailableRequestTypes(): string[] {
    return ['Complete', 'Anonymization', 'Partial'];
  }

  /**
   * Get available statuses
   */
  getAvailableStatuses(): string[] {
    return ['Pending', 'Processing', 'Completed', 'Rejected'];
  }

  /**
   * Get status badge color
   */
  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'warning';
      case 'Processing': return 'info';
      case 'Completed': return 'success';
      case 'Rejected': return 'danger';
      default: return 'secondary';
    }
  }

  /**
   * Get available affected data types
   */
  getAvailableDataTypes(): string[] {
    return [
      'PERSONAL_DATA',
      'MEDICAL_RECORDS',
      'APPOINTMENTS',
      'PRESCRIPTIONS',
      'EXAMS',
      'BILLING',
      'CONSENTS',
      'ALL'
    ];
  }
}
