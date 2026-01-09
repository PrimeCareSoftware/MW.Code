# Frontend Integration Guide - PrimeCare Software API

## Quick Start for Frontend Developers

This guide provides everything you need to integrate your frontend application with the PrimeCare Software API.

## 🔐 Authentication Flow

### Step 1: Login

**For Regular Users** (doctors, secretaries, etc.):
```typescript
const loginUser = async (username: string, password: string, tenantId: string) => {
  const response = await fetch('http://localhost:5000/api/auth/login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      username,
      password,
      tenantId
    })
  });

  if (!response.ok) {
    throw new Error('Login failed');
  }

  const data = await response.json();
  // Store token securely (sessionStorage or memory, not localStorage)
  sessionStorage.setItem('authToken', data.token);
  sessionStorage.setItem('user', JSON.stringify(data));
  
  return data;
  // Returns: { token, username, tenantId, role, clinicId, isSystemOwner, expiresAt }
};
```

**For Owners** (clinic owners and system administrators):
```typescript
const loginOwner = async (username: string, password: string, tenantId: string) => {
  const response = await fetch('http://localhost:5000/api/auth/owner-login', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      username,
      password,
      tenantId
    })
  });

  if (!response.ok) {
    throw new Error('Login failed');
  }

  const data = await response.json();
  sessionStorage.setItem('authToken', data.token);
  sessionStorage.setItem('user', JSON.stringify(data));
  
  return data;
};
```

### Step 2: Making Authenticated Requests

```typescript
const fetchPatients = async () => {
  const token = sessionStorage.getItem('authToken');
  
  const response = await fetch('http://localhost:5000/api/patients', {
    method: 'GET',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    }
  });

  if (response.status === 401) {
    // Token expired or invalid - redirect to login
    window.location.href = '/login';
    return;
  }

  return await response.json();
};
```

### Step 3: Token Validation

```typescript
const validateToken = async (token: string): Promise<boolean> => {
  try {
    const response = await fetch('http://localhost:5000/api/auth/validate', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ token })
    });

    const data = await response.json();
    return data.isValid;
  } catch {
    return false;
  }
};
```

---

## 🎯 Angular Integration Example

### 1. Auth Service

