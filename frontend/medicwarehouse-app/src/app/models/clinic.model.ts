/**
 * Represents a clinic that a user has access to
 */
export interface UserClinicDto {
  clinicId: string;
  clinicName: string;
  clinicAddress: string;
  isPreferred: boolean;
  isActive: boolean;
  linkedDate: string;
}

/**
 * Request to switch user's current clinic
 */
export interface SwitchClinicRequest {
  clinicId: string;
}

/**
 * Response after switching clinic
 */
export interface SwitchClinicResponse {
  success: boolean;
  message: string;
  currentClinicId?: string;
  currentClinicName?: string;
}
