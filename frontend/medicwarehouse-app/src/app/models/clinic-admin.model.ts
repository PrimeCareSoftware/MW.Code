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
  email: string;
  fullName?: string;
  isActive: boolean;
  profileId?: string;
  profileName?: string;
  createdAt: string;
}

export interface CreateClinicUserRequest {
  username: string;
  email: string;
  password: string;
  fullName?: string;
  profileId?: string;
}

export interface UpdateClinicUserRequest {
  email?: string;
  fullName?: string;
  isActive?: boolean;
  profileId?: string;
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
