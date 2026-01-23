import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicalRecord, CreateMedicalRecord, UpdateMedicalRecord, CompleteMedicalRecord, Cfm1821ValidationResult } from '../models/medical-record.model';
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
}
