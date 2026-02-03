export const environment = {
  production: true,
  // Default API URL (for backward compatibility)
  apiUrl: 'https://api.omnicare.com/api',
  
  // Microservices URLs (configure with actual production URLs)
  microservices: {
    auth: 'https://auth.omnicare.com/api',
    patients: 'https://patients.omnicare.com/api',
    appointments: 'https://appointments.omnicare.com/api',
    medicalRecords: 'https://medicalrecords.omnicare.com/api',
    billing: 'https://billing.omnicare.com/api',
    systemAdmin: 'https://systemadmin.omnicare.com/api',
    telemedicine: 'https://telemedicine.omnicare.com/api'
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
    excludedPaths: ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets', 'health', 'swagger'],
    // Domain suffix for subdomain display (can be changed to any domain)
    // Examples: 'omnicare.com', 'omnicare.com.br', 'yourdomain.com'
    domainSuffix: 'omnicare.com'
  },
  
  // Documentation repository
  documentation: {
    repositoryUrl: 'https://github.com/Omni CareSoftware/MW.Code/blob/main'
  }
};
