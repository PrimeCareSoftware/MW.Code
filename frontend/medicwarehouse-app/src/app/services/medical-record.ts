import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicalRecord, CreateMedicalRecord, UpdateMedicalRecord, CompleteMedicalRecord, Cfm1821ValidationResult, MedicalRecordVersion, MedicalRecordAccessLog, ReopenMedicalRecordRequest } from '../models/medical-record.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MedicalRecordService {
  private apiUrl = `${environment.apiUrl}/medical-records`;

  constructor(private http: HttpClient) {}

  create(medicalRecord: CreateMedicalRecord): Observable<MedicalRecord> {
    return this.http.post<MedicalRecord>(this.apiUrl, medicalRecord);
  }

  update(id: string, medicalRecord: UpdateMedicalRecord): Observable<MedicalRecord> {
    return this.http.put<MedicalRecord>(`${this.apiUrl}/${id}`, medicalRecord);
  }

  complete(id: string, data: CompleteMedicalRecord): Observable<MedicalRecord> {
    return this.http.post<MedicalRecord>(`${this.apiUrl}/${id}/complete`, data);
  }

  getByAppointment(appointmentId: string): Observable<MedicalRecord> {
    return this.http.get<MedicalRecord>(`${this.apiUrl}/appointment/${appointmentId}`);
  }

  getPatientRecords(patientId: string): Observable<MedicalRecord[]> {
    return this.http.get<MedicalRecord[]>(`${this.apiUrl}/patient/${patientId}`);
  }

  getCfm1821Status(id: string): Observable<Cfm1821ValidationResult> {
    return this.http.get<Cfm1821ValidationResult>(`${this.apiUrl}/${id}/cfm1821-status`);
  }

  // CFM 1.638/2002 - Versioning and Audit methods

  /**
   * Close a medical record and make it immutable
   */
  close(id: string): Observable<MedicalRecord> {
    return this.http.post<MedicalRecord>(`${this.apiUrl}/${id}/close`, {});
  }

  /**
   * Reopen a closed medical record with mandatory justification
   */
  reopen(id: string, reason: string): Observable<MedicalRecord> {
    const request: ReopenMedicalRecordRequest = { reason };
    return this.http.post<MedicalRecord>(`${this.apiUrl}/${id}/reopen`, request);
  }

  /**
   * Get version history for a medical record
   */
  getVersionHistory(id: string): Observable<MedicalRecordVersion[]> {
    return this.http.get<MedicalRecordVersion[]>(`${this.apiUrl}/${id}/versions`);
  }

  /**
   * Get access logs for a medical record
   */
  getAccessLogs(id: string, startDate?: Date, endDate?: Date): Observable<MedicalRecordAccessLog[]> {
    let params = new HttpParams();
    
    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }
    
    return this.http.get<MedicalRecordAccessLog[]>(`${this.apiUrl}/${id}/access-logs`, { params });
  }
}

