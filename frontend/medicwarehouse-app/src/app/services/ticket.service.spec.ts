import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TicketService } from './ticket.service';
import { ApiConfigService } from './api-config.service';
import { Auth } from './auth';
import { signal } from '@angular/core';

describe('TicketService', () => {
  let service: TicketService;
  let httpMock: HttpTestingController;
  let mockApiConfig: jasmine.SpyObj<ApiConfigService>;

  function createMockAuth(isAuthenticated: boolean): any {
    return {
      isAuthenticated: signal(isAuthenticated),
      login: jasmine.createSpy('login'),
      logout: jasmine.createSpy('logout')
    };
  }

  function setupTestBed(isAuthenticated: boolean): void {
    const mockAuth = createMockAuth(isAuthenticated);
    
    mockApiConfig = jasmine.createSpyObj('ApiConfigService', [], {
      systemAdminUrl: 'http://localhost:5293/api'
    });

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        TicketService,
        { provide: Auth, useValue: mockAuth },
        { provide: ApiConfigService, useValue: mockApiConfig }
      ]
    });

    httpMock = TestBed.inject(HttpTestingController);
    service = TestBed.inject(TicketService);
  }

  afterEach(() => {
    httpMock?.verify();
  });

  it('should be created', () => {
    setupTestBed(false);
    expect(service).toBeTruthy();
  });

  it('should NOT load unread count when user is not authenticated', (done) => {
    setupTestBed(false);
    
    // Wait for setTimeout to execute
    setTimeout(() => {
      // Verify no HTTP requests were made
      httpMock.expectNone('http://localhost:5293/api/tickets/unread-count');
      done();
    }, 10);
  });

  it('should load unread count when user is authenticated', (done) => {
    setupTestBed(true);
    
    // Wait for setTimeout to execute
    setTimeout(() => {
      // Verify HTTP request was made
      const req = httpMock.expectOne('http://localhost:5293/api/tickets/unread-count');
      expect(req.request.method).toBe('GET');
      
      // Respond with mock data
      req.flush({ count: 5 });
      
      // Verify the signal was updated
      expect(service.unreadTicketCount()).toBe(5);
      done();
    }, 10);
  });
});
