export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  enableDebug: true,
  useMockData: false, // Enable to use mocked data instead of real API calls
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5 // minutes before expiry to show warning
  }
};
