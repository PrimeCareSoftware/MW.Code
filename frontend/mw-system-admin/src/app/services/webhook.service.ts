import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  WebhookSubscriptionDto,
  CreateWebhookSubscriptionDto,
  UpdateWebhookSubscriptionDto,
  WebhookDeliveryDto,
  WebhookEvent
} from '../models/webhook.model';

@Injectable({
  providedIn: 'root'
})
export class WebhookService {
  private apiUrl = `${environment.apiUrl}/crm/webhooks`;

  constructor(private http: HttpClient) {}

  /**
   * Get all webhook subscriptions
   */
  getAllSubscriptions(): Observable<WebhookSubscriptionDto[]> {
    return this.http.get<WebhookSubscriptionDto[]>(this.apiUrl);
  }

  /**
   * Get webhook subscription by ID
   */
  getSubscription(id: string): Observable<WebhookSubscriptionDto> {
    return this.http.get<WebhookSubscriptionDto>(`${this.apiUrl}/${id}`);
  }

  /**
   * Create webhook subscription
   */
  createSubscription(dto: CreateWebhookSubscriptionDto): Observable<WebhookSubscriptionDto> {
    return this.http.post<WebhookSubscriptionDto>(this.apiUrl, dto);
  }

  /**
   * Update webhook subscription
   */
  updateSubscription(id: string, dto: UpdateWebhookSubscriptionDto): Observable<WebhookSubscriptionDto> {
    return this.http.put<WebhookSubscriptionDto>(`${this.apiUrl}/${id}`, dto);
  }

  /**
   * Delete webhook subscription
   */
  deleteSubscription(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  /**
   * Activate webhook subscription
   */
  activateSubscription(id: string): Observable<WebhookSubscriptionDto> {
    return this.http.post<WebhookSubscriptionDto>(`${this.apiUrl}/${id}/activate`, {});
  }

  /**
   * Deactivate webhook subscription
   */
  deactivateSubscription(id: string): Observable<WebhookSubscriptionDto> {
    return this.http.post<WebhookSubscriptionDto>(`${this.apiUrl}/${id}/deactivate`, {});
  }

  /**
   * Regenerate webhook secret
   */
  regenerateSecret(id: string): Observable<WebhookSubscriptionDto> {
    return this.http.post<WebhookSubscriptionDto>(`${this.apiUrl}/${id}/regenerate-secret`, {});
  }

  /**
   * Get webhook deliveries
   */
  getDeliveries(subscriptionId: string, limit: number = 50): Observable<WebhookDeliveryDto[]> {
    return this.http.get<WebhookDeliveryDto[]>(`${this.apiUrl}/${subscriptionId}/deliveries`, {
      params: { limit: limit.toString() }
    });
  }

  /**
   * Get single delivery
   */
  getDelivery(id: string): Observable<WebhookDeliveryDto> {
    return this.http.get<WebhookDeliveryDto>(`${this.apiUrl}/deliveries/${id}`);
  }

  /**
   * Retry failed delivery
   */
  retryDelivery(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/deliveries/${id}/retry`, {});
  }

  /**
   * Get available webhook events
   */
  getAvailableEvents(): Observable<Record<number, string>> {
    return this.http.get<Record<number, string>>(`${this.apiUrl}/events`);
  }
}
