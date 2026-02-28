export interface LoginRequest {
  username: string;
  password: string;
  tenantId?: string; // Optional - can be resolved from subdomain
}

export interface AuthResponse {
  token: string;
  username: string;
  tenantId: string;
  clinicId?: string;
  role?: string;
  isSystemOwner?: boolean;
  expiresAt: string;
  availableClinics?: UserClinicInfo[];
  currentClinicId?: string;
  // Password change required
  requiresPasswordChange?: boolean;
  tempToken?: string;
  // 2FA required
  requiresTwoFactor?: boolean;
  method?: string;
  message?: string;
}

export interface UserClinicInfo {
  clinicId: string;
  clinicName: string;
  isPreferred: boolean;
}

export interface SessionValidationRequest {
  token: string;
}

export interface SessionValidationResponse {
  isValid: boolean;
  message: string;
}

export interface UserInfo {
  username: string;
  tenantId: string;
  clinicId?: string;
  role?: string;
  isSystemOwner?: boolean;
  availableClinics?: UserClinicInfo[];
  currentClinicId?: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  tenantId: string;
}
