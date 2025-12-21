export interface ClinicSummary {
  id: string;
  name: string;
  document: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
  tenantId: string;
  createdAt: string;
  subscriptionStatus: string;
  planName: string;
  nextBillingDate?: string;
}

export interface ClinicDetail extends ClinicSummary {
  planPrice: number;
  trialEndsAt?: string;
  totalUsers: number;
  activeUsers: number;
}

export interface PaginatedClinics {
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  clinics: ClinicSummary[];
}

export interface SystemAnalytics {
  totalClinics: number;
  activeClinics: number;
  inactiveClinics: number;
  totalUsers: number;
  activeUsers: number;
  totalPatients: number;
  monthlyRecurringRevenue: number;
  subscriptionsByStatus: { [key: string]: number };
  subscriptionsByPlan: { [key: string]: number };
}

export interface UpdateSubscriptionRequest {
  newPlanId: string;
  status?: string;
}

export interface ManualOverrideRequest {
  reason: string;
}

export interface CreateClinicRequest {
  name: string;
  document: string;
  email: string;
  phone: string;
  address: string;
  ownerUsername: string;
  ownerPassword: string;
  ownerFullName: string;
  planId: string;
}

export interface SystemOwner {
  id: string;
  username: string;
  email: string;
  fullName: string;
  phone: string;
  isActive: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export interface CreateSystemOwnerRequest {
  username: string;
  email: string;
  password: string;
  fullName: string;
  phone: string;
}

export interface SubscriptionPlan {
  id: string;
  name: string;
  description?: string;
  monthlyPrice: number;
  yearlyPrice: number;
  maxUsers: number;
  maxPatients: number;
  trialDays: number;
  isActive: boolean;
  createdAt: string;
}

export interface CreateSubscriptionPlanRequest {
  name: string;
  description?: string;
  monthlyPrice: number;
  yearlyPrice: number;
  maxUsers: number;
  maxPatients: number;
  trialDays: number;
}

export interface UpdateSubscriptionPlanRequest extends CreateSubscriptionPlanRequest {
  isActive: boolean;
}

export interface UpdateClinicRequest {
  name: string;
  document: string;
  email: string;
  phone: string;
  address: string;
}

export interface ClinicOwner {
  id: string;
  username: string;
  email: string;
  fullName: string;
  phone: string;
  isActive: boolean;
  clinicId?: string;
  clinicName?: string;
  lastLoginAt?: string;
  createdAt: string;
}

export interface ResetPasswordRequest {
  newPassword: string;
}

export interface Subdomain {
  id: string;
  subdomain: string;
  clinicId: string;
  clinicName: string;
  tenantId: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateSubdomainRequest {
  subdomain: string;
  clinicId: string;
}

export interface EnableManualOverrideRequest {
  reason: string;
  extendUntil?: string;
}
