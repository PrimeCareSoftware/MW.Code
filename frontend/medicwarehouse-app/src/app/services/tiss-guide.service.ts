import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TissGuide, CreateTissGuide, UpdateTissGuide, GuideStatus } from '../models/tiss.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TissGuideService {
  private apiUrl = `${environment.apiUrl}/tiss-guides`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<TissGuide[]> {
    return this.http.get<TissGuide[]>(this.apiUrl);
  }

  getByStatus(status: GuideStatus): Observable<TissGuide[]> {
    return this.http.get<TissGuide[]>(`${this.apiUrl}/status/${status}`);
  }

  getUnbilledByOperator(operatorId: string): Observable<TissGuide[]> {
    return this.http.get<TissGuide[]>(`${this.apiUrl}/unbilled/operator/${operatorId}`);
  }

  getById(id: string): Observable<TissGuide> {
    return this.http.get<TissGuide>(`${this.apiUrl}/${id}`);
  }

  create(guide: CreateTissGuide): Observable<TissGuide> {
    return this.http.post<TissGuide>(this.apiUrl, guide);
  }

  update(id: string, guide: UpdateTissGuide): Observable<TissGuide> {
    return this.http.put<TissGuide>(`${this.apiUrl}/${id}`, guide);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
