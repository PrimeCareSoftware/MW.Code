import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PatientData {
  metadata: {
    exportDate: string;
    patientId: string;
    dataFormat: string;
    lgpdCompliance: string;
  };
  personalData: any;
  medicalRecords: any[];
  appointments: any[];
  prescriptions: any[];
  exams: any[];
  consents: any[];
  accessHistory: any[];
}

export interface ExportHistoryItem {
  id: string;
  patientId: string;
  exportDate: string;
  format: string;
  status: string;
  fileSize?: number;
}

@Injectable({
  providedIn: 'root'
})
export class DataPortabilityService {
  private apiUrl = `${environment.apiUrl}/dataportability`;
  
  loading = signal(false);
  
  constructor(private http: HttpClient) {}

  /**
   * Export patient data as JSON
   */
  exportAsJson(patientId: string): Observable<PatientData> {
    this.loading.set(true);
    return this.http.get<PatientData>(`${this.apiUrl}/patient/${patientId}/export/json`)
      .pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Export patient data as XML (returns blob)
   */
  exportAsXml(patientId: string): Observable<Blob> {
    this.loading.set(true);
    return this.http.get(`${this.apiUrl}/patient/${patientId}/export/xml`, {
      responseType: 'blob',
      headers: new HttpHeaders({
        'Accept': 'application/xml'
      })
    }).pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Export patient data as PDF (returns blob)
   */
  exportAsPdf(patientId: string): Observable<Blob> {
    this.loading.set(true);
    return this.http.get(`${this.apiUrl}/patient/${patientId}/export/pdf`, {
      responseType: 'blob',
      headers: new HttpHeaders({
        'Accept': 'application/pdf'
      })
    }).pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Export patient data as complete package (ZIP)
   */
  exportAsPackage(patientId: string): Observable<Blob> {
    this.loading.set(true);
    return this.http.get(`${this.apiUrl}/patient/${patientId}/export/package`, {
      responseType: 'blob',
      headers: new HttpHeaders({
        'Accept': 'application/zip'
      })
    }).pipe(tap(() => this.loading.set(false)));
  }

  /**
   * Get information about data portability service
   */
  getInfo(): Observable<any> {
    return this.http.get(`${this.apiUrl}/info`);
  }

  /**
   * Download blob as file
   */
  downloadBlob(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    window.URL.revokeObjectURL(url);
  }

  /**
   * Download JSON data as file
   */
  downloadJson(data: any, filename: string): void {
    const json = JSON.stringify(data, null, 2);
    const blob = new Blob([json], { type: 'application/json' });
    this.downloadBlob(blob, filename);
  }

  /**
   * Get available export formats
   */
  getAvailableFormats(): string[] {
    return ['JSON', 'XML', 'PDF', 'ZIP'];
  }
}
