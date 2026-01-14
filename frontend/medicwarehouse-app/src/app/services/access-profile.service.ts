import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  AccessProfile,
  CreateAccessProfile,
  UpdateAccessProfile,
  PermissionCategory,
  AssignProfile
} from '../models/access-profile.model';

@Injectable({
  providedIn: 'root'
})
export class AccessProfileService {
  private apiUrl = `${environment.apiUrl}/AccessProfiles`;

  constructor(private http: HttpClient) {}

  /**
   * Get all access profiles for the clinic
   */
  getProfiles(): Observable<AccessProfile[]> {
    return this.http.get<AccessProfile[]>(this.apiUrl);
  }

  /**
   * Get profile by ID
   */
  getProfile(id: string): Observable<AccessProfile> {
    return this.http.get<AccessProfile>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create new access profile
   */
  createProfile(profile: CreateAccessProfile): Observable<AccessProfile> {
    return this.http.post<AccessProfile>(this.apiUrl, profile);
  }

  /**
   * Update access profile
   */
  updateProfile(id: string, profile: UpdateAccessProfile): Observable<AccessProfile> {
    return this.http.put<AccessProfile>(`${this.apiUrl}/${id}`, profile);
  }

  /**
   * Delete access profile
   */
  deleteProfile(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Get all available permissions grouped by category
   */
  getAllPermissions(): Observable<PermissionCategory[]> {
    return this.http.get<PermissionCategory[]>(`${this.apiUrl}/permissions`);
  }

  /**
   * Assign profile to user
   */
  assignProfileToUser(data: AssignProfile): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/assign`, data);
  }

  /**
   * Create default profiles for the clinic
   */
  createDefaultProfiles(): Observable<AccessProfile[]> {
    return this.http.post<AccessProfile[]>(`${this.apiUrl}/create-defaults`, {});
  }
}
