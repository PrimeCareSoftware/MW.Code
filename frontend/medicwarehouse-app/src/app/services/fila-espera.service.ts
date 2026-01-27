import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  FilaEspera,
  SenhaFila,
  GerarSenhaRequest,
  ChamarSenhaRequest,
  FilaSummary,
  ConsultarSenhaResponse,
  FilaMetrics
} from '../models/fila-espera.model';

@Injectable({
  providedIn: 'root'
})
export class FilaEsperaService {
  private apiUrl = `${environment.apiUrl}/FilaEspera`;
  private analyticsUrl = `${environment.apiUrl}/FilaAnalytics`;

  constructor(private http: HttpClient) {}

  // Fila Management
  getFila(filaId: string): Observable<FilaEspera> {
    return this.http.get<FilaEspera>(`${this.apiUrl}/${filaId}`);
  }

  getFilaSummary(filaId: string): Observable<FilaSummary> {
    return this.http.get<FilaSummary>(`${this.apiUrl}/${filaId}/summary`);
  }

  // Senha Generation
  gerarSenha(filaId: string, request: GerarSenhaRequest, tenantId?: string): Observable<SenhaFila> {
    const headers = tenantId ? { 'X-Tenant-Id': tenantId } : {};
    return this.http.post<SenhaFila>(
      `${this.apiUrl}/${filaId}/senha`,
      request,
      { headers }
    );
  }

  // Senha Consultation
  consultarSenha(filaId: string, numeroSenha: string, tenantId?: string): Observable<ConsultarSenhaResponse> {
    const headers = tenantId ? { 'X-Tenant-Id': tenantId } : {};
    return this.http.get<ConsultarSenhaResponse>(
      `${this.apiUrl}/${filaId}/senha/${numeroSenha}`,
      { headers }
    );
  }

  getSenhaById(senhaId: string): Observable<SenhaFila> {
    return this.http.get<SenhaFila>(`${this.apiUrl}/senha/${senhaId}`);
  }

  // Senha Management (for medical staff)
  chamarProximaSenha(filaId: string, request: ChamarSenhaRequest): Observable<SenhaFila> {
    return this.http.post<SenhaFila>(`${this.apiUrl}/${filaId}/chamar`, request);
  }

  iniciarAtendimento(senhaId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/senha/${senhaId}/iniciar`, {});
  }

  finalizarAtendimento(senhaId: string, observacoes?: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/senha/${senhaId}/finalizar`, { observacoes });
  }

  cancelarSenha(senhaId: string, motivo: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/senha/${senhaId}/cancelar`, { motivo });
  }

  marcarNaoCompareceu(senhaId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/senha/${senhaId}/nao-compareceu`, {});
  }

  // Queue Status
  getSenhasAguardando(filaId: string): Observable<SenhaFila[]> {
    return this.http.get<SenhaFila[]>(`${this.apiUrl}/${filaId}/senhas/aguardando`);
  }

  getSenhasEmAtendimento(filaId: string): Observable<SenhaFila[]> {
    return this.http.get<SenhaFila[]>(`${this.apiUrl}/${filaId}/senhas/atendimento`);
  }

  getUltimasChamadas(filaId: string, quantidade: number = 5): Observable<SenhaFila[]> {
    return this.http.get<SenhaFila[]>(
      `${this.apiUrl}/${filaId}/senhas/ultimas-chamadas`,
      { params: new HttpParams().set('quantidade', quantidade.toString()) }
    );
  }

  // Analytics
  getMetricasDoDia(filaId: string, data: Date): Observable<FilaMetrics> {
    const dataStr = data.toISOString().split('T')[0];
    return this.http.get<FilaMetrics>(
      `${this.analyticsUrl}/${filaId}/metricas`,
      { params: new HttpParams().set('data', dataStr) }
    );
  }

  getTempoMedioEspera(filaId: string): Observable<number> {
    return this.http.get<number>(`${this.analyticsUrl}/${filaId}/tempo-medio-espera`);
  }

  // Promise-based helpers for easier async/await usage
  async gerarSenhaAsync(filaId: string, request: GerarSenhaRequest, tenantId?: string): Promise<SenhaFila> {
    return firstValueFrom(this.gerarSenha(filaId, request, tenantId));
  }

  async consultarSenhaAsync(filaId: string, numeroSenha: string, tenantId?: string): Promise<ConsultarSenhaResponse> {
    return firstValueFrom(this.consultarSenha(filaId, numeroSenha, tenantId));
  }

  async getFilaAsync(filaId: string): Promise<FilaEspera> {
    return firstValueFrom(this.getFila(filaId));
  }

  async getFilaSummaryAsync(filaId: string): Promise<FilaSummary> {
    return firstValueFrom(this.getFilaSummary(filaId));
  }
}
