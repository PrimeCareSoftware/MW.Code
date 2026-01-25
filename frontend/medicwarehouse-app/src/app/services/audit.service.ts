import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

// Enums
export enum AuditAction {
  CREATE = 'CREATE',
  READ = 'READ',
  UPDATE = 'UPDATE',
  DELETE = 'DELETE',
  LOGIN = 'LOGIN',
  LOGOUT = 'LOGOUT',
  LOGIN_FAILED = 'LOGIN_FAILED',
  PASSWORD_CHANGED = 'PASSWORD_CHANGED',
  PASSWORD_RESET_REQUESTED = 'PASSWORD_RESET_REQUESTED',
  MFA_ENABLED = 'MFA_ENABLED',
  MFA_DISABLED = 'MFA_DISABLED',
  ACCESS_DENIED = 'ACCESS_DENIED',
  PERMISSION_CHANGED = 'PERMISSION_CHANGED',
  ROLE_CHANGED = 'ROLE_CHANGED',
  EXPORT = 'EXPORT',
  DOWNLOAD = 'DOWNLOAD',
  PRINT = 'PRINT',
  DATA_ACCESS_REQUEST = 'DATA_ACCESS_REQUEST',
  DATA_DELETION_REQUEST = 'DATA_DELETION_REQUEST',
  DATA_PORTABILITY_REQUEST = 'DATA_PORTABILITY_REQUEST',
  DATA_CORRECTION_REQUEST = 'DATA_CORRECTION_REQUEST'
}

export enum OperationResult {
  SUCCESS = 'SUCCESS',
  FAILED = 'FAILED',
  UNAUTHORIZED = 'UNAUTHORIZED',
  PARTIAL_SUCCESS = 'PARTIAL_SUCCESS'
}

export enum DataCategory {
  PUBLIC = 'PUBLIC',
  PERSONAL = 'PERSONAL',
  SENSITIVE = 'SENSITIVE',
  CONFIDENTIAL = 'CONFIDENTIAL'
}

export enum LgpdPurpose {
  HEALTHCARE = 'HEALTHCARE',
  BILLING = 'BILLING',
  LEGAL_OBLIGATION = 'LEGAL_OBLIGATION',
  LEGITIMATE_INTEREST = 'LEGITIMATE_INTEREST',
  CONSENT = 'CONSENT'
}

export enum AuditSeverity {
  INFO = 'INFO',
  WARNING = 'WARNING',
  ERROR = 'ERROR',
  CRITICAL = 'CRITICAL'
}

// Interfaces
export interface AuditLog {
  id: string;
  timestamp: string;
  userName: string;
  userEmail: string;
  action: string;
  actionDescription: string;
  entityType: string;
  entityId: string;
  entityDisplayName?: string;
  result: string;
  ipAddress: string;
  userAgent: string;
  requestPath: string;
  httpMethod: string;
  oldValues?: string;
  newValues?: string;
  changedFields?: string[];
  failureReason?: string;
  statusCode?: number;
  dataCategory: string;
  purpose: string;
  severity: string;
}

export interface AuditFilter {
  startDate?: string;
  endDate?: string;
  userId?: string;
  entityType?: string;
  entityId?: string;
  action?: AuditAction;
  result?: OperationResult;
  severity?: AuditSeverity;
  pageNumber?: number;
  pageSize?: number;
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

export interface PagedAuditLogs {
  data: AuditLog[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface LogDataAccessRequest {
  entityType: string;
  entityId: string;
  entityDisplayName: string;
  dataCategory: DataCategory;
  purpose: LgpdPurpose;
}

@Injectable({
  providedIn: 'root'
})
export class AuditService {
  private apiUrl = `${environment.apiUrl}/Audit`;

  constructor(private http: HttpClient) {}

  /**
   * Query audit logs with filters
   */
  queryLogs(filter: AuditFilter): Observable<PagedAuditLogs> {
    return this.http.post<PagedAuditLogs>(`${this.apiUrl}/query`, filter);
  }

  /**
   * Get user activity logs
   */
  getUserActivity(userId: string, startDate?: string, endDate?: string): Observable<AuditLog[]> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);

    return this.http.get<AuditLog[]>(`${this.apiUrl}/user/${userId}`, { params });
  }

  /**
   * Get entity history
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
   * Log data access (for sensitive data)
   */
  logDataAccess(request: LogDataAccessRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/log-data-access`, request);
  }

  /**
   * Helper method to get action color for UI
   */
  getActionColor(action: string): string {
    switch (action) {
      case 'CREATE':
        return 'primary';
      case 'READ':
        return 'accent';
      case 'UPDATE':
        return 'warn';
      case 'DELETE':
        return 'error';
      case 'LOGIN':
      case 'LOGOUT':
        return 'info';
      case 'LOGIN_FAILED':
      case 'ACCESS_DENIED':
        return 'error';
      case 'EXPORT':
      case 'DOWNLOAD':
      case 'PRINT':
        return 'warn';
      default:
        return 'default';
    }
  }

  /**
   * Helper method to get action text for UI
   */
  getActionText(action: string): string {
    const actionTexts: { [key: string]: string } = {
      'CREATE': 'Criação',
      'READ': 'Leitura',
      'UPDATE': 'Atualização',
      'DELETE': 'Exclusão',
      'LOGIN': 'Login',
      'LOGOUT': 'Logout',
      'LOGIN_FAILED': 'Login Falhou',
      'PASSWORD_CHANGED': 'Senha Alterada',
      'PASSWORD_RESET_REQUESTED': 'Redefinição de Senha',
      'MFA_ENABLED': 'MFA Habilitado',
      'MFA_DISABLED': 'MFA Desabilitado',
      'ACCESS_DENIED': 'Acesso Negado',
      'PERMISSION_CHANGED': 'Permissões Alteradas',
      'ROLE_CHANGED': 'Papel Alterado',
      'EXPORT': 'Exportação',
      'DOWNLOAD': 'Download',
      'PRINT': 'Impressão',
      'DATA_ACCESS_REQUEST': 'Solicitação de Acesso',
      'DATA_DELETION_REQUEST': 'Solicitação de Exclusão',
      'DATA_PORTABILITY_REQUEST': 'Solicitação de Portabilidade',
      'DATA_CORRECTION_REQUEST': 'Solicitação de Correção'
    };
    return actionTexts[action] || action;
  }

  /**
   * Helper method to get result icon for UI
   */
  getResultIcon(result: string): string {
    switch (result) {
      case 'SUCCESS':
        return 'check_circle';
      case 'FAILED':
        return 'error';
      case 'UNAUTHORIZED':
        return 'block';
      case 'PARTIAL_SUCCESS':
        return 'warning';
      default:
        return 'help';
    }
  }

  /**
   * Helper method to get result color for UI
   */
  getResultColor(result: string): string {
    switch (result) {
      case 'SUCCESS':
        return 'primary';
      case 'FAILED':
        return 'warn';
      case 'UNAUTHORIZED':
        return 'error';
      case 'PARTIAL_SUCCESS':
        return 'accent';
      default:
        return 'default';
    }
  }

  /**
   * Helper method to get severity color for UI
   */
  getSeverityColor(severity: string): string {
    switch (severity) {
      case 'INFO':
        return 'primary';
      case 'WARNING':
        return 'accent';
      case 'ERROR':
        return 'warn';
      case 'CRITICAL':
        return 'error';
      default:
        return 'default';
    }
  }
}
