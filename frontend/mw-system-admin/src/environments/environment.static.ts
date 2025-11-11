export const environment = {
  production: true,
  apiUrl: 'http://localhost:5000/api',
  enableDebug: false,
  useMockData: true, // Enable mocked data for static build
  security: {
    enableCSRFProtection: false, // Disable CSRF for static build
    tokenExpiryWarning: 5
  }
};
