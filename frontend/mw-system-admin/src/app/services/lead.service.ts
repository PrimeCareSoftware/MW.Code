import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  Lead,
  LeadActivity,
  LeadStatistics,
  UserLeadStatistics,
  LeadStatus,
  AssignLeadRequest,
  UpdateLeadStatusRequest,
  ScheduleFollowUpRequest,
  AddActivityRequest,
  UpdateContactInfoRequest,
  AddNotesRequest,
  SetTagsRequest
} from '../models/lead.model';

@Injectable({
  providedIn: 'root'
})
export class LeadService {
  private apiUrl = `${environment.apiUrl}/api/leads`;

  constructor(private http: HttpClient) {}

  // Lead retrieval methods
  getUnassignedLeads(): Observable<Lead[]> {
    return this.http.get<Lead[]>(`${this.apiUrl}/unassigned`);
  }

  getLeadsAssignedToUser(userId: string): Observable<Lead[]> {
    return this.http.get<Lead[]>(`${this.apiUrl}/assigned/${userId}`);
  }

  getLeadsByStatus(status: LeadStatus): Observable<Lead[]> {
    return this.http.get<Lead[]>(`${this.apiUrl}/by-status/${status}`);
  }

  getLeadsNeedingFollowUp(): Observable<Lead[]> {
    return this.http.get<Lead[]>(`${this.apiUrl}/needing-followup`);
  }

  searchLeads(searchTerm: string): Observable<Lead[]> {
    return this.http.get<Lead[]>(`${this.apiUrl}/search`, {
      params: { searchTerm }
    });
  }

  // Statistics
  getStatistics(): Observable<LeadStatistics> {
    return this.http.get<LeadStatistics>(`${this.apiUrl}/statistics`);
  }

  getStatisticsByUser(): Observable<{ [userId: string]: UserLeadStatistics }> {
    return this.http.get<{ [userId: string]: UserLeadStatistics }>(`${this.apiUrl}/statistics/by-user`);
  }

  // Lead creation
  createLeadFromFunnel(sessionId: string): Observable<Lead> {
    return this.http.post<Lead>(`${this.apiUrl}/create-from-funnel/${sessionId}`, {});
  }

  // Lead management
  assignLead(leadId: string, request: AssignLeadRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${leadId}/assign`, request);
  }

  updateLeadStatus(leadId: string, request: UpdateLeadStatusRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${leadId}/status`, request);
  }

  scheduleFollowUp(leadId: string, request: ScheduleFollowUpRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${leadId}/followup`, request);
  }

  updateContactInfo(leadId: string, request: UpdateContactInfoRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${leadId}/contact-info`, request);
  }

  addNotes(leadId: string, request: AddNotesRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${leadId}/notes`, request);
  }

  setTags(leadId: string, request: SetTagsRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${leadId}/tags`, request);
  }

  // Activities
  addActivity(leadId: string, request: AddActivityRequest): Observable<LeadActivity> {
    return this.http.post<LeadActivity>(`${this.apiUrl}/${leadId}/activities`, request);
  }

  getLeadActivities(leadId: string): Observable<LeadActivity[]> {
    return this.http.get<LeadActivity[]>(`${this.apiUrl}/${leadId}/activities`);
  }
}
