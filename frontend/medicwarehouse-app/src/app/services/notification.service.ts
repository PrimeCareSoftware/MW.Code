import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { tap } from 'rxjs/operators';
import type { Notification, AppointmentCompletedNotification, CallNextPatientNotification } from '../models/notification.model';
import { NotificationType } from '../models/notification.model';
import { environment } from '../../environments/environment';

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
  private apiUrl = `${environment.apiUrl}/notifications`;
  private notificationSubject = new Subject<Notification>();
  private toastSubject = new Subject<ToastMessage>();
  
  public notifications = signal<Notification[]>([]);
  public unreadCount = signal<number>(0);
  public toasts = signal<ToastMessage[]>([]);

  constructor(private http: HttpClient) {
    // Don't load notifications in constructor to avoid 401 errors on public pages
    // Notifications will be loaded when explicitly requested by authenticated components
  }

  // Observable for real-time notifications
  get onNotification$(): Observable<Notification> {
    return this.notificationSubject.asObservable();
  }

  // Observable for toast messages
  get onToast$(): Observable<ToastMessage> {
    return this.toastSubject.asObservable();
  }

  // Show toast message
  showToast(message: string, type: 'success' | 'error' | 'info' | 'warning' = 'info', duration: number = 3000): void {
    const toast: ToastMessage = {
      id: crypto.randomUUID(),
      message,
      type,
      duration
    };
    
    this.toasts.update(toasts => [...toasts, toast]);
    this.toastSubject.next(toast);
    
    // Auto-remove toast after duration
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

  loadNotifications(): void {
    // Check if user has a token before making the request
    // We check localStorage directly to avoid circular dependency with Auth service
    // (error interceptor -> NotificationService -> Auth service -> error interceptor)
    const token = localStorage.getItem('auth_token');
    if (!token) {
      // User is not authenticated, skip loading notifications
      return;
    }

    this.http.get<Notification[]>(this.apiUrl).subscribe({
      next: (notifications) => {
        this.notifications.set(notifications);
        this.updateUnreadCount();
      },
      error: (error) => {
        // Only log error if it's not a 401 (unauthorized)
        if (error.status !== 401) {
          console.error('Error loading notifications:', error);
        }
      }
    });
  }

  notifyAppointmentCompleted(data: AppointmentCompletedNotification): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/appointment-completed`, data).pipe(
      tap(() => {
        // Create local notification with client prefix to avoid ID conflicts
        const notification: Notification = {
          id: `client-${crypto.randomUUID()}`,
          type: NotificationType.AppointmentCompleted,
          title: 'Consulta Finalizada',
          message: `Dr(a). ${data.doctorName} finalizou o atendimento de ${data.patientName}`,
          data,
          isRead: false,
          createdAt: new Date()
        };
        
        this.notifications.update(notifications => [notification, ...notifications]);
        this.notificationSubject.next(notification);
        this.updateUnreadCount();
      })
    );
  }

  callNextPatient(data: CallNextPatientNotification): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/call-next-patient`, data).pipe(
      tap(() => {
        // Create local notification with client prefix to avoid ID conflicts
        const notification: Notification = {
          id: `client-${crypto.randomUUID()}`,
          type: NotificationType.CallNextPatient,
          title: 'Chamar Próximo Paciente',
          message: `Dr(a). ${data.doctorName} está chamando ${data.patientName}`,
          data,
          isRead: false,
          createdAt: new Date()
        };
        
        this.notifications.update(notifications => [notification, ...notifications]);
        this.notificationSubject.next(notification);
        this.updateUnreadCount();
      })
    );
  }

  markAsRead(notificationId: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${notificationId}/read`, {}).pipe(
      tap(() => {
        this.notifications.update(notifications => 
          notifications.map(n => 
            n.id === notificationId ? { ...n, isRead: true } : n
          )
        );
        this.updateUnreadCount();
      })
    );
  }

  markAllAsRead(): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/read-all`, {}).pipe(
      tap(() => {
        this.notifications.update(notifications => 
          notifications.map(n => ({ ...n, isRead: true }))
        );
        this.updateUnreadCount();
      })
    );
  }

  deleteNotification(notificationId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${notificationId}`).pipe(
      tap(() => {
        this.notifications.update(notifications => 
          notifications.filter(n => n.id !== notificationId)
        );
        this.updateUnreadCount();
      })
    );
  }

  private updateUnreadCount(): void {
    const count = this.notifications().filter(n => !n.isRead).length;
    this.unreadCount.set(count);
  }

  // Play notification sound
  playNotificationSound(): void {
    // Audio path is configurable via assets folder
    const audio = new Audio('assets/notification-sound.mp3');
    audio.play().catch(err => console.error('Error playing notification sound:', err));
  }
}
