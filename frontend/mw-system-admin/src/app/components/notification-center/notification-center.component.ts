import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SystemNotificationService } from '../../services/system-notification.service';
import { SystemNotification } from '../../models/system-admin.model';

@Component({
  selector: 'app-notification-center',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notification-center">
      <button class="notification-button" (click)="togglePanel()">
        <span class="bell-icon">🔔</span>
        <span class="badge" *ngIf="unreadCount > 0">{{ unreadCount }}</span>
      </button>

      <div class="notification-panel" *ngIf="isOpen">
        <div class="panel-header">
          <h3>Notificações</h3>
          <button class="mark-all-read" (click)="markAllAsRead()" *ngIf="unreadCount > 0">
            Marcar todas como lidas
          </button>
        </div>

        <div class="notification-list">
          <div *ngFor="let notification of notifications" 
               class="notification-item"
               [class.unread]="!notification.isRead"
               [class]="'type-' + notification.type"
               (click)="markAsRead(notification)">
            <div class="notification-content">
              <div class="notification-title">{{ notification.title }}</div>
              <div class="notification-message">{{ notification.message }}</div>
              <div class="notification-time">{{ formatTime(notification.createdAt) }}</div>
            </div>
          </div>

          <div class="no-notifications" *ngIf="notifications.length === 0">
            Nenhuma notificação
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .notification-center {
      position: relative;
    }

    .notification-button {
      position: relative;
      background: none;
      border: none;
      font-size: 24px;
      cursor: pointer;
      padding: 8px;
    }

    .badge {
      position: absolute;
      top: 4px;
      right: 4px;
      background: #ef4444;
      color: white;
      border-radius: 10px;
      padding: 2px 6px;
      font-size: 11px;
      font-weight: 600;
    }

    .notification-panel {
      position: absolute;
      top: 50px;
      right: 0;
      width: 400px;
      max-height: 600px;
      background: white;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      z-index: 1000;
      display: flex;
      flex-direction: column;
    }

    .panel-header {
      padding: 16px;
      border-bottom: 1px solid #e5e7eb;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .panel-header h3 {
      margin: 0;
      font-size: 18px;
    }

    .mark-all-read {
      background: none;
      border: none;
      color: #667eea;
      cursor: pointer;
      font-size: 12px;
    }

    .notification-list {
      overflow-y: auto;
      max-height: 500px;
    }

    .notification-item {
      padding: 16px;
      border-bottom: 1px solid #f3f4f6;
      cursor: pointer;
      transition: background 0.2s;
    }

    .notification-item:hover {
      background: #f9fafb;
    }

    .notification-item.unread {
      background: #eff6ff;
    }

    .notification-item.type-critical {
      border-left: 4px solid #ef4444;
    }

    .notification-item.type-warning {
      border-left: 4px solid #f59e0b;
    }

    .notification-item.type-info {
      border-left: 4px solid #3b82f6;
    }

    .notification-item.type-success {
      border-left: 4px solid #10b981;
    }

    .notification-title {
      font-weight: 600;
      margin-bottom: 4px;
      color: #1f2937;
    }

    .notification-message {
      font-size: 14px;
      color: #6b7280;
      margin-bottom: 8px;
    }

    .notification-time {
      font-size: 12px;
      color: #9ca3af;
    }

    .no-notifications {
      padding: 40px;
      text-align: center;
      color: #9ca3af;
    }
  `]
})
export class NotificationCenterComponent implements OnInit, OnDestroy {
  isOpen = false;
  notifications: SystemNotification[] = [];
  unreadCount = 0;

  constructor(private notificationService: SystemNotificationService) {}

  ngOnInit(): void {
    this.loadNotifications();
    this.notificationService.startConnection();

    // Subscribe to real-time notifications
    this.notificationService.notification$.subscribe(notification => {
      this.notifications.unshift(notification);
      if (!notification.isRead) {
        this.unreadCount++;
      }
    });
  }

  ngOnDestroy(): void {
    this.notificationService.stopConnection();
  }

  loadNotifications(): void {
    this.notificationService.getUnreadNotifications().subscribe(notifications => {
      this.notifications = notifications;
      this.unreadCount = notifications.filter(n => !n.isRead).length;
    });
  }

  togglePanel(): void {
    this.isOpen = !this.isOpen;
  }

  markAsRead(notification: SystemNotification): void {
    if (!notification.isRead) {
      this.notificationService.markAsRead(notification.id).subscribe(() => {
        notification.isRead = true;
        this.unreadCount--;
      });
    }
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications.forEach(n => n.isRead = true);
      this.unreadCount = 0;
    });
  }

  formatTime(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);

    if (diffMins < 1) return 'Agora';
    if (diffMins < 60) return `${diffMins}m atrás`;
    if (diffMins < 1440) return `${Math.floor(diffMins / 60)}h atrás`;
    return `${Math.floor(diffMins / 1440)}d atrás`;
  }
}
