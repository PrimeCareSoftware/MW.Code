import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TissBatch, CreateTissBatch, UpdateTissBatch } from '../models/tiss.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TissBatchService {
  private apiUrl = `${environment.apiUrl}/tiss-batches`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<TissBatch[]> {
    return this.http.get<TissBatch[]>(this.apiUrl);
  }

  getById(id: string): Observable<TissBatch> {
    return this.http.get<TissBatch>(`${this.apiUrl}/${id}`);
  }

  create(batch: CreateTissBatch): Observable<TissBatch> {
    return this.http.post<TissBatch>(this.apiUrl, batch);
  }

  update(id: string, batch: UpdateTissBatch): Observable<TissBatch> {
    return this.http.put<TissBatch>(`${this.apiUrl}/${id}`, batch);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  generateXml(id: string): Observable<{ xmlFilePath: string }> {
    return this.http.post<{ xmlFilePath: string }>(`${this.apiUrl}/${id}/generate-xml`, {});
  }

  downloadXml(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/download-xml`, { responseType: 'blob' });
  }
}
