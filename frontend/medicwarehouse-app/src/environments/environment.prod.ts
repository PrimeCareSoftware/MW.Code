export const environment = {
  production: true,
  // Default API URL (for backward compatibility)
  apiUrl: 'https://api.medicwarehouse.com/api',
  
  // Microservices URLs (configure with actual production URLs)
  microservices: {
    auth: 'https://auth.medicwarehouse.com/api',
    patients: 'https://patients.medicwarehouse.com/api',
    appointments: 'https://appointments.medicwarehouse.com/api',
    medicalRecords: 'https://medicalrecords.medicwarehouse.com/api',
    billing: 'https://billing.medicwarehouse.com/api',
    systemAdmin: 'https://systemadmin.medicwarehouse.com/api'
  },
  
  // Flag to enable microservices mode
  useMicroservices: false, // Set to true when ready to use microservices in production
  
  enableDebug: false,
  useMockData: false, // Enable to use mocked data instead of real API calls
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5 // minutes before expiry to show warning
  },
  tenant: {
    // Paths that should not be treated as tenant identifiers
    excludedPaths: ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets', 'health', 'swagger']
  }
};
