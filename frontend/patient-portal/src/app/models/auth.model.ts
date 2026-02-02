export interface User {
  id: string;
  email: string;
  fullName: string;
  cpf: string;
  phoneNumber: string;
  dateOfBirth: Date;
  twoFactorEnabled: boolean;
}

export interface LoginRequest {
  emailOrCPF: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: Date;
  user: User;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  cpf: string;
  phoneNumber: string;
  dateOfBirth: Date;
  password: string;
  confirmPassword: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

export interface TwoFactorRequiredResponse {
  requiresTwoFactor: boolean;
  tempToken: string;
  message: string;
}

export interface VerifyTwoFactorRequest {
  tempToken: string;
  code: string;
}

export interface ResendTwoFactorCodeRequest {
  tempToken: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  token: string;
  newPassword: string;
}
