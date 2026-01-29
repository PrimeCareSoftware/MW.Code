export interface WebhookSubscriptionDto {
  id: string;
  name: string;
  description?: string;
  targetUrl: string;
  subscribedEvents: number[];
  secret: string;
  isActive: boolean;
  maxRetries: number;
  retryDelaySeconds: number;
  createdAt: Date;
  updatedAt?: Date;
}

export interface CreateWebhookSubscriptionDto {
  name: string;
  description?: string;
  targetUrl: string;
  subscribedEvents: number[];
  maxRetries: number;
  retryDelaySeconds: number;
}

export interface UpdateWebhookSubscriptionDto {
  name?: string;
  description?: string;
  targetUrl?: string;
  subscribedEvents?: number[];
  maxRetries?: number;
  retryDelaySeconds?: number;
}

export interface WebhookDeliveryDto {
  id: string;
  webhookSubscriptionId: string;
  event: string;
  payload: string;
  status: 'pending' | 'delivered' | 'failed';
  attemptCount: number;
  responseCode?: string;
  responseBody?: string;
  createdAt: Date;
  deliveredAt?: Date;
}

export enum WebhookEvent {
  ClinicCreated = 0,
  ClinicUpdated = 1,
  ClinicSuspended = 2,
  SubscriptionChanged = 3,
  PaymentFailed = 4,
  TrialEnding = 5,
  HealthScoreChanged = 6
}

export const WebhookEventLabels: Record<WebhookEvent, string> = {
  [WebhookEvent.ClinicCreated]: 'Clinic Created',
  [WebhookEvent.ClinicUpdated]: 'Clinic Updated',
  [WebhookEvent.ClinicSuspended]: 'Clinic Suspended',
  [WebhookEvent.SubscriptionChanged]: 'Subscription Changed',
  [WebhookEvent.PaymentFailed]: 'Payment Failed',
  [WebhookEvent.TrialEnding]: 'Trial Ending',
  [WebhookEvent.HealthScoreChanged]: 'Health Score Changed'
};
