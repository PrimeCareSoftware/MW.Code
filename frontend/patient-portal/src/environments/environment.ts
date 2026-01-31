export const environment = {
  production: false,
  apiUrl: 'http://localhost:5101/api', // Patient Portal API (not main MedicSoft API)
  // Default clinic ID - should be configured per deployment
  // In production, this should come from user profile or tenant configuration
  defaultClinicId: '00000000-0000-0000-0000-000000000001'
};
