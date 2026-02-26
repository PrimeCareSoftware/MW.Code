/**
 * Models for Telemedicine/Video Call functionality
 */

/**
 * Session status enum
 */
export enum SessionStatus {
  Scheduled = 'Scheduled',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Cancelled = 'Cancelled',
  Failed = 'Failed'
}

/**
 * Participant role enum
 */
export enum ParticipantRole {
  Provider = 'provider',
  Patient = 'patient'
}

/**
 * Request to create a new telemedicine session
 */
export interface CreateSessionRequest {
  appointmentId: string;
  clinicId: string;
  providerId: string;
  patientId: string;
}

/**
 * Request to join an existing session
 */
export interface JoinSessionRequest {
  userId: string;
  userName: string;
  role: ParticipantRole;
}

/**
 * Response when joining a session (contains access info)
 */
export interface JoinSessionResponse {
  provider: 'Twilio' | string;
  roomName: string;
  roomUrl: string;
  accessToken: string;
  expiresAt: string;
  waitingRoomEnabled: boolean;
  recordingAvailable: boolean;
  screenSharingAvailable: boolean;
  chatAvailable: boolean;
}

/**
 * Request to complete a session
 */
export interface CompleteSessionRequest {
  notes?: string;
}

/**
 * Telemedicine session response
 */
export interface TelemedicineSession {
  id: string;
  tenantId: string;
  appointmentId: string;
  clinicId: string;
  providerId: string;
  patientId: string;
  roomId: string;
  roomUrl: string;
  status: SessionStatus;
  durationMinutes?: number;
  recordingUrl?: string;
  sessionNotes?: string;
  createdAt: string;
  updatedAt?: string;
  startedAt?: string;
  completedAt?: string;
}
