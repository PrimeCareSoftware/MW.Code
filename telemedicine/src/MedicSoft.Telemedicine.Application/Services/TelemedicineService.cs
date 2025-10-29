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
    private readonly IVideoCallService _videoCallService;

    public TelemedicineService(
        ITelemedicineSessionRepository sessionRepository,
        IVideoCallService videoCallService)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
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
            RoomUrl = session.RoomUrl,
            Status = session.Status.ToString(),
            CreatedAt = session.CreatedAt,
            DurationMinutes = session.Duration?.GetDurationInMinutes(),
            RecordingUrl = session.RecordingUrl
        };
    }
}
