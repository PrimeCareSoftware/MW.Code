import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Document } from '../models/document.model';

@Injectable({
  providedIn: 'root'
})
export class DocumentService {
  private apiUrl = `${environment.apiUrl}/documents`;

  constructor(private http: HttpClient) {}

  getMyDocuments(skip: number = 0, take: number = 50): Observable<Document[]> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());
    return this.http.get<Document[]>(this.apiUrl, { params });
  }

  getRecentDocuments(take: number = 5): Observable<Document[]> {
    const params = new HttpParams().set('take', take.toString());
    return this.http.get<Document[]>(`${this.apiUrl}/recent`, { params });
  }

  getDocumentById(id: string): Observable<Document> {
    return this.http.get<Document>(`${this.apiUrl}/${id}`);
  }

  getDocumentsByType(type: string, skip: number = 0, take: number = 50): Observable<Document[]> {
    const params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());
    return this.http.get<Document[]>(`${this.apiUrl}/type/${type}`, { params });
  }

  getDocumentsCount(): Observable<{ count: number }> {
    return this.http.get<{ count: number }>(`${this.apiUrl}/count`);
  }

  downloadDocument(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/download`, { 
      responseType: 'blob'
    });
  }
}
