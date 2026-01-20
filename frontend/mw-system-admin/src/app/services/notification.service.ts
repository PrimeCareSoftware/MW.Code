import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { Notification } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  notifications = signal<Notification[]>([]);
  unreadCount = signal<number>(0);
  onNotification$ = new Subject<Notification>();

  constructor(private http: HttpClient) {}

  getNotifications(): Observable<Notification[]> {
    return this.http.get<Notification[]>('/api/notifications');
  }

  markAsRead(id: string): Observable<void> {
    return this.http.put<void>(`/api/notifications/${id}/read`, {});
  }

  markAllAsRead(): Observable<void> {
    return this.http.put<void>('/api/notifications/read-all', {});
  }

  deleteNotification(id: string): Observable<void> {
    return this.http.delete<void>(`/api/notifications/${id}`);
  }

  playNotificationSound(): void {
    // Play notification sound
    const audio = new Audio('/assets/sounds/notification.mp3');
    audio.play().catch(err => console.error('Error playing sound:', err));
  }

  error(message: string): void {
    console.error('Notification Error:', message);
  }

  success(message: string): void {
    console.log('Notification Success:', message);
  }

  warning(message: string): void {
    console.warn('Notification Warning:', message);
  }

  info(message: string): void {
    console.info('Notification Info:', message);
  }
}
