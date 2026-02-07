import { Injectable, LOCALE_ID, Inject } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LocaleService {
  constructor(@Inject(LOCALE_ID) private localeId: string) {}

  /**
   * Get the current locale
   */
  getLocale(): string {
    return this.localeId;
  }

  /**
   * Format date according to current locale
   */
  formatDate(date: Date | string, options?: Intl.DateTimeFormatOptions): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const defaultOptions: Intl.DateTimeFormatOptions = {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    };
    return dateObj.toLocaleDateString(this.localeId, options || defaultOptions);
  }

  /**
   * Format date and time according to current locale
   */
  formatDateTime(date: Date | string, options?: Intl.DateTimeFormatOptions): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    const defaultOptions: Intl.DateTimeFormatOptions = {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    };
    return dateObj.toLocaleString(this.localeId, options || defaultOptions);
  }

  /**
   * Format time according to current locale
   */
  formatTime(date: Date | string): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleTimeString(this.localeId, {
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  /**
   * Format number according to current locale
   */
  formatNumber(value: number, options?: Intl.NumberFormatOptions): string {
    return value.toLocaleString(this.localeId, options);
  }

  /**
   * Format currency according to current locale
   */
  formatCurrency(value: number, currency: string = 'BRL'): string {
    return value.toLocaleString(this.localeId, {
      style: 'currency',
      currency: currency
    });
  }
}
