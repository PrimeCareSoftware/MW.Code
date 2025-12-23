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
  isOwner?: boolean;
  expiresAt: string;
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
  isOwner?: boolean;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  tenantId: string;
}
