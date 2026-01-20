import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicationAutocomplete } from '../models/medication.model';
import { Medication, CreateMedicationRequest, UpdateMedicationRequest } from '../models/medication-admin.model';

@Injectable({
  providedIn: 'root'
})
export class MedicationService {
  private readonly baseUrl = '/api/medications';

  constructor(private http: HttpClient) {}

  getMedications(query: string): Observable<MedicationAutocomplete[]> {
    return this.http.get<MedicationAutocomplete[]>(`${this.baseUrl}?q=${query}`);
  }

  getMedicationById(id: string): Observable<Medication> {
    return this.http.get<Medication>(`${this.baseUrl}/${id}`);
  }

  search(term: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/search?term=${term}`);
  }

  // Admin methods
  getAllMedications(searchTerm?: string, category?: number, page = 1, pageSize = 50): Observable<Medication[]> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (searchTerm) {
      params = params.set('search', searchTerm);
    }
    if (category !== undefined && category !== null) {
      params = params.set('category', category.toString());
    }

    return this.http.get<Medication[]>(this.baseUrl, { params });
  }

  createMedication(medication: CreateMedicationRequest): Observable<Medication> {
    return this.http.post<Medication>(this.baseUrl, medication);
  }

  updateMedication(id: string, medication: UpdateMedicationRequest): Observable<Medication> {
    return this.http.put<Medication>(`${this.baseUrl}/${id}`, medication);
  }

  deactivateMedication(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  activateMedication(id: string): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/activate`, {});
  }
}
