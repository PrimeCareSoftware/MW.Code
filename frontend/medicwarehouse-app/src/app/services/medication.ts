import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Medication, MedicationAutocomplete, MedicationCategory } from '../models/medication.model';

@Injectable({
  providedIn: 'root'
})
export class MedicationService {
  private apiUrl = `${environment.apiUrl}/medications`;

  constructor(private http: HttpClient) {}

  getAll(activeOnly: boolean = true): Observable<Medication[]> {
    const params = new HttpParams().set('activeOnly', activeOnly.toString());
    return this.http.get<Medication[]>(this.apiUrl, { params });
  }

  search(term: string): Observable<MedicationAutocomplete[]> {
    const params = new HttpParams().set('term', term);
    return this.http.get<MedicationAutocomplete[]>(`${this.apiUrl}/search`, { params });
  }

  getById(id: string): Observable<Medication> {
    return this.http.get<Medication>(`${this.apiUrl}/${id}`);
  }

  getByCategory(category: MedicationCategory): Observable<Medication[]> {
    return this.http.get<Medication[]>(`${this.apiUrl}/category/${category}`);
  }

  create(medication: Partial<Medication>): Observable<Medication> {
    return this.http.post<Medication>(this.apiUrl, medication);
  }

  update(id: string, medication: Partial<Medication>): Observable<Medication> {
    return this.http.put<Medication>(`${this.apiUrl}/${id}`, medication);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
