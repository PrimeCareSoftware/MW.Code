export const environment = {
  production: true,
  apiUrl: '/patient-portal-api', // Patient Portal API via reverse proxy (nginx routes to backend)
  // Default clinic ID - should be configured per deployment
  // In production, this should come from user profile or tenant configuration  
  defaultClinicId: '00000000-0000-0000-0000-000000000001'
};
