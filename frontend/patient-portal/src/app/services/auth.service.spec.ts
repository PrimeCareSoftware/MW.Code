import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { LoginRequest, LoginResponse, RegisterRequest, ChangePasswordRequest, User } from '../models/auth.model';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerSpy: jasmine.SpyObj<Router>;

  const mockUser: User = {
    id: '123e4567-e89b-12d3-a456-426614174000',
    email: 'test@example.com',
    fullName: 'Test User',
    cpf: '12345678901',
    phoneNumber: '+5511987654321',
    dateOfBirth: new Date('1990-01-15'),
    twoFactorEnabled: false
  };

  const mockLoginResponse: LoginResponse = {
    accessToken: 'mock-access-token',
    refreshToken: 'mock-refresh-token',
    expiresAt: new Date(Date.now() + 900000),
    user: mockUser
  };

  beforeEach(() => {
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: Router, useValue: routerSpy }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    
    // Clear localStorage before each test
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should login successfully with email', (done) => {
      const loginRequest: LoginRequest = {
        emailOrCPF: 'test@example.com',
        password: 'Password123!'
      };

      service.login(loginRequest).subscribe(response => {
        expect(response).toEqual(mockLoginResponse);
        expect(service.getAccessToken()).toBe(mockLoginResponse.accessToken);
        expect(service.getRefreshToken()).toBe(mockLoginResponse.refreshToken);
        expect(service.isAuthenticated()).toBe(true);
        expect(service.getCurrentUser()).toEqual(mockUser);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/auth/login');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(loginRequest);
      req.flush(mockLoginResponse);
    });

    it('should handle login failure', (done) => {
      const loginRequest: LoginRequest = {
        emailOrCPF: 'test@example.com',
        password: 'wrong-password'
      };

      service.login(loginRequest).subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(401);
          expect(service.isAuthenticated()).toBe(false);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/auth/login');
      req.flush({ message: 'Invalid credentials' }, { status: 401, statusText: 'Unauthorized' });
    });
  });

  describe('register', () => {
    it('should register successfully', (done) => {
      const registerRequest: RegisterRequest = {
        email: 'newuser@example.com',
        password: 'Password123!',
        confirmPassword: 'Password123!',
        fullName: 'New User',
        cpf: '98765432109',
        phoneNumber: '+5511987654321',
        dateOfBirth: new Date('1990-01-15')
      };

      service.register(registerRequest).subscribe(response => {
        expect(response).toEqual(mockLoginResponse);
        expect(service.isAuthenticated()).toBe(true);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/auth/register');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(registerRequest);
      req.flush(mockLoginResponse);
    });
  });

  describe('refreshToken', () => {
    it('should refresh token successfully', (done) => {
      // Set initial refresh token
      localStorage.setItem('refresh_token', 'old-refresh-token');

      const newLoginResponse: LoginResponse = {
        ...mockLoginResponse,
        accessToken: 'new-access-token',
        refreshToken: 'new-refresh-token'
      };

      service.refreshToken().subscribe(response => {
        expect(response).toEqual(newLoginResponse);
        expect(service.getAccessToken()).toBe('new-access-token');
        expect(service.getRefreshToken()).toBe('new-refresh-token');
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/auth/refresh');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ refreshToken: 'old-refresh-token' });
      req.flush(newLoginResponse);
    });

    it('should throw error when no refresh token available', () => {
      expect(() => service.refreshToken()).toThrowError('No refresh token available');
    });
  });

  describe('logout', () => {
    beforeEach(() => {
      localStorage.setItem('access_token', 'some-token');
      localStorage.setItem('refresh_token', 'some-refresh-token');
      localStorage.setItem('current_user', JSON.stringify(mockUser));
    });

    it('should logout successfully', (done) => {
      service.logout().subscribe(() => {
        expect(service.getAccessToken()).toBeNull();
        expect(service.getRefreshToken()).toBeNull();
        expect(service.getCurrentUser()).toBeNull();
        expect(service.isAuthenticated()).toBe(false);
        expect(routerSpy.navigate).toHaveBeenCalledWith(['/auth/login']);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/auth/logout');
      expect(req.request.method).toBe('POST');
      req.flush({});
    });
  });

  describe('changePassword', () => {
    it('should change password successfully', (done) => {
      const changePasswordRequest: ChangePasswordRequest = {
        currentPassword: 'OldPassword123!',
        newPassword: 'NewPassword123!'
      };

      service.changePassword(changePasswordRequest).subscribe(response => {
        expect(response).toEqual({ message: 'Password changed successfully' });
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/auth/change-password');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(changePasswordRequest);
      req.flush({ message: 'Password changed successfully' });
    });
  });

  describe('isAuthenticated', () => {
    it('should return false when no token', () => {
      expect(service.isAuthenticated()).toBe(false);
    });

    it('should return true when token exists', () => {
      localStorage.setItem('access_token', 'some-token');
      expect(service.isAuthenticated()).toBe(true);
    });
  });

  describe('getCurrentUser', () => {
    it('should return null when no user', () => {
      expect(service.getCurrentUser()).toBeNull();
    });

    it('should return user when stored', () => {
      const userJson = JSON.stringify({
        ...mockUser,
        dateOfBirth: mockUser.dateOfBirth.toISOString()
      });
      localStorage.setItem('current_user', userJson);
      service['loadUserFromStorage']();
      const currentUser = service.getCurrentUser();
      expect(currentUser).toBeTruthy();
      expect(currentUser?.email).toBe(mockUser.email);
    });
  });

  describe('currentUser$ observable', () => {
    it('should emit current user changes', (done) => {
      service.currentUser$.subscribe(user => {
        if (user) {
          expect(user).toEqual(mockUser);
          done();
        }
      });

      service.login({ emailOrCPF: 'test@example.com', password: 'Password123!' }).subscribe();

      const req = httpMock.expectOne('http://localhost:5000/api/auth/login');
      req.flush(mockLoginResponse);
    });
  });
});
