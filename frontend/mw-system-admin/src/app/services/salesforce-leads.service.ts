import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  SalesforceLead, 
  LeadStatistics, 
  SyncResult, 
  LeadStatus 
} from '../models/salesforce-lead.model';

@Injectable({
  providedIn: 'root'
})
export class SalesforceLeadsService {
  private apiUrl = `${environment.apiUrl}/salesforceleads`;

  constructor(private http: HttpClient) {}

  /**
   * Get all unsynced leads
   */
  getUnsyncedLeads(): Observable<SalesforceLead[]> {
    return this.http.get<SalesforceLead[]>(`${this.apiUrl}/unsynced`);
  }

  /**
   * Get leads by status
   */
  getLeadsByStatus(status: LeadStatus): Observable<SalesforceLead[]> {
    return this.http.get<SalesforceLead[]>(`${this.apiUrl}/by-status/${status}`);
  }

  /**
   * Get lead statistics
   */
  getStatistics(): Observable<LeadStatistics> {
    return this.http.get<LeadStatistics>(`${this.apiUrl}/statistics`);
  }

  /**
   * Create lead from abandoned funnel session
   */
  createLeadFromFunnel(sessionId: string): Observable<SalesforceLead> {
    return this.http.post<SalesforceLead>(`${this.apiUrl}/create-from-funnel/${sessionId}`, {});
  }

  /**
   * Sync a specific lead to Salesforce
   */
  syncLead(leadId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/sync/${leadId}`, {});
  }

  /**
   * Sync all unsynced leads to Salesforce
   */
  syncAllLeads(): Observable<SyncResult> {
    return this.http.post<SyncResult>(`${this.apiUrl}/sync-all`, {});
  }

  /**
   * Update lead status
   */
  updateLeadStatus(leadId: string, status: LeadStatus): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${leadId}/status`, { status });
  }

  /**
   * Test Salesforce connection
   */
  testConnection(): Observable<{ connected: boolean; error?: string }> {
    return this.http.get<{ connected: boolean; error?: string }>(`${this.apiUrl}/test-connection`);
  }

  /**
   * Get lead status display text
   */
  getStatusText(status: LeadStatus): string {
    switch (status) {
      case LeadStatus.New:
        return 'Novo';
      case LeadStatus.Contacted:
        return 'Contactado';
      case LeadStatus.Qualified:
        return 'Qualificado';
      case LeadStatus.Converted:
        return 'Convertido';
      case LeadStatus.Lost:
        return 'Perdido';
      case LeadStatus.Nurturing:
        return 'Nutrição';
      default:
        return 'Desconhecido';
    }
  }

  /**
   * Get lead status color class
   */
  getStatusColor(status: LeadStatus): string {
    switch (status) {
      case LeadStatus.New:
        return 'bg-blue-100 text-blue-800';
      case LeadStatus.Contacted:
        return 'bg-yellow-100 text-yellow-800';
      case LeadStatus.Qualified:
        return 'bg-purple-100 text-purple-800';
      case LeadStatus.Converted:
        return 'bg-green-100 text-green-800';
      case LeadStatus.Lost:
        return 'bg-red-100 text-red-800';
      case LeadStatus.Nurturing:
        return 'bg-indigo-100 text-indigo-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  /**
   * Get step name in Portuguese
   */
  getStepName(step: number): string {
    switch (step) {
      case 1:
        return 'Informações da Clínica';
      case 2:
        return 'Endereço';
      case 3:
        return 'Informações do Proprietário';
      case 4:
        return 'Credenciais de Login';
      case 5:
        return 'Seleção de Plano';
      case 6:
        return 'Confirmação';
      default:
        return `Passo ${step}`;
    }
  }
}
