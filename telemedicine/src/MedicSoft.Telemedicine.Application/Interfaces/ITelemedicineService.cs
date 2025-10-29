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
}
