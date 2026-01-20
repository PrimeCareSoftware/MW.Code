import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExamAutocomplete } from '../models/exam-catalog.model';

@Injectable({
  providedIn: 'root'
})
export class ExamCatalogService {
  constructor(private http: HttpClient) {}

  getExams(query: string): Observable<ExamAutocomplete[]> {
    return this.http.get<ExamAutocomplete[]>(`/api/exams?q=${query}`);
  }

  getExamById(id: string): Observable<ExamAutocomplete> {
    return this.http.get<ExamAutocomplete>(`/api/exams/${id}`);
  }

  search(term: string): Observable<any[]> {
    return this.http.get<any[]>(`/api/exams/search?term=${term}`);
  }
}
