import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ClinicalExamination, CreateClinicalExamination, UpdateClinicalExamination } from '../models/medical-record.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ClinicalExaminationService {
  private apiUrl = `${environment.apiUrl}/clinical-examinations`;

  constructor(private http: HttpClient) {}

  create(examination: CreateClinicalExamination): Observable<ClinicalExamination> {
    return this.http.post<ClinicalExamination>(this.apiUrl, examination);
  }

  update(id: string, examination: UpdateClinicalExamination): Observable<ClinicalExamination> {
    return this.http.put<ClinicalExamination>(`${this.apiUrl}/${id}`, examination);
  }

  getByMedicalRecord(medicalRecordId: string): Observable<ClinicalExamination[]> {
    return this.http.get<ClinicalExamination[]>(`${this.apiUrl}/medical-record/${medicalRecordId}`);
  }
}
