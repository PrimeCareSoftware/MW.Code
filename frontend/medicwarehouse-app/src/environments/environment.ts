export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  enableDebug: true,
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5 // minutes before expiry to show warning
  }
};
