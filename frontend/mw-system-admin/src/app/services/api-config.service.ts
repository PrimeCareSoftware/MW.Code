import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';

/**
 * Service to manage API endpoints for both monolithic and microservices architecture.
 * When useMicroservices is enabled, routes requests to appropriate microservices.
 * All microservices share the same JWT token for authentication.
 */
@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  
  /**
   * Gets the base URL for auth-related endpoints
   */
  get authUrl(): string {
    if (environment.useMicroservices && environment.microservices) {
      return environment.microservices.auth;
    }
    return environment.apiUrl;
  }

  /**
   * Gets the base URL for patient-related endpoints
   */
  get patientsUrl(): string {
    if (environment.useMicroservices && environment.microservices) {
      return environment.microservices.patients;
    }
    return environment.apiUrl;
  }

  /**
   * Gets the base URL for appointment-related endpoints
   */
  get appointmentsUrl(): string {
    if (environment.useMicroservices && environment.microservices) {
      return environment.microservices.appointments;
    }
    return environment.apiUrl;
  }

  /**
   * Gets the base URL for medical records-related endpoints
   */
  get medicalRecordsUrl(): string {
    if (environment.useMicroservices && environment.microservices) {
      return environment.microservices.medicalRecords;
    }
    return environment.apiUrl;
  }

  /**
   * Gets the base URL for billing-related endpoints
   */
  get billingUrl(): string {
    if (environment.useMicroservices && environment.microservices) {
      return environment.microservices.billing;
    }
    return environment.apiUrl;
  }

  /**
   * Gets the base URL for system admin-related endpoints
   */
  get systemAdminUrl(): string {
    if (environment.useMicroservices && environment.microservices) {
      return environment.microservices.systemAdmin;
    }
    return environment.apiUrl;
  }

  /**
   * Checks if microservices mode is enabled
   */
  get isMicroservicesMode(): boolean {
    return environment.useMicroservices === true;
  }

  /**
   * Gets the appropriate URL for a given service type
   */
  getServiceUrl(serviceType: 'auth' | 'patients' | 'appointments' | 'medicalRecords' | 'billing' | 'systemAdmin'): string {
    switch (serviceType) {
      case 'auth':
        return this.authUrl;
      case 'patients':
        return this.patientsUrl;
      case 'appointments':
        return this.appointmentsUrl;
      case 'medicalRecords':
        return this.medicalRecordsUrl;
      case 'billing':
        return this.billingUrl;
      case 'systemAdmin':
        return this.systemAdminUrl;
      default:
        return environment.apiUrl;
    }
  }
}
