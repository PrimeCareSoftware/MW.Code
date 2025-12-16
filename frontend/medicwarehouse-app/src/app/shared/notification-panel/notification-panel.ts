import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { NotificationService } from '../../services/notification.service';
import { Notification } from '../../models/notification.model';

@Component({
  selector: 'app-notification-panel',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification-panel.html',
  styleUrl: './notification-panel.scss'
})
export class NotificationPanel implements OnInit, OnDestroy {
  isOpen = signal<boolean>(false);
  notifications = signal<Notification[]>([]);
  unreadCount = signal<number>(0);
  
  private notificationSubscription?: Subscription;

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.notifications = this.notificationService.notifications;
    this.unreadCount = this.notificationService.unreadCount;
    
    // Subscribe to real-time notifications
    this.notificationSubscription = this.notificationService.onNotification$.subscribe(
      (notification) => {
        // Show browser notification if permission granted
        this.showBrowserNotification(notification);
        // Play sound
        this.notificationService.playNotificationSound();
      }
    );
  }

  ngOnDestroy(): void {
    if (this.notificationSubscription) {
      this.notificationSubscription.unsubscribe();
    }
  }

  togglePanel(): void {
    this.isOpen.set(!this.isOpen());
  }

  markAsRead(notification: Notification): void {
    if (!notification.isRead) {
      this.notificationService.markAsRead(notification.id).subscribe();
    }
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe();
  }

  deleteNotification(notification: Notification): void {
    this.notificationService.deleteNotification(notification.id).subscribe();
  }

  getNotificationIcon(notification: Notification): string {
    switch (notification.type) {
      case NotificationType.AppointmentCompleted:
        return '✓';
      case NotificationType.PatientReady:
        return '👤';
      case NotificationType.AppointmentReminder:
        return '⏰';
      default:
        return 'ℹ';
    }
  }

  formatTime(date: Date): string {
    const now = new Date();
    const notificationDate = new Date(date);
    const diffMs = now.getTime() - notificationDate.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    
    if (diffMins < 1) return 'Agora';
    if (diffMins < 60) return `${diffMins} min atrás`;
    
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return `${diffHours}h atrás`;
    
    const diffDays = Math.floor(diffHours / 24);
    return `${diffDays}d atrás`;
  }

  private showBrowserNotification(notification: Notification): void {
    if ('Notification' in window && Notification.permission === 'granted') {
      new Notification(notification.title, {
        body: notification.message,
        icon: '/assets/icons/icon-192x192.png'
      });
    }
  }

  requestNotificationPermission(): void {
    if ('Notification' in window && Notification.permission === 'default') {
      Notification.requestPermission();
    }
  }
}
