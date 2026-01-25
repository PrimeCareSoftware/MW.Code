using MedicSoft.Telemedicine.Application.DTOs;

namespace MedicSoft.Telemedicine.Application.Interfaces;

/// <summary>
/// Application service for managing telemedicine sessions
/// Orchestrates domain logic and infrastructure
/// </summary>
public interface ITelemedicineService
{
    /// <summary>
    /// Creates a new telemedicine session for an appointment
    /// </summary>
    Task<SessionResponse> CreateSessionAsync(CreateSessionRequest request, string tenantId);
    
    /// <summary>
    /// Generates access token for a user to join a session
    /// </summary>
    Task<JoinSessionResponse> JoinSessionAsync(JoinSessionRequest request, string tenantId);
    
    /// <summary>
    /// Starts a scheduled session
    /// </summary>
    Task<SessionResponse> StartSessionAsync(Guid sessionId, string tenantId);
    
    /// <summary>
    /// Completes an active session
    /// </summary>
    Task<SessionResponse> CompleteSessionAsync(CompleteSessionRequest request, string tenantId);
    
    /// <summary>
    /// Cancels a session
    /// </summary>
    Task<SessionResponse> CancelSessionAsync(Guid sessionId, string reason, string tenantId);
    
    /// <summary>
    /// Gets session by ID
    /// </summary>
    Task<SessionResponse?> GetSessionByIdAsync(Guid sessionId, string tenantId);
    
    /// <summary>
    /// Gets session by appointment ID
    /// </summary>
    Task<SessionResponse?> GetSessionByAppointmentIdAsync(Guid appointmentId, string tenantId);
    
    /// <summary>
    /// Gets all sessions for a clinic
    /// </summary>
    Task<IEnumerable<SessionResponse>> GetClinicSessionsAsync(Guid clinicId, string tenantId, int skip = 0, int take = 50);
    
    /// <summary>
    /// Gets all sessions for a provider
    /// </summary>
    Task<IEnumerable<SessionResponse>> GetProviderSessionsAsync(Guid providerId, string tenantId, int skip = 0, int take = 50);
    
    /// <summary>
    /// Gets all sessions for a patient
    /// </summary>
    Task<IEnumerable<SessionResponse>> GetPatientSessionsAsync(Guid patientId, string tenantId, int skip = 0, int take = 50);
    
    // CFM 2.314/2022 Compliance - Consent Management
    
    /// <summary>
    /// Records patient consent for telemedicine (CFM 2.314 requirement)
    /// </summary>
    Task<ConsentResponse> RecordConsentAsync(CreateConsentRequest request, string ipAddress, string userAgent, string tenantId);
    
    /// <summary>
    /// Revokes an existing consent
    /// </summary>
    Task<ConsentResponse> RevokeConsentAsync(Guid consentId, string reason, string tenantId);
    
    /// <summary>
    /// Gets consent by ID
    /// </summary>
    Task<ConsentResponse?> GetConsentByIdAsync(Guid consentId, string tenantId);
    
    /// <summary>
    /// Gets all consents for a patient
    /// </summary>
    Task<IEnumerable<ConsentResponse>> GetPatientConsentsAsync(Guid patientId, string tenantId, bool activeOnly = true);
    
    /// <summary>
    /// Checks if patient has valid active consent
    /// </summary>
    Task<bool> HasValidConsentAsync(Guid patientId, string tenantId);
    
    /// <summary>
    /// Gets most recent consent for a patient
    /// </summary>
    Task<ConsentResponse?> GetMostRecentConsentAsync(Guid patientId, string tenantId);
    
    /// <summary>
    /// Validates first appointment rule (CFM 2.314 requirement)
    /// </summary>
    Task<FirstAppointmentValidationResponse> ValidateFirstAppointmentAsync(ValidateFirstAppointmentRequest request, string tenantId);
    
    // CFM 2.314/2022 Compliance - Identity Verification
    
    /// <summary>
    /// Creates identity verification for a user (CFM 2.314 requirement)
    /// </summary>
    Task<IdentityVerificationResponse> CreateIdentityVerificationAsync(CreateIdentityVerificationRequest request, string documentPhotoPath, string? selfiePath, string? crmCardPhotoPath, string tenantId);
    
    /// <summary>
    /// Approves or rejects identity verification
    /// </summary>
    Task<IdentityVerificationResponse> VerifyIdentityAsync(Guid verificationId, VerifyIdentityRequest request, Guid verifiedByUserId, string tenantId);
    
    /// <summary>
    /// Gets identity verification by ID
    /// </summary>
    Task<IdentityVerificationResponse?> GetIdentityVerificationByIdAsync(Guid verificationId, string tenantId);
    
    /// <summary>
    /// Gets latest identity verification for a user
    /// </summary>
    Task<IdentityVerificationResponse?> GetLatestIdentityVerificationAsync(Guid userId, string userType, string tenantId);
    
    /// <summary>
    /// Checks if user has valid identity verification
    /// </summary>
    Task<bool> HasValidIdentityVerificationAsync(Guid userId, string userType, string tenantId);
    
    /// <summary>
    /// Gets pending identity verifications
    /// </summary>
    Task<IEnumerable<IdentityVerificationResponse>> GetPendingVerificationsAsync(string tenantId, int skip = 0, int take = 50);
    
    // CFM 2.314/2022 Compliance - Recording Management
    
    /// <summary>
    /// Creates a recording for a session (CFM 2.314 requirement)
    /// </summary>
    Task<RecordingResponse> CreateRecordingAsync(CreateRecordingRequest request, string tenantId);
    
    /// <summary>
    /// Marks recording as started
    /// </summary>
    Task<RecordingResponse> StartRecordingAsync(Guid recordingId, string tenantId);
    
    /// <summary>
    /// Marks recording as completed
    /// </summary>
    Task<RecordingResponse> CompleteRecordingAsync(Guid recordingId, CompleteRecordingRequest request, string tenantId);
    
    /// <summary>
    /// Marks recording as failed
    /// </summary>
    Task<RecordingResponse> FailRecordingAsync(Guid recordingId, string reason, string tenantId);
    
    /// <summary>
    /// Deletes a recording (LGPD compliance)
    /// </summary>
    Task<RecordingResponse> DeleteRecordingAsync(Guid recordingId, DeleteRecordingRequest request, Guid deletedByUserId, string tenantId);
    
    /// <summary>
    /// Gets recording by ID
    /// </summary>
    Task<RecordingResponse?> GetRecordingByIdAsync(Guid recordingId, string tenantId);
    
    /// <summary>
    /// Gets recording by session ID
    /// </summary>
    Task<RecordingResponse?> GetRecordingBySessionIdAsync(Guid sessionId, string tenantId);
    
    /// <summary>
    /// Gets available recordings for tenant
    /// </summary>
    Task<IEnumerable<RecordingResponse>> GetAvailableRecordingsAsync(string tenantId, int skip = 0, int take = 50);
}
