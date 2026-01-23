import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { UserClinicDto, SwitchClinicResponse } from '../models/clinic.model';

@Injectable({
  providedIn: 'root'
})
export class ClinicSelectionService {
  private apiUrl = environment.apiUrl;
  
  // Signal to track current clinic
  public currentClinic = signal<UserClinicDto | null>(null);
  public availableClinics = signal<UserClinicDto[]>([]);

  constructor(private http: HttpClient) {}

  /**
   * Get list of clinics the current user can access
   */
  getUserClinics(): Observable<UserClinicDto[]> {
    return this.http.get<UserClinicDto[]>(`${this.apiUrl}/users/clinics`)
      .pipe(
        tap(clinics => {
          this.availableClinics.set(clinics);
          
          // If we have clinics and don't have a current clinic, set the preferred one
          if (clinics.length > 0 && !this.currentClinic()) {
            const preferred = clinics.find(c => c.isPreferred) || clinics[0];
            this.currentClinic.set(preferred);
          }
        })
      );
  }

  /**
   * Get the current clinic the user is working in
   */
  getCurrentClinic(): Observable<UserClinicDto> {
    return this.http.get<UserClinicDto>(`${this.apiUrl}/users/current-clinic`)
      .pipe(
        tap(clinic => {
          this.currentClinic.set(clinic);
        })
      );
  }

  /**
   * Switch to a different clinic
   */
  selectClinic(clinicId: string): Observable<SwitchClinicResponse> {
    return this.http.post<SwitchClinicResponse>(`${this.apiUrl}/users/select-clinic/${clinicId}`, {})
      .pipe(
        tap(response => {
          if (response.success && response.currentClinicId) {
            // Update current clinic in the signal
            const clinic = this.availableClinics().find(c => c.clinicId === response.currentClinicId);
            if (clinic) {
              this.currentClinic.set(clinic);
            }
          }
        })
      );
  }

  /**
   * Check if user has access to multiple clinics
   */
  hasMultipleClinics(): boolean {
    return this.availableClinics().length > 1;
  }

  /**
   * Clear clinic data (e.g., on logout)
   */
  clearClinicData(): void {
    this.currentClinic.set(null);
    this.availableClinics.set([]);
  }
}
