import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthResponse, LoginRequest, UserInfo } from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class Auth {
  private apiUrl = 'http://localhost:5000/api';
  private tokenKey = 'auth_token';
  private userKey = 'user_info';
  
  public isAuthenticated = signal<boolean>(this.hasToken());
  public currentUser = signal<UserInfo | null>(this.getUserInfo());

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {
          this.setToken(response.token);
          this.setUserInfo({ username: response.username, tenantId: response.tenantId });
          this.isAuthenticated.set(true);
          this.currentUser.set({ username: response.username, tenantId: response.tenantId });
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.userKey);
    this.isAuthenticated.set(false);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
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
}
