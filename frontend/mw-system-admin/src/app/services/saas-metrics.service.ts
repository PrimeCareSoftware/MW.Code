import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  SaasDashboard,
  MrrBreakdown,
  ChurnAnalysis,
  GrowthMetrics,
  RevenueTimeline,
  CustomerBreakdown
} from '../models/system-admin.model';

@Injectable({
  providedIn: 'root'
})
export class SaasMetricsService {
  private apiUrl = `${environment.apiUrl}/system-admin/saas-metrics`;

  constructor(private http: HttpClient) {}

  /**
   * Get comprehensive SaaS dashboard metrics
   */
  getDashboard(): Observable<SaasDashboard> {
    return this.http.get<SaasDashboard>(`${this.apiUrl}/dashboard`);
  }

  /**
   * Get detailed MRR breakdown
   */
  getMrrBreakdown(): Observable<MrrBreakdown> {
    return this.http.get<MrrBreakdown>(`${this.apiUrl}/mrr-breakdown`);
  }

  /**
   * Get churn analysis
   */
  getChurnAnalysis(): Observable<ChurnAnalysis> {
    return this.http.get<ChurnAnalysis>(`${this.apiUrl}/churn-analysis`);
  }

  /**
   * Get growth metrics
   */
  getGrowthMetrics(): Observable<GrowthMetrics> {
    return this.http.get<GrowthMetrics>(`${this.apiUrl}/growth`);
  }

  /**
   * Get revenue timeline
   */
  getRevenueTimeline(months: number = 12): Observable<RevenueTimeline[]> {
    return this.http.get<RevenueTimeline[]>(`${this.apiUrl}/revenue-timeline`, {
      params: { months: months.toString() }
    });
  }

  /**
   * Get customer breakdown by plan and status
   */
  getCustomerBreakdown(): Observable<CustomerBreakdown> {
    return this.http.get<CustomerBreakdown>(`${this.apiUrl}/customer-breakdown`);
  }
}
