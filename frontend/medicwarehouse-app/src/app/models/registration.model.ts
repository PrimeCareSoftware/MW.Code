export interface RegistrationRequest {
  // Clinic Information
  clinicName: string;
  clinicCNPJ: string;
  clinicPhone: string;
  clinicEmail: string;
  
  // Address
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  state: string;
  zipCode: string;
  
  // Owner Information
  ownerName: string;
  ownerCPF: string;
  ownerPhone: string;
  ownerEmail: string;
  
  // Login Credentials
  username: string;
  password: string;
  
  // Subscription
  planId: string;
  acceptTerms: boolean;
  
  // Trial
  useTrial: boolean;
  
  // Sales Funnel Tracking (optional)
  sessionId?: string;
}

export interface RegistrationResponse {
  success: boolean;
  message: string;
  clinicId?: string;
  userId?: string;
  trialEndDate?: Date;
  tenantId?: string;
  subdomain?: string;
  clinicName?: string;
  ownerName?: string;
  ownerEmail?: string;
  username?: string;
}
