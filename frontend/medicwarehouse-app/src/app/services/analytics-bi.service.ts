import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  DashboardClinico, 
  DashboardFinanceiro, 
  ProjecaoReceita,
  MedicoOption 
} from '../models/analytics-bi.model';

@Injectable({
  providedIn: 'root'
})
export class AnalyticsBIService {
  private apiUrl = `${environment.apiUrl}/Analytics`;

  constructor(private http: HttpClient) {}

  /**
   * Get clinical dashboard data
   * @param inicio Start date (yyyy-MM-dd)
   * @param fim End date (yyyy-MM-dd)
   * @param medicoId Optional doctor filter
   */
  getDashboardClinico(
    inicio: string, 
    fim: string, 
    medicoId?: string
  ): Observable<DashboardClinico> {
    let params = new HttpParams()
      .set('inicio', inicio)
      .set('fim', fim);
    
    if (medicoId) {
      params = params.set('medicoId', medicoId);
    }
    
    return this.http.get<DashboardClinico>(`${this.apiUrl}/dashboard/clinico`, { params });
  }

  /**
   * Get financial dashboard data
   * @param inicio Start date (yyyy-MM-dd)
   * @param fim End date (yyyy-MM-dd)
   */
  getDashboardFinanceiro(
    inicio: string, 
    fim: string
  ): Observable<DashboardFinanceiro> {
    const params = new HttpParams()
      .set('inicio', inicio)
      .set('fim', fim);
    
    return this.http.get<DashboardFinanceiro>(`${this.apiUrl}/dashboard/financeiro`, { params });
  }

  /**
   * Get revenue projection for current month
   */
  getProjecaoReceitaMes(): Observable<ProjecaoReceita> {
    return this.http.get<ProjecaoReceita>(`${this.apiUrl}/projecao/receita-mes`);
  }

  /**
   * Manually trigger data consolidation for a specific date
   * @param data Date to consolidate
   */
  consolidarDia(data: string): Observable<any> {
    const params = new HttpParams().set('data', data);
    return this.http.post<any>(`${this.apiUrl}/consolidar/dia`, null, { params });
  }

  /**
   * Manually trigger data consolidation for a date range
   * @param inicio Start date
   * @param fim End date
   */
  consolidarPeriodo(inicio: string, fim: string): Observable<any> {
    const params = new HttpParams()
      .set('inicio', inicio)
      .set('fim', fim);
    return this.http.post<any>(`${this.apiUrl}/consolidar/periodo`, null, { params });
  }

  /**
   * Get list of doctors for filter dropdown
   * This would ideally come from a doctors service, but we'll mock it here
   * In a real implementation, this should call a doctors endpoint
   */
  getMedicosForFilter(): Observable<MedicoOption[]> {
    // This is a placeholder - in production, this should call the doctors API
    // For now, we'll return empty array and the component should handle this
    return this.http.get<MedicoOption[]>(`${environment.apiUrl}/Medicos/simple-list`);
  }
}
