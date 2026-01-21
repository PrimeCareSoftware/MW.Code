import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface HealthInsurancePlan {
  id: string;
  operatorId: string;
  planName: string;
  planCode: string;
  registerNumber?: string;
  type: string;
  isActive: boolean;
  coversConsultations: boolean;
  coversExams: boolean;
  coversProcedures: boolean;
  requiresPriorAuthorization: boolean;
  tissEnabled: boolean;
  operatorName?: string;
  createdAt: string;
  updatedAt?: string;
}

@Injectable({
  providedIn: 'root'
})
export class HealthInsurancePlanService {
  private apiUrl = `${environment.apiUrl}/health-insurance-plans`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<HealthInsurancePlan[]> {
    return this.http.get<HealthInsurancePlan[]>(this.apiUrl);
  }

  getByOperatorId(operatorId: string): Observable<HealthInsurancePlan[]> {
    return this.http.get<HealthInsurancePlan[]>(`${this.apiUrl}/operator/${operatorId}`);
  }

  getById(id: string): Observable<HealthInsurancePlan> {
    return this.http.get<HealthInsurancePlan>(`${this.apiUrl}/${id}`);
  }
}
