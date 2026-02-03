export interface SalesforceLead {
  id: string;
  sessionId: string;
  companyName?: string;
  contactName?: string;
  email?: string;
  phone?: string;
  city?: string;
  state?: string;
  planId?: string;
  planName?: string;
  lastStepReached: number;
  leadSource: string;
  status: LeadStatus;
  referrer?: string;
  utmCampaign?: string;
  utmSource?: string;
  utmMedium?: string;
  capturedAt: Date;
  lastActivityAt?: Date;
  salesforceLeadId?: string;
  isSyncedToSalesforce: boolean;
  syncedAt?: Date;
  syncAttempts: number;
  lastSyncError?: string;
  metadata?: string;
  createdAt: Date;
  updatedAt?: Date;
}

export enum LeadStatus {
  New = 0,
  Contacted = 1,
  Qualified = 2,
  Converted = 3,
  Lost = 4,
  Nurturing = 5
}

export interface LeadStatistics {
  totalLeads: number;
  newLeads: number;
  contactedLeads: number;
  qualifiedLeads: number;
  convertedLeads: number;
  lostLeads: number;
  syncedLeads: number;
  unsyncedLeads: number;
  leadsByStep: { [key: number]: number };
}

export interface SyncResult {
  totalLeads: number;
  successfulSyncs: number;
  failedSyncs: number;
  errors: string[];
}

export interface SalesforceConfiguration {
  enabled: boolean;
  instanceUrl?: string;
  clientId?: string;
  clientSecret?: string;
  username?: string;
  password?: string;
  securityToken?: string;
  apiVersion?: string;
  autoSyncEnabled: boolean;
  syncIntervalMinutes: number;
  maxSyncAttempts: number;
}
