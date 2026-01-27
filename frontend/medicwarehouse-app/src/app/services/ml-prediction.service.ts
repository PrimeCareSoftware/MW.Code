import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  PrevisaoConsultas, 
  PrevisaoDataEspecifica,
  DadosNoShow,
  RiscoNoShow 
} from '../models/ml-prediction.model';

@Injectable({
  providedIn: 'root'
})
export class MLPredictionService {
  private apiUrl = `${environment.apiUrl}/MLPrediction`;

  constructor(private http: HttpClient) {}

  /**
   * Get demand forecast for the next 7 days
   */
  getPrevisaoProximaSemana(): Observable<PrevisaoConsultas> {
    return this.http.get<PrevisaoConsultas>(`${this.apiUrl}/demanda/proxima-semana`);
  }

  /**
   * Get demand forecast for a specific date
   * @param data Date (yyyy-MM-dd)
   */
  getPrevisaoParaData(data: string): Observable<PrevisaoDataEspecifica> {
    const params = new HttpParams().set('data', data);
    return this.http.get<PrevisaoDataEspecifica>(`${this.apiUrl}/demanda/data`, { params });
  }

  /**
   * Calculate no-show risk for an appointment
   * @param dados No-show prediction data
   */
  calcularRiscoNoShow(dados: DadosNoShow): Observable<RiscoNoShow> {
    return this.http.post<RiscoNoShow>(`${this.apiUrl}/noshow/calcular-risco`, dados);
  }

  /**
   * Load ML models (Admin only)
   */
  carregarModelos(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/admin/carregar-modelos`, null);
  }

  /**
   * Train demand prediction model (Admin only)
   */
  treinarModeloDemanda(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/admin/treinar/demanda`, null);
  }

  /**
   * Train no-show prediction model (Admin only)
   */
  treinarModeloNoShow(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/admin/treinar/noshow`, null);
  }
}
