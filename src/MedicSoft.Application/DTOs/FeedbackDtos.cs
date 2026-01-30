using System;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for creating user feedback
    /// </summary>
    public record CreateUserFeedbackDto(
        FeedbackType Type,
        FeedbackSeverity? Severity,
        string Page,
        string Description,
        string? ScreenshotUrl,
        string BrowserName,
        string BrowserVersion,
        string OperatingSystem,
        string ScreenResolution
    );

    /// <summary>
    /// DTO for reading user feedback
    /// </summary>
    public record UserFeedbackDto(
        Guid Id,
        string UserId,
        string UserName,
        FeedbackType Type,
        FeedbackSeverity? Severity,
        string Page,
        string Description,
        string? ScreenshotUrl,
        string BrowserName,
        string BrowserVersion,
        string OperatingSystem,
        string ScreenResolution,
        FeedbackStatus Status,
        string? ResolvedBy,
        DateTime? ResolvedAt,
        string? ResolutionNotes,
        DateTime CreatedAt
    );

    /// <summary>
    /// DTO for updating feedback status
    /// </summary>
    public record UpdateFeedbackStatusDto(
        FeedbackStatus Status,
        string? ResolutionNotes
    );

    /// <summary>
    /// DTO for feedback statistics
    /// </summary>
    public record FeedbackStatisticsDto(
        int TotalFeedback,
        int NewFeedback,
        int InProgressFeedback,
        int ResolvedFeedback,
        int WontFixFeedback,
        int CriticalBugs,
        int HighPriorityBugs,
        int FeatureRequests,
        int UxIssues,
        double AverageResolutionTimeHours
    );

    /// <summary>
    /// DTO for creating NPS survey response
    /// </summary>
    public record CreateNpsSurveyDto(
        int Score,
        string? Feedback
    );

    /// <summary>
    /// DTO for reading NPS survey
    /// </summary>
    public record NpsSurveyDto(
        Guid Id,
        string UserId,
        string UserName,
        int Score,
        string? Feedback,
        NpsCategory Category,
        DateTime RespondedAt,
        string? UserRole,
        int DaysAsUser
    );

    /// <summary>
    /// DTO for NPS statistics
    /// </summary>
    public record NpsStatisticsDto(
        int TotalResponses,
        double NpsScore, // Calculated as (% Promoters - % Detractors)
        int Promoters,
        int Passives,
        int Detractors,
        double AverageScore,
        double PromoterPercentage,
        double PassivePercentage,
        double DetractorPercentage
    );
}
