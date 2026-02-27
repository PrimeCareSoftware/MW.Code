import { Injectable, signal } from '@angular/core';
import { Subject, Observable } from 'rxjs';

export interface ToastMessage {
  id: string;
  message: string;
  type: 'success' | 'error' | 'info' | 'warning';
  duration: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private toastSubject = new Subject<ToastMessage>();

  public toasts = signal<ToastMessage[]>([]);

  get onToast$(): Observable<ToastMessage> {
    return this.toastSubject.asObservable();
  }

  showToast(message: string, type: 'success' | 'error' | 'info' | 'warning' = 'info', duration: number = 3000): void {
    const toast: ToastMessage = {
      id: crypto.randomUUID(),
      message,
      type,
      duration
    };

    this.toasts.update(toasts => [...toasts, toast]);
    this.toastSubject.next(toast);

    setTimeout(() => {
      this.removeToast(toast.id);
    }, duration);
  }

  removeToast(id: string): void {
    this.toasts.update(toasts => toasts.filter(t => t.id !== id));
  }

  success(message: string): void {
    this.showToast(message, 'success');
  }

  error(message: string): void {
    this.showToast(message, 'error', 5000);
  }

  info(message: string): void {
    this.showToast(message, 'info');
  }

  warning(message: string): void {
    this.showToast(message, 'warning', 4000);
  }
}
