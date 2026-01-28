import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { SystemNotification, CreateSystemNotification } from '../models/system-admin.model';

@Injectable({
  providedIn: 'root'
})
export class SystemNotificationService {
  private apiUrl = `${environment.apiUrl}/system-admin/notifications`;
  private hubConnection?: signalR.HubConnection;
  private notificationReceived = new Subject<SystemNotification>();

  public notification$ = this.notificationReceived.asObservable();

  constructor(private http: HttpClient) {}

  /**
   * Initialize SignalR connection for real-time notifications
   */
  startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl.replace('/api', '')}/hubs/system-notifications`, {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connection started'))
      .catch(err => console.error('Error starting SignalR connection: ', err));

    this.hubConnection.on('ReceiveNotification', (notification: SystemNotification) => {
      this.notificationReceived.next(notification);
    });
  }

  /**
   * Stop SignalR connection
   */
  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  /**
   * Get unread notifications
   */
  getUnreadNotifications(): Observable<SystemNotification[]> {
    return this.http.get<SystemNotification[]>(`${this.apiUrl}/unread`);
  }

  /**
   * Get all notifications with pagination
   */
  getAllNotifications(page: number = 1, pageSize: number = 20): Observable<SystemNotification[]> {
    return this.http.get<SystemNotification[]>(this.apiUrl, {
      params: {
        page: page.toString(),
        pageSize: pageSize.toString()
      }
    });
  }

  /**
   * Get unread count
   */
  getUnreadCount(): Observable<{ count: number }> {
    return this.http.get<{ count: number }>(`${this.apiUrl}/unread/count`);
  }

  /**
   * Mark notification as read
   */
  markAsRead(id: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/${id}/read`, {});
  }

  /**
   * Mark all notifications as read
   */
  markAllAsRead(): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/read-all`, {});
  }

  /**
   * Create a new notification (for testing)
   */
  createNotification(notification: CreateSystemNotification): Observable<SystemNotification> {
    return this.http.post<SystemNotification>(this.apiUrl, notification);
  }
}
