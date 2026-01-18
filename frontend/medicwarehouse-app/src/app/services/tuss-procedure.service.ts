import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TussProcedure, CreateTussProcedure, UpdateTussProcedure, TussCategory } from '../models/tiss.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TussProcedureService {
  private apiUrl = `${environment.apiUrl}/tuss-procedures`;
  private categoriesUrl = `${environment.apiUrl}/tuss-categories`;

  constructor(private http: HttpClient) { }

  getAll(): Observable<TussProcedure[]> {
    return this.http.get<TussProcedure[]>(this.apiUrl);
  }

  getById(id: string): Observable<TussProcedure> {
    return this.http.get<TussProcedure>(`${this.apiUrl}/${id}`);
  }

  create(procedure: CreateTussProcedure): Observable<TussProcedure> {
    return this.http.post<TussProcedure>(this.apiUrl, procedure);
  }

  update(id: string, procedure: UpdateTussProcedure): Observable<TussProcedure> {
    return this.http.put<TussProcedure>(`${this.apiUrl}/${id}`, procedure);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  search(searchTerm: string): Observable<TussProcedure[]> {
    return this.http.get<TussProcedure[]>(`${this.apiUrl}/search?searchTerm=${encodeURIComponent(searchTerm)}`);
  }

  getCategories(): Observable<TussCategory[]> {
    return this.http.get<TussCategory[]>(this.categoriesUrl);
  }
}
