export interface ClinicAdminInfoDto {
  id: string;
  name: string;
  subdomain: string;
  phone?: string;
  email?: string;
  address?: string;
  workingHours?: string;
  tenantId: string;
}

export interface UpdateClinicInfoRequest {
  phone?: string;
  email?: string;
  address?: string;
  workingHours?: string;
}

export interface ClinicUserDto {
  id: string;
  username: string;
  name: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateClinicUserRequest {
  username: string;
  email: string;
  password: string;
  name: string;
  phone?: string;
  role: string;
}

export interface UpdateClinicUserRequest {
  email?: string;
  name?: string;
  phone?: string;
  isActive?: boolean;
}

export interface ChangeUserPasswordRequest {
  newPassword: string;
}

export interface ChangeUserRoleRequest {
  newRole: string;
}

export interface SubscriptionDto {
  id: string;
  planName: string;
  status: string;
  startDate: string;
  endDate?: string;
  maxUsers?: number;
  currentUsers?: number;
  features?: string[];
}

export interface SubscriptionDetailsDto {
  id: string;
  planId: string;
  planName: string;
  planType: string;
  status: string;
  startDate: string;
  endDate?: string;
  nextBillingDate?: string;
  currentPrice: number;
  isTrial: boolean;
  isActive: boolean;
  limits: {
    maxUsers: number;
    maxPatients: number;
    currentUsers: number;
  };
  features: {
    hasReports: boolean;
    hasWhatsAppIntegration: boolean;
    hasSMSNotifications: boolean;
    hasTissExport: boolean;
  };
  createdAt: string;
}

export interface MyClinicDto {
  clinicId: string;
  name: string;
  tradeName?: string;
  document: string;
  subdomain: string;
  tenantId: string;
  isActive: boolean;
  isPrimaryOwner: boolean;
  hasActiveSubscription: boolean;
  subscriptionStatus?: string;
}

export interface PublicDisplaySettingsDto {
  showOnPublicSite: boolean;
  clinicType: string;
  whatsAppNumber?: string;
}

export interface UpdatePublicDisplaySettingsRequest {
  showOnPublicSite: boolean;
  clinicType: string;
  whatsAppNumber?: string;
}
