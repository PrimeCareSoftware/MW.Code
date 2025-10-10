import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Patient, CreatePatient, UpdatePatient } from '../models/patient.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private apiUrl = `${environment.apiUrl}/patients`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<Patient[]> {
    return this.http.get<Patient[]>(this.apiUrl);
  }

  getById(id: string): Observable<Patient> {
    return this.http.get<Patient>(`${this.apiUrl}/${id}`);
  }

  create(patient: CreatePatient): Observable<Patient> {
    return this.http.post<Patient>(this.apiUrl, patient);
  }

  update(id: string, patient: UpdatePatient): Observable<Patient> {
    return this.http.put<Patient>(`${this.apiUrl}/${id}`, patient);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  linkChildToGuardian(childId: string, guardianId: string): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.apiUrl}/${childId}/link-guardian/${guardianId}`, {});
  }

  getChildren(guardianId: string): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.apiUrl}/${guardianId}/children`);
  }

  search(searchTerm: string): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.apiUrl}/search?searchTerm=${encodeURIComponent(searchTerm)}`);
  }
}
