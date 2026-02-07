export const environment = {
  production: true,
  apiUrl: '/patient-portal-api', // Patient Portal API via reverse proxy (nginx routes to backend)
  // Default clinic ID - FALLBACK ONLY
  // Primary source is user.clinicId from authenticated user profile
  // This fallback is used only when user profile doesn't contain clinicId
  // In production deployment, configure this per environment if needed
  defaultClinicId: '00000000-0000-0000-0000-000000000001'
};
