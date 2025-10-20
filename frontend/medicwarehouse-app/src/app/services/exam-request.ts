import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  ExamRequest, 
  CreateExamRequest, 
  UpdateExamRequest, 
  CompleteExamRequest 
} from '../models/exam-request.model';

@Injectable({
  providedIn: 'root'
})
export class ExamRequestService {
  private apiUrl = 'http://localhost:5000/api/exam-requests';

  constructor(private http: HttpClient) {}

  create(examRequest: CreateExamRequest): Observable<ExamRequest> {
    return this.http.post<ExamRequest>(this.apiUrl, examRequest);
  }

  update(id: string, examRequest: UpdateExamRequest): Observable<ExamRequest> {
    return this.http.put<ExamRequest>(`${this.apiUrl}/${id}`, examRequest);
  }

  complete(id: string, data: CompleteExamRequest): Observable<ExamRequest> {
    return this.http.post<ExamRequest>(`${this.apiUrl}/${id}/complete`, data);
  }

  cancel(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/cancel`, {});
  }

  getById(id: string): Observable<ExamRequest> {
    return this.http.get<ExamRequest>(`${this.apiUrl}/${id}`);
  }

  getByAppointment(appointmentId: string): Observable<ExamRequest[]> {
    return this.http.get<ExamRequest[]>(`${this.apiUrl}/appointment/${appointmentId}`);
  }

  getByPatient(patientId: string): Observable<ExamRequest[]> {
    return this.http.get<ExamRequest[]>(`${this.apiUrl}/patient/${patientId}`);
  }

  getPending(): Observable<ExamRequest[]> {
    return this.http.get<ExamRequest[]>(`${this.apiUrl}/pending`);
  }

  getUrgent(): Observable<ExamRequest[]> {
    return this.http.get<ExamRequest[]>(`${this.apiUrl}/urgent`);
  }
}
