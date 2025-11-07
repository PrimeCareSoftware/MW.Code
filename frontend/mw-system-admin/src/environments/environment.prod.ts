export const environment = {
  production: true,
  apiUrl: 'https://api.medicwarehouse.com/api',
  enableDebug: false,
  useMockData: false, // Enable to use mocked data instead of real API calls
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5
  }
};
