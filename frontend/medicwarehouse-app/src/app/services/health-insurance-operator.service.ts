import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HealthInsuranceOperator, CreateHealthInsuranceOperator, UpdateHealthInsuranceOperator } from '../models/tiss.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class HealthInsuranceOperatorService {
  private apiUrl = `${environment.apiUrl}/health-insurance-operators`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<HealthInsuranceOperator[]> {
    return this.http.get<HealthInsuranceOperator[]>(this.apiUrl);
  }

  getById(id: string): Observable<HealthInsuranceOperator> {
    return this.http.get<HealthInsuranceOperator>(`${this.apiUrl}/${id}`);
  }

  create(operator: CreateHealthInsuranceOperator): Observable<HealthInsuranceOperator> {
    return this.http.post<HealthInsuranceOperator>(this.apiUrl, operator);
  }

  update(id: string, operator: UpdateHealthInsuranceOperator): Observable<HealthInsuranceOperator> {
    return this.http.put<HealthInsuranceOperator>(`${this.apiUrl}/${id}`, operator);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  search(searchTerm: string): Observable<HealthInsuranceOperator[]> {
    return this.http.get<HealthInsuranceOperator[]>(`${this.apiUrl}/search?name=${encodeURIComponent(searchTerm)}`);
  }
}
