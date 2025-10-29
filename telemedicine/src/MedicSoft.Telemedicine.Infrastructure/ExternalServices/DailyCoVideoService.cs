using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MedicSoft.Telemedicine.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MedicSoft.Telemedicine.Infrastructure.ExternalServices;

/// <summary>
/// Daily.co video service implementation
/// Documentation: https://docs.daily.co/reference/rest-api
/// </summary>
public class DailyCoVideoService : IVideoCallService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.daily.co/v1";

    public DailyCoVideoService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = configuration["DailyCo:ApiKey"] ?? throw new InvalidOperationException("DailyCo:ApiKey not configured");
        
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<VideoRoomInfo> CreateRoomAsync(string roomName, int expirationHours = 1)
    {
        var expirationTime = DateTimeOffset.UtcNow.AddHours(expirationHours).ToUnixTimeSeconds();
        
        var request = new
        {
            name = roomName,
            properties = new
            {
                enable_chat = true,
                enable_screenshare = true,
                enable_recording = "cloud",
                enable_knocking = false,
                start_video_off = false,
                start_audio_off = false,
                exp = expirationTime,
                eject_at_room_exp = true
            }
        };

        var response = await _httpClient.PostAsJsonAsync("/rooms", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DailyRoomResponse>();
        if (result == null)
            throw new InvalidOperationException("Failed to parse Daily.co response");

        return new VideoRoomInfo
        {
            RoomName = result.Name,
            RoomUrl = result.Url,
            ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expirationTime).UtcDateTime
        };
    }

    public async Task<string> GenerateTokenAsync(string roomName, string userId, string userName, int expirationMinutes = 60)
    {
        var expirationTime = DateTimeOffset.UtcNow.AddMinutes(expirationMinutes).ToUnixTimeSeconds();
        
        var request = new
        {
            properties = new
            {
                room_name = roomName,
                user_name = userName,
                user_id = userId,
                exp = expirationTime,
                enable_recording = "cloud",
                enable_screenshare = true
            }
        };

        var response = await _httpClient.PostAsJsonAsync("/meeting-tokens", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DailyTokenResponse>();
        if (result == null || string.IsNullOrWhiteSpace(result.Token))
            throw new InvalidOperationException("Failed to generate token");

        return result.Token;
    }

    public async Task DeleteRoomAsync(string roomName)
    {
        var response = await _httpClient.DeleteAsync($"/rooms/{roomName}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<string?> GetRecordingUrlAsync(string roomName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/recordings?room_name={roomName}");
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<DailyRecordingsResponse>();
            if (result?.Data == null || result.Data.Length == 0)
                return null;

            // Return the most recent recording
            var latestRecording = result.Data
                .OrderByDescending(r => r.StartedAt)
                .FirstOrDefault();

            return latestRecording?.DownloadUrl;
        }
        catch
        {
            return null;
        }
    }

    #region Response DTOs
    
    private class DailyRoomResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
        
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; } = string.Empty;
    }

    private class DailyTokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }

    private class DailyRecordingsResponse
    {
        [JsonPropertyName("data")]
        public DailyRecording[]? Data { get; set; }
    }

    private class DailyRecording
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        
        [JsonPropertyName("room_name")]
        public string RoomName { get; set; } = string.Empty;
        
        [JsonPropertyName("start_ts")]
        public long StartedAt { get; set; }
        
        [JsonPropertyName("download_link")]
        public string? DownloadUrl { get; set; }
    }
    
    #endregion
}
