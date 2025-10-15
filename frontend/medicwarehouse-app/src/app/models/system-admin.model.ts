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
