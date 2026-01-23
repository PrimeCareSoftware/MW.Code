import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  AnamnesisTemplate, 
  AnamnesisResponse, 
  MedicalSpecialty,
  CreateAnamnesisTemplateRequest,
  UpdateAnamnesisTemplateRequest,
  CreateAnamnesisResponseRequest,
  SaveAnswersRequest
} from '../models/anamnesis.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AnamnesisService {
  private apiUrl = `${environment.apiUrl}/anamnesis`;

  constructor(private http: HttpClient) { }

  // Template endpoints
  getTemplatesBySpecialty(specialty: MedicalSpecialty): Observable<AnamnesisTemplate[]> {
    const params = new HttpParams().set('specialty', specialty.toString());
    return this.http.get<AnamnesisTemplate[]>(`${this.apiUrl}/templates`, { params });
  }

  getTemplateById(templateId: string): Observable<AnamnesisTemplate> {
    return this.http.get<AnamnesisTemplate>(`${this.apiUrl}/templates/${templateId}`);
  }

  createTemplate(request: CreateAnamnesisTemplateRequest): Observable<AnamnesisTemplate> {
    return this.http.post<AnamnesisTemplate>(`${this.apiUrl}/templates`, request);
  }

  updateTemplate(templateId: string, request: UpdateAnamnesisTemplateRequest): Observable<AnamnesisTemplate> {
    return this.http.put<AnamnesisTemplate>(`${this.apiUrl}/templates/${templateId}`, request);
  }

  // Response endpoints
  createResponse(request: CreateAnamnesisResponseRequest): Observable<AnamnesisResponse> {
    return this.http.post<AnamnesisResponse>(`${this.apiUrl}/responses`, request);
  }

  saveAnswers(responseId: string, request: SaveAnswersRequest): Observable<AnamnesisResponse> {
    return this.http.put<AnamnesisResponse>(`${this.apiUrl}/responses/${responseId}/answers`, request);
  }

  getResponseById(responseId: string): Observable<AnamnesisResponse> {
    return this.http.get<AnamnesisResponse>(`${this.apiUrl}/responses/${responseId}`);
  }

  getResponseByAppointment(appointmentId: string): Observable<AnamnesisResponse> {
    return this.http.get<AnamnesisResponse>(`${this.apiUrl}/responses/by-appointment/${appointmentId}`);
  }

  getPatientHistory(patientId: string): Observable<AnamnesisResponse[]> {
    return this.http.get<AnamnesisResponse[]>(`${this.apiUrl}/responses/patient/${patientId}`);
  }
}
