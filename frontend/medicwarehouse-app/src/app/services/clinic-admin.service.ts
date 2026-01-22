import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  ClinicAdminInfoDto, 
  UpdateClinicInfoRequest, 
  ClinicUserDto,
  CreateClinicUserRequest,
  UpdateClinicUserRequest,
  ChangeUserPasswordRequest,
  ChangeUserRoleRequest,
  SubscriptionDto,
  SubscriptionDetailsDto,
  MyClinicDto,
  PublicDisplaySettingsDto,
  UpdatePublicDisplaySettingsRequest
} from '../models/clinic-admin.model';

@Injectable({
  providedIn: 'root'
})
export class ClinicAdminService {
  private apiUrl = `${environment.apiUrl}/ClinicAdmin`;

  constructor(private http: HttpClient) {}

  // Clinic Info
  getClinicInfo(): Observable<ClinicAdminInfoDto> {
    return this.http.get<ClinicAdminInfoDto>(`${this.apiUrl}/info`);
  }

  updateClinicInfo(request: UpdateClinicInfoRequest): Observable<ClinicAdminInfoDto> {
    return this.http.put<ClinicAdminInfoDto>(`${this.apiUrl}/info`, request);
  }

  // User Management
  getClinicUsers(): Observable<ClinicUserDto[]> {
    return this.http.get<ClinicUserDto[]>(`${this.apiUrl}/users`);
  }

  createUser(request: CreateClinicUserRequest): Observable<ClinicUserDto> {
    return this.http.post<ClinicUserDto>(`${this.apiUrl}/users`, request);
  }

  updateUser(userId: string, request: UpdateClinicUserRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/users/${userId}`, request);
  }

  changeUserPassword(userId: string, request: ChangeUserPasswordRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/users/${userId}/password`, request);
  }

  changeUserRole(userId: string, request: ChangeUserRoleRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/users/${userId}/role`, request);
  }

  activateUser(userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/users/${userId}/activate`, {});
  }

  deactivateUser(userId: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/users/${userId}/deactivate`, {});
  }

  // Subscription
  getSubscription(): Observable<SubscriptionDto> {
    return this.http.get<SubscriptionDto>(`${this.apiUrl}/subscription`);
  }

  getSubscriptionDetails(): Observable<SubscriptionDetailsDto> {
    return this.http.get<SubscriptionDetailsDto>(`${this.apiUrl}/subscription/details`);
  }

  cancelSubscription(): Observable<any> {
    return this.http.put(`${this.apiUrl}/subscription/cancel`, {});
  }

  // My Clinics
  getMyClinics(): Observable<MyClinicDto[]> {
    return this.http.get<MyClinicDto[]>(`${this.apiUrl}/my-clinics`);
  }

  // Public Display Settings
  getPublicDisplaySettings(): Observable<PublicDisplaySettingsDto> {
    return this.http.get<PublicDisplaySettingsDto>(`${this.apiUrl}/public-display-settings`);
  }

  updatePublicDisplaySettings(request: UpdatePublicDisplaySettingsRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/public-display-settings`, request);
  }
}
