import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InformedConsent, CreateInformedConsent, AcceptInformedConsent } from '../models/medical-record.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class InformedConsentService {
  private apiUrl = `${environment.apiUrl}/informed-consents`;

  constructor(private http: HttpClient) {}

  create(consent: CreateInformedConsent): Observable<InformedConsent> {
    return this.http.post<InformedConsent>(this.apiUrl, consent);
  }

  accept(id: string, data: AcceptInformedConsent): Observable<InformedConsent> {
    return this.http.post<InformedConsent>(`${this.apiUrl}/${id}/accept`, data);
  }

  getByMedicalRecord(medicalRecordId: string): Observable<InformedConsent[]> {
    return this.http.get<InformedConsent[]>(`${this.apiUrl}/medical-record/${medicalRecordId}`);
  }
}
