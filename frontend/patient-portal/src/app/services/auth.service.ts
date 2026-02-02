import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RefreshTokenRequest,
  ChangePasswordRequest,
  User,
  TwoFactorRequiredResponse,
  VerifyTwoFactorRequest,
  ResendTwoFactorCodeRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'current_user';
  private readonly REMEMBER_ME_KEY = 'remember_me';

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadUserFromStorage();
  }

  private loadUserFromStorage(): void {
    // Try localStorage first (remember me), then sessionStorage
    const userJson = localStorage.getItem(this.USER_KEY) || sessionStorage.getItem(this.USER_KEY);
    if (userJson) {
      try {
        const user = JSON.parse(userJson);
        this.currentUserSubject.next(user);
      } catch {
        this.clearStorage();
      }
    }
  }

  login(request: LoginRequest, rememberMe: boolean = false): Observable<LoginResponse | TwoFactorRequiredResponse> {
    if (rememberMe) {
      localStorage.setItem(this.REMEMBER_ME_KEY, 'true');
    } else {
      localStorage.removeItem(this.REMEMBER_ME_KEY);
    }
    
    return this.http.post<LoginResponse | TwoFactorRequiredResponse>(`${environment.apiUrl}/auth/login`, request)
      .pipe(
        tap(response => {
          // Check if response is 2FA required or login response
          if ('requiresTwoFactor' in response && response.requiresTwoFactor) {
            // Don't store tokens yet, wait for 2FA verification
            return;
          }
          this.handleAuthResponse(response as LoginResponse);
        })
      );
  }

  register(request: RegisterRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/register`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      throw new Error('No refresh token available');
    }

    const request: RefreshTokenRequest = { refreshToken };
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/refresh`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  logout(): Observable<any> {
    const refreshToken = this.getRefreshToken();
    const request: RefreshTokenRequest = { refreshToken: refreshToken || '' };
    
    return this.http.post(`${environment.apiUrl}/auth/logout`, request)
      .pipe(
        tap(() => {
          this.clearStorage();
          this.router.navigate(['/auth/login']);
        })
      );
  }

  changePassword(request: ChangePasswordRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/change-password`, request);
  }

  verifyTwoFactor(request: VerifyTwoFactorRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/auth/verify-2fa`, request)
      .pipe(
        tap(response => this.handleAuthResponse(response))
      );
  }

  resendTwoFactorCode(request: ResendTwoFactorCodeRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/resend-2fa-code`, request);
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/forgot-password`, request);
  }

  resetPassword(request: ResetPasswordRequest): Observable<any> {
    return this.http.post(`${environment.apiUrl}/auth/reset-password`, request);
  }

  isRememberMeEnabled(): boolean {
    return localStorage.getItem(this.REMEMBER_ME_KEY) === 'true';
  }

  private handleAuthResponse(response: LoginResponse): void {
    const rememberMe = this.isRememberMeEnabled();
    
    if (rememberMe) {
      // Store in localStorage for persistence
      localStorage.setItem(this.TOKEN_KEY, response.accessToken);
      localStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
      localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
    } else {
      // Store in sessionStorage for session-only persistence
      sessionStorage.setItem(this.TOKEN_KEY, response.accessToken);
      sessionStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
      sessionStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
    }
    
    this.currentUserSubject.next(response.user);
  }

  private clearStorage(): void {
    // Clear from both localStorage and sessionStorage
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.REMEMBER_ME_KEY);
    sessionStorage.removeItem(this.TOKEN_KEY);
    sessionStorage.removeItem(this.REFRESH_TOKEN_KEY);
    sessionStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY) || sessionStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY) || sessionStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    return this.getAccessToken() !== null;
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }
}
