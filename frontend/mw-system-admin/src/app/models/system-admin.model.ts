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
  tags?: Tag[];
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

// Phase 1: SaaS Metrics
export interface SaasDashboard {
  mrr: number;
  arr: number;
  activeCustomers: number;
  newCustomers: number;
  churnedCustomers: number;
  churnRate: number;
  arpu: number;
  ltv: number;
  cac: number;
  ltvCacRatio: number;
  mrrGrowthMoM: number;
  growthRateYoY: number;
  quickRatio: number;
  mrrTrend: 'up' | 'down' | 'stable';
  trialCustomers: number;
  atRiskCustomers: number;
}

export interface MrrBreakdown {
  totalMrr: number;
  newMrr: number;
  expansionMrr: number;
  contractionMrr: number;
  churnedMrr: number;
  netNewMrr: number;
}

export interface ChurnAnalysis {
  revenueChurnRate: number;
  customerChurnRate: number;
  monthlyRevenueChurn: number;
  monthlyCustomerChurn: number;
  annualRevenueChurn: number;
  annualCustomerChurn: number;
  churnHistory: ChurnDataPoint[];
}

export interface ChurnDataPoint {
  month: string;
  revenueChurn: number;
  customerChurn: number;
  churnedCount: number;
}

export interface GrowthMetrics {
  moMGrowthRate: number;
  yoYGrowthRate: number;
  quickRatio: number;
  trialConversionRate: number;
  growthHistory: GrowthDataPoint[];
}

export interface GrowthDataPoint {
  month: string;
  growthRate: number;
  mrr: number;
}

export interface RevenueTimeline {
  month: string;
  date: string;
  totalMrr: number;
  newMrr: number;
  expansionMrr: number;
  contractionMrr: number;
  churnedMrr: number;
  activeCustomers: number;
}

export interface CustomerBreakdown {
  byPlan: { [key: string]: number };
  byStatus: { [key: string]: number };
}

// Phase 1: Global Search
export interface GlobalSearchResult {
  clinics: ClinicSearchResult[];
  users: UserSearchResult[];
  tickets: TicketSearchResult[];
  plans: PlanSearchResult[];
  auditLogs: AuditLogSearchResult[];
  totalResults: number;
  searchDurationMs: number;
}

export interface ClinicSearchResult {
  id: string;
  name: string;
  document: string;
  email: string;
  tenantId: string;
  planName: string;
  status: string;
  isActive: boolean;
}

export interface UserSearchResult {
  id: string;
  username: string;
  fullName: string;
  email: string;
  role: string;
  clinicName: string;
  isActive: boolean;
}

export interface TicketSearchResult {
  id: string;
  title: string;
  description: string;
  status: string;
  priority: string;
  clinicName: string;
  createdAt: string;
}

export interface PlanSearchResult {
  id: string;
  name: string;
  description: string;
  monthlyPrice: number;
  activeSubscriptions: number;
  isActive: boolean;
}

export interface AuditLogSearchResult {
  id: string;
  action: string;
  entityType: string;
  entityId: string;
  userName: string;
  timestamp: string;
}

// Phase 1: System Notifications
export interface SystemNotification {
  id: number;
  type: 'critical' | 'warning' | 'info' | 'success';
  category: 'subscription' | 'customer' | 'system' | 'ticket';
  title: string;
  message: string;
  actionUrl?: string;
  actionLabel?: string;
  isRead: boolean;
  createdAt: string;
  readAt?: string;
  data?: string;
}

export interface CreateSystemNotification {
  type: string;
  category: string;
  title: string;
  message: string;
  actionUrl?: string;
  actionLabel?: string;
  data?: string;
}

// Phase 2: Advanced Clinic Management
export interface ClinicHealthScore {
  clinicId: string;
  totalScore: number;
  healthStatus: 'Healthy' | 'NeedsAttention' | 'AtRisk';
  usageScore: number;
  userEngagementScore: number;
  supportScore: number;
  paymentScore: number;
  lastActivity?: string;
  daysSinceActivity?: number;
  activeUsersPercentage?: number;
  openTicketsCount?: number;
  hasPaymentIssues: boolean;
  calculatedAt: string;
}

export interface ClinicTimelineEvent {
  type: string;
  title: string;
  description: string;
  date: string;
  icon: string;
  metadata?: any;
}

export interface ClinicUsageMetrics {
  clinicId: string;
  periodStart: string;
  periodEnd: string;
  totalLogins: number;
  uniqueActiveUsers: number;
  totalAppointments: number;
  totalPatientRecords: number;
  totalDocuments: number;
  averageSessionDuration: number;
  peakUsageHours: number[];
  featureUsage: { [key: string]: number };
}

