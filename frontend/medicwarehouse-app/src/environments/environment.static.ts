export const environment = {
  production: true,
  apiUrl: 'http://localhost:5000/api',
  enableDebug: false,
  useMockData: true, // Enable mocked data for static build
  security: {
    enableCSRFProtection: false, // Disable CSRF for static build
    tokenExpiryWarning: 5 // minutes before expiry to show warning
  },
  tenant: {
    // Paths that should not be treated as tenant identifiers
    excludedPaths: ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets', 'health', 'swagger']
  }
};
