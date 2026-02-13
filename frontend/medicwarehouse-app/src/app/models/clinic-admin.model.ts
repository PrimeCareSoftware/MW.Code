export interface ClinicAdminInfoDto {
  clinicId: string;
  name: string;
  tradeName: string;
  document: string;
  subdomain?: string;
  phone: string;
  email: string;
  address: string;
  openingTime: string; // TimeSpan as string (HH:mm:ss)
  closingTime: string; // TimeSpan as string (HH:mm:ss)
  appointmentDurationMinutes: number;
  allowEmergencySlots: boolean;
  enableOnlineAppointmentScheduling: boolean;
  isActive: boolean;
  showOnPublicSite: boolean;
  clinicType: string;
  whatsAppNumber?: string;
  defaultPaymentReceiverType: string;
  numberOfRooms: number;
  notifyPrimaryDoctorOnOtherDoctorAppointment: boolean;
}

export interface UpdateClinicInfoRequest {
  phone?: string;
  email?: string;
  address?: string;
  openingTime?: string; // TimeSpan as string (HH:mm:ss)
  closingTime?: string; // TimeSpan as string (HH:mm:ss)
  appointmentDurationMinutes?: number;
  allowEmergencySlots?: boolean;
  numberOfRooms?: number;
  notifyPrimaryDoctorOnOtherDoctorAppointment?: boolean;
  enableOnlineAppointmentScheduling?: boolean;
}

export interface ClinicUserDto {
  id: string;
  username: string;
  name: string;
  email: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  professionalId?: string;
  specialty?: string;
  showInAppointmentScheduling: boolean;
}

export interface CreateClinicUserRequest {
  username: string;
  email: string;
  password: string;
  name: string;
  phone?: string;
  role: string;
  professionalId?: string;
  specialty?: string;
  showInAppointmentScheduling?: boolean;
}

export interface UpdateClinicUserRequest {
  email?: string;
  name?: string;
  phone?: string;
  isActive?: boolean;
  professionalId?: string;
  specialty?: string;
  showInAppointmentScheduling?: boolean;
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
    maxClinics: number;
    currentUsers: number;
    currentClinics: number;
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

export interface DoctorFieldsConfigDto {
  professionalIdRequired: boolean;
  specialtyRequired: boolean;
}
