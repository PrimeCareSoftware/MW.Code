export enum ExternalServiceType {
  // Email Services
  SendGrid = 1,
  MailGun = 2,
  AmazonSES = 3,
  
  // SMS Services
  Twilio = 10,
  AmazonSNS = 11,
  
  // Video/Telemedicine
  DailyCo = 20,
  Zoom = 21,
  
  // Analytics
  GoogleAnalytics = 30,
  MixPanel = 31,
  Segment = 32,
  
  // CRM & Sales
  Salesforce = 40,
  HubSpot = 41,
  
  // Payment Gateways
  Stripe = 50,
  PagSeguro = 51,
  MercadoPago = 52,
  
  // Accounting/Fiscal
  Dominio = 60,
  ContaAzul = 61,
  Omie = 62,
  
  // Cloud Storage
  AmazonS3 = 70,
  GoogleCloudStorage = 71,
  Azure = 72,
  
  // Other
  Other = 100
}

export const ExternalServiceTypeLabels: Record<ExternalServiceType, string> = {
  [ExternalServiceType.SendGrid]: 'SendGrid (Email)',
  [ExternalServiceType.MailGun]: 'MailGun (Email)',
  [ExternalServiceType.AmazonSES]: 'Amazon SES (Email)',
  [ExternalServiceType.Twilio]: 'Twilio (SMS)',
  [ExternalServiceType.AmazonSNS]: 'Amazon SNS (SMS)',
  [ExternalServiceType.DailyCo]: 'Daily.co (Video)',
  [ExternalServiceType.Zoom]: 'Zoom (Video)',
  [ExternalServiceType.GoogleAnalytics]: 'Google Analytics',
  [ExternalServiceType.MixPanel]: 'MixPanel (Analytics)',
  [ExternalServiceType.Segment]: 'Segment (Analytics)',
  [ExternalServiceType.Salesforce]: 'Salesforce (CRM)',
  [ExternalServiceType.HubSpot]: 'HubSpot (CRM)',
  [ExternalServiceType.Stripe]: 'Stripe (Payment)',
  [ExternalServiceType.PagSeguro]: 'PagSeguro (Payment)',
  [ExternalServiceType.MercadoPago]: 'Mercado Pago (Payment)',
  [ExternalServiceType.Dominio]: 'Domínio (Contabilidade)',
  [ExternalServiceType.ContaAzul]: 'ContaAzul (Contabilidade)',
  [ExternalServiceType.Omie]: 'Omie (Contabilidade)',
  [ExternalServiceType.AmazonS3]: 'Amazon S3 (Storage)',
  [ExternalServiceType.GoogleCloudStorage]: 'Google Cloud Storage',
  [ExternalServiceType.Azure]: 'Azure (Storage)',
  [ExternalServiceType.Other]: 'Outro'
};

export interface ExternalServiceConfigurationDto {
  id: string;
  serviceType: ExternalServiceType;
  serviceName: string;
  description?: string;
  isActive: boolean;
  
  // Configuration (sensitive data masked)
  hasApiKey: boolean;
  hasApiSecret: boolean;
  hasClientId: boolean;
  hasClientSecret: boolean;
  hasAccessToken: boolean;
  hasRefreshToken: boolean;
  tokenExpiresAt?: string;
  isTokenExpired: boolean;
  
  // Service configuration
  apiUrl?: string;
  webhookUrl?: string;
  accountId?: string;
  projectId?: string;
  region?: string;
  additionalConfiguration?: string;
  
  // Metadata
  lastSyncAt?: string;
  lastError?: string;
  errorCount: number;
  hasValidConfiguration: boolean;
  
  // Clinic
  clinicId?: string;
  clinicName?: string;
  
  createdAt: string;
  updatedAt?: string;
}

export interface CreateExternalServiceConfigurationDto {
  serviceType: ExternalServiceType;
  serviceName: string;
  description?: string;
  clinicId?: string;
  
  // Configuration
  apiKey?: string;
  apiSecret?: string;
  clientId?: string;
  clientSecret?: string;
  accessToken?: string;
  refreshToken?: string;
  tokenExpiresAt?: string;
  
  // Service configuration
  apiUrl?: string;
  webhookUrl?: string;
  accountId?: string;
  projectId?: string;
  region?: string;
  additionalConfiguration?: string;
  
  isActive: boolean;
}

export interface UpdateExternalServiceConfigurationDto {
  serviceName: string;
  description?: string;
  
  // Configuration
  apiKey?: string;
  apiSecret?: string;
  clientId?: string;
  clientSecret?: string;
  accessToken?: string;
  refreshToken?: string;
  tokenExpiresAt?: string;
  
  // Service configuration
  apiUrl?: string;
  webhookUrl?: string;
  accountId?: string;
  projectId?: string;
  region?: string;
  additionalConfiguration?: string;
  
  isActive: boolean;
}
