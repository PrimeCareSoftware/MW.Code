namespace MedicSoft.Telemedicine.Domain.Interfaces;

/// <summary>
/// Interface for video call service provider integration
/// Abstracts the external video service provider (Twilio).
/// </summary>
public interface IVideoCallService
{
    /// <summary>
    /// Creates a new video room
    /// </summary>
    /// <param name="roomName">Unique identifier for the room</param>
    /// <param name="expirationHours">Hours until room expires</param>
    /// <returns>Room URL and metadata</returns>
    Task<VideoRoomInfo> CreateRoomAsync(string roomName, int expirationHours = 1);
    
    /// <summary>
    /// Generates an access token for a participant
    /// </summary>
    /// <param name="roomName">Room to join</param>
    /// <param name="userId">User identifier</param>
    /// <param name="userName">Display name</param>
    /// <param name="expirationMinutes">Token validity in minutes</param>
    /// <returns>JWT token for room access</returns>
    Task<string> GenerateTokenAsync(string roomName, string userId, string userName, int expirationMinutes = 60);
    
    /// <summary>
    /// Deletes a video room
    /// </summary>
    /// <param name="roomName">Room to delete</param>
    Task DeleteRoomAsync(string roomName);
    
    /// <summary>
    /// Gets recording URL if available
    /// </summary>
    /// <param name="roomName">Room name</param>
    /// <returns>Recording URL or null</returns>
    Task<string?> GetRecordingUrlAsync(string roomName);
}

/// <summary>
/// Information about a video room
/// </summary>
public class VideoRoomInfo
{
    public string RoomName { get; set; } = string.Empty;
    public string RoomUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
