import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ClinicSummary,
  ClinicDetail,
  PaginatedClinics,
  SystemAnalytics,
  UpdateSubscriptionRequest,
  ManualOverrideRequest,
  CreateClinicRequest,
  SystemOwner,
  CreateSystemOwnerRequest,
  SubscriptionPlan,
  CreateSubscriptionPlanRequest,
  UpdateSubscriptionPlanRequest,
  UpdateClinicRequest,
  ClinicOwner,
  ResetPasswordRequest,
  Subdomain,
  CreateSubdomainRequest,
  EnableManualOverrideRequest,
  ClinicHealthScore,
  ClinicTimelineEvent,
  ClinicUsageMetrics,
  Tag,
  ClinicFilter,
  CrossTenantUser,
  CrossTenantUserFilter,
  BulkActionRequest,
  BusinessConfiguration,
  CreateBusinessConfigurationRequest,
  UpdateBusinessTypeRequest,
  UpdatePrimarySpecialtyRequest,
  UpdateFeatureRequest
} from '../models/system-admin.model';

@Injectable({
  providedIn: 'root'
})
export class SystemAdminService {
  private apiUrl = `${environment.apiUrl}/system-admin`;

  constructor(private http: HttpClient) {}

  /**
   * Get all clinics with pagination and filtering
   */
  getClinics(status?: string, page: number = 1, pageSize: number = 20): Observable<PaginatedClinics> {
    const params: any = { page, pageSize };
    if (status) {
      params.status = status;
    }
    return this.http.get<PaginatedClinics>(`${this.apiUrl}/clinics`, { params });
  }

  /**
   * Get detailed information about a specific clinic
   */
  getClinic(id: string): Observable<ClinicDetail> {
    return this.http.get<ClinicDetail>(`${this.apiUrl}/clinics/${id}`);
  }

  /**
   * Create a new clinic
   */
  createClinic(request: CreateClinicRequest): Observable<{ message: string; clinicId: string }> {
    return this.http.post<{ message: string; clinicId: string }>(
      `${this.apiUrl}/clinics`,
      request
    );
  }

  /**
   * Toggle clinic active status
   */
  toggleClinicStatus(id: string): Observable<{ message: string; isActive: boolean }> {
    return this.http.post<{ message: string; isActive: boolean }>(
      `${this.apiUrl}/clinics/${id}/toggle-status`,
      {}
    );
  }

