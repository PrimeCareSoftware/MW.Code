import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TenantResolverService } from './tenant-resolver.service';

describe('TenantResolverService', () => {
  let service: TenantResolverService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TenantResolverService]
    });
    service = TestBed.inject(TenantResolverService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('extractTenantFromUrl', () => {
    it('should return null for localhost', () => {
      // Mock window.location
      Object.defineProperty(window, 'location', {
        value: {
          hostname: 'localhost',
          pathname: '/login'
        },
        writable: true
      });

      const result = service.extractTenantFromUrl();
      expect(result).toBeNull();
    });

    it('should extract subdomain from multi-level domain', () => {
      Object.defineProperty(window, 'location', {
        value: {
          hostname: 'clinic1.mwsistema.com.br',
          pathname: '/login'
        },
        writable: true
      });

      const result = service.extractTenantFromUrl();
      expect(result).toBe('clinic1');
    });

    it('should return null for www subdomain', () => {
      Object.defineProperty(window, 'location', {
        value: {
          hostname: 'www.mwsistema.com.br',
          pathname: '/login'
        },
        writable: true
      });

      const result = service.extractTenantFromUrl();
      expect(result).toBeNull();
    });

    it('should extract tenant from path when no subdomain', () => {
      Object.defineProperty(window, 'location', {
        value: {
          hostname: 'mwsistema.com.br',
          pathname: '/clinic1/login'
        },
        writable: true
      });

      const result = service.extractTenantFromUrl();
      expect(result).toBe('clinic1');
    });

    it('should not extract common paths as tenant', () => {
      Object.defineProperty(window, 'location', {
        value: {
          hostname: 'mwsistema.com.br',
          pathname: '/login'
        },
        writable: true
      });

      const result = service.extractTenantFromUrl();
      expect(result).toBeNull();
    });

    it('should return null for root path', () => {
      Object.defineProperty(window, 'location', {
        value: {
          hostname: 'mwsistema.com.br',
          pathname: '/'
        },
        writable: true
      });

      const result = service.extractTenantFromUrl();
      expect(result).toBeNull();
    });
  });
});
