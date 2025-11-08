import { AuthResponse, UserInfo } from '../models/auth.model';

export const MOCK_AUTH_RESPONSE: AuthResponse = {
  token: 'mock-jwt-token-system-admin-12345',
  username: 'admin',
  tenantId: 'system',
  expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString(), // 24 hours from now
  isSystemOwner: true
};

export const MOCK_USER_INFO: UserInfo = {
  username: 'admin',
  tenantId: 'system',
  isSystemOwner: true
};
