import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { 
  ClinicCustomizationDto, 
  ClinicCustomizationPublicDto, 
  UpdateClinicCustomizationRequest 
} from '../models/clinic-customization.model';

@Injectable({
  providedIn: 'root'
})
export class ClinicCustomizationService {
  private apiUrl = `${environment.apiUrl}/clinic-customization`;

  constructor(private http: HttpClient) {}

  getBySubdomain(subdomain: string): Observable<ClinicCustomizationPublicDto> {
    return this.http.get<ClinicCustomizationPublicDto>(`${this.apiUrl}/by-subdomain/${subdomain}`);
  }

  getCurrentClinicCustomization(): Observable<ClinicCustomizationDto> {
    return this.http.get<ClinicCustomizationDto>(this.apiUrl);
  }

  updateColors(request: UpdateClinicCustomizationRequest): Observable<ClinicCustomizationDto> {
    return this.http.put<ClinicCustomizationDto>(`${this.apiUrl}/colors`, request);
  }

  updateLogo(logoUrl: string): Observable<ClinicCustomizationDto> {
    return this.http.put<ClinicCustomizationDto>(`${this.apiUrl}/logo`, JSON.stringify(logoUrl), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  updateBackground(backgroundUrl: string): Observable<ClinicCustomizationDto> {
    return this.http.put<ClinicCustomizationDto>(`${this.apiUrl}/background`, JSON.stringify(backgroundUrl), {
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
