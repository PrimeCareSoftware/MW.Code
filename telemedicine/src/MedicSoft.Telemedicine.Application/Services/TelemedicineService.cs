using MedicSoft.Telemedicine.Application.DTOs;
using MedicSoft.Telemedicine.Application.Interfaces;
using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Enums;
using MedicSoft.Telemedicine.Domain.Interfaces;

namespace MedicSoft.Telemedicine.Application.Services;

/// <summary>
/// Implementation of telemedicine service following clean architecture
/// Coordinates between domain and infrastructure layers
/// </summary>
public class TelemedicineService : ITelemedicineService
{
    private readonly ITelemedicineSessionRepository _sessionRepository;
    private readonly ITelemedicineConsentRepository _consentRepository;
    private readonly IIdentityVerificationRepository _verificationRepository;
    private readonly ITelemedicineRecordingRepository _recordingRepository;
    private readonly IVideoCallService _videoCallService;

    public TelemedicineService(
        ITelemedicineSessionRepository sessionRepository,
        ITelemedicineConsentRepository consentRepository,
        IIdentityVerificationRepository verificationRepository,
        ITelemedicineRecordingRepository recordingRepository,
        IVideoCallService videoCallService)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _consentRepository = consentRepository ?? throw new ArgumentNullException(nameof(consentRepository));
        _verificationRepository = verificationRepository ?? throw new ArgumentNullException(nameof(verificationRepository));
        _recordingRepository = recordingRepository ?? throw new ArgumentNullException(nameof(recordingRepository));
        _videoCallService = videoCallService ?? throw new ArgumentNullException(nameof(videoCallService));
    }

    public async Task<SessionResponse> CreateSessionAsync(CreateSessionRequest request, string tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
            
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required", nameof(tenantId));

        // Check if session already exists for this appointment
        var existing = await _sessionRepository.GetByAppointmentIdAsync(request.AppointmentId, tenantId);
        if (existing != null)
            throw new InvalidOperationException($"Session already exists for appointment {request.AppointmentId}");

        // Create video room
        var roomName = $"session-{Guid.NewGuid():N}";
        var roomInfo = await _videoCallService.CreateRoomAsync(roomName, expirationHours: 2);

        // Create domain entity
        var session = new TelemedicineSession(
            tenantId,
            request.AppointmentId,
            request.ClinicId,
            request.ProviderId,
            request.PatientId,
            roomInfo.RoomName,
            roomInfo.RoomUrl
        );

        // Persist
        await _sessionRepository.AddAsync(session);

        return MapToResponse(session);
    }

    public async Task<JoinSessionResponse> JoinSessionAsync(JoinSessionRequest request, string tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, tenantId)
            ?? throw new InvalidOperationException($"Session {request.SessionId} not found");

        if (session.Status == SessionStatus.Completed || session.Status == SessionStatus.Cancelled)
            throw new InvalidOperationException($"Cannot join a {session.Status} session");

        // Generate access token
        var token = await _videoCallService.GenerateTokenAsync(
            session.RoomId,
            request.UserId.ToString(),
            request.UserName,
            expirationMinutes: 120
        );

        return new JoinSessionResponse
        {
            RoomUrl = session.RoomUrl,
            AccessToken = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(120)
        };
    }

    public async Task<SessionResponse> StartSessionAsync(Guid sessionId, string tenantId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, tenantId)
            ?? throw new InvalidOperationException($"Session {sessionId} not found");

        session.StartSession();
        await _sessionRepository.UpdateAsync(session);

        return MapToResponse(session);
    }

    public async Task<SessionResponse> CompleteSessionAsync(CompleteSessionRequest request, string tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, tenantId)
            ?? throw new InvalidOperationException($"Session {request.SessionId} not found");

        session.CompleteSession(request.Notes);

        // Try to get recording URL if available
        try
        {
            var recordingUrl = await _videoCallService.GetRecordingUrlAsync(session.RoomId);
            if (!string.IsNullOrWhiteSpace(recordingUrl))
            {
                session.SetRecordingUrl(recordingUrl);
            }
        }
        catch
        {
            // Recording might not be available yet, ignore
        }

        await _sessionRepository.UpdateAsync(session);

        return MapToResponse(session);
    }

    public async Task<SessionResponse> CancelSessionAsync(Guid sessionId, string reason, string tenantId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, tenantId)
            ?? throw new InvalidOperationException($"Session {sessionId} not found");

        session.CancelSession(reason);
        await _sessionRepository.UpdateAsync(session);

        // Clean up video room
        try
        {
            await _videoCallService.DeleteRoomAsync(session.RoomId);
        }
        catch
        {
            // Room might already be deleted or expired, ignore
        }

        return MapToResponse(session);
    }

    public async Task<SessionResponse?> GetSessionByIdAsync(Guid sessionId, string tenantId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId, tenantId);
        return session != null ? MapToResponse(session) : null;
    }

    public async Task<SessionResponse?> GetSessionByAppointmentIdAsync(Guid appointmentId, string tenantId)
    {
        var session = await _sessionRepository.GetByAppointmentIdAsync(appointmentId, tenantId);
        return session != null ? MapToResponse(session) : null;
    }

    public async Task<IEnumerable<SessionResponse>> GetClinicSessionsAsync(Guid clinicId, string tenantId, int skip = 0, int take = 50)
    {
        var sessions = await _sessionRepository.GetByClinicIdAsync(clinicId, tenantId, skip, take);
        return sessions.Select(MapToResponse);
    }

    public async Task<IEnumerable<SessionResponse>> GetProviderSessionsAsync(Guid providerId, string tenantId, int skip = 0, int take = 50)
    {
        var sessions = await _sessionRepository.GetByProviderIdAsync(providerId, tenantId, skip, take);
        return sessions.Select(MapToResponse);
    }

    public async Task<IEnumerable<SessionResponse>> GetPatientSessionsAsync(Guid patientId, string tenantId, int skip = 0, int take = 50)
    {
        var sessions = await _sessionRepository.GetByPatientIdAsync(patientId, tenantId, skip, take);
        return sessions.Select(MapToResponse);
    }

    private static SessionResponse MapToResponse(TelemedicineSession session)
    {
        return new SessionResponse
        {
            Id = session.Id,
            AppointmentId = session.AppointmentId,
            ClinicId = session.ClinicId,
            ProviderId = session.ProviderId,
            PatientId = session.PatientId,
            RoomUrl = session.RoomUrl,
            Status = session.Status.ToString(),
            CreatedAt = session.CreatedAt,
            DurationMinutes = session.Duration?.GetDurationInMinutes(),
            RecordingUrl = session.RecordingUrl
        };
    }
    
    // ===== CFM 2.314/2022 Compliance - Consent Management =====
    
    public async Task<ConsentResponse> RecordConsentAsync(CreateConsentRequest request, string ipAddress, string userAgent, string tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
            
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address is required", nameof(ipAddress));
            
        if (string.IsNullOrWhiteSpace(userAgent))
            throw new ArgumentException("User agent is required", nameof(userAgent));
            
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required", nameof(tenantId));
        
        // Build consent text
        var consentText = ConsentTexts.TelemedicineConsentText
            .Replace("[DATA_HORA_UTC]", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"))
            .Replace("[IP_ADDRESS]", ipAddress)
            .Replace("[USER_AGENT]", userAgent);
        
        if (request.AcceptsRecording)
        {
            consentText += "\n\n" + ConsentTexts.RecordingConsentText;
        }
        
        // Create consent entity
        var consent = new TelemedicineConsent(
            tenantId,
            request.PatientId,
            consentText,
            ipAddress,
            userAgent,
            request.AcceptsRecording,
            request.AcceptsDataSharing,
            request.AppointmentId,
            request.DigitalSignature
        );
        
        // Persist
        await _consentRepository.AddAsync(consent);
        
        // If linked to appointment, update session
        if (request.AppointmentId.HasValue)
        {
            var session = await _sessionRepository.GetByAppointmentIdAsync(request.AppointmentId.Value, tenantId);
            if (session != null)
            {
                session.RecordConsent(consent.Id, ipAddress);
                await _sessionRepository.UpdateAsync(session);
            }
        }
        
        return MapToConsentResponse(consent);
    }
    
    public async Task<ConsentResponse> RevokeConsentAsync(Guid consentId, string reason, string tenantId)
    {
        var consent = await _consentRepository.GetByIdAsync(consentId, tenantId)
            ?? throw new InvalidOperationException($"Consent {consentId} not found");
        
        consent.Revoke(reason);
        await _consentRepository.UpdateAsync(consent);
        
        return MapToConsentResponse(consent);
    }
    
    public async Task<ConsentResponse?> GetConsentByIdAsync(Guid consentId, string tenantId)
    {
        var consent = await _consentRepository.GetByIdAsync(consentId, tenantId);
        return consent != null ? MapToConsentResponse(consent) : null;
    }
    
    public async Task<IEnumerable<ConsentResponse>> GetPatientConsentsAsync(Guid patientId, string tenantId, bool activeOnly = true)
    {
        var consents = await _consentRepository.GetByPatientIdAsync(patientId, tenantId, activeOnly);
        return consents.Select(MapToConsentResponse);
    }
    
    public async Task<bool> HasValidConsentAsync(Guid patientId, string tenantId)
    {
        return await _consentRepository.HasValidConsentAsync(patientId, tenantId);
    }
    
    public async Task<ConsentResponse?> GetMostRecentConsentAsync(Guid patientId, string tenantId)
    {
        var consent = await _consentRepository.GetMostRecentConsentAsync(patientId, tenantId);
        return consent != null ? MapToConsentResponse(consent) : null;
    }
    
    public async Task<FirstAppointmentValidationResponse> ValidateFirstAppointmentAsync(ValidateFirstAppointmentRequest request, string tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        // Check if there are any previous sessions between this patient and provider
        var previousSessions = await _sessionRepository.GetByPatientIdAsync(request.PatientId, tenantId, 0, 1);
        var hasCompletedSessions = previousSessions.Any(s => 
            s.ProviderId == request.ProviderId && 
            s.Status == SessionStatus.Completed);
        
        var isFirstAppointment = !hasCompletedSessions;
        
        if (!isFirstAppointment)
        {
            // Not first appointment, can proceed with telemedicine
            return new FirstAppointmentValidationResponse
            {
                IsFirstAppointment = false,
                CanProceedWithTelemedicine = true,
                ValidationMessage = "Paciente já possui histórico de atendimento com este profissional."
            };
        }
        
        // First appointment - check if justification is provided
        if (string.IsNullOrWhiteSpace(request.Justification))
        {
            return new FirstAppointmentValidationResponse
            {
                IsFirstAppointment = true,
                CanProceedWithTelemedicine = false,
                ValidationMessage = "CFM 2.314/2022: Primeiro atendimento deve ser presencial, exceto em casos justificados.",
                RequiredJustification = "Informe a justificativa para realização de teleconsulta no primeiro atendimento."
            };
        }
        
        // First appointment with justification - can proceed
        return new FirstAppointmentValidationResponse
        {
            IsFirstAppointment = true,
            CanProceedWithTelemedicine = true,
            ValidationMessage = "Primeiro atendimento com justificativa para teleconsulta.",
            RequiredJustification = null
        };
    }
    
    private static ConsentResponse MapToConsentResponse(TelemedicineConsent consent)
    {
        return new ConsentResponse
        {
            Id = consent.Id,
            PatientId = consent.PatientId,
            AppointmentId = consent.AppointmentId,
            ConsentDate = consent.ConsentDate,
            AcceptsRecording = consent.AcceptsRecording,
            AcceptsDataSharing = consent.AcceptsDataSharing,
            IsActive = consent.IsActive,
            RevokedAt = consent.RevokedAt,
            RevocationReason = consent.RevocationReason
        };
    }
    
    // ===== CFM 2.314/2022 Compliance - Identity Verification =====
    
    public async Task<IdentityVerificationResponse> CreateIdentityVerificationAsync(
        CreateIdentityVerificationRequest request, 
        string documentPhotoPath, 
        string? selfiePath, 
        string? crmCardPhotoPath, 
        string tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
            
        if (string.IsNullOrWhiteSpace(documentPhotoPath))
            throw new ArgumentException("Document photo path is required", nameof(documentPhotoPath));
            
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required", nameof(tenantId));
        
        var verification = new IdentityVerification(
            tenantId,
            request.UserId,
            request.UserType,
            request.DocumentType,
            request.DocumentNumber,
            documentPhotoPath,
            selfiePath,
            crmCardPhotoPath,
            request.CrmNumber,
            request.CrmState,
            request.TelemedicineSessionId
        );
        
        await _verificationRepository.AddAsync(verification);
        
        return MapToVerificationResponse(verification);
    }
    
    public async Task<IdentityVerificationResponse> VerifyIdentityAsync(
        Guid verificationId, 
        VerifyIdentityRequest request, 
        Guid verifiedByUserId, 
        string tenantId)
    {
        var verification = await _verificationRepository.GetByIdAsync(verificationId, tenantId)
            ?? throw new InvalidOperationException($"Verification {verificationId} not found");
        
        if (request.Approved)
        {
            verification.Approve(verifiedByUserId, request.Notes);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.Notes))
                throw new ArgumentException("Rejection reason is required", nameof(request.Notes));
                
            verification.Reject(request.Notes, verifiedByUserId);
        }
        
        await _verificationRepository.UpdateAsync(verification);
        
        return MapToVerificationResponse(verification);
    }
    
    public async Task<IdentityVerificationResponse?> GetIdentityVerificationByIdAsync(Guid verificationId, string tenantId)
    {
        var verification = await _verificationRepository.GetByIdAsync(verificationId, tenantId);
        return verification != null ? MapToVerificationResponse(verification) : null;
    }
    
    public async Task<IdentityVerificationResponse?> GetLatestIdentityVerificationAsync(Guid userId, string userType, string tenantId)
    {
        var verification = await _verificationRepository.GetLatestByUserIdAsync(userId, userType, tenantId);
        return verification != null ? MapToVerificationResponse(verification) : null;
    }
    
    public async Task<bool> HasValidIdentityVerificationAsync(Guid userId, string userType, string tenantId)
    {
        return await _verificationRepository.HasValidVerificationAsync(userId, userType, tenantId);
    }
    
    public async Task<IEnumerable<IdentityVerificationResponse>> GetPendingVerificationsAsync(string tenantId, int skip = 0, int take = 50)
    {
        var verifications = await _verificationRepository.GetPendingVerificationsAsync(tenantId, skip, take);
        return verifications.Select(MapToVerificationResponse);
    }
    
    private static IdentityVerificationResponse MapToVerificationResponse(IdentityVerification verification)
    {
        return new IdentityVerificationResponse
        {
            Id = verification.Id,
            UserId = verification.UserId,
            UserType = verification.UserType,
            DocumentType = verification.DocumentType,
            DocumentNumber = verification.DocumentNumber,
            CrmNumber = verification.CrmNumber,
            CrmState = verification.CrmState,
            Status = verification.Status.ToString(),
            VerifiedAt = verification.VerifiedAt,
            VerifiedByUserId = verification.VerifiedByUserId,
            VerificationNotes = verification.VerificationNotes,
            ValidUntil = verification.ValidUntil,
            IsValid = verification.IsValid(),
            HasExpired = verification.HasExpired()
        };
    }
    
    // ===== CFM 2.314/2022 Compliance - Recording Management =====
    
    public async Task<RecordingResponse> CreateRecordingAsync(CreateRecordingRequest request, string tenantId)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
            
        if (string.IsNullOrWhiteSpace(tenantId))
            throw new ArgumentException("TenantId is required", nameof(tenantId));
        
        // Verify session exists
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, tenantId)
            ?? throw new InvalidOperationException($"Session {request.SessionId} not found");
        
        // Verify consent exists and allows recording
        var consent = await _consentRepository.GetByIdAsync(request.ConsentId, tenantId)
            ?? throw new InvalidOperationException($"Consent {request.ConsentId} not found");
            
        if (!consent.AcceptsRecording)
            throw new InvalidOperationException("Patient has not consented to recording");
        
        var recording = new TelemedicineRecording(
            tenantId,
            request.SessionId,
            request.RecordingPath,
            request.FileFormat,
            request.ConsentId,
            request.IsEncrypted,
            request.EncryptionKeyId
        );
        
        await _recordingRepository.AddAsync(recording);
        
        return MapToRecordingResponse(recording);
    }
    
    public async Task<RecordingResponse> StartRecordingAsync(Guid recordingId, string tenantId)
    {
        var recording = await _recordingRepository.GetByIdAsync(recordingId, tenantId)
            ?? throw new InvalidOperationException($"Recording {recordingId} not found");
        
        recording.StartRecording();
        await _recordingRepository.UpdateAsync(recording);
        
        return MapToRecordingResponse(recording);
    }
    
    public async Task<RecordingResponse> CompleteRecordingAsync(Guid recordingId, CompleteRecordingRequest request, string tenantId)
    {
        var recording = await _recordingRepository.GetByIdAsync(recordingId, tenantId)
            ?? throw new InvalidOperationException($"Recording {recordingId} not found");
        
        recording.CompleteRecording(request.FileSizeBytes, request.DurationSeconds);
        await _recordingRepository.UpdateAsync(recording);
        
        return MapToRecordingResponse(recording);
    }
    
    public async Task<RecordingResponse> FailRecordingAsync(Guid recordingId, string reason, string tenantId)
    {
        var recording = await _recordingRepository.GetByIdAsync(recordingId, tenantId)
            ?? throw new InvalidOperationException($"Recording {recordingId} not found");
        
        recording.FailRecording(reason);
        await _recordingRepository.UpdateAsync(recording);
        
        return MapToRecordingResponse(recording);
    }
    
    public async Task<RecordingResponse> DeleteRecordingAsync(Guid recordingId, DeleteRecordingRequest request, Guid deletedByUserId, string tenantId)
    {
        var recording = await _recordingRepository.GetByIdAsync(recordingId, tenantId)
            ?? throw new InvalidOperationException($"Recording {recordingId} not found");
        
        recording.Delete(deletedByUserId, request.Reason);
        await _recordingRepository.UpdateAsync(recording);
        
        return MapToRecordingResponse(recording);
    }
    
    public async Task<RecordingResponse?> GetRecordingByIdAsync(Guid recordingId, string tenantId)
    {
        var recording = await _recordingRepository.GetByIdAsync(recordingId, tenantId);
        return recording != null ? MapToRecordingResponse(recording) : null;
    }
    
    public async Task<RecordingResponse?> GetRecordingBySessionIdAsync(Guid sessionId, string tenantId)
    {
        var recording = await _recordingRepository.GetBySessionIdAsync(sessionId, tenantId);
        return recording != null ? MapToRecordingResponse(recording) : null;
    }
    
    public async Task<IEnumerable<RecordingResponse>> GetAvailableRecordingsAsync(string tenantId, int skip = 0, int take = 50)
    {
        var recordings = await _recordingRepository.GetAvailableRecordingsAsync(tenantId, skip, take);
        return recordings.Select(MapToRecordingResponse);
    }
    
    private static RecordingResponse MapToRecordingResponse(TelemedicineRecording recording)
    {
        return new RecordingResponse
        {
            Id = recording.Id,
            SessionId = recording.SessionId,
            Status = recording.Status.ToString(),
            RecordingStartedAt = recording.RecordingStartedAt,
            RecordingCompletedAt = recording.RecordingCompletedAt,
            DurationSeconds = recording.DurationSeconds,
            FileSizeBytes = recording.FileSizeBytes,
            FormattedFileSize = recording.GetFormattedFileSize(),
            FormattedDuration = recording.GetFormattedDuration(),
            IsEncrypted = recording.IsEncrypted,
            RetentionUntil = recording.RetentionUntil,
            IsDeleted = recording.IsDeleted
        };
    }
}
