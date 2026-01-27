import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AssinaturaDigital,
  AssinarDocumentoRequest,
  ResultadoAssinatura,
  ResultadoValidacao
} from '../models/assinatura-digital.model';

@Injectable({
  providedIn: 'root'
})
export class AssinaturaDigitalService {
  private apiUrl = `${environment.apiUrl}/assinaturadigital`;

  constructor(private http: HttpClient) { }

  /**
   * Assina um documento digitalmente
   */
  assinarDocumento(request: AssinarDocumentoRequest): Observable<ResultadoAssinatura> {
    return this.http.post<ResultadoAssinatura>(`${this.apiUrl}/assinar`, request);
  }

  /**
   * Valida uma assinatura digital
   */
  validarAssinatura(assinaturaId: string): Observable<ResultadoValidacao> {
    return this.http.get<ResultadoValidacao>(`${this.apiUrl}/${assinaturaId}/validar`);
  }

  /**
   * Lista todas as assinaturas de um documento
   */
  listarAssinaturasPorDocumento(documentoId: string): Observable<AssinaturaDigital[]> {
    return this.http.get<AssinaturaDigital[]>(`${this.apiUrl}/documento/${documentoId}`);
  }
}
