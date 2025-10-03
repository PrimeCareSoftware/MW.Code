export interface LoginRequest {
  username: string;
  password: string;
  tenantId: string;
}

export interface AuthResponse {
  token: string;
  username: string;
  tenantId: string;
  expiresAt: string;
}

export interface UserInfo {
  username: string;
  tenantId: string;
}

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  tenantId: string;
}
