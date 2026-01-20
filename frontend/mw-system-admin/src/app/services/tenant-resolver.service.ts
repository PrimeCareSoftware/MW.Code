import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TenantResolverService {
  constructor(private http: HttpClient) {}

  resolveTenant(): Observable<any> {
    return this.http.get('/api/tenant/info');
  }

  getTenantId(): string {
    return localStorage.getItem('tenantId') || '';
  }

  setTenantId(tenantId: string): void {
    localStorage.setItem('tenantId', tenantId);
  }

  extractTenantFromUrl(): string | null {
    const pathname = window.location.pathname;
    // Extract tenant from URL path like /tenant-name/... or subdomain like tenant-name.example.com
    const parts = pathname.split('/').filter(p => p);
    if (parts.length > 0) {
      // Check if the first part looks like a tenant identifier
      const potential = parts[0];
      if (potential && !potential.startsWith('app') && !potential.startsWith('admin')) {
        return potential;
      }
    }
    
    // Check subdomain
    const hostname = window.location.hostname;
    const subdomainMatch = hostname.match(/^([a-z0-9-]+)\./);
    if (subdomainMatch) {
      return subdomainMatch[1];
    }
    
    return null;
  }
}
