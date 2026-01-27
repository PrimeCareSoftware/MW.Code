import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CertificadoDigital,
  CertificateInfo,
  ImportarCertificadoA1Request,
  RegistrarCertificadoA3Request
} from '../models/certificado-digital.model';

@Injectable({
  providedIn: 'root'
})
export class CertificadoDigitalService {
  private apiUrl = `${environment.apiUrl}/certificadodigital`;

  constructor(private http: HttpClient) { }

  /**
   * Lista todos os certificados do médico autenticado
   */
  listarCertificados(): Observable<CertificadoDigital[]> {
    return this.http.get<CertificadoDigital[]>(this.apiUrl);
  }

  /**
   * Obtém detalhes de um certificado específico
   */
  obterPorId(id: string): Observable<CertificadoDigital> {
    return this.http.get<CertificadoDigital>(`${this.apiUrl}/${id}`);
  }

  /**
   * Importa um certificado A1 (arquivo PFX)
   */
  importarCertificadoA1(arquivo: File, senha: string): Observable<CertificadoDigital> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    formData.append('senha', senha);

    return this.http.post<CertificadoDigital>(`${this.apiUrl}/a1/importar`, formData);
  }

  /**
   * Lista certificados A3 disponíveis no Windows Certificate Store
   */
  listarCertificadosA3Disponiveis(): Observable<CertificateInfo[]> {
    return this.http.get<CertificateInfo[]>(`${this.apiUrl}/a3/disponiveis`);
  }

  /**
   * Registra um certificado A3 (token/smartcard)
   */
  registrarCertificadoA3(thumbprint: string): Observable<CertificadoDigital> {
    const request: RegistrarCertificadoA3Request = { thumbprint };
    return this.http.post<CertificadoDigital>(`${this.apiUrl}/a3/registrar`, request);
  }

  /**
   * Invalida um certificado
   */
  invalidarCertificado(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
