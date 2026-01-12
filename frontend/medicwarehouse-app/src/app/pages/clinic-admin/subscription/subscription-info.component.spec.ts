import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { SubscriptionInfoComponent } from './subscription-info.component';
import { ClinicAdminService } from '../../../services/clinic-admin.service';
import { SubscriptionDetailsDto } from '../../../models/clinic-admin.model';

describe('SubscriptionInfoComponent', () => {
  let component: SubscriptionInfoComponent;
  let fixture: ComponentFixture<SubscriptionInfoComponent>;
  let clinicAdminService: jasmine.SpyObj<ClinicAdminService>;

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
      hasSMSNotifications: false,
      hasTissExport: true
    },
    createdAt: '2024-01-01T00:00:00Z'
  };

  beforeEach(async () => {
    const clinicAdminServiceSpy = jasmine.createSpyObj('ClinicAdminService', [
      'getSubscriptionDetails',
      'cancelSubscription'
    ]);

    await TestBed.configureTestingModule({
      imports: [
        SubscriptionInfoComponent,
        HttpClientTestingModule
      ],
      providers: [
        { provide: ClinicAdminService, useValue: clinicAdminServiceSpy }
      ]
    }).compileComponents();

    clinicAdminService = TestBed.inject(ClinicAdminService) as jasmine.SpyObj<ClinicAdminService>;
    fixture = TestBed.createComponent(SubscriptionInfoComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Component Initialization', () => {
    it('should load subscription on init', () => {
      clinicAdminService.getSubscriptionDetails.and.returnValue(of(mockSubscription));
      
      component.ngOnInit();
      
      expect(clinicAdminService.getSubscriptionDetails).toHaveBeenCalled();
      expect(component.subscription()).toEqual(mockSubscription);
      expect(component.isLoading()).toBe(false);
    });

    it('should handle error when loading subscription', () => {
      const error = { error: { message: 'Error loading subscription' } };
      clinicAdminService.getSubscriptionDetails.and.returnValue(throwError(() => error));
      
      component.ngOnInit();
      
      expect(component.errorMessage()).toContain('Error loading subscription');
      expect(component.isLoading()).toBe(false);
    });
  });

  describe('Subscription Cancellation', () => {
    beforeEach(() => {
      clinicAdminService.getSubscriptionDetails.and.returnValue(of(mockSubscription));
      component.ngOnInit();
    });

    it('should open cancel confirmation dialog', () => {
      component.requestCancellation();
      
      expect(component.showCancelConfirm()).toBe(true);
    });

    it('should close cancel confirmation dialog', () => {
      component.requestCancellation();
      component.cancelCancellationRequest();
      
      expect(component.showCancelConfirm()).toBe(false);
    });

    it('should cancel subscription successfully', () => {
      clinicAdminService.cancelSubscription.and.returnValue(of({}));
      clinicAdminService.getSubscriptionDetails.and.returnValue(of({
        ...mockSubscription,
        status: 'Cancelled'
      }));

      component.requestCancellation();
      component.confirmCancellation();

      expect(clinicAdminService.cancelSubscription).toHaveBeenCalled();
      expect(component.successMessage()).toContain('cancelamento enviada com sucesso');
      expect(component.showCancelConfirm()).toBe(false);
    });

    it('should handle error when cancelling subscription', () => {
      const error = { error: { message: 'Cannot cancel subscription' } };
      clinicAdminService.cancelSubscription.and.returnValue(throwError(() => error));

      component.requestCancellation();
      component.confirmCancellation();

      expect(component.errorMessage()).toContain('Cannot cancel subscription');
      expect(component.isCancelling()).toBe(false);
    });
  });

  describe('Status Display', () => {
    it('should return correct status class for active', () => {
      expect(component.getStatusClass('Active')).toBe('badge-success');
      expect(component.getStatusClass('ativo')).toBe('badge-success');
    });

    it('should return correct status class for cancelled', () => {
      expect(component.getStatusClass('Cancelled')).toBe('badge-error');
      expect(component.getStatusClass('cancelado')).toBe('badge-error');
    });

    it('should return correct status class for pending', () => {
      expect(component.getStatusClass('Pending')).toBe('badge-warning');
      expect(component.getStatusClass('pendente')).toBe('badge-warning');
    });

    it('should return correct status class for trial', () => {
      expect(component.getStatusClass('Trial')).toBe('badge-info');
    });

    it('should return default class for unknown status', () => {
      expect(component.getStatusClass('Unknown')).toBe('badge-default');
    });

    it('should return correct status text', () => {
      expect(component.getStatusText('active')).toBe('Ativo');
      expect(component.getStatusText('cancelled')).toBe('Cancelado');
      expect(component.getStatusText('pending')).toBe('Pendente');
      expect(component.getStatusText('suspended')).toBe('Suspenso');
      expect(component.getStatusText('trial')).toBe('Trial');
    });

    it('should return original status for unknown status', () => {
      expect(component.getStatusText('Unknown')).toBe('Unknown');
    });
  });

  describe('Usage Calculations', () => {
    beforeEach(() => {
      clinicAdminService.getSubscriptionDetails.and.returnValue(of(mockSubscription));
      component.ngOnInit();
    });

    it('should calculate user usage percentage correctly', () => {
      const percentage = component.getUserUsagePercentage();
      
      expect(percentage).toBe((8 / 15) * 100); // ~53.33%
    });

    it('should return 0 when no subscription', () => {
      component.subscription.set(null);
      
      expect(component.getUserUsagePercentage()).toBe(0);
    });

    it('should return normal usage class for low usage', () => {
      // 8 out of 15 = 53.33%
      expect(component.getUserUsageClass()).toBe('usage-normal');
    });

    it('should return warning usage class for medium usage', () => {
      const highUsageSubscription = {
        ...mockSubscription,
        limits: {
          ...mockSubscription.limits,
          currentUsers: 12, // 12/15 = 80%
          maxUsers: 15
        }
      };
      
      clinicAdminService.getSubscriptionDetails.and.returnValue(of(highUsageSubscription));
      component.ngOnInit();
      
      expect(component.getUserUsageClass()).toBe('usage-warning');
    });

    it('should return critical usage class for high usage', () => {
      const criticalUsageSubscription = {
        ...mockSubscription,
        limits: {
          ...mockSubscription.limits,
          currentUsers: 14, // 14/15 = 93.33%
          maxUsers: 15
        }
      };
      
      clinicAdminService.getSubscriptionDetails.and.returnValue(of(criticalUsageSubscription));
      component.ngOnInit();
      
      expect(component.getUserUsageClass()).toBe('usage-critical');
    });

    it('should handle exactly 90% usage as critical', () => {
      const exactCriticalSubscription = {
        ...mockSubscription,
        limits: {
          ...mockSubscription.limits,
          currentUsers: 90,
          maxUsers: 100
        }
      };
      
      clinicAdminService.getSubscriptionDetails.and.returnValue(of(exactCriticalSubscription));
      component.ngOnInit();
      
      expect(component.getUserUsagePercentage()).toBe(90);
      expect(component.getUserUsageClass()).toBe('usage-critical');
    });

    it('should handle exactly 75% usage as warning', () => {
      const exactWarningSubscription = {
        ...mockSubscription,
        limits: {
          ...mockSubscription.limits,
          currentUsers: 75,
          maxUsers: 100
        }
      };
      
      clinicAdminService.getSubscriptionDetails.and.returnValue(of(exactWarningSubscription));
      component.ngOnInit();
      
      expect(component.getUserUsagePercentage()).toBe(75);
      expect(component.getUserUsageClass()).toBe('usage-warning');
    });
  });

  describe('Feature Display', () => {
    beforeEach(() => {
      clinicAdminService.getSubscriptionDetails.and.returnValue(of(mockSubscription));
      component.ngOnInit();
    });

    it('should display features correctly', () => {
      const sub = component.subscription();
      
      expect(sub?.features.hasReports).toBe(true);
      expect(sub?.features.hasWhatsAppIntegration).toBe(true);
      expect(sub?.features.hasSMSNotifications).toBe(false);
      expect(sub?.features.hasTissExport).toBe(true);
    });

    it('should show all plan details', () => {
      const sub = component.subscription();
      
      expect(sub?.planName).toBe('Premium');
      expect(sub?.planType).toBe('Premium');
      expect(sub?.currentPrice).toBe(299.90);
      expect(sub?.isTrial).toBe(false);
      expect(sub?.isActive).toBe(true);
    });

    it('should show limits correctly', () => {
      const sub = component.subscription();
      
      expect(sub?.limits.maxUsers).toBe(15);
      expect(sub?.limits.maxPatients).toBe(1000);
      expect(sub?.limits.currentUsers).toBe(8);
    });
  });

  describe('Trial Subscription', () => {
    it('should display trial subscription correctly', () => {
      const trialSubscription = {
        ...mockSubscription,
        isTrial: true,
        status: 'Trial'
      };

      clinicAdminService.getSubscriptionDetails.and.returnValue(of(trialSubscription));
      component.ngOnInit();

      expect(component.subscription()?.isTrial).toBe(true);
      expect(component.getStatusClass('Trial')).toBe('badge-info');
    });
  });
});
