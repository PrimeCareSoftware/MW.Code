import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProfileService, UserProfile, UpdateProfileRequest } from './profile.service';

describe('ProfileService', () => {
  let service: ProfileService;
  let httpMock: HttpTestingController;

  const mockProfile: UserProfile = {
    id: '123e4567-e89b-12d3-a456-426614174000',
    email: 'john.doe@example.com',
    fullName: 'John Doe',
    cpf: '12345678901',
    phoneNumber: '+5511987654321',
    dateOfBirth: new Date('1990-01-15'),
    emailConfirmed: true,
    phoneConfirmed: false,
    twoFactorEnabled: false,
    lastLoginAt: new Date('2026-01-14T10:00:00Z'),
    createdAt: new Date('2025-12-01T08:00:00Z')
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProfileService]
    });

    service = TestBed.inject(ProfileService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getProfile', () => {
    it('should retrieve user profile', (done) => {
      service.getProfile().subscribe(profile => {
        expect(profile).toEqual(mockProfile);
        expect(profile.email).toBe('john.doe@example.com');
        expect(profile.fullName).toBe('John Doe');
        expect(profile.emailConfirmed).toBe(true);
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      expect(req.request.method).toBe('GET');
      req.flush(mockProfile);
    });

    it('should handle 401 when not authenticated', (done) => {
      service.getProfile().subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(401);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      req.flush({ message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });
    });

    it('should handle 404 when profile not found', (done) => {
      service.getProfile().subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(404);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      req.flush({ message: 'User not found' }, { status: 404, statusText: 'Not Found' });
    });
  });

  describe('updateProfile', () => {
    it('should update full name successfully', (done) => {
      const updateRequest: UpdateProfileRequest = {
        fullName: 'John Doe Santos'
      };

      service.updateProfile(updateRequest).subscribe(response => {
        expect(response.message).toBe('Profile updated successfully');
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updateRequest);
      req.flush({ message: 'Profile updated successfully' });
    });

    it('should update phone number successfully', (done) => {
      const updateRequest: UpdateProfileRequest = {
        phoneNumber: '+5511999887766'
      };

      service.updateProfile(updateRequest).subscribe(response => {
        expect(response.message).toBe('Profile updated successfully');
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updateRequest);
      req.flush({ message: 'Profile updated successfully' });
    });

    it('should update both full name and phone number', (done) => {
      const updateRequest: UpdateProfileRequest = {
        fullName: 'John Doe Santos',
        phoneNumber: '+5511999887766'
      };

      service.updateProfile(updateRequest).subscribe(response => {
        expect(response.message).toBe('Profile updated successfully');
        done();
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updateRequest);
      req.flush({ message: 'Profile updated successfully' });
    });

    it('should handle 401 when not authenticated', (done) => {
      const updateRequest: UpdateProfileRequest = {
        fullName: 'John Doe Santos'
      };

      service.updateProfile(updateRequest).subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(401);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      req.flush({ message: 'Unauthorized' }, { status: 401, statusText: 'Unauthorized' });
    });

    it('should handle validation errors', (done) => {
      const updateRequest: UpdateProfileRequest = {
        fullName: ''
      };

      service.updateProfile(updateRequest).subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(400);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      req.flush(
        { message: 'Validation failed', errors: { fullName: 'Full name cannot be empty' } },
        { status: 400, statusText: 'Bad Request' }
      );
    });
  });

  describe('error handling', () => {
    it('should handle network errors on getProfile', (done) => {
      service.getProfile().subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(0);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      req.error(new ProgressEvent('error'), { status: 0, statusText: 'Network error' });
    });

    it('should handle server errors on updateProfile', (done) => {
      const updateRequest: UpdateProfileRequest = {
        fullName: 'John Doe Santos'
      };

      service.updateProfile(updateRequest).subscribe({
        next: () => fail('should have failed'),
        error: (error) => {
          expect(error.status).toBe(500);
          done();
        }
      });

      const req = httpMock.expectOne('http://localhost:5000/api/profile/me');
      req.flush({ message: 'Internal server error' }, { status: 500, statusText: 'Internal Server Error' });
    });
  });
});
