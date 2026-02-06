export const environment = {
  production: false,
  // Default API URL (for backward compatibility)
  apiUrl: 'http://localhost:5000/api',
  
  // App URL (for marketing site redirects)
  appUrl: 'http://localhost:4200',
  
  // Company contact information (for marketing site)
  whatsappNumber: '5511999999999',
  companyEmail: 'contato@omnicare.com',
  companyPhone: '(11) 99999-9999',
  
  // Microservices URLs
  microservices: {
    auth: 'http://localhost:5001/api',
    patients: 'http://localhost:5002/api',
    appointments: 'http://localhost:5003/api',
    medicalRecords: 'http://localhost:5004/api',
    billing: 'http://localhost:5005/api',
    systemAdmin: 'http://localhost:5006/api',
    telemedicine: 'http://localhost:5084/api'
  },
  
  // Flag to enable microservices mode
  useMicroservices: false, // Set to true to use microservices
  
  enableDebug: true,
  useMockData: false, // Enable to use mocked data instead of real API calls
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5 // minutes before expiry to show warning
  },
  tenant: {
    // Paths that should not be treated as tenant identifiers
    excludedPaths: ['api', 'login', 'register', 'dashboard', 'patients', 'appointments', 'assets', 'health', 'swagger'],
    // Domain suffix for subdomain display (can be changed to any domain)
    // Examples: 'localhost:4200', 'omnicare.com', 'yourdomain.com'
    domainSuffix: 'localhost:4200'
  },
  
  // Documentation repository
  documentation: {
    repositoryUrl: 'https://github.com/Omni CareSoftware/MW.Code/blob/main'
  }
};
