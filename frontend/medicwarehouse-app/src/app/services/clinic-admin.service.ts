import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  ClinicAdminInfoDto, 
  UpdateClinicInfoRequest, 
  ClinicUserDto,
  SubscriptionDto
} from '../models/clinic-admin.model';

@Injectable({
  providedIn: 'root'
})
export class ClinicAdminService {
  private apiUrl = `${environment.apiUrl}/clinic-admin`;

  constructor(private http: HttpClient) {}

  getClinicInfo(): Observable<ClinicAdminInfoDto> {
    return this.http.get<ClinicAdminInfoDto>(`${this.apiUrl}/info`);
  }

  updateClinicInfo(request: UpdateClinicInfoRequest): Observable<ClinicAdminInfoDto> {
    return this.http.put<ClinicAdminInfoDto>(`${this.apiUrl}/info`, request);
  }

  getClinicUsers(): Observable<ClinicUserDto[]> {
    return this.http.get<ClinicUserDto[]>(`${this.apiUrl}/users`);
  }

  getSubscription(): Observable<SubscriptionDto> {
    return this.http.get<SubscriptionDto>(`${this.apiUrl}/subscription`);
  }

  cancelSubscription(): Observable<any> {
    return this.http.put(`${this.apiUrl}/subscription/cancel`, {});
  }
}
