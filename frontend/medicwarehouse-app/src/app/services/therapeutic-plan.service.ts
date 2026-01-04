import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TherapeuticPlan, CreateTherapeuticPlan, UpdateTherapeuticPlan } from '../models/medical-record.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TherapeuticPlanService {
  private apiUrl = `${environment.apiUrl}/therapeutic-plans`;

  constructor(private http: HttpClient) {}

  create(plan: CreateTherapeuticPlan): Observable<TherapeuticPlan> {
    return this.http.post<TherapeuticPlan>(this.apiUrl, plan);
  }

  update(id: string, plan: UpdateTherapeuticPlan): Observable<TherapeuticPlan> {
    return this.http.put<TherapeuticPlan>(`${this.apiUrl}/${id}`, plan);
  }

  getByMedicalRecord(medicalRecordId: string): Observable<TherapeuticPlan[]> {
    return this.http.get<TherapeuticPlan[]>(`${this.apiUrl}/medical-record/${medicalRecordId}`);
  }
}
