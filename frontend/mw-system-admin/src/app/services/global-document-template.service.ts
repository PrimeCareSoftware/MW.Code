import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  GlobalDocumentTemplate,
  CreateGlobalTemplateDto,
  UpdateGlobalTemplateDto,
  GlobalTemplateFilter,
  DocumentTemplateType,
  ProfessionalSpecialty
} from '../models/global-document-template.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GlobalDocumentTemplateService {
  private readonly apiUrl = `${environment.apiUrl}/system-admin/global-templates`;

  constructor(private http: HttpClient) {}

  /**
   * Get all global document templates with optional filters
   */
  getAll(filter?: GlobalTemplateFilter): Observable<GlobalDocumentTemplate[]> {
    let params = new HttpParams();

    if (filter) {
      if (filter.type !== undefined) {
        params = params.set('type', filter.type.toString());
      }
      if (filter.specialty !== undefined) {
        params = params.set('specialty', filter.specialty.toString());
      }
      if (filter.isActive !== undefined) {
        params = params.set('isActive', filter.isActive.toString());
      }
      if (filter.searchTerm) {
        params = params.set('searchTerm', filter.searchTerm);
      }
    }

    return this.http.get<GlobalDocumentTemplate[]>(this.apiUrl, { params });
  }

  /**
   * Get a specific global template by ID
   */
  getById(id: string): Observable<GlobalDocumentTemplate> {
    return this.http.get<GlobalDocumentTemplate>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get global templates by document type
   */
  getByType(type: DocumentTemplateType): Observable<GlobalDocumentTemplate[]> {
    return this.http.get<GlobalDocumentTemplate[]>(`${this.apiUrl}/type/${type}`);
  }

  /**
   * Get global templates by professional specialty
   */
  getBySpecialty(specialty: ProfessionalSpecialty): Observable<GlobalDocumentTemplate[]> {
    return this.http.get<GlobalDocumentTemplate[]>(`${this.apiUrl}/specialty/${specialty}`);
  }

  /**
   * Create a new global document template
   */
  create(dto: CreateGlobalTemplateDto): Observable<GlobalDocumentTemplate> {
    return this.http.post<GlobalDocumentTemplate>(this.apiUrl, dto);
  }

  /**
   * Update an existing global document template
   */
  update(id: string, dto: UpdateGlobalTemplateDto): Observable<GlobalDocumentTemplate> {
    return this.http.put<GlobalDocumentTemplate>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * Delete a global document template
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Set active status of a global template
   * Note: Backend expects raw boolean value, not an object wrapper
   */
  setActiveStatus(id: string, isActive: boolean): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/active`, isActive);
  }
}
