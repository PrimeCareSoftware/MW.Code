import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DigitalPrescription,
  CreateDigitalPrescription,
  SNGPCReport,
  CreateSNGPCReport
} from '../../models/prescriptions/digital-prescription.model';

@Injectable({
  providedIn: 'root'
})
export class DigitalPrescriptionService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/DigitalPrescriptions`;
  private sngpcUrl = `${environment.apiUrl}/SNGPCReports`;

  // Digital Prescription methods

  create(prescription: CreateDigitalPrescription): Observable<DigitalPrescription> {
    return this.createPrescription(prescription);
  }

  createPrescription(prescription: CreateDigitalPrescription): Observable<DigitalPrescription> {
    return this.http.post<DigitalPrescription>(this.apiUrl, prescription);
  }

  getPrescriptionById(id: string): Observable<DigitalPrescription> {
    return this.http.get<DigitalPrescription>(`${this.apiUrl}/${id}`);
  }

  getPrescriptionsByPatient(patientId: string): Observable<DigitalPrescription[]> {
    return this.http.get<DigitalPrescription[]>(`${this.apiUrl}/patient/${patientId}`);
  }

  getPrescriptionsByMedicalRecord(medicalRecordId: string): Observable<DigitalPrescription[]> {
    return this.http.get<DigitalPrescription[]>(`${this.apiUrl}/medical-record/${medicalRecordId}`);
  }

  getPrescriptionsByDoctor(doctorId: string): Observable<DigitalPrescription[]> {
    return this.http.get<DigitalPrescription[]>(`${this.apiUrl}/doctor/${doctorId}`);
  }

  getActivePrescriptionsForPatient(patientId: string): Observable<DigitalPrescription[]> {
    return this.http.get<DigitalPrescription[]>(`${this.apiUrl}/patient/${patientId}/active`);
  }

  getPrescriptionByVerificationCode(verificationCode: string): Observable<DigitalPrescription> {
    return this.http.get<DigitalPrescription>(`${this.apiUrl}/verify/${verificationCode}`);
  }

  signPrescription(id: string, digitalSignature: string, certificateThumbprint: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/sign`, {
      digitalSignature,
      certificateThumbprint
    });
  }

  deactivatePrescription(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/deactivate`, {});
  }

  getUnreportedToSNGPC(startDate?: Date, endDate?: Date): Observable<DigitalPrescription[]> {
    let params = new HttpParams();
    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }
    return this.http.get<DigitalPrescription[]>(`${this.apiUrl}/sngpc/unreported`, { params });
  }

  // SNGPC Report methods

  createSNGPCReport(report: CreateSNGPCReport): Observable<SNGPCReport> {
    return this.http.post<SNGPCReport>(this.sngpcUrl, report);
  }

  getSNGPCReportById(id: string): Observable<SNGPCReport> {
    return this.http.get<SNGPCReport>(`${this.sngpcUrl}/${id}`);
  }

  getSNGPCReportByMonthYear(year: number, month: number): Observable<SNGPCReport> {
    return this.http.get<SNGPCReport>(`${this.sngpcUrl}/${year}/${month}`);
  }

  getSNGPCReportsByYear(year: number): Observable<SNGPCReport[]> {
    return this.http.get<SNGPCReport[]>(`${this.sngpcUrl}/year/${year}`);
  }

  getSNGPCReportsByStatus(status: string): Observable<SNGPCReport[]> {
    return this.http.get<SNGPCReport[]>(`${this.sngpcUrl}/status/${status}`);
  }

  getOverdueSNGPCReports(): Observable<SNGPCReport[]> {
    return this.http.get<SNGPCReport[]>(`${this.sngpcUrl}/overdue`);
  }

  getLatestSNGPCReport(): Observable<SNGPCReport> {
    return this.http.get<SNGPCReport>(`${this.sngpcUrl}/latest`);
  }

  getSNGPCReportHistory(count: number = 12): Observable<SNGPCReport[]> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<SNGPCReport[]>(`${this.sngpcUrl}/history`, { params });
  }

  generateSNGPCXML(id: string): Observable<any> {
    return this.http.post(`${this.sngpcUrl}/${id}/generate-xml`, {});
  }

  markSNGPCAsTransmitted(id: string, transmissionProtocol: string): Observable<any> {
    return this.http.post(`${this.sngpcUrl}/${id}/transmit`, { transmissionProtocol });
  }

  markSNGPCTransmissionFailed(id: string, errorMessage: string): Observable<any> {
    return this.http.post(`${this.sngpcUrl}/${id}/transmission-failed`, { errorMessage });
  }

  downloadSNGPCXML(id: string): Observable<Blob> {
    return this.http.get(`${this.sngpcUrl}/${id}/download-xml`, {
      responseType: 'blob'
    });
  }
}
