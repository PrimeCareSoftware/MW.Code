import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Notification, AppointmentCompletedNotification } from '../models/notification.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = `${environment.apiUrl}/notifications`;
  private notificationSubject = new Subject<Notification>();
  
  public notifications = signal<Notification[]>([]);
  public unreadCount = signal<number>(0);

  constructor(private http: HttpClient) {
    this.loadNotifications();
  }

  // Observable for real-time notifications
  get onNotification$(): Observable<Notification> {
    return this.notificationSubject.asObservable();
  }

  loadNotifications(): void {
    this.http.get<Notification[]>(this.apiUrl).subscribe({
      next: (notifications) => {
        this.notifications.set(notifications);
        this.updateUnreadCount();
      },
      error: (error) => {
        console.error('Error loading notifications:', error);
      }
    });
  }

  notifyAppointmentCompleted(data: AppointmentCompletedNotification): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/appointment-completed`, data).pipe(
      tap(() => {
        // Create local notification
        const notification: Notification = {
          id: Math.random().toString(),
          type: 'AppointmentCompleted' as any,
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
    const audio = new Audio('assets/notification-sound.mp3');
    audio.play().catch(err => console.error('Error playing notification sound:', err));
  }
}
