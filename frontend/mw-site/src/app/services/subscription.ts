import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { SubscriptionPlan, AVAILABLE_PLANS } from '../models/subscription-plan.model';
import { RegistrationRequest, RegistrationResponse } from '../models/registration.model';
import { ContactRequest, ContactResponse } from '../models/contact.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SubscriptionService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getPlans(): Observable<SubscriptionPlan[]> {
    return this.http.get<SubscriptionPlan[]>(`${this.apiUrl}/registration/plans`).pipe(
      map(plans => plans.map(p => ({
        ...p,
        id: String(p.id) // Convert Guid to string for frontend compatibility
      }))),
      catchError(error => {
        console.error('Error fetching plans from API, using fallback plans', error);
        return of(AVAILABLE_PLANS); // Fallback to hardcoded plans if API fails
      })
    );
  }

  getPlanById(id: string): Observable<SubscriptionPlan | undefined> {
    return this.getPlans().pipe(
      map(plans => plans.find(p => p.id === id))
    );
  }

  register(request: RegistrationRequest): Observable<RegistrationResponse> {
    return this.http.post<RegistrationResponse>(`${this.apiUrl}/registration`, request);
  }

  sendContactMessage(request: ContactRequest): Observable<ContactResponse> {
    return this.http.post<ContactResponse>(`${this.apiUrl}/contact`, request);
  }

  checkCNPJ(cnpj: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/registration/check-cnpj/${cnpj}`);
  }

  checkUsername(username: string): Observable<{ available: boolean }> {
    return this.http.get<{ available: boolean }>(`${this.apiUrl}/registration/check-username/${username}`);
  }
}
