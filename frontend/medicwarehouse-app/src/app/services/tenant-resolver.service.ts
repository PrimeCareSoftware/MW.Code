import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TenantResolverService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * Extract subdomain from current URL
   * Supports: subdomain.domain.com or domain.com/subdomain
   */
  extractTenantFromUrl(): string | null {
    // Try subdomain first
    const subdomain = this.extractSubdomain();
    if (subdomain) {
      return subdomain;
    }

    // Try path-based
    return this.extractFromPath();
  }

  /**
   * Extract subdomain from hostname
   * Example: clinic1.mwsistema.com.br -> clinic1
   */
  private extractSubdomain(): string | null {
    const host = window.location.hostname;

    // Skip localhost and IP addresses
    if (host === 'localhost' || host.startsWith('127.') || host.startsWith('192.168.')) {
      return null;
    }

    const parts = host.split('.');
    
    // Need at least 3 parts for subdomain (subdomain.domain.com)
    if (parts.length >= 3) {
      const subdomain = parts[0].toLowerCase();
      // Exclude 'www'
      return subdomain === 'www' ? null : subdomain;
    }

    return null;
  }

  /**
   * Extract tenant from path
   * Example: /clinic1/login -> clinic1
   */
  private extractFromPath(): string | null {
    const path = window.location.pathname;
    
    if (!path || path === '/') {
      return null;
    }

    // Extract first path segment
    const segments = path.split('/').filter(s => s.length > 0);
    if (segments.length > 0) {
      const firstSegment = segments[0].toLowerCase();
      
      // Exclude common paths (configurable via environment)
      const excludedPaths = environment.tenant?.excludedPaths || 
        ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets'];
      
      if (!excludedPaths.includes(firstSegment)) {
        return firstSegment;
      }
    }

    return null;
  }

  /**
   * Resolve tenant information by subdomain from the backend
   */
  resolveTenantBySubdomain(subdomain: string): Observable<TenantInfo> {
    return this.http.get<TenantInfo>(`${this.apiUrl}/tenant/resolve/${subdomain}`);
  }

  /**
   * Get current tenant from backend (resolved by middleware)
   */
  getCurrentTenant(): Observable<TenantInfo> {
    return this.http.get<TenantInfo>(`${this.apiUrl}/tenant/current`);
  }
}

export interface TenantInfo {
  tenantId: string;
  subdomain?: string;
  clinicName?: string;
  clinicId?: string;
  isActive: boolean;
}
