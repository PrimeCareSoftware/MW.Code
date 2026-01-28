import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CohortRetention,
  CohortRevenue,
  CohortBehavior
} from '../models/system-admin.model';

@Injectable({
  providedIn: 'root'
})
export class CohortAnalysisService {
  private apiUrl = `${environment.apiUrl}/system-admin/cohorts`;

  constructor(private http: HttpClient) {}

  getRetentionAnalysis(months: number = 12): Observable<CohortRetention> {
    return this.http.get<CohortRetention>(`${this.apiUrl}/retention?months=${months}`);
  }

  getRevenueAnalysis(months: number = 12): Observable<CohortRevenue> {
    return this.http.get<CohortRevenue>(`${this.apiUrl}/revenue?months=${months}`);
  }

  getBehaviorAnalysis(months: number = 12): Observable<CohortBehavior> {
    return this.http.get<CohortBehavior>(`${this.apiUrl}/behavior?months=${months}`);
  }
}
