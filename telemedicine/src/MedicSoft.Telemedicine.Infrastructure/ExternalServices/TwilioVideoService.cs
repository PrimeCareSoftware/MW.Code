using System.Text.Json.Serialization;
using MedicSoft.Telemedicine.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Video.V1;
using Twilio.Rest.Video.V1.Room;

namespace MedicSoft.Telemedicine.Infrastructure.ExternalServices;

/// <summary>
/// Twilio Video service implementation
/// Documentation: https://www.twilio.com/docs/video/api
/// </summary>
public class TwilioVideoService : IVideoCallService
{
    private readonly ILogger<TwilioVideoService> _logger;
    private readonly string _accountSid;
    private readonly string _apiKeySid;
    private readonly string _apiKeySecret;
    private const int DefaultExpirationHours = 1;
    private const int DefaultTokenExpirationMinutes = 60;

    public TwilioVideoService(
        ILogger<TwilioVideoService> logger,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Get Twilio configuration from appsettings
        _accountSid = configuration["TwilioVideo:AccountSid"] 
            ?? throw new InvalidOperationException("TwilioVideo:AccountSid not configured");
        _apiKeySid = configuration["TwilioVideo:ApiKeySid"] 
            ?? throw new InvalidOperationException("TwilioVideo:ApiKeySid not configured");
        _apiKeySecret = configuration["TwilioVideo:ApiKeySecret"] 
            ?? throw new InvalidOperationException("TwilioVideo:ApiKeySecret not configured");

        // Initialize Twilio client
        TwilioClient.Init(_accountSid, _apiKeySecret);
        
        _logger.LogInformation("Twilio Video Service initialized with Account SID: {AccountSid}", 
            MaskSensitiveData(_accountSid));
    }

    public async Task<VideoRoomInfo> CreateRoomAsync(string roomName, int expirationHours = DefaultExpirationHours)
    {
        if (string.IsNullOrWhiteSpace(roomName))
            throw new ArgumentException("Room name cannot be null or empty", nameof(roomName));

        try
        {
            _logger.LogInformation("Creating Twilio video room: {RoomName}", roomName);

            // Create a room with the specified name
            var room = await RoomResource.CreateAsync(
                uniqueName: roomName,
                type: RoomResource.RoomTypeEnum.Group, // Group room supports up to 50 participants
                recordParticipantsOnConnect: false, // Recording must be enabled explicitly
                maxParticipants: 10, // Limit for medical consultations
                statusCallback: null // Can be configured for webhooks
            );

            var expiresAt = DateTime.UtcNow.AddHours(expirationHours);

            _logger.LogInformation("Twilio room created successfully: {RoomSid}, Name: {RoomName}", 
                room.Sid, room.UniqueName);

            return new VideoRoomInfo
            {
                RoomName = room.UniqueName,
                RoomUrl = $"https://video.twilio.com/v1/Rooms/{room.Sid}", // This is the API URL, clients use SDK
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create Twilio video room: {RoomName}", roomName);
            throw new InvalidOperationException($"Failed to create video room: {ex.Message}", ex);
        }
    }

    public Task<string> GenerateTokenAsync(string roomName, string userId, string userName, int expirationMinutes = DefaultTokenExpirationMinutes)
    {
        if (string.IsNullOrWhiteSpace(roomName))
            throw new ArgumentException("Room name cannot be null or empty", nameof(roomName));
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("User name cannot be null or empty", nameof(userName));

        try
        {
            _logger.LogInformation("Generating Twilio access token for user {UserId} in room {RoomName}", 
                userId, roomName);

            // Create Access Token with Video Grant
            var grants = new HashSet<IGrant>
            {
                new VideoGrant { Room = roomName }
            };

            var token = new Token(
                _accountSid,
                _apiKeySid,
                _apiKeySecret,
                identity: userId, // Unique identifier for the participant
                expiration: DateTime.UtcNow.AddMinutes(expirationMinutes),
                nbf: DateTime.UtcNow,
                grants: grants
            );

            var jwtToken = token.ToJwt();

            _logger.LogInformation("Twilio access token generated successfully for user {UserId}", userId);

            return Task.FromResult(jwtToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate Twilio access token for user {UserId}", userId);
            throw new InvalidOperationException($"Failed to generate access token: {ex.Message}", ex);
        }
    }

    public async Task DeleteRoomAsync(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName))
            throw new ArgumentException("Room name cannot be null or empty", nameof(roomName));

        try
        {
            _logger.LogInformation("Deleting Twilio video room: {RoomName}", roomName);

            // Find the room by unique name
            var rooms = await RoomResource.ReadAsync(uniqueName: roomName, limit: 1);
            var room = rooms.FirstOrDefault();

            if (room == null)
            {
                _logger.LogWarning("Twilio room not found: {RoomName}", roomName);
                return;
            }

            // Complete the room (Twilio doesn't support direct deletion, but completing ends it)
            await RoomResource.UpdateAsync(
                pathSid: room.Sid,
                status: RoomResource.RoomStatusEnum.Completed
            );

            _logger.LogInformation("Twilio room completed successfully: {RoomName}", roomName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete Twilio video room: {RoomName}", roomName);
            throw new InvalidOperationException($"Failed to delete video room: {ex.Message}", ex);
        }
    }

    public Task<string?> GetRecordingUrlAsync(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName))
            throw new ArgumentException("Room name cannot be null or empty", nameof(roomName));

        // Note: Twilio Video recordings require additional setup and webhook configuration
        // For production use, implement the recording retrieval logic based on your storage setup
        // Recordings are typically stored in cloud storage and accessed via signed URLs
        
        _logger.LogInformation("GetRecordingUrlAsync called for room: {RoomName}. Recording retrieval not yet implemented for Twilio provider.", roomName);
        
        // Return null indicating no recording available
        // TODO: Implement recording retrieval when Twilio recording storage is configured
        return Task.FromResult<string?>(null);
    }

    private static string MaskSensitiveData(string data)
    {
        if (string.IsNullOrWhiteSpace(data) || data.Length < 8)
            return "****";

        return $"{data.Substring(0, 4)}****{data.Substring(data.Length - 4)}";
    }
}
