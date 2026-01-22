import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  GlosasSummary,
  GlosasByOperator,
  GlosasTrend,
  ProcedureGlosas,
  AuthorizationRate,
  ApprovalTime,
  MonthlyPerformance,
  GlosaAlert
} from '../models/tiss.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TissAnalyticsService {
  private apiUrl = `${environment.apiUrl}/tiss-analytics`;

  constructor(private http: HttpClient) { }

  getGlosasSummary(clinicId: string, startDate: string, endDate: string): Observable<GlosasSummary> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<GlosasSummary>(`${this.apiUrl}/glosas-summary`, { params });
  }

  getGlosasByOperator(clinicId: string, startDate: string, endDate: string): Observable<GlosasByOperator[]> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<GlosasByOperator[]>(`${this.apiUrl}/glosas-by-operator`, { params });
  }

  getGlosasTrend(clinicId: string, months: number = 6): Observable<GlosasTrend[]> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('months', months.toString());
    return this.http.get<GlosasTrend[]>(`${this.apiUrl}/glosas-trend`, { params });
  }

  getProcedureGlosas(clinicId: string, startDate: string, endDate: string): Observable<ProcedureGlosas[]> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<ProcedureGlosas[]>(`${this.apiUrl}/procedure-glosas`, { params });
  }

  getAuthorizationRate(clinicId: string, startDate: string, endDate: string): Observable<AuthorizationRate[]> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<AuthorizationRate[]>(`${this.apiUrl}/authorization-rate`, { params });
  }

  getApprovalTime(clinicId: string, startDate: string, endDate: string): Observable<ApprovalTime[]> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<ApprovalTime[]>(`${this.apiUrl}/approval-time`, { params });
  }

  getMonthlyPerformance(clinicId: string, months: number = 12): Observable<MonthlyPerformance[]> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('months', months.toString());
    return this.http.get<MonthlyPerformance[]>(`${this.apiUrl}/monthly-performance`, { params });
  }

  getGlosaAlerts(clinicId: string, startDate: string, endDate: string): Observable<GlosaAlert[]> {
    const params = new HttpParams()
      .set('clinicId', clinicId)
      .set('startDate', startDate)
      .set('endDate', endDate);
    return this.http.get<GlosaAlert[]>(`${this.apiUrl}/glosa-alerts`, { params });
  }
}