```typescript
// auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';

interface LoginResponse {
  token: string;
  username: string;
  tenantId: string;
  role: string;
  clinicId?: string;
  isSystemOwner: boolean;
  expiresAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api';
  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Load user from session storage on service init
    const userJson = sessionStorage.getItem('user');
    if (userJson) {
      this.currentUserSubject.next(JSON.parse(userJson));
    }
  }

  login(username: string, password: string, tenantId: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/login`, {
      username,
      password,
      tenantId
    }).pipe(
      tap(response => {
        sessionStorage.setItem('authToken', response.token);
        sessionStorage.setItem('user', JSON.stringify(response));
        this.currentUserSubject.next(response);
      })
    );
  }

  ownerLogin(username: string, password: string, tenantId: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/auth/owner-login`, {
      username,
      password,
      tenantId
    }).pipe(
      tap(response => {
        sessionStorage.setItem('authToken', response.token);
        sessionStorage.setItem('user', JSON.stringify(response));
        this.currentUserSubject.next(response);
      })
    );
  }

  logout(): void {
    sessionStorage.removeItem('authToken');
    sessionStorage.removeItem('user');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return sessionStorage.getItem('authToken');
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;

    // Check if token is expired
    const user = this.currentUserSubject.value;
    if (user && new Date(user.expiresAt) < new Date()) {
      this.logout();
      return false;
    }

    return true;
  }

  isSystemOwner(): boolean {
    const user = this.currentUserSubject.value;
    return user?.isSystemOwner || false;
  }

  hasRole(role: string): boolean {
    const user = this.currentUserSubject.value;
    return user?.role === role;
  }
}
```

### 2. HTTP Interceptor

```typescript
// auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();

    if (token) {
      // Clone the request and add the Authorization header
      const clonedRequest = req.clone({
        headers: req.headers.set('Authorization', `Bearer ${token}`)
      });
      return next.handle(clonedRequest);
    }

    return next.handle(req);
  }
}
```

### 3. Auth Guard

```typescript
// auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return false;
    }

    // Check role-based access if specified in route data
    const requiredRole = route.data['role'];
    if (requiredRole && !this.authService.hasRole(requiredRole)) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    // Check if system owner access is required
    const requireSystemOwner = route.data['requireSystemOwner'];
    if (requireSystemOwner && !this.authService.isSystemOwner()) {
      this.router.navigate(['/unauthorized']);
      return false;
    }

    return true;
  }
}
```

### 4. App Module Configuration

```typescript
// app.module.ts
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';

@NgModule({
  // ...
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ]
})
export class AppModule { }
```

### 5. Route Configuration

```typescript
// app-routing.module.ts
import { AuthGuard } from './auth.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'admin',
    component: AdminComponent,
    canActivate: [AuthGuard],
    data: { requireSystemOwner: true }
  },
  {
    path: 'medical-records',
    component: MedicalRecordsComponent,
    canActivate: [AuthGuard],
    data: { role: 'Doctor' }
  }
];
```

---

## 🔑 Environment Configuration

### Development
```typescript
// environment.ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  jwtExpiration: 60 * 60 * 1000 // 60 minutes in milliseconds
};
```

### Production
```typescript
// environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.medicwarehouse.com/api',
  jwtExpiration: 60 * 60 * 1000
};
```

---

## 📝 Common Patterns

### Handling Token Expiration

```typescript
// Use a service to check token expiration periodically
@Injectable({ providedIn: 'root' })
export class TokenExpirationService {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  startMonitoring(): void {
    // Check every minute
    setInterval(() => {
      if (!this.authService.isLoggedIn()) {
        this.router.navigate(['/login'], {
          queryParams: { returnUrl: this.router.url, reason: 'expired' }
        });
      }
    }, 60000);
  }
}
```

### Displaying User Info

```typescript
// In your component
export class HeaderComponent implements OnInit {
  currentUser$ = this.authService.currentUser$;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.currentUser$.subscribe(user => {
      console.log('Current user:', user);
    });
  }

  logout(): void {
    this.authService.logout();
    // Redirect to login
  }
}
```

```html
<!-- In your template -->
<div *ngIf="currentUser$ | async as user">
  <p>Welcome, {{ user.username }}!</p>
  <p>Role: {{ user.role }}</p>
  <p *ngIf="user.isSystemOwner" class="badge">System Owner</p>
  <button (click)="logout()">Logout</button>
</div>
```

---

## 🚨 Error Handling

```typescript
// Global error handler
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  handleError(error: any): void {
    if (error?.status === 401) {
      // Unauthorized - token expired or invalid
      this.authService.logout();
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: this.router.url }
      });
    } else if (error?.status === 403) {
      // Forbidden - user doesn't have permission
      this.router.navigate(['/unauthorized']);
    } else {
      // Other errors
      console.error('An error occurred:', error);
    }
  }
}
```

---

## 🎨 UI Components Example

### Login Component

```typescript
// login.component.ts
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})
export class LoginComponent {
  loginForm: FormGroup;
  loginType: 'user' | 'owner' = 'user';
  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
      tenantId: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.error = '';

    const { username, password, tenantId } = this.loginForm.value;

    const login$ = this.loginType === 'owner' 
      ? this.authService.ownerLogin(username, password, tenantId)
      : this.authService.login(username, password, tenantId);

    login$.subscribe({
      next: () => {
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Login failed. Please try again.';
      }
    });
  }
}
```

```html
<!-- login.component.html -->
<div class="login-container">
  <h2>PrimeCare Software Login</h2>
  
  <div class="login-type-toggle">
    <button 
      [class.active]="loginType === 'user'"
      (click)="loginType = 'user'">
      User Login
    </button>
    <button 
      [class.active]="loginType === 'owner'"
      (click)="loginType = 'owner'">
      Owner Login
    </button>
  </div>

  <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
    <div class="form-group">
      <label for="username">Username</label>
      <input 
        id="username" 
        type="text" 
        formControlName="username"
        placeholder="Enter username">
    </div>

    <div class="form-group">
      <label for="password">Password</label>
      <input 
        id="password" 
        type="password" 
        formControlName="password"
        placeholder="Enter password">
    </div>

    <div class="form-group">
      <label for="tenantId">Tenant ID</label>
      <input 
        id="tenantId" 
        type="text" 
        formControlName="tenantId"
        placeholder="Enter tenant ID">
    </div>

    <div *ngIf="error" class="error-message">
      {{ error }}
    </div>

    <button 
      type="submit" 
      [disabled]="loading || loginForm.invalid">
      {{ loading ? 'Logging in...' : 'Login' }}
    </button>
  </form>
</div>
```

---

## 📚 Additional Resources

- **Full Authentication Guide**: [`AUTHENTICATION_GUIDE.md`](../AUTHENTICATION_GUIDE.md)
- **API Documentation**: Available at `/swagger` when running the API
- **Security Guide**: [`frontend/mw-docs/src/assets/docs/SECURITY_GUIDE.md`](../frontend/mw-docs/src/assets/docs/SECURITY_GUIDE.md)

---

## 🐛 Troubleshooting

### CORS Issues
If you encounter CORS errors, make sure the backend is configured with your frontend URL in `appsettings.json`:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:4200"
    ]
  }
}
```

### 401 Unauthorized
- Check if token is included in the Authorization header
- Verify token hasn't expired
- Ensure token format is `Bearer <token>`

### Token Not Persisting
- Don't use localStorage for tokens (security risk)
- Use sessionStorage or in-memory storage
- Token is cleared on browser close (by design)

---

## ✅ Checklist for Frontend Integration

- [ ] Implement AuthService with login methods
- [ ] Add HTTP Interceptor for automatic token injection
- [ ] Create AuthGuard for route protection
- [ ] Handle 401 errors globally
- [ ] Implement token expiration monitoring
- [ ] Add logout functionality
- [ ] Test with both user and owner login
- [ ] Handle role-based access control
- [ ] Store tokens securely (sessionStorage, not localStorage)
- [ ] Display user info in UI
- [ ] Add error messages for failed login
- [ ] Test token expiration flow
- [ ] Document environment configuration

---

## 🤝 Support

For issues or questions:
- Email: contato@primecaresoftware.com
- GitHub: https://github.com/PrimeCare Software/MW.Code
