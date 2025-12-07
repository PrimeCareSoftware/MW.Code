export const environment = {
  production: false,
  // Default API URL (for backward compatibility)
  apiUrl: 'http://localhost:5293/api',

  // Microservices URLs
  microservices: {
    auth: 'http://localhost:5001/api',
    patients: 'http://localhost:5002/api',
    appointments: 'http://localhost:5003/api',
    medicalRecords: 'http://localhost:5004/api',
    billing: 'http://localhost:5005/api',
    systemAdmin: 'http://localhost:5006/api'
  },

  // Flag to enable microservices mode
  useMicroservices: false, // Set to true to use microservices

  enableDebug: true,
  useMockData: false, // Enable to use mocked data instead of real API calls
  security: {
    enableCSRFProtection: true,
    tokenExpiryWarning: 5 // minutes before expiry to show warning
  }
};
