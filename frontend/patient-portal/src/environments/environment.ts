export const environment = {
  production: false,
  apiUrl: 'http://localhost:5101/api', // Patient Portal API (not main MedicSoft API)
  // Default clinic ID - FALLBACK ONLY
  // Primary source is user.clinicId from authenticated user profile
  // This fallback is used only when user profile doesn't contain clinicId
  // In production deployment, configure this per environment if needed
  defaultClinicId: '00000000-0000-0000-0000-000000000001'
};
