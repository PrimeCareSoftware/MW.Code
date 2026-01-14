import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface UserProfile {
  id: string;
  email: string;
  fullName: string;
  cpf: string;
  phoneNumber: string;
  dateOfBirth: Date;
  emailConfirmed: boolean;
  phoneConfirmed: boolean;
  twoFactorEnabled: boolean;
  lastLoginAt: Date;
  createdAt: Date;
}

export interface UpdateProfileRequest {
  fullName?: string;
  phoneNumber?: string;
}

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private apiUrl = `${environment.apiUrl}/profile`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/me`);
  }

  updateProfile(request: UpdateProfileRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/me`, request);
  }
}
