import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  private apiUrl = '/api';
  systemAdminUrl = '/api';

  getApiUrl(): string {
    return this.apiUrl;
  }

  setApiUrl(url: string): void {
    this.apiUrl = url;
  }

  getEndpoint(endpoint: string): string {
    return `${this.apiUrl}/${endpoint}`;
  }
}
