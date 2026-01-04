import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DiagnosticHypothesis, CreateDiagnosticHypothesis, UpdateDiagnosticHypothesis } from '../models/medical-record.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DiagnosticHypothesisService {
  private apiUrl = `${environment.apiUrl}/diagnostic-hypotheses`;

  constructor(private http: HttpClient) {}

  create(hypothesis: CreateDiagnosticHypothesis): Observable<DiagnosticHypothesis> {
    return this.http.post<DiagnosticHypothesis>(this.apiUrl, hypothesis);
  }

  update(id: string, hypothesis: UpdateDiagnosticHypothesis): Observable<DiagnosticHypothesis> {
    return this.http.put<DiagnosticHypothesis>(`${this.apiUrl}/${id}`, hypothesis);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getByMedicalRecord(medicalRecordId: string): Observable<DiagnosticHypothesis[]> {
    return this.http.get<DiagnosticHypothesis[]>(`${this.apiUrl}/medical-record/${medicalRecordId}`);
  }
}
