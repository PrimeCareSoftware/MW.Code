import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import {
  Ticket,
  TicketSummary,
  CreateTicketRequest,
  UpdateTicketRequest,
  UpdateTicketStatusRequest,
  AddTicketCommentRequest,
  UploadAttachmentRequest,
  TicketStatistics,
  AssignTicketRequest
} from '../models/ticket.model';
import { ApiConfigService } from './api-config.service';
import { Auth } from './auth';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private apiUrl: string;
  
  // Signal to track unread ticket updates
  public unreadTicketCount = signal<number>(0);

  constructor(
    private http: HttpClient,
    private apiConfig: ApiConfigService,
    private auth: Auth
  ) {
    this.apiUrl = `${this.apiConfig.systemAdminUrl}/tickets`;
    // Defer loading to avoid circular dependency with error interceptor
    // The error interceptor injects NotificationService which makes HTTP calls
    // Only load unread count if user is authenticated to prevent 401 errors on public pages
    setTimeout(() => {
      if (this.auth.isAuthenticated()) {
        this.loadUnreadCount();
      }
    }, 0);
  }

  /**
   * Create a new ticket
   */
  createTicket(request: CreateTicketRequest): Observable<{ message: string; ticketId: string }> {
    return this.http.post<{ message: string; ticketId: string }>(this.apiUrl, request)
      .pipe(
        tap(() => this.loadUnreadCount())
      );
  }

  /**
   * Get ticket by ID
   */
  getTicket(id: string): Observable<Ticket> {
    return this.http.get<Ticket>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get current user's tickets
   */
  getMyTickets(): Observable<TicketSummary[]> {
    return this.http.get<TicketSummary[]>(`${this.apiUrl}/my-tickets`);
  }

  /**
   * Get tickets for a specific clinic
   */
  getClinicTickets(clinicId: string): Observable<TicketSummary[]> {
    return this.http.get<TicketSummary[]>(`${this.apiUrl}/clinic/${clinicId}`);
  }

  /**
   * Get all tickets (system owners only)
   */
  getAllTickets(filters?: {
    status?: number;
    type?: number;
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
   * Update ticket details
   */
  updateTicket(id: string, request: UpdateTicketRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, request);
  }

  /**
   * Update ticket status
   */
  updateTicketStatus(id: string, request: UpdateTicketStatusRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}/status`, request)
      .pipe(
        tap(() => this.loadUnreadCount())
      );
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
    return this.http.post<{ message: string; commentId: string }>(`${this.apiUrl}/${id}/comments`, request)
      .pipe(
        tap(() => this.loadUnreadCount())
      );
  }

  /**
   * Upload attachment to ticket
   */
  uploadAttachment(id: string, request: UploadAttachmentRequest): Observable<{ message: string; attachmentId: string }> {
    return this.http.post<{ message: string; attachmentId: string }>(`${this.apiUrl}/${id}/attachments`, request);
  }

  /**
   * Get unread updates count
   */
  getUnreadCount(): Observable<{ count: number }> {
    return this.http.get<{ count: number }>(`${this.apiUrl}/unread-count`);
  }

  /**
   * Mark ticket as read
   */
  markAsRead(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/mark-read`, {})
      .pipe(
        tap(() => this.loadUnreadCount())
      );
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
   * Load unread count and update signal
   */
  private loadUnreadCount(): void {
    this.getUnreadCount().subscribe({
      next: (response) => this.unreadTicketCount.set(response.count),
      error: () => this.unreadTicketCount.set(0)
    });
  }

  /**
   * Convert image file to base64
   */
  async fileToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        const result = reader.result as string;
        // Remove the data:image/...;base64, prefix
        const base64 = result.split(',')[1];
        resolve(base64);
      };
      reader.onerror = error => reject(error);
    });
  }

  /**
   * Get image from clipboard paste event
   */
  getImageFromClipboard(event: ClipboardEvent): File | null {
    const items = event.clipboardData?.items;
    if (!items) return null;

    for (let i = 0; i < items.length; i++) {
      if (items[i].type.indexOf('image') !== -1) {
        return items[i].getAsFile();
      }
    }
    return null;
  }
}
