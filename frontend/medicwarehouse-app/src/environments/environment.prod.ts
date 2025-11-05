export const environment = {
  production: true,
  apiUrl: 'https://api.medicwarehouse.com/api',
  enableDebug: false,
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5 // minutes before expiry to show warning
  },
  tenant: {
    // Paths that should not be treated as tenant identifiers
    excludedPaths: ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets', 'health', 'swagger']
  }
};
