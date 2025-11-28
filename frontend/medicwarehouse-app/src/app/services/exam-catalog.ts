import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ExamCatalog, ExamAutocomplete } from '../models/exam-catalog.model';
import { ExamType } from '../models/exam-request.model';

@Injectable({
  providedIn: 'root'
})
export class ExamCatalogService {
  private apiUrl = `${environment.apiUrl}/exam-catalog`;

  constructor(private http: HttpClient) {}

  getAll(activeOnly: boolean = true): Observable<ExamCatalog[]> {
    const params = new HttpParams().set('activeOnly', activeOnly.toString());
    return this.http.get<ExamCatalog[]>(this.apiUrl, { params });
  }

  search(term: string): Observable<ExamAutocomplete[]> {
    const params = new HttpParams().set('term', term);
    return this.http.get<ExamAutocomplete[]>(`${this.apiUrl}/search`, { params });
  }

  getById(id: string): Observable<ExamCatalog> {
    return this.http.get<ExamCatalog>(`${this.apiUrl}/${id}`);
  }

  getByExamType(examType: ExamType): Observable<ExamCatalog[]> {
    return this.http.get<ExamCatalog[]>(`${this.apiUrl}/type/${examType}`);
  }

  getByCategory(category: string): Observable<ExamCatalog[]> {
    return this.http.get<ExamCatalog[]>(`${this.apiUrl}/category/${encodeURIComponent(category)}`);
  }

  create(exam: Partial<ExamCatalog>): Observable<ExamCatalog> {
    return this.http.post<ExamCatalog>(this.apiUrl, exam);
  }

  update(id: string, exam: Partial<ExamCatalog>): Observable<ExamCatalog> {
    return this.http.put<ExamCatalog>(`${this.apiUrl}/${id}`, exam);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
