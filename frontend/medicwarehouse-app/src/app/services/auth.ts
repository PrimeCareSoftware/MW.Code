import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, interval } from 'rxjs';
import { Router } from '@angular/router';
import { AuthResponse, LoginRequest, UserInfo, SessionValidationRequest, SessionValidationResponse } from '../models/auth.model';
import { environment } from '../../environments/environment';
import { TenantResolverService } from './tenant-resolver.service';

@Injectable({
  providedIn: 'root'
})
export class Auth {
  private apiUrl = environment.apiUrl;
  private tokenKey = 'auth_token';
  private userKey = 'user_info';
  private sessionCheckInterval = 30000; // Check every 30 seconds
  private sessionCheckSubscription: any;
  
  public isAuthenticated = signal<boolean>(this.hasToken());
  public currentUser = signal<UserInfo | null>(this.getUserInfo());

  constructor(
    private http: HttpClient, 
    private router: Router,
    private tenantResolver: TenantResolverService
  ) { 
    // Start session validation if user is authenticated
    if (this.hasToken()) {
      this.startSessionValidation();
    }
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    // Try to get tenant from URL if not provided in credentials
    if (!credentials.tenantId) {
      const tenantFromUrl = this.tenantResolver.extractTenantFromUrl();
      if (tenantFromUrl) {
        credentials.tenantId = tenantFromUrl;
      }
    }

    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {
          // Ensure clinicId is converted to string if it exists
          const clinicId = response.clinicId ? String(response.clinicId) : undefined;
          
          this.setToken(response.token);
          this.setUserInfo({ 
            username: response.username, 
            tenantId: response.tenantId,
            clinicId: clinicId,
            isOwner: response.isOwner
          });
          this.isAuthenticated.set(true);
          this.currentUser.set({ 
            username: response.username, 
            tenantId: response.tenantId,
            clinicId: clinicId,
            isOwner: response.isOwner
          });
          this.startSessionValidation();
        })
      );
  }

  logout(showMessage: boolean = false): void {
    this.stopSessionValidation();
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
    
    if (showMessage) {
      // Navigate with a state to show the message on the login page
      this.router.navigate(['/login'], { 
        state: { message: 'Sua sessão foi encerrada porque você fez login em outro dispositivo ou navegador.' }
      });
    } else {
      this.router.navigate(['/login']);
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  private hasToken(): boolean {
    return !!this.getToken();
  }

  private setUserInfo(user: UserInfo): void {
    localStorage.setItem(this.userKey, JSON.stringify(user));
  }

  private getUserInfo(): UserInfo | null {
    const userStr = localStorage.getItem(this.userKey);
    return userStr ? JSON.parse(userStr) : null;
  }

  getCurrentUser(): Observable<UserInfo> {
    return this.http.get<UserInfo>(`${this.apiUrl}/auth/me`);
  }

  validateSession(): Observable<SessionValidationResponse> {
    const token = this.getToken();
    if (!token) {
      // No token means not authenticated
      return new Observable(observer => {
        observer.next({ isValid: false, message: 'No token found' });
        observer.complete();
      });
    }

    const request: SessionValidationRequest = { token };
    return this.http.post<SessionValidationResponse>(`${this.apiUrl}/auth/validate-session`, request);
  }

  private startSessionValidation(): void {
    // Stop any existing validation
    this.stopSessionValidation();

    // Start periodic session validation
    this.sessionCheckSubscription = interval(this.sessionCheckInterval).subscribe(() => {
      this.validateSession().subscribe({
        next: (response) => {
          if (!response.isValid) {
            // Session is no longer valid - user logged in elsewhere
            console.warn('Session invalidated:', response.message);
            this.logout(true);
          }
        },
        error: (error) => {
          console.error('Error validating session:', error);
          // Don't logout on network errors, only on explicit session invalidation
        }
      });
    });
  }

  private stopSessionValidation(): void {
    if (this.sessionCheckSubscription) {
      this.sessionCheckSubscription.unsubscribe();
      this.sessionCheckSubscription = null;
    }
  }

  getClinicId(): string | null {
    const user = this.getUserInfo();
    return user?.clinicId || null;
  }
}