  /**
   * Update clinic subscription
   */
  updateSubscription(id: string, request: UpdateSubscriptionRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
      `${this.apiUrl}/clinics/${id}/subscription`,
      request
    );
  }

  /**
   * Get system-wide analytics
   */
  getAnalytics(): Observable<SystemAnalytics> {
    return this.http.get<SystemAnalytics>(`${this.apiUrl}/analytics`);
  }

  /**
   * Enable manual override for a clinic subscription
   */
  enableManualOverride(id: string, request: ManualOverrideRequest): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/clinics/${id}/subscription/manual-override/enable`,
      request
    );
  }

  /**
   * Disable manual override for a clinic subscription
   */
  disableManualOverride(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/clinics/${id}/subscription/manual-override/disable`,
      {}
    );
  }

  /**
   * Get all system owners
   */
  getSystemOwners(): Observable<SystemOwner[]> {
    return this.http.get<SystemOwner[]>(`${this.apiUrl}/system-owners`);
  }

  /**
   * Create a new system owner
   */
  createSystemOwner(request: CreateSystemOwnerRequest): Observable<{ message: string; ownerId: string }> {
    return this.http.post<{ message: string; ownerId: string }>(
      `${this.apiUrl}/system-owners`,
      request
    );
  }

  /**
   * Toggle system owner active status
   */
  toggleSystemOwnerStatus(id: string): Observable<{ message: string; isActive: boolean }> {
    return this.http.post<{ message: string; isActive: boolean }>(
      `${this.apiUrl}/system-owners/${id}/toggle-status`,
      {}
    );
  }

  // Subscription Plans Management
  /**
   * Get all subscription plans
   */
  getSubscriptionPlans(activeOnly?: boolean): Observable<SubscriptionPlan[]> {
    const params: any = {};
    if (activeOnly !== undefined) {
      params.activeOnly = activeOnly;
    }
    return this.http.get<SubscriptionPlan[]>(`${this.apiUrl}/subscription-plans`, { params });
  }

  /**
   * Get a specific subscription plan
   */
  getSubscriptionPlan(id: string): Observable<SubscriptionPlan> {
    return this.http.get<SubscriptionPlan>(`${this.apiUrl}/subscription-plans/${id}`);
  }

  /**
   * Create a new subscription plan
   */
  createSubscriptionPlan(request: CreateSubscriptionPlanRequest): Observable<{ message: string; planId: string }> {
    return this.http.post<{ message: string; planId: string }>(
      `${this.apiUrl}/subscription-plans`,
      request
    );
  }

  /**
   * Update a subscription plan
   */
  updateSubscriptionPlan(id: string, request: UpdateSubscriptionPlanRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
      `${this.apiUrl}/subscription-plans/${id}`,
      request
    );
  }

  /**
   * Delete a subscription plan
   */
  deleteSubscriptionPlan(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/subscription-plans/${id}`
    );
  }

  /**
   * Toggle subscription plan status
   */
  toggleSubscriptionPlanStatus(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/subscription-plans/${id}/toggle-status`,
      {}
    );
  }

  /**
   * Update modules for a subscription plan
   */
  updatePlanModules(planId: string, enabledModules: string[]): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
      `${this.apiUrl}/subscription-plans/${planId}/modules`,
      { enabledModules }
    );
  }

  // Clinic Management
  /**
   * Update clinic information
   */
  updateClinic(id: string, request: UpdateClinicRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
      `${this.apiUrl}/clinics/${id}`,
      request
    );
  }

  // Clinic Owners Management
  /**
   * Get all clinic owners
   */
  getClinicOwners(clinicId?: string): Observable<ClinicOwner[]> {
    const params: any = {};
    if (clinicId) {
      params.clinicId = clinicId;
    }
    return this.http.get<ClinicOwner[]>(`${this.apiUrl}/clinic-owners`, { params });
  }

  /**
   * Reset clinic owner password
   */
  resetOwnerPassword(id: string, request: ResetPasswordRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/clinic-owners/${id}/reset-password`,
      request
    );
  }

  /**
   * Toggle clinic owner status
   */
  toggleClinicOwnerStatus(id: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/clinic-owners/${id}/toggle-status`,
      {}
    );
  }

  // Subdomain Management
  /**
   * Get all subdomains
   */
  getSubdomains(): Observable<Subdomain[]> {
    return this.http.get<Subdomain[]>(`${this.apiUrl}/subdomains`);
  }

  /**
   * Resolve subdomain to tenant
   */
  resolveSubdomain(subdomain: string): Observable<Subdomain> {
    return this.http.get<Subdomain>(`${this.apiUrl}/subdomains/resolve/${subdomain}`);
  }

  /**
   * Create a new subdomain
   */
  createSubdomain(request: CreateSubdomainRequest): Observable<{ message: string; subdomainId: string }> {
    return this.http.post<{ message: string; subdomainId: string }>(
      `${this.apiUrl}/subdomains`,
      request
    );
  }

  /**
   * Delete a subdomain
   */
  deleteSubdomain(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/subdomains/${id}`
    );
  }

  // Manual Override
  /**
   * Enable manual override for clinic subscription
   */
  enableManualOverrideExtended(id: string, request: EnableManualOverrideRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/clinics/${id}/subscription/manual-override`,
      request
    );
  }

  /**
   * Disable manual override for clinic subscription
   */
  disableManualOverrideExtended(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/clinics/${id}/subscription/manual-override`
    );
  }

  // Phase 2: Advanced Clinic Management
  
  /**
   * Get clinic health score
   */
  getClinicHealthScore(id: string): Observable<ClinicHealthScore> {
    return this.http.get<ClinicHealthScore>(
      `${this.apiUrl}/clinic-management/${id}/health-score`
    );
  }

  /**
   * Get clinic timeline events
   */
  getClinicTimeline(id: string, limit: number = 50): Observable<ClinicTimelineEvent[]> {
    return this.http.get<ClinicTimelineEvent[]>(
      `${this.apiUrl}/clinic-management/${id}/timeline`,
      { params: { limit: limit.toString() } }
    );
  }

  /**
   * Get clinic usage metrics
   */
  getClinicUsageMetrics(id: string, periodStart?: string, periodEnd?: string): Observable<ClinicUsageMetrics> {
    const params: any = {};
    if (periodStart) params.periodStart = periodStart;
    if (periodEnd) params.periodEnd = periodEnd;
    
    return this.http.get<ClinicUsageMetrics>(
      `${this.apiUrl}/clinic-management/${id}/usage-metrics`,
      { params }
    );
  }

  /**
   * Filter clinics with advanced criteria
   */
  filterClinics(filters: ClinicFilter): Observable<{ data: ClinicSummary[]; totalCount: number; page: number; pageSize: number; totalPages: number }> {
    return this.http.post<{ data: ClinicSummary[]; totalCount: number; page: number; pageSize: number; totalPages: number }>(
      `${this.apiUrl}/clinic-management/filter`,
      filters
    );
  }

  /**
   * Get clinics by segment
   */
  getClinicsBySegment(segment: string): Observable<{ segment: string; data: ClinicSummary[]; totalCount: number }> {
    return this.http.get<{ segment: string; data: ClinicSummary[]; totalCount: number }>(
      `${this.apiUrl}/clinic-management/segment/${segment}`
    );
  }

  // Tags Management
  
  /**
   * Get all tags
   */
  getTags(): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.apiUrl}/tags`);
  }

  /**
   * Get tags by category
   */
  getTagsByCategory(category: string): Observable<Tag[]> {
    return this.http.get<Tag[]>(`${this.apiUrl}/tags/category/${category}`);
  }

  /**
   * Assign tag to clinic
   */
  assignTagToClinic(clinicId: string, tagId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/tags/${tagId}/assign/${clinicId}`,
      {}
    );
  }

  /**
   * Remove tag from clinic
   */
  removeTagFromClinic(clinicId: string, tagId: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(
      `${this.apiUrl}/tags/${tagId}/remove/${clinicId}`
    );
  }

  // Cross-Tenant User Management
  
  /**
   * Get cross-tenant users with filters
   */
  getCrossTenantUsers(filters: CrossTenantUserFilter): Observable<{ users: CrossTenantUser[]; totalCount: number }> {
    return this.http.post<{ users: CrossTenantUser[]; totalCount: number }>(
      `${this.apiUrl}/cross-tenant-users/filter`,
      filters
    );
  }

  /**
   * Reset password for cross-tenant user
   */
  resetCrossTenantUserPassword(userId: string, newPassword: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/cross-tenant-users/${userId}/reset-password`,
      { newPassword }
    );
  }

  /**
   * Toggle cross-tenant user status
   */
  toggleCrossTenantUserStatus(userId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/cross-tenant-users/${userId}/toggle-status`,
      {}
    );
  }

  /**
   * Transfer ownership between users
   */
  transferOwnership(currentOwnerId: string, newOwnerId: string): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/cross-tenant-users/transfer-ownership`,
      { currentOwnerId, newOwnerId }
    );
  }

  /**
   * Execute bulk action on multiple clinics
   */
  bulkAction(request: BulkActionRequest): Observable<{ successCount: number; failureCount: number; errors: string[]; message: string }> {
    return this.http.post<{ successCount: number; failureCount: number; errors: string[]; message: string }>(
      `${this.apiUrl}/clinic-management/bulk-action`,
      request
    );
  }

  /**
   * Export clinics to CSV, Excel, or PDF
   */
  exportClinics(request: { clinicIds: string[]; format: string; includeHealthScore: boolean; includeTags: boolean; includeUsageMetrics: boolean }): Observable<Blob> {
    return this.http.post(
      `${this.apiUrl}/clinic-management/export`,
      request,
      { responseType: 'blob' }
    );
  }

  // Business Configuration Management

  /**
   * Get business configuration for a clinic
   */
  getBusinessConfiguration(clinicId: string, tenantId: string): Observable<BusinessConfiguration> {
    return this.http.get<BusinessConfiguration>(`${this.apiUrl}/business-configuration/clinic/${clinicId}`, {
      params: { tenantId }
    });
  }

  /**
   * Create business configuration for a clinic
   */
  createBusinessConfiguration(request: CreateBusinessConfigurationRequest): Observable<BusinessConfiguration> {
    return this.http.post<BusinessConfiguration>(`${this.apiUrl}/business-configuration`, request);
  }

  /**
   * Update business type
   */
  updateBusinessType(configId: string, request: UpdateBusinessTypeRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/business-configuration/${configId}/business-type`, request);
  }

  /**
   * Update primary specialty
   */
  updatePrimarySpecialty(configId: string, request: UpdatePrimarySpecialtyRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/business-configuration/${configId}/primary-specialty`, request);
  }

  /**
   * Update feature flag
   */
  updateFeature(configId: string, request: UpdateFeatureRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/business-configuration/${configId}/feature`, request);
  }
}

