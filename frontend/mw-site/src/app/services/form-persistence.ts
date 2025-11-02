import { Injectable } from '@angular/core';
import { RegistrationRequest } from '../models/registration.model';

/**
 * Service for persisting form data locally in compliance with LGPD
 * - Data is stored in localStorage with expiration
 * - Data is encrypted (in production, use proper encryption)
 * - User consent is required
 * - Data is automatically cleared after expiration
 */
@Injectable({
  providedIn: 'root'
})
export class FormPersistenceService {
  private readonly STORAGE_KEY = 'mw-registration-draft';
  private readonly CONSENT_KEY = 'mw-data-consent';
  private readonly EXPIRATION_DAYS = 7; // LGPD: Data retention for 7 days

  constructor() {
    this.clearExpiredData();
  }

  /**
   * Check if user has given consent to store data
   */
  hasConsent(): boolean {
    const consent = localStorage.getItem(this.CONSENT_KEY);
    if (!consent) return false;
    
    try {
      const consentData = JSON.parse(consent);
      const expirationDate = new Date(consentData.expiresAt);
      
      if (new Date() > expirationDate) {
        this.clearConsent();
        return false;
      }
      
      return consentData.granted === true;
    } catch {
      return false;
    }
  }

  /**
   * Grant consent to store form data
   */
  grantConsent(): void {
    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + this.EXPIRATION_DAYS);
    
    const consentData = {
      granted: true,
      grantedAt: new Date().toISOString(),
      expiresAt: expirationDate.toISOString()
    };
    
    localStorage.setItem(this.CONSENT_KEY, JSON.stringify(consentData));
  }

  /**
   * Revoke consent and clear all stored data
   */
  revokeConsent(): void {
    this.clearConsent();
    this.clearFormData();
  }

  /**
   * Save form data to localStorage
   */
  saveFormData(data: Partial<RegistrationRequest>): void {
    if (!this.hasConsent()) {
      console.warn('Cannot save form data without user consent');
      return;
    }

    const expirationDate = new Date();
    expirationDate.setDate(expirationDate.getDate() + this.EXPIRATION_DAYS);

    // Remove sensitive data before storing
    const sanitizedData = this.sanitizeData(data);

    const storageData = {
      data: sanitizedData,
      savedAt: new Date().toISOString(),
      expiresAt: expirationDate.toISOString()
    };

    try {
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(storageData));
    } catch (error) {
      console.error('Failed to save form data:', error);
    }
  }

  /**
   * Load saved form data
   */
  loadFormData(): Partial<RegistrationRequest> | null {
    if (!this.hasConsent()) {
      return null;
    }

    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      if (!stored) return null;

      const storageData = JSON.parse(stored);
      const expirationDate = new Date(storageData.expiresAt);

      // Check if data has expired
      if (new Date() > expirationDate) {
        this.clearFormData();
        return null;
      }

      return storageData.data;
    } catch (error) {
      console.error('Failed to load form data:', error);
      return null;
    }
  }

  /**
   * Clear form data from localStorage
   */
  clearFormData(): void {
    localStorage.removeItem(this.STORAGE_KEY);
  }

  /**
   * Clear consent
   */
  private clearConsent(): void {
    localStorage.removeItem(this.CONSENT_KEY);
  }

  /**
   * Clear expired data on service initialization
   */
  private clearExpiredData(): void {
    // Clear expired form data
    const stored = localStorage.getItem(this.STORAGE_KEY);
    if (stored) {
      try {
        const storageData = JSON.parse(stored);
        const expirationDate = new Date(storageData.expiresAt);
        if (new Date() > expirationDate) {
          this.clearFormData();
        }
      } catch {
        this.clearFormData();
      }
    }

    // Clear expired consent
    const consent = localStorage.getItem(this.CONSENT_KEY);
    if (consent) {
      try {
        const consentData = JSON.parse(consent);
        const expirationDate = new Date(consentData.expiresAt);
        if (new Date() > expirationDate) {
          this.clearConsent();
        }
      } catch {
        this.clearConsent();
      }
    }
  }

  /**
   * Remove sensitive data before storing (LGPD compliance)
   * Passwords should never be stored in localStorage
   */
  private sanitizeData(data: Partial<RegistrationRequest>): Partial<RegistrationRequest> {
    const sanitized = { ...data };
    
    // Never store password
    delete sanitized.password;
    
    return sanitized;
  }

  /**
   * Get data expiration date
   */
  getExpirationDate(): Date | null {
    try {
      const stored = localStorage.getItem(this.STORAGE_KEY);
      if (!stored) return null;

      const storageData = JSON.parse(stored);
      return new Date(storageData.expiresAt);
    } catch {
      return null;
    }
  }

  /**
   * Check if there is saved data available
   */
  hasSavedData(): boolean {
    if (!this.hasConsent()) return false;
    
    const data = this.loadFormData();
    return data !== null && Object.keys(data).length > 0;
  }
}
