import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExamAutocomplete } from '../models/exam-catalog.model';
import { ExamCatalog, CreateExamCatalogRequest, UpdateExamCatalogRequest } from '../models/exam-catalog-admin.model';

@Injectable({
  providedIn: 'root'
})
export class ExamCatalogService {
  private readonly baseUrl = '/api/exam-catalog';

  constructor(private http: HttpClient) {}

  getExams(query: string): Observable<ExamAutocomplete[]> {
    return this.http.get<ExamAutocomplete[]>(`${this.baseUrl}?q=${query}`);
  }

  getExamById(id: string): Observable<ExamCatalog> {
    return this.http.get<ExamCatalog>(`${this.baseUrl}/${id}`);
  }

  search(term: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/search?term=${term}`);
  }

  // Admin methods
  getAllExams(searchTerm?: string, examType?: number, page = 1, pageSize = 50): Observable<ExamCatalog[]> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (searchTerm) {
      params = params.set('search', searchTerm);
    }
    if (examType !== undefined && examType !== null) {
      params = params.set('examType', examType.toString());
    }

    return this.http.get<ExamCatalog[]>(this.baseUrl, { params });
  }

  createExam(exam: CreateExamCatalogRequest): Observable<ExamCatalog> {
    return this.http.post<ExamCatalog>(this.baseUrl, exam);
  }

  updateExam(id: string, exam: UpdateExamCatalogRequest): Observable<ExamCatalog> {
    return this.http.put<ExamCatalog>(`${this.baseUrl}/${id}`, exam);
  }

  deactivateExam(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  activateExam(id: string): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/activate`, {});
  }
}
