import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthorizationRequest, CreateAuthorizationRequest, UpdateAuthorizationRequest, AuthorizationStatus } from '../models/tiss.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationRequestService {
  private apiUrl = `${environment.apiUrl}/authorization-requests`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<AuthorizationRequest[]> {
    return this.http.get<AuthorizationRequest[]>(this.apiUrl);
  }

  getByStatus(status: AuthorizationStatus): Observable<AuthorizationRequest[]> {
    return this.http.get<AuthorizationRequest[]>(`${this.apiUrl}/status/${status}`);
  }

  getByPatientInsurance(patientHealthInsuranceId: string): Observable<AuthorizationRequest[]> {
    return this.http.get<AuthorizationRequest[]>(`${this.apiUrl}/patient-insurance/${patientHealthInsuranceId}`);
  }

  getById(id: string): Observable<AuthorizationRequest> {
    return this.http.get<AuthorizationRequest>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateAuthorizationRequest): Observable<AuthorizationRequest> {
    return this.http.post<AuthorizationRequest>(this.apiUrl, request);
  }

  update(id: string, request: UpdateAuthorizationRequest): Observable<AuthorizationRequest> {
    return this.http.put<AuthorizationRequest>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
