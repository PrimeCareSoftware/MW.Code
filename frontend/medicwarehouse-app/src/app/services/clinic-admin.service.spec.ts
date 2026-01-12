import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ClinicAdminService } from './clinic-admin.service';
import { environment } from '../../environments/environment';
import {
  ClinicUserDto,
  CreateClinicUserRequest,
  UpdateClinicUserRequest,
  ChangeUserPasswordRequest,
  ChangeUserRoleRequest,
  SubscriptionDetailsDto
} from '../models/clinic-admin.model';

describe('ClinicAdminService', () => {
  let service: ClinicAdminService;
  let httpMock: HttpTestingController;
  const apiUrl = `${environment.apiUrl}/ClinicAdmin`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ClinicAdminService]
    });
    service = TestBed.inject(ClinicAdminService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('User Management', () => {
    it('should get clinic users', () => {
      const mockUsers: ClinicUserDto[] = [
        {
          id: '1',
          username: 'test',
          name: 'Test User',
          email: 'test@test.com',
          role: 'Doctor',
          isActive: true,
          createdAt: '2024-01-01T00:00:00Z'
        }
      ];

      service.getClinicUsers().subscribe(users => {
        expect(users).toEqual(mockUsers);
        expect(users.length).toBe(1);
        expect(users[0].username).toBe('test');
      });

      const req = httpMock.expectOne(`${apiUrl}/users`);
      expect(req.request.method).toBe('GET');
      req.flush(mockUsers);
    });

    it('should create a user', () => {
      const createRequest: CreateClinicUserRequest = {
        username: 'newuser',
        email: 'new@test.com',
        password: 'Password123!',
        name: 'New User',
        phone: '1234567890',
        role: 'Nurse'
      };

      const mockResponse: ClinicUserDto = {
        id: '2',
        username: 'newuser',
        name: 'New User',
        email: 'new@test.com',
        role: 'Nurse',
        isActive: true,
        createdAt: '2024-01-01T00:00:00Z'
      };

      service.createUser(createRequest).subscribe(user => {
        expect(user).toEqual(mockResponse);
        expect(user.username).toBe('newuser');
      });

      const req = httpMock.expectOne(`${apiUrl}/users`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(createRequest);
      req.flush(mockResponse);
    });

    it('should update a user', () => {
      const userId = '1';
      const updateRequest: UpdateClinicUserRequest = {
        email: 'updated@test.com',
        name: 'Updated Name',
        phone: '9876543210'
      };

      service.updateUser(userId, updateRequest).subscribe(response => {
        expect(response).toBeTruthy();
      });

      const req = httpMock.expectOne(`${apiUrl}/users/${userId}`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(updateRequest);
      req.flush({ message: 'User updated successfully' });
    });

    it('should change user password', () => {
      const userId = '1';
      const passwordRequest: ChangeUserPasswordRequest = {
        newPassword: 'NewPassword123!'
      };

      service.changeUserPassword(userId, passwordRequest).subscribe(response => {
        expect(response).toBeTruthy();
      });

      const req = httpMock.expectOne(`${apiUrl}/users/${userId}/password`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(passwordRequest);
      req.flush({ message: 'Password changed successfully' });
    });

    it('should change user role', () => {
      const userId = '1';
      const roleRequest: ChangeUserRoleRequest = {
        newRole: 'Admin'
      };

      service.changeUserRole(userId, roleRequest).subscribe(response => {
        expect(response).toBeTruthy();
      });

      const req = httpMock.expectOne(`${apiUrl}/users/${userId}/role`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual(roleRequest);
      req.flush({ message: 'Role changed successfully' });
    });

    it('should activate user', () => {
      const userId = '1';

      service.activateUser(userId).subscribe(response => {
        expect(response).toBeTruthy();
      });

      const req = httpMock.expectOne(`${apiUrl}/users/${userId}/activate`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({});
      req.flush({ message: 'User activated successfully' });
    });

    it('should deactivate user', () => {
      const userId = '1';

      service.deactivateUser(userId).subscribe(response => {
        expect(response).toBeTruthy();
      });

      const req = httpMock.expectOne(`${apiUrl}/users/${userId}/deactivate`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({});
      req.flush({ message: 'User deactivated successfully' });
    });
  });

  describe('Subscription Management', () => {
    it('should get subscription details', () => {
      const mockSubscription: SubscriptionDetailsDto = {
        id: '1',
        planId: 'plan-1',
        planName: 'Premium',
        planType: 'Premium',
        status: 'Active',
        startDate: '2024-01-01T00:00:00Z',
        endDate: '2025-01-01T00:00:00Z',
        nextBillingDate: '2024-02-01T00:00:00Z',
        currentPrice: 299.90,
        isTrial: false,
        isActive: true,
        limits: {
          maxUsers: 15,
          maxPatients: 1000,
          currentUsers: 8
        },
        features: {
          hasReports: true,
          hasWhatsAppIntegration: true,
          hasSMSNotifications: true,
          hasTissExport: true
        },
        createdAt: '2024-01-01T00:00:00Z'
      };

      service.getSubscriptionDetails().subscribe(subscription => {
        expect(subscription).toEqual(mockSubscription);
        expect(subscription.planName).toBe('Premium');
        expect(subscription.limits.maxUsers).toBe(15);
        expect(subscription.features.hasReports).toBe(true);
      });

      const req = httpMock.expectOne(`${apiUrl}/subscription/details`);
      expect(req.request.method).toBe('GET');
      req.flush(mockSubscription);
    });

    it('should cancel subscription', () => {
      service.cancelSubscription().subscribe(response => {
        expect(response).toBeTruthy();
      });

      const req = httpMock.expectOne(`${apiUrl}/subscription/cancel`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual({});
      req.flush({ message: 'Subscription cancelled successfully' });
    });

    it('should get my clinics', () => {
      const mockClinics = [
        {
          clinicId: '1',
          name: 'Clinic 1',
          tradeName: 'C1',
          document: '12345678000190',
          subdomain: 'clinic1',
          tenantId: 'tenant-1',
          isActive: true,
          isPrimaryOwner: true,
          hasActiveSubscription: true,
          subscriptionStatus: 'Active'
        }
      ];

      service.getMyClinics().subscribe(clinics => {
        expect(clinics).toEqual(mockClinics);
        expect(clinics.length).toBe(1);
        expect(clinics[0].name).toBe('Clinic 1');
      });

      const req = httpMock.expectOne(`${apiUrl}/my-clinics`);
      expect(req.request.method).toBe('GET');
      req.flush(mockClinics);
    });
  });

  describe('Error Handling', () => {
    it('should handle 401 unauthorized error', () => {
      const errorMessage = 'Unauthorized';

      service.getClinicUsers().subscribe(
        () => fail('should have failed with 401 error'),
        (error) => {
          expect(error.status).toBe(401);
        }
      );

      const req = httpMock.expectOne(`${apiUrl}/users`);
      req.flush({ message: errorMessage }, { status: 401, statusText: 'Unauthorized' });
    });

    it('should handle 403 forbidden error', () => {
      const errorMessage = 'Access denied';

      service.createUser({} as CreateClinicUserRequest).subscribe(
        () => fail('should have failed with 403 error'),
        (error) => {
          expect(error.status).toBe(403);
        }
      );

      const req = httpMock.expectOne(`${apiUrl}/users`);
      req.flush({ message: errorMessage }, { status: 403, statusText: 'Forbidden' });
    });

    it('should handle 400 bad request error for user limit', () => {
      const errorMessage = 'User limit reached';

      service.createUser({} as CreateClinicUserRequest).subscribe(
        () => fail('should have failed with 400 error'),
        (error) => {
          expect(error.status).toBe(400);
        }
      );

      const req = httpMock.expectOne(`${apiUrl}/users`);
      req.flush({ message: errorMessage }, { status: 400, statusText: 'Bad Request' });
    });
  });
});
