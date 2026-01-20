import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MedicationAutocomplete } from '../models/medication.model';

@Injectable({
  providedIn: 'root'
})
export class MedicationService {
  constructor(private http: HttpClient) {}

  getMedications(query: string): Observable<MedicationAutocomplete[]> {
    return this.http.get<MedicationAutocomplete[]>(`/api/medications?q=${query}`);
  }

  getMedicationById(id: string): Observable<MedicationAutocomplete> {
    return this.http.get<MedicationAutocomplete>(`/api/medications/${id}`);
  }

  search(term: string): Observable<any[]> {
    return this.http.get<any[]>(`/api/medications/search?term=${term}`);
  }
}
