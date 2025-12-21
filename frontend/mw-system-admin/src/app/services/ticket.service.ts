import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Ticket,
  TicketSummary,
  CreateTicketRequest,
  UpdateTicketRequest,
  UpdateTicketStatusRequest,
  AddTicketCommentRequest,
  UploadAttachmentRequest,
  TicketStatistics,
  AssignTicketRequest,
  TicketStatus,
  TicketType
} from '../models/ticket.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl = `${environment.apiUrl}/tickets`;

  constructor(private http: HttpClient) {}

  /**
   * Get all tickets with filters (system owners only)
   */
  getAllTickets(filters?: {
    status?: TicketStatus;
    type?: TicketType;
    clinicId?: string;
    tenantId?: string;
  }): Observable<TicketSummary[]> {
    let url = this.apiUrl;
    const params: string[] = [];
    
    if (filters) {
      if (filters.status !== undefined) params.push(`status=${filters.status}`);
      if (filters.type !== undefined) params.push(`type=${filters.type}`);
      if (filters.clinicId) params.push(`clinicId=${filters.clinicId}`);
      if (filters.tenantId) params.push(`tenantId=${filters.tenantId}`);
    }
    
    if (params.length > 0) {
      url += '?' + params.join('&');
    }
    
    return this.http.get<TicketSummary[]>(url);
  }

  /**
   * Get ticket by ID
   */
  getTicket(id: string): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get tickets for a specific clinic
   */
  getClinicTickets(clinicId: string): Observable<TicketSummary[]> {
    return this.http.get<TicketSummary[]>(`${this.apiUrl}/clinic/${clinicId}`);
  }

  /**
   * Update ticket status
   */
  updateTicketStatus(id: string, request: UpdateTicketStatusRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}/status`, request);
  }

  /**
   * Assign ticket to system owner
   */
  assignTicket(id: string, request: AssignTicketRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}/assign`, request);
  }

  /**
   * Add comment to ticket
   */
  addComment(id: string, request: AddTicketCommentRequest): Observable<{ message: string; commentId: string }> {
    return this.http.post<{ message: string; commentId: string }>(`${this.apiUrl}/${id}/comments`, request);
  }

  /**
   * Get ticket statistics
   */
  getStatistics(clinicId?: string, tenantId?: string): Observable<TicketStatistics> {
    let url = `${this.apiUrl}/statistics`;
    const params: string[] = [];
    
    if (clinicId) params.push(`clinicId=${clinicId}`);
    if (tenantId) params.push(`tenantId=${tenantId}`);
    
    if (params.length > 0) {
      url += '?' + params.join('&');
    }
    
    return this.http.get<TicketStatistics>(url);
  }

  /**
   * Update ticket details
   */
  updateTicket(id: string, request: UpdateTicketRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, request);
  }
}
