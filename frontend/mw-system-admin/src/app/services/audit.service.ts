import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface AuditLog {
  id: string;
  timestamp: string;
  userName: string;
  userEmail: string;
  action: string;
  actionDescription: string;
  entityType: string;
  entityId: string;
  entityDisplayName: string | null;
  result: string;
  ipAddress: string;
  userAgent: string;
  requestPath: string;
  httpMethod: string;
  oldValues: string | null;
  newValues: string | null;
  changedFields: string[] | null;
  failureReason: string | null;
  statusCode: number | null;
  dataCategory: string;
  purpose: string;
  severity: string;
}

export interface AuditFilter {
  startDate?: string;
  endDate?: string;
  userId?: string;
  tenantId?: string;
  entityType?: string;
  entityId?: string;
  action?: string;
  result?: string;
  severity?: string;
  pageNumber?: number;
  pageSize?: number;
}

export interface AuditQueryResponse {
  data: AuditLog[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface AuditReport {
  userId: string;
  userName: string;
  generatedAt: string;
  totalAccesses: number;
  dataModifications: number;
  dataExports: number;
  recentActivity: AuditLog[];
}

@Injectable({
  providedIn: 'root'
})
export class AuditService {
  private apiUrl = `${environment.apiUrl}/audit`;
  
  // Signal to track loading state
  loading = signal(false);
  
  constructor(private http: HttpClient) {}

  /**
   * Query audit logs with advanced filters
   */
  queryAuditLogs(filter: AuditFilter): Observable<AuditQueryResponse> {
    this.loading.set(true);
    return this.http.post<AuditQueryResponse>(`${this.apiUrl}/query`, filter)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Get user activity for a specific user
   */
  getUserActivity(userId: string, startDate?: string, endDate?: string): Observable<AuditLog[]> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    
    return this.http.get<AuditLog[]>(`${this.apiUrl}/user/${userId}`, { params });
  }

  /**
   * Get entity history for a specific entity
   */
  getEntityHistory(entityType: string, entityId: string): Observable<AuditLog[]> {
    return this.http.get<AuditLog[]>(`${this.apiUrl}/entity/${entityType}/${entityId}`);
  }

  /**
   * Get security events
   */
  getSecurityEvents(startDate?: string, endDate?: string): Observable<AuditLog[]> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    
    return this.http.get<AuditLog[]>(`${this.apiUrl}/security-events`, { params });
  }

  /**
   * Generate LGPD report for a user
   */
  getLgpdReport(userId: string): Observable<AuditReport> {
    return this.http.get<AuditReport>(`${this.apiUrl}/lgpd-report/${userId}`);
  }

  /**
   * Get available actions for filtering
   */
  getAvailableActions(): string[] {
    return [
      'CREATE', 'READ', 'UPDATE', 'DELETE',
      'LOGIN', 'LOGOUT', 'LOGIN_FAILED',
      'PASSWORD_CHANGED', 'PASSWORD_RESET_REQUESTED',
      'ACCESS_DENIED', 'PERMISSION_CHANGED', 'ROLE_CHANGED',
      'EXPORT', 'DOWNLOAD', 'PRINT',
      'DATA_ACCESS_REQUEST', 'DATA_DELETION_REQUEST',
      'DATA_PORTABILITY_REQUEST', 'DATA_CORRECTION_REQUEST'
    ];
  }

  /**
   * Get available results for filtering
   */
  getAvailableResults(): string[] {
    return ['SUCCESS', 'FAILED', 'UNAUTHORIZED', 'PARTIAL_SUCCESS'];
  }

  /**
   * Get available severities for filtering
   */
  getAvailableSeverities(): string[] {
    return ['INFO', 'WARNING', 'ERROR', 'CRITICAL'];
  }

  /**
   * Format timestamp to local datetime
   */
  formatTimestamp(timestamp: string): string {
    const date = new Date(timestamp);
    return date.toLocaleString('pt-BR', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    });
  }

  /**
   * Get severity color class
   */
  getSeverityClass(severity: string): string {
    switch (severity.toUpperCase()) {
      case 'CRITICAL':
        return 'severity-critical';
      case 'ERROR':
        return 'severity-error';
      case 'WARNING':
        return 'severity-warning';
      case 'INFO':
      default:
        return 'severity-info';
    }
  }

  /**
   * Get result color class
   */
  getResultClass(result: string): string {
    switch (result.toUpperCase()) {
      case 'SUCCESS':
        return 'result-success';
      case 'FAILED':
        return 'result-failed';
      case 'UNAUTHORIZED':
        return 'result-unauthorized';
      case 'PARTIAL_SUCCESS':
        return 'result-partial';
      default:
        return 'result-unknown';
    }
  }
}
