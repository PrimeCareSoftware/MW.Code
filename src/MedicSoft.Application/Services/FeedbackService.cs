using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IFeedbackService
    {
        Task<UserFeedbackDto> CreateFeedbackAsync(CreateUserFeedbackDto dto, string userId, string tenantId);
        Task<UserFeedbackDto?> GetFeedbackByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<UserFeedbackDto>> GetAllFeedbackAsync(string tenantId);
        Task<IEnumerable<UserFeedbackDto>> GetFeedbackByUserAsync(string userId, string tenantId);
        Task<IEnumerable<UserFeedbackDto>> GetFeedbackByStatusAsync(FeedbackStatus status, string tenantId);
        Task<UserFeedbackDto> UpdateFeedbackStatusAsync(Guid id, UpdateFeedbackStatusDto dto, string resolvedBy, string tenantId);
        Task<FeedbackStatisticsDto> GetStatisticsAsync(string tenantId);
    }

    public interface INpsSurveyService
    {
        Task<NpsSurveyDto> CreateSurveyResponseAsync(CreateNpsSurveyDto dto, string userId, string tenantId);
        Task<NpsSurveyDto?> GetSurveyByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<NpsSurveyDto>> GetAllSurveysAsync(string tenantId);
        Task<bool> HasUserRespondedAsync(string userId, string tenantId);
        Task<NpsStatisticsDto> GetStatisticsAsync(string tenantId);
    }

    public class FeedbackService : IFeedbackService
    {
        private readonly IUserFeedbackRepository _feedbackRepository;
        private readonly IUserRepository _userRepository;

        public FeedbackService(
            IUserFeedbackRepository feedbackRepository,
            IUserRepository userRepository)
        {
            _feedbackRepository = feedbackRepository;
            _userRepository = userRepository;
        }

        public async Task<UserFeedbackDto> CreateFeedbackAsync(CreateUserFeedbackDto dto, string userId, string tenantId)
        {
            var feedback = new UserFeedback(
                userId,
                dto.Type,
                dto.Page,
                dto.Description,
                dto.BrowserName,
                dto.BrowserVersion,
                dto.OperatingSystem,
                dto.ScreenResolution,
                tenantId,
                dto.Severity,
                dto.ScreenshotUrl
            );

            await _feedbackRepository.AddAsync(feedback);
            
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            return MapToDto(feedback, user?.Name ?? "Unknown");
        }

        public async Task<UserFeedbackDto?> GetFeedbackByIdAsync(Guid id, string tenantId)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id, tenantId);
            if (feedback == null) return null;

            var user = await _userRepository.GetByIdAsync(feedback.UserId, tenantId);
            return MapToDto(feedback, user?.Name ?? "Unknown");
        }

        public async Task<IEnumerable<UserFeedbackDto>> GetAllFeedbackAsync(string tenantId)
        {
            var feedbacks = await _feedbackRepository.GetAllAsync(tenantId);
            var result = new List<UserFeedbackDto>();

            foreach (var feedback in feedbacks)
            {
                var user = await _userRepository.GetByIdAsync(feedback.UserId, tenantId);
                result.Add(MapToDto(feedback, user?.Name ?? "Unknown"));
            }

            return result;
        }

        public async Task<IEnumerable<UserFeedbackDto>> GetFeedbackByUserAsync(string userId, string tenantId)
        {
            var feedbacks = await _feedbackRepository.GetByUserIdAsync(userId, tenantId);
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            var userName = user?.Name ?? "Unknown";

            return feedbacks.Select(f => MapToDto(f, userName));
        }

        public async Task<IEnumerable<UserFeedbackDto>> GetFeedbackByStatusAsync(FeedbackStatus status, string tenantId)
        {
            var feedbacks = await _feedbackRepository.GetByStatusAsync(status, tenantId);
            var result = new List<UserFeedbackDto>();

            foreach (var feedback in feedbacks)
            {
                var user = await _userRepository.GetByIdAsync(feedback.UserId, tenantId);
                result.Add(MapToDto(feedback, user?.Name ?? "Unknown"));
            }

            return result;
        }

        public async Task<UserFeedbackDto> UpdateFeedbackStatusAsync(Guid id, UpdateFeedbackStatusDto dto, string resolvedBy, string tenantId)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id, tenantId);
            if (feedback == null)
                throw new InvalidOperationException("Feedback not found");

            feedback.UpdateStatus(dto.Status, resolvedBy, dto.ResolutionNotes);
            await _feedbackRepository.UpdateAsync(feedback);

            var user = await _userRepository.GetByIdAsync(feedback.UserId, tenantId);
            return MapToDto(feedback, user?.Name ?? "Unknown");
        }

        public async Task<FeedbackStatisticsDto> GetStatisticsAsync(string tenantId)
        {
            var allFeedback = await _feedbackRepository.GetAllAsync(tenantId);
            var feedbackList = allFeedback.ToList();

            var newCount = await _feedbackRepository.GetCountByStatusAsync(FeedbackStatus.New, tenantId);
            var inProgressCount = await _feedbackRepository.GetCountByStatusAsync(FeedbackStatus.InProgress, tenantId);
            var resolvedCount = await _feedbackRepository.GetCountByStatusAsync(FeedbackStatus.Resolved, tenantId);
            var wontFixCount = await _feedbackRepository.GetCountByStatusAsync(FeedbackStatus.WontFix, tenantId);

            var criticalBugs = feedbackList.Count(f => f.Type == FeedbackType.Bug && f.Severity == FeedbackSeverity.Critical);
            var highPriorityBugs = feedbackList.Count(f => f.Type == FeedbackType.Bug && f.Severity == FeedbackSeverity.High);
            var featureRequests = feedbackList.Count(f => f.Type == FeedbackType.FeatureRequest);
            var uxIssues = feedbackList.Count(f => f.Type == FeedbackType.UxIssue);

            var resolvedFeedback = feedbackList.Where(f => f.ResolvedAt.HasValue).ToList();
            var avgResolutionTimeHours = resolvedFeedback.Any()
                ? resolvedFeedback.Average(f => (f.ResolvedAt!.Value - f.CreatedAt).TotalHours)
                : 0;

            return new FeedbackStatisticsDto(
                feedbackList.Count,
                newCount,
                inProgressCount,
                resolvedCount,
                wontFixCount,
                criticalBugs,
                highPriorityBugs,
                featureRequests,
                uxIssues,
                avgResolutionTimeHours
            );
        }

        private UserFeedbackDto MapToDto(UserFeedback feedback, string userName)
        {
            return new UserFeedbackDto(
                feedback.Id,
                feedback.UserId,
                userName,
                feedback.Type,
                feedback.Severity,
                feedback.Page,
                feedback.Description,
                feedback.ScreenshotUrl,
                feedback.BrowserName,
                feedback.BrowserVersion,
                feedback.OperatingSystem,
                feedback.ScreenResolution,
                feedback.Status,
                feedback.ResolvedBy,
                feedback.ResolvedAt,
                feedback.ResolutionNotes,
                feedback.CreatedAt
            );
        }
    }

    public class NpsSurveyService : INpsSurveyService
    {
        private readonly INpsSurveyRepository _surveyRepository;
        private readonly IUserRepository _userRepository;

        public NpsSurveyService(
            INpsSurveyRepository surveyRepository,
            IUserRepository userRepository)
        {
            _surveyRepository = surveyRepository;
            _userRepository = userRepository;
        }

        public async Task<NpsSurveyDto> CreateSurveyResponseAsync(CreateNpsSurveyDto dto, string userId, string tenantId)
        {
            // Check if user already responded
            var hasResponded = await _surveyRepository.HasUserRespondedAsync(userId, tenantId);
            if (hasResponded)
                throw new InvalidOperationException("User has already responded to NPS survey");

            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            var daysAsUser = user != null ? (DateTime.UtcNow - user.CreatedAt).Days : 0;
            var userRole = user?.Role ?? "Unknown";

            var survey = new NpsSurvey(
                userId,
                dto.Score,
                tenantId,
                dto.Feedback,
                userRole,
                daysAsUser
            );

            await _surveyRepository.AddAsync(survey);
            
            return MapToDto(survey, user?.Name ?? "Unknown");
        }

        public async Task<NpsSurveyDto?> GetSurveyByIdAsync(Guid id, string tenantId)
        {
            var survey = await _surveyRepository.GetByIdAsync(id, tenantId);
            if (survey == null) return null;

            var user = await _userRepository.GetByIdAsync(survey.UserId, tenantId);
            return MapToDto(survey, user?.Name ?? "Unknown");
        }

        public async Task<IEnumerable<NpsSurveyDto>> GetAllSurveysAsync(string tenantId)
        {
            var surveys = await _surveyRepository.GetAllAsync(tenantId);
            var result = new List<NpsSurveyDto>();

            foreach (var survey in surveys)
            {
                var user = await _userRepository.GetByIdAsync(survey.UserId, tenantId);
                result.Add(MapToDto(survey, user?.Name ?? "Unknown"));
            }

            return result;
        }

        public async Task<bool> HasUserRespondedAsync(string userId, string tenantId)
        {
            return await _surveyRepository.HasUserRespondedAsync(userId, tenantId);
        }

        public async Task<NpsStatisticsDto> GetStatisticsAsync(string tenantId)
        {
            var allSurveys = await _surveyRepository.GetAllAsync(tenantId);
            var surveyList = allSurveys.ToList();

            if (!surveyList.Any())
            {
                return new NpsStatisticsDto(0, 0, 0, 0, 0, 0, 0, 0, 0);
            }

            var promoters = await _surveyRepository.GetCountByCategoryAsync(NpsCategory.Promoter, tenantId);
            var passives = await _surveyRepository.GetCountByCategoryAsync(NpsCategory.Passive, tenantId);
            var detractors = await _surveyRepository.GetCountByCategoryAsync(NpsCategory.Detractor, tenantId);

            var total = surveyList.Count;
            var promoterPercentage = (double)promoters / total * 100;
            var passivePercentage = (double)passives / total * 100;
            var detractorPercentage = (double)detractors / total * 100;

            var npsScore = promoterPercentage - detractorPercentage;
            var averageScore = surveyList.Average(s => s.Score);

            return new NpsStatisticsDto(
                total,
                npsScore,
                promoters,
                passives,
                detractors,
                averageScore,
                promoterPercentage,
                passivePercentage,
                detractorPercentage
            );
        }

        private NpsSurveyDto MapToDto(NpsSurvey survey, string userName)
        {
            return new NpsSurveyDto(
                survey.Id,
                survey.UserId,
                userName,
                survey.Score,
                survey.Feedback,
                survey.GetCategory(),
                survey.RespondedAt,
                survey.UserRole,
                survey.DaysAsUser
            );
        }
    }
}
