import { AuthResponse, UserInfo } from '../models/auth.model';

export const MOCK_AUTH_RESPONSE: AuthResponse = {
  token: 'mock-jwt-token-12345',
  username: 'admin',
  tenantId: 'clinic1',
  expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString() // 24 hours from now
};

export const MOCK_USER_INFO: UserInfo = {
  username: 'admin',
  tenantId: 'clinic1'
};