export interface Tag {
  id: string;
  name: string;
  description?: string;
  category: string;
  color: string;
  isAutomatic: boolean;
  order: number;
  createdAt: string;
}

export interface ClinicFilter {
  searchTerm?: string;
  isActive?: boolean;
  subscriptionStatus?: string;
  healthStatus?: string;
  tagIds?: string[];
  planIds?: string[];
  createdAfter?: string;
  createdBefore?: string;
  page?: number;
  pageSize?: number;
}

export interface CrossTenantUser {
  id: string;
  username: string;
  email: string;
  fullName: string;
  role: string;
  tenantId: string;
  clinicName: string;
  isActive: boolean;
  lastLoginAt?: string;
  createdAt: string;
}

export interface CrossTenantUserFilter {
  searchTerm?: string;
  clinicId?: string;
  role?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface BulkActionRequest {
  clinicIds: string[];
  action: 'send-email' | 'change-plan' | 'assign-tags' | 'remove-tags';
  actionData?: any;
}

// Phase 3: Analytics & BI - Custom Dashboards
export interface CustomDashboard {
  id: number;
  name: string;
  description: string;
  layout: string;
  isDefault: boolean;
  isPublic: boolean;
  createdBy: string;
  createdAt: string;
  widgets: DashboardWidget[];
}

export interface DashboardWidget {
  id: number;
  dashboardId: number;
  type: 'line' | 'bar' | 'pie' | 'metric' | 'table' | 'map' | 'markdown';
  title: string;
  config: string;
  query: string;
  refreshInterval: number;
  gridX: number;
  gridY: number;
  gridWidth: number;
  gridHeight: number;
}

export interface WidgetConfig {
  xAxis?: string;
  yAxis?: string;
  labelField?: string;
  valueField?: string;
  color?: string;
  format?: string;
  icon?: string;
  threshold?: {
    warning?: number;
    critical?: number;
  };
  endpoint?: string;
  parameters?: any;
}

export interface WidgetTemplate {
  id: number;
  name: string;
  description: string;
  category: 'financial' | 'customer' | 'operational';
  type: string;
  defaultConfig: string;
  defaultQuery: string;
  isSystem: boolean;
}

export interface CreateDashboardDto {
  name: string;
  description: string;
  layout?: string;
  isDefault?: boolean;
  isPublic?: boolean;
}

export interface UpdateDashboardDto {
  name?: string;
  description?: string;
  layout?: string;
  isDefault?: boolean;
  isPublic?: boolean;
}

export interface CreateWidgetDto {
  type: string;
  title: string;
  config: string;
  query?: string;
  refreshInterval?: number;
  gridX: number;
  gridY: number;
  gridWidth: number;
  gridHeight: number;
}

export interface WidgetPositionDto {
  gridX: number;
  gridY: number;
  gridWidth: number;
  gridHeight: number;
}

// Phase 3: Analytics & BI - Reports
export interface ReportTemplate {
  id: number;
  name: string;
  description: string;
  category: 'financial' | 'customer' | 'operational';
  parameters: ReportParameter[];
  isSystem: boolean;
}

export interface ReportParameter {
  name: string;
  label: string;
  type: 'string' | 'number' | 'date' | 'select';
  required: boolean;
  defaultValue?: any;
  options?: { label: string; value: any }[];
}

export interface ReportParameters {
  startDate: string;
  endDate: string;
  [key: string]: any;
}

export interface ReportResult {
  id: string;
  title: string;
  generatedAt: string;
  data: any[];
  charts: ReportChart[];
}

export interface ReportChart {
  type: string;
  title: string;
  series: any[];
  data?: any[];
}

export interface ScheduledReport {
  id: number;
  templateId: number;
  templateName: string;
  frequency: 'daily' | 'weekly' | 'monthly';
  recipients: string[];
  parameters: ReportParameters;
  isActive: boolean;
  lastRunAt?: string;
  nextRunAt: string;
}

export interface ScheduleReportDto {
  templateId: number;
  frequency: 'daily' | 'weekly' | 'monthly';
  recipients: string[];
  parameters: ReportParameters;
  isActive?: boolean;
}

// Phase 3: Analytics & BI - Cohort Analysis
export interface CohortRetention {
  cohorts: RetentionCohort[];
}

export interface RetentionCohort {
  month: string;
  size: number;
  retentionRates: number[];
}

export interface CohortRevenue {
  cohorts: RevenueCohort[];
}

export interface RevenueCohort {
  month: string;
  size: number;
  mrrByMonth: number[];
  expansionMrr: number[];
  contractionMrr: number[];
  ltv: number;
}

export interface CohortBehavior {
  cohorts: BehaviorCohort[];
}

export interface BehaviorCohort {
  month: string;
  size: number;
  features: { [key: string]: number[] };
}
