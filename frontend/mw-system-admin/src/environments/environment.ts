export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  enableDebug: true,
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5 // minutes before expiry to show warning
  }
};
