import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
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

  getPlans(): SubscriptionPlan[] {
    return AVAILABLE_PLANS;
  }

  getPlanById(id: string): SubscriptionPlan | undefined {
    return AVAILABLE_PLANS.find(p => p.id === id);
  }

  register(request: RegistrationRequest): Observable<RegistrationResponse> {
    return this.http.post<RegistrationResponse>(`${this.apiUrl}/api/registration`, request);
  }

  sendContactMessage(request: ContactRequest): Observable<ContactResponse> {
    return this.http.post<ContactResponse>(`${this.apiUrl}/api/contact`, request);
  }

  checkCNPJ(cnpj: string): Observable<{ exists: boolean }> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/api/registration/check-cnpj/${cnpj}`);
  }

  checkUsername(username: string): Observable<{ available: boolean }> {
    return this.http.get<{ available: boolean }>(`${this.apiUrl}/api/registration/check-username/${username}`);
  }
}
