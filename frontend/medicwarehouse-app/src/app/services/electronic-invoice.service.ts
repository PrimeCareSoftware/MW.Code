import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CreateElectronicInvoice,
  ElectronicInvoice,
  ElectronicInvoiceListItem,
  ElectronicInvoiceStatistics,
  CreateInvoiceConfiguration,
  UpdateInvoiceConfiguration,
  InvoiceConfiguration,
  CancelInvoiceRequest,
  SendInvoiceEmailRequest,
  UploadCertificateRequest
} from '../models/electronic-invoice.model';

@Injectable({
  providedIn: 'root'
})
export class ElectronicInvoiceService {
  private readonly apiUrl = `${environment.apiUrl}/ElectronicInvoices`;

  constructor(private http: HttpClient) {}

  // ===== INVOICE OPERATIONS =====

  createInvoice(data: CreateElectronicInvoice): Observable<ElectronicInvoice> {
    return this.http.post<ElectronicInvoice>(this.apiUrl, data);
  }

  issueInvoice(id: string): Observable<ElectronicInvoice> {
    return this.http.post<ElectronicInvoice>(`${this.apiUrl}/${id}/issue`, {});
  }

  cancelInvoice(id: string, request: CancelInvoiceRequest): Observable<ElectronicInvoice> {
    return this.http.post<ElectronicInvoice>(`${this.apiUrl}/${id}/cancel`, request);
  }

  replaceInvoice(id: string, request: CancelInvoiceRequest): Observable<ElectronicInvoice> {
    return this.http.post<ElectronicInvoice>(`${this.apiUrl}/${id}/replace`, request);
  }

  getInvoiceById(id: string): Observable<ElectronicInvoice> {
    return this.http.get<ElectronicInvoice>(`${this.apiUrl}/${id}`);
  }

  getInvoices(
    startDate?: Date,
    endDate?: Date,
    status?: string,
    clientCpfCnpj?: string
  ): Observable<ElectronicInvoiceListItem[]> {
    let params = new HttpParams();

    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }
    if (status) {
      params = params.set('status', status);
    }
    if (clientCpfCnpj) {
      params = params.set('clientCpfCnpj', clientCpfCnpj);
    }

    return this.http.get<ElectronicInvoiceListItem[]>(this.apiUrl, { params });
  }

  getStatistics(startDate?: Date, endDate?: Date): Observable<ElectronicInvoiceStatistics> {
    let params = new HttpParams();

    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }

    return this.http.get<ElectronicInvoiceStatistics>(`${this.apiUrl}/statistics`, { params });
  }

  downloadPdf(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/pdf`, { 
      responseType: 'blob' 
    });
  }

  downloadXml(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/xml`, { 
      responseType: 'blob' 
    });
  }

  sendByEmail(id: string, request: SendInvoiceEmailRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/send-email`, request);
  }

  // ===== CONFIGURATION OPERATIONS =====

  getConfiguration(): Observable<InvoiceConfiguration> {
    return this.http.get<InvoiceConfiguration>(`${this.apiUrl}/configuration`);
  }

  createConfiguration(data: CreateInvoiceConfiguration): Observable<InvoiceConfiguration> {
    return this.http.post<InvoiceConfiguration>(`${this.apiUrl}/configuration`, data);
  }

  updateConfiguration(data: UpdateInvoiceConfiguration): Observable<InvoiceConfiguration> {
    return this.http.put<InvoiceConfiguration>(`${this.apiUrl}/configuration`, data);
  }

  uploadCertificate(request: UploadCertificateRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/configuration/certificate`, request);
  }

  activateConfiguration(): Observable<any> {
    return this.http.post(`${this.apiUrl}/configuration/activate`, {});
  }

  deactivateConfiguration(): Observable<any> {
    return this.http.post(`${this.apiUrl}/configuration/deactivate`, {});
  }

  // ===== HELPER METHODS =====

  calculateTaxes(serviceAmount: number, issRate: number): {
    issAmount: number;
    pisAmount: number;
    cofinsAmount: number;
    csllAmount: number;
    totalTaxes: number;
    netAmount: number;
  } {
    const issAmount = serviceAmount * (issRate / 100);
    const pisAmount = serviceAmount * 0.0065;  // 0.65%
    const cofinsAmount = serviceAmount * 0.03;  // 3.00%
    const csllAmount = serviceAmount * 0.01;  // 1.00%
    const totalTaxes = issAmount + pisAmount + cofinsAmount + csllAmount;
    const netAmount = serviceAmount - totalTaxes;

    return {
      issAmount: Math.round(issAmount * 100) / 100,
      pisAmount: Math.round(pisAmount * 100) / 100,
      cofinsAmount: Math.round(cofinsAmount * 100) / 100,
      csllAmount: Math.round(csllAmount * 100) / 100,
      totalTaxes: Math.round(totalTaxes * 100) / 100,
      netAmount: Math.round(netAmount * 100) / 100
    };
  }
}
