import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  DocumentTemplate, 
  CreateDocumentTemplateDto, 
  UpdateDocumentTemplateDto,
  DocumentTemplateFilter,
  DocumentTemplateType,
  ProfessionalSpecialty
} from '../models/document-template.model';
import { GlobalDocumentTemplate } from '../models/global-document-template.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DocumentTemplateService {
  private readonly apiUrl = `${environment.apiUrl}/document-templates`;

  constructor(private http: HttpClient) {}

  /**
   * Get all document templates with optional filters
   */
  getAll(filter?: DocumentTemplateFilter): Observable<DocumentTemplate[]> {
    let params = new HttpParams();
    
    if (filter) {
      if (filter.specialty !== undefined) {
        params = params.set('specialty', filter.specialty.toString());
      }
      if (filter.type !== undefined) {
        params = params.set('type', filter.type.toString());
      }
      if (filter.isActive !== undefined) {
        params = params.set('isActive', filter.isActive.toString());
      }
      if (filter.isSystem !== undefined) {
        params = params.set('isSystem', filter.isSystem.toString());
      }
      if (filter.clinicId) {
        params = params.set('clinicId', filter.clinicId);
      }
    }

    return this.http.get<DocumentTemplate[]>(this.apiUrl, { params });
  }

  /**
   * Get a specific template by ID
   */
  getById(id: string): Observable<DocumentTemplate> {
    return this.http.get<DocumentTemplate>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get templates by professional specialty
   */
  getBySpecialty(specialty: ProfessionalSpecialty, activeOnly: boolean = false): Observable<DocumentTemplate[]> {
    let params = new HttpParams();
    if (activeOnly) {
      params = params.set('activeOnly', 'true');
    }
    return this.http.get<DocumentTemplate[]>(`${this.apiUrl}/by-specialty/${specialty}`, { params });
  }

  /**
   * Get templates by document type
   */
  getByType(type: DocumentTemplateType): Observable<DocumentTemplate[]> {
    return this.http.get<DocumentTemplate[]>(`${this.apiUrl}/by-type/${type}`);
  }

  /**
   * Get templates by clinic
   */
  getByClinic(clinicId: string): Observable<DocumentTemplate[]> {
    return this.http.get<DocumentTemplate[]>(`${this.apiUrl}/by-clinic/${clinicId}`);
  }

  /**
   * Create a new document template
   */
  create(dto: CreateDocumentTemplateDto): Observable<DocumentTemplate> {
    return this.http.post<DocumentTemplate>(this.apiUrl, dto);
  }

  /**
   * Update an existing document template
   */
  update(id: string, dto: UpdateDocumentTemplateDto): Observable<DocumentTemplate> {
    return this.http.put<DocumentTemplate>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * Delete a document template
   */
  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Activate a template
   */
  activate(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/activate`, {});
  }

  /**
   * Deactivate a template
   */
  deactivate(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/deactivate`, {});
  }

  /**
   * Get all global document templates available to clinic users
   * Returns only active global templates
   */
  getGlobalTemplates(specialty?: ProfessionalSpecialty, type?: DocumentTemplateType): Observable<GlobalDocumentTemplate[]> {
    const globalApiUrl = `${this.apiUrl}/global-templates`;
    let params = new HttpParams();
    
    if (specialty !== undefined) {
      params = params.set('specialty', specialty.toString());
    }
    if (type !== undefined) {
      params = params.set('type', type.toString());
    }
    
    return this.http.get<GlobalDocumentTemplate[]>(globalApiUrl, { params });
  }

  /**
   * Create a document template from a global template
   */
  createFromGlobalTemplate(globalTemplateId: string): Observable<DocumentTemplate> {
    return this.http.post<DocumentTemplate>(`${this.apiUrl}/from-global/${globalTemplateId}`, {});
  }
}
