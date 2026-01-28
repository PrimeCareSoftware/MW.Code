import { Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import type { Notification } from '../../models/notification.model';
import { NotificationType } from '../../models/notification.model';
import { FocusTrapDirective } from '../accessibility/directives/focus-trap.directive';

@Component({
  selector: 'app-notification-modal',
  standalone: true,
  imports: [CommonModule, FocusTrapDirective],
  templateUrl: './notification-modal.html',
  styleUrl: './notification-modal.scss'
})
export class NotificationModalComponent {
  @Input() notification: Notification | null = null;
  @Output() confirmed = new EventEmitter<string>();
  
  isVisible = signal<boolean>(false);

  get isCallNextPatient(): boolean {
    return this.notification?.type === NotificationType.CallNextPatient;
  }

  show(notification: Notification): void {
    this.notification = notification;
    this.isVisible.set(true);
  }

  hide(): void {
    this.isVisible.set(false);
    this.notification = null;
  }

  onConfirm(): void {
    if (this.notification) {
      this.confirmed.emit(this.notification.id);
      this.hide();
    }
  }

  onClose(): void {
    this.hide();
  }
}
