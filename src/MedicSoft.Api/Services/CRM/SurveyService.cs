using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.DTOs.Common;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;
using System.Text.Json;

namespace MedicSoft.Api.Services.CRM
{
    public class SurveyService : ISurveyService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<SurveyService> _logger;

        public SurveyService(
            MedicSoftDbContext context,
            ILogger<SurveyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<SurveyDto> CreateAsync(CreateSurveyDto dto, string tenantId)
        {
            var survey = new Survey(
                dto.Name,
                dto.Description,
                dto.Type,
                tenantId);

            if (dto.TriggerStage.HasValue || !string.IsNullOrEmpty(dto.TriggerEvent) || dto.DelayHours.HasValue)
            {
                survey.ConfigureTrigger(dto.TriggerStage, dto.TriggerEvent, dto.DelayHours);
            }

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            foreach (var questionDto in dto.Questions)
            {
                var question = new SurveyQuestion(
                    survey.Id,
                    questionDto.Order,
                    questionDto.Text,
                    questionDto.Type,
                    questionDto.IsRequired,
                    tenantId);

                if (questionDto.Type == QuestionType.MultipleChoice && questionDto.Options.Any())
                {
                    var optionsJson = JsonSerializer.Serialize(questionDto.Options);
                    question.SetOptions(optionsJson);
                }

                survey.AddQuestion(question);
                _context.SurveyQuestions.Add(question);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Created survey {SurveyId} with {QuestionCount} questions for tenant {TenantId}",
                survey.Id, dto.Questions.Count, tenantId);

            return MapToDto(survey);
        }

        public async Task<SurveyDto> UpdateAsync(Guid id, UpdateSurveyDto dto, string tenantId)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (survey == null)
                throw new KeyNotFoundException($"Survey {id} not found");

            if (dto.TriggerStage.HasValue || dto.TriggerEvent != null || dto.DelayHours.HasValue)
            {
                survey.ConfigureTrigger(
                    dto.TriggerStage ?? survey.TriggerStage,
                    dto.TriggerEvent ?? survey.TriggerEvent,
                    dto.DelayHours ?? survey.DelayHours);
            }

            if (dto.Questions != null)
            {
                var existingQuestions = survey.Questions.ToList();
                foreach (var question in existingQuestions)
                {
                    _context.SurveyQuestions.Remove(question);
                }

                foreach (var questionDto in dto.Questions)
                {
                    var question = new SurveyQuestion(
                        survey.Id,
                        questionDto.Order,
                        questionDto.Text,
                        questionDto.Type,
                        questionDto.IsRequired,
                        tenantId);

                    if (questionDto.Type == QuestionType.MultipleChoice && questionDto.Options.Any())
                    {
                        var optionsJson = JsonSerializer.Serialize(questionDto.Options);
                        question.SetOptions(optionsJson);
                    }

                    survey.AddQuestion(question);
                    _context.SurveyQuestions.Add(question);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated survey {SurveyId}", id);

            return MapToDto(survey);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var survey = await _context.Surveys
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (survey == null)
                return false;

            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted survey {SurveyId}", id);

            return true;
        }

        public async Task<SurveyDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var survey = await _context.Surveys
                .AsNoTracking()
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            return survey == null ? null : MapToDto(survey);
        }

        public async Task<IEnumerable<SurveyDto>> GetAllAsync(string tenantId)
        {
            var surveys = await _context.Surveys
                .AsNoTracking()
                .Include(s => s.Questions)
                .Where(s => s.TenantId == tenantId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return surveys.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<SurveyDto>> GetAllPagedAsync(string tenantId, int pageNumber = 1, int pageSize = 25)
        {
            var query = _context.Surveys
                .AsNoTracking()
                .Include(s => s.Questions)
                .Where(s => s.TenantId == tenantId)
                .OrderByDescending(s => s.CreatedAt);

            var totalCount = await query.CountAsync();
            
            var surveys = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var surveyDtos = surveys.Select(MapToDto).ToList();

            return new PagedResult<SurveyDto>(surveyDtos, totalCount, pageNumber, pageSize);
        }

        public async Task<IEnumerable<SurveyDto>> GetActiveAsync(string tenantId)
        {
            var surveys = await _context.Surveys
                .AsNoTracking()
                .Include(s => s.Questions)
                .Where(s => s.TenantId == tenantId && s.IsActive)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return surveys.Select(MapToDto).ToList();
        }

        public async Task<PagedResult<SurveyDto>> GetActivePagedAsync(string tenantId, int pageNumber = 1, int pageSize = 25)
        {
            var query = _context.Surveys
                .AsNoTracking()
                .Include(s => s.Questions)
                .Where(s => s.TenantId == tenantId && s.IsActive)
                .OrderByDescending(s => s.CreatedAt);

            var totalCount = await query.CountAsync();
            
            var surveys = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var surveyDtos = surveys.Select(MapToDto).ToList();

            return new PagedResult<SurveyDto>(surveyDtos, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> ActivateAsync(Guid id, string tenantId)
        {
            var survey = await _context.Surveys
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (survey == null)
                return false;

            survey.Activate();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Activated survey {SurveyId}", id);

            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, string tenantId)
        {
            var survey = await _context.Surveys
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (survey == null)
                return false;

            survey.Deactivate();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deactivated survey {SurveyId}", id);

            return true;
        }

        public async Task<SurveyResponseDto> SubmitResponseAsync(SubmitSurveyResponseDto dto, string tenantId)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == dto.SurveyId && s.TenantId == tenantId);

            if (survey == null)
                throw new KeyNotFoundException($"Survey {dto.SurveyId} not found");

            if (!survey.IsActive)
                throw new InvalidOperationException($"Survey {dto.SurveyId} is not active");

            var surveyResponse = new SurveyResponse(
                dto.SurveyId,
                dto.PatientId,
                tenantId);

            _context.SurveyResponses.Add(surveyResponse);
            await _context.SaveChangesAsync();

            int? npsScore = null;
            int? csatScore = null;

            foreach (var responseDto in dto.Responses)
            {
                var question = survey.Questions.FirstOrDefault(q => q.Id == responseDto.QuestionId);
                if (question == null)
                    continue;

                var questionResponse = new SurveyQuestionResponse(
                    surveyResponse.Id,
                    responseDto.QuestionId,
                    tenantId);

                if (!string.IsNullOrEmpty(responseDto.TextAnswer))
                {
                    questionResponse.SetTextAnswer(responseDto.TextAnswer);
                }

                if (responseDto.NumericAnswer.HasValue)
                {
                    questionResponse.SetNumericAnswer(responseDto.NumericAnswer.Value);

                    if (survey.Type == SurveyType.NPS && question.Type == QuestionType.NumericScale)
                    {
                        npsScore = responseDto.NumericAnswer.Value;
                    }
                    else if (survey.Type == SurveyType.CSAT && question.Type == QuestionType.StarRating)
                    {
                        csatScore = responseDto.NumericAnswer.Value;
                    }
                }

                surveyResponse.AddQuestionResponse(questionResponse);
                _context.SurveyQuestionResponses.Add(questionResponse);
            }

            surveyResponse.Complete(npsScore, csatScore);
            survey.RecordResponse(surveyResponse);

            await _context.SaveChangesAsync();

            await RecalculateMetricsAsync(dto.SurveyId, tenantId);

            _logger.LogInformation("Submitted survey response {ResponseId} for survey {SurveyId} by patient {PatientId}",
                surveyResponse.Id, dto.SurveyId, dto.PatientId);

            return await MapResponseToDtoAsync(surveyResponse);
        }

        public async Task<SurveyResponseDto?> GetResponseAsync(Guid responseId, string tenantId)
        {
            var response = await _context.SurveyResponses
                .Include(r => r.Survey)
                .Include(r => r.Patient)
                .Include(r => r.QuestionResponses)
                    .ThenInclude(qr => qr.SurveyQuestion)
                .FirstOrDefaultAsync(r => r.Id == responseId && r.TenantId == tenantId);

            return response == null ? null : await MapResponseToDtoAsync(response);
        }

        public async Task<IEnumerable<SurveyResponseDto>> GetPatientResponsesAsync(Guid patientId, string tenantId)
        {
            var responses = await _context.SurveyResponses
                .AsNoTracking()
                .Include(r => r.Survey)
                .Include(r => r.Patient)
                .Include(r => r.QuestionResponses)
                    .ThenInclude(qr => qr.SurveyQuestion)
                .Where(r => r.PatientId == patientId && r.TenantId == tenantId)
                .OrderByDescending(r => r.CompletedAt ?? r.StartedAt)
                .ToListAsync();

            return await Task.WhenAll(responses.Select(MapResponseToDtoAsync));
        }

        public async Task<IEnumerable<SurveyResponseDto>> GetSurveyResponsesAsync(Guid surveyId, string tenantId)
        {
            var responses = await _context.SurveyResponses
                .AsNoTracking()
                .Include(r => r.Survey)
                .Include(r => r.Patient)
                .Include(r => r.QuestionResponses)
                    .ThenInclude(qr => qr.SurveyQuestion)
                .Where(r => r.SurveyId == surveyId && r.TenantId == tenantId)
                .OrderByDescending(r => r.CompletedAt ?? r.StartedAt)
                .ToListAsync();

            return await Task.WhenAll(responses.Select(MapResponseToDtoAsync));
        }

        public async Task<SurveyAnalyticsDto?> GetAnalyticsAsync(Guid surveyId, string tenantId)
        {
            var survey = await _context.Surveys
                .AsNoTracking()
                .Include(s => s.Responses)
                    .ThenInclude(r => r.QuestionResponses)
                .Include(s => s.Responses)
                    .ThenInclude(r => r.Patient)
                .FirstOrDefaultAsync(s => s.Id == surveyId && s.TenantId == tenantId);

            if (survey == null)
                return null;

            var completedResponses = survey.Responses.Where(r => r.IsCompleted).ToList();
            var totalResponses = completedResponses.Count;

            var analytics = new SurveyAnalyticsDto
            {
                SurveyId = survey.Id,
                SurveyName = survey.Name,
                Type = survey.Type,
                TotalSent = totalResponses,
                TotalResponses = totalResponses,
                ResponseRate = totalResponses > 0 ? 100.0 : 0.0,
                AverageScore = survey.AverageScore
            };

            if (survey.Type == SurveyType.NPS)
            {
                var npsResponses = completedResponses
                    .Where(r => r.NpsScore.HasValue)
                    .Select(r => r.NpsScore!.Value)
                    .ToList();

                if (npsResponses.Any())
                {
                    var promoters = npsResponses.Count(s => s >= 9);
                    var passives = npsResponses.Count(s => s >= 7 && s <= 8);
                    var detractors = npsResponses.Count(s => s <= 6);

                    analytics.Promoters = promoters;
                    analytics.Passives = passives;
                    analytics.Detractors = detractors;
                    analytics.NpsScore = ((double)(promoters - detractors) / npsResponses.Count) * 100;
                }
            }
            else if (survey.Type == SurveyType.CSAT)
            {
                var csatResponses = completedResponses
                    .Where(r => r.CsatScore.HasValue)
                    .Select(r => r.CsatScore!.Value)
                    .ToList();

                if (csatResponses.Any())
                {
                    analytics.VerySatisfied = csatResponses.Count(s => s == 5);
                    analytics.Satisfied = csatResponses.Count(s => s == 4);
                    analytics.Neutral = csatResponses.Count(s => s == 3);
                    analytics.Unsatisfied = csatResponses.Count(s => s == 2);
                    analytics.VeryUnsatisfied = csatResponses.Count(s => s == 1);
                }
            }

            var recentResponses = completedResponses
                .OrderByDescending(r => r.CompletedAt)
                .Take(10)
                .ToList();

            analytics.RecentResponses = (await Task.WhenAll(recentResponses.Select(MapResponseToDtoAsync))).ToList();

            return analytics;
        }

        public async Task RecalculateMetricsAsync(Guid surveyId, string tenantId)
        {
            var survey = await _context.Surveys
                .Include(s => s.Responses)
                .FirstOrDefaultAsync(s => s.Id == surveyId && s.TenantId == tenantId);

            if (survey == null)
            {
                _logger.LogWarning("Survey {SurveyId} not found for recalculation", surveyId);
                return;
            }

            survey.RecalculateMetrics();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Recalculated metrics for survey {SurveyId}: Average={Average}, Total={Total}",
                surveyId, survey.AverageScore, survey.TotalResponses);
        }

        public async Task SendSurveyToPatientAsync(Guid surveyId, Guid patientId, string tenantId)
        {
            var survey = await _context.Surveys
                .FirstOrDefaultAsync(s => s.Id == surveyId && s.TenantId == tenantId);

            if (survey == null)
            {
                _logger.LogWarning("Survey {SurveyId} not found", surveyId);
                return;
            }

            if (!survey.IsActive)
            {
                _logger.LogWarning("Survey {SurveyId} is not active", surveyId);
                return;
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId && p.TenantId == tenantId);
            
            if (patient == null)
            {
                _logger.LogWarning("Patient {PatientId} not found", patientId);
                return;
            }

            _logger.LogInformation("Sending survey {SurveyId} to patient {PatientId}", surveyId, patientId);
        }

        private SurveyDto MapToDto(Survey survey)
        {
            var questions = survey.Questions.OrderBy(q => q.Order).ToList();

            return new SurveyDto
            {
                Id = survey.Id,
                Name = survey.Name,
                Description = survey.Description,
                Type = survey.Type,
                TypeName = survey.Type.ToString(),
                IsActive = survey.IsActive,
                TriggerStage = survey.TriggerStage,
                TriggerEvent = survey.TriggerEvent,
                DelayHours = survey.DelayHours,
                AverageScore = survey.AverageScore,
                TotalResponses = survey.TotalResponses,
                ResponseRate = survey.ResponseRate,
                Questions = questions.Select(q => MapQuestionToDto(q)).ToList(),
                CreatedAt = survey.CreatedAt,
                UpdatedAt = survey.UpdatedAt ?? survey.CreatedAt
            };
        }

        private SurveyQuestionDto MapQuestionToDto(SurveyQuestion question)
        {
            var options = new List<string>();
            if (!string.IsNullOrEmpty(question.OptionsJson))
            {
                try
                {
                    options = JsonSerializer.Deserialize<List<string>>(question.OptionsJson) ?? new List<string>();
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize options for question {QuestionId}", question.Id);
                }
            }

            return new SurveyQuestionDto
            {
                Id = question.Id,
                Order = question.Order,
                Text = question.QuestionText,
                Type = question.Type,
                TypeName = question.Type.ToString(),
                IsRequired = question.IsRequired,
                Options = options
            };
        }

        private async Task<SurveyResponseDto> MapResponseToDtoAsync(SurveyResponse response)
        {
            var patient = response.Patient ?? await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == response.PatientId && p.TenantId == response.TenantId);
            
            var survey = response.Survey ?? await _context.Surveys
                .FirstOrDefaultAsync(s => s.Id == response.SurveyId && s.TenantId == response.TenantId);

            var questionResponses = response.QuestionResponses
                .OrderBy(qr => qr.AnsweredAt)
                .ToList();

            return new SurveyResponseDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                SurveyName = survey?.Name ?? "Unknown",
                PatientId = response.PatientId,
                PatientName = patient?.Name ?? "Unknown",
                StartedAt = response.StartedAt,
                CompletedAt = response.CompletedAt,
                IsCompleted = response.IsCompleted,
                NpsScore = response.NpsScore,
                CsatScore = response.CsatScore,
                QuestionResponses = questionResponses.Select(qr => MapQuestionResponseToDto(qr)).ToList()
            };
        }

        private SurveyQuestionResponseDto MapQuestionResponseToDto(SurveyQuestionResponse questionResponse)
        {
            return new SurveyQuestionResponseDto
            {
                Id = questionResponse.Id,
                QuestionId = questionResponse.SurveyQuestionId,
                QuestionText = questionResponse.SurveyQuestion?.QuestionText ?? "Unknown",
                TextAnswer = questionResponse.TextAnswer,
                NumericAnswer = questionResponse.NumericAnswer,
                BooleanAnswer = null
            };
        }
    }
}
