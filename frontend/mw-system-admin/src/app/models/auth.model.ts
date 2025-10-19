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
  isSystemOwner: boolean;
}

export interface UserInfo {
  username: string;
  tenantId: string;
  isSystemOwner: boolean;
}
