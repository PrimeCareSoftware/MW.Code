import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuditService, AuditFilter } from './audit.service';

describe('AuditService', () => {
  let service: AuditService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuditService]
    });

    httpMock = TestBed.inject(HttpTestingController);
    service = TestBed.inject(AuditService);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should construct correct URL for queryAuditLogs without duplicate /api', () => {
    const filter: AuditFilter = {
      pageNumber: 1,
      pageSize: 50
    };

    service.queryAuditLogs(filter).subscribe();

    // Verify the URL does NOT have duplicate /api
    const req = httpMock.expectOne('http://localhost:5293/api/audit/query');
    expect(req.request.method).toBe('POST');
    expect(req.request.url).not.toContain('/api/api/');
    
    req.flush({ data: [], totalCount: 0, pageNumber: 1, pageSize: 50 });
  });

  it('should construct correct URL for getUserActivity', () => {
    const userId = 'user123';

    service.getUserActivity(userId).subscribe();

    const req = httpMock.expectOne('http://localhost:5293/api/audit/user/user123');
    expect(req.request.method).toBe('GET');
    expect(req.request.url).not.toContain('/api/api/');
    
    req.flush([]);
  });

  it('should construct correct URL for getEntityHistory', () => {
    const entityType = 'Patient';
    const entityId = 'patient123';

    service.getEntityHistory(entityType, entityId).subscribe();

    const req = httpMock.expectOne('http://localhost:5293/api/audit/entity/Patient/patient123');
    expect(req.request.method).toBe('GET');
    expect(req.request.url).not.toContain('/api/api/');
    
    req.flush([]);
  });

  it('should construct correct URL for getSecurityEvents', () => {
    service.getSecurityEvents().subscribe();

    const req = httpMock.expectOne('http://localhost:5293/api/audit/security-events');
    expect(req.request.method).toBe('GET');
    expect(req.request.url).not.toContain('/api/api/');
    
    req.flush([]);
  });

  it('should construct correct URL for getLgpdReport', () => {
    const userId = 'user123';

    service.getLgpdReport(userId).subscribe();

    const req = httpMock.expectOne('http://localhost:5293/api/audit/lgpd-report/user123');
    expect(req.request.method).toBe('GET');
    expect(req.request.url).not.toContain('/api/api/');
    
    req.flush({
      userId: 'user123',
      userName: 'Test User',
      generatedAt: '2024-01-01T00:00:00Z',
      totalAccesses: 0,
      dataModifications: 0,
      dataExports: 0,
      recentActivity: []
    });
  });
});
