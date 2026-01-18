import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PatientHealthInsurance, CreatePatientHealthInsurance, UpdatePatientHealthInsurance } from '../models/tiss.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PatientHealthInsuranceService {
  private apiUrl = `${environment.apiUrl}/patient-health-insurance`;

  constructor(private http: HttpClient) { }

  getByPatientId(patientId: string): Observable<PatientHealthInsurance[]> {
    return this.http.get<PatientHealthInsurance[]>(`${this.apiUrl}/patient/${patientId}`);
  }

  getById(id: string): Observable<PatientHealthInsurance> {
    return this.http.get<PatientHealthInsurance>(`${this.apiUrl}/${id}`);
  }

  create(insurance: CreatePatientHealthInsurance): Observable<PatientHealthInsurance> {
    return this.http.post<PatientHealthInsurance>(this.apiUrl, insurance);
  }

  update(id: string, insurance: UpdatePatientHealthInsurance): Observable<PatientHealthInsurance> {
    return this.http.put<PatientHealthInsurance>(`${this.apiUrl}/${id}`, insurance);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
