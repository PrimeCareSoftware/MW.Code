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

  login(credentials: LoginRequest, isOwner: boolean = false): Observable<AuthResponse> {
    // Try to get tenant from URL if not provided in credentials
    if (!credentials.tenantId) {
      const tenantFromUrl = this.tenantResolver.extractTenantFromUrl();
      if (tenantFromUrl) {
        credentials.tenantId = tenantFromUrl;
      }
    }

    // Use owner-login endpoint for owners, regular login for users
    const endpoint = isOwner ? '/auth/owner-login' : '/auth/login';

    return this.http.post<AuthResponse>(`${this.apiUrl}${endpoint}`, credentials)
      .pipe(
        tap(response => {
          // Don't store tokens if additional action is required
          if (response.requiresPasswordChange || response.requiresTwoFactor) {
            return;
          }
          // Ensure clinicId is converted to string if it exists
          const clinicId = response.clinicId ? String(response.clinicId) : undefined;
          
          this.setToken(response.token);
          this.setUserInfo({ 
            username: response.username, 
            tenantId: response.tenantId,
            clinicId: clinicId,
            role: response.role,
            isSystemOwner: response.isSystemOwner
          });
          this.isAuthenticated.set(true);
          this.currentUser.set({ 
            username: response.username, 
            tenantId: response.tenantId,
            clinicId: clinicId,
            role: response.role,
            isSystemOwner: response.isSystemOwner
          });
          this.startSessionValidation();
        })
      );
  }

  verify2faEmail(tempToken: string, code: string, isOwner: boolean = false): Observable<AuthResponse> {
    const endpoint = isOwner ? '/auth/owner-verify-2fa-email' : '/auth/verify-2fa-email';
    return this.http.post<AuthResponse>(`${this.apiUrl}${endpoint}`, { tempToken, code })
      .pipe(
        tap(response => {
          const clinicId = response.clinicId ? String(response.clinicId) : undefined;
          this.setToken(response.token);
          this.setUserInfo({
            username: response.username,
            tenantId: response.tenantId,
            clinicId: clinicId,
            role: response.role,
            isSystemOwner: response.isSystemOwner
          });
          this.isAuthenticated.set(true);
          this.currentUser.set({
            username: response.username,
            tenantId: response.tenantId,
            clinicId: clinicId,
            role: response.role,
            isSystemOwner: response.isSystemOwner
          });
          this.startSessionValidation();
        })
      );
  }

  resend2faEmail(tempToken: string, isOwner: boolean = false): Observable<any> {
    const endpoint = isOwner ? '/auth/owner-resend-2fa-email' : '/auth/resend-2fa-email';
    return this.http.post<any>(`${this.apiUrl}${endpoint}`, { tempToken });
  }

  completePasswordChange(tempToken: string, newPassword: string, confirmPassword: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/complete-password-change`, { tempToken, newPassword, confirmPassword })
      .pipe(
        tap(response => {
          const clinicId = response.clinicId ? String(response.clinicId) : undefined;
          this.setToken(response.token);
          this.setUserInfo({
            username: response.username,
            tenantId: response.tenantId,
            clinicId: clinicId,
            role: response.role,
            isSystemOwner: response.isSystemOwner
          });
          this.isAuthenticated.set(true);
          this.currentUser.set({
            username: response.username,
            tenantId: response.tenantId,
            clinicId: clinicId,
            role: response.role,
            isSystemOwner: response.isSystemOwner
          });
          this.startSessionValidation();
        })
      );
  }

  /**
   * Login as owner (clinic owner or system owner)
   */
  ownerLogin(credentials: LoginRequest): Observable<AuthResponse> {
    return this.login(credentials, true);
  }

  logout(showMessage: boolean = false): void {
    const userInfo = this.getUserInfo();
    const isSystemOwner = userInfo?.isSystemOwner || false;
    
    this.stopSessionValidation();
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
    
    // Determine correct login page based on user role
    const loginPath = '/login';
    
    if (showMessage) {
      // Navigate with a state to show the message on the login page
      this.router.navigate([loginPath], { 
        state: { message: 'Sua sessão foi encerrada porque você fez login em outro dispositivo ou navegador.' }
      });
    } else {
      this.router.navigate([loginPath]);
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  hasToken(): boolean {
    return !!this.getToken();
  }

  private setUserInfo(user: UserInfo): void {
    localStorage.setItem(this.userKey, JSON.stringify(user));
  }

  getUserInfo(): UserInfo | null {
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
