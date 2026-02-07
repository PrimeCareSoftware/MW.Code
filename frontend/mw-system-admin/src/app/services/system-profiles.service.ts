import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DefaultProfileTemplate {
  name: string;
  description: string;
  permissions: string[];
  applicableClinicTypes: string[];
}

export interface ClinicType {
  value: string;
  name: string;
}

export interface PermissionCategory {
  category: string;
  permissions: Permission[];
}

export interface Permission {
  key: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class SystemProfilesService {
  private readonly baseUrl = '/api/system-admin/default-profiles';

  constructor(private http: HttpClient) {}

  /**
   * Get all default profile templates
   */
  getDefaultTemplates(): Observable<DefaultProfileTemplate[]> {
    return this.http.get<DefaultProfileTemplate[]>(`${this.baseUrl}/templates`);
  }

  /**
   * Get default profiles for a specific clinic type
   */
  getTemplatesByClinicType(clinicType: string): Observable<DefaultProfileTemplate[]> {
    return this.http.get<DefaultProfileTemplate[]>(`${this.baseUrl}/templates/clinic-type/${clinicType}`);
  }

  /**
   * Get all available clinic types
   */
  getClinicTypes(): Observable<ClinicType[]> {
    return this.http.get<ClinicType[]>(`${this.baseUrl}/clinic-types`);
  }

  /**
   * Get all available permissions grouped by category
   */
  getAllPermissions(): Observable<PermissionCategory[]> {
    return this.http.get<PermissionCategory[]>(`${this.baseUrl}/permissions`);
  }
}
