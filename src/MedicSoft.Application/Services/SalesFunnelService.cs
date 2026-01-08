using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.SalesFunnel;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface ISalesFunnelService
    {
        Task<TrackEventResponseDto> TrackEventAsync(TrackSalesFunnelEventDto eventDto, string? ipAddress, string? userAgent);
        Task<TrackEventResponseDto> MarkConversionAsync(MarkConversionDto conversionDto);
        Task<FunnelStatsDto> GetFunnelStatsAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<IncompleteSessionDto>> GetIncompleteSessions(int hoursOld = 24, int limit = 100);
        Task<IEnumerable<SalesFunnelMetricDto>> GetSessionMetricsAsync(string sessionId);
        Task<IEnumerable<SalesFunnelMetricDto>> GetRecentSessions(int limit = 100);
    }

    public class SalesFunnelService : ISalesFunnelService
    {
        private readonly ISalesFunnelMetricRepository _metricRepository;

        public SalesFunnelService(ISalesFunnelMetricRepository metricRepository)
        {
            _metricRepository = metricRepository;
        }

        public async Task<TrackEventResponseDto> TrackEventAsync(
            TrackSalesFunnelEventDto eventDto, 
            string? ipAddress, 
            string? userAgent)
        {
            try
            {
                // Validate step
                if (eventDto.Step < 1 || eventDto.Step > 6)
                {
                    return new TrackEventResponseDto
                    {
                        Success = false,
                        Message = "Invalid step number. Must be between 1 and 6."
                    };
                }

                // Validate action
                var validActions = new[] { "entered", "completed", "abandoned" };
                if (!validActions.Contains(eventDto.Action.ToLower()))
                {
                    return new TrackEventResponseDto
                    {
                        Success = false,
                        Message = "Invalid action. Must be 'entered', 'completed', or 'abandoned'."
                    };
                }

                // Create metric
                var metric = new SalesFunnelMetric(
                    eventDto.SessionId,
                    eventDto.Step,
                    SalesFunnelMetric.GetStepName(eventDto.Step),
                    eventDto.Action.ToLower(),
                    eventDto.CapturedData,
                    eventDto.PlanId,
                    ipAddress,
                    userAgent,
                    eventDto.Referrer,
                    eventDto.DurationMs,
                    eventDto.Metadata
                );

                await _metricRepository.AddAsync(metric);

                return new TrackEventResponseDto
                {
                    Success = true,
                    Message = "Event tracked successfully"
                };
            }
            catch (Exception ex)
            {
                // Log error but don't expose internal details
                Console.Error.WriteLine($"Error tracking sales funnel event: {ex.Message}");
                return new TrackEventResponseDto
                {
                    Success = false,
                    Message = "Failed to track event"
                };
            }
        }

        public async Task<TrackEventResponseDto> MarkConversionAsync(MarkConversionDto conversionDto)
        {
            try
            {
                // Get all metrics for this session
                var metrics = await _metricRepository.GetBySessionIdAsync(conversionDto.SessionId);
                
                if (!metrics.Any())
                {
                    return new TrackEventResponseDto
                    {
                        Success = false,
                        Message = "Session not found"
                    };
                }

                // Mark all metrics in this session as converted
                foreach (var metric in metrics)
                {
                    metric.MarkAsConverted(conversionDto.ClinicId, conversionDto.OwnerId);
                    await _metricRepository.UpdateAsync(metric);
                }

                return new TrackEventResponseDto
                {
                    Success = true,
                    Message = "Conversion tracked successfully"
                };
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error marking conversion: {ex.Message}");
                return new TrackEventResponseDto
                {
                    Success = false,
                    Message = "Failed to mark conversion"
                };
            }
        }

        public async Task<FunnelStatsDto> GetFunnelStatsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var (totalSessions, conversions, conversionRate) = await _metricRepository.GetConversionStatsAsync(startDate, endDate);
            var stepStats = await _metricRepository.GetStepStatsAsync(startDate, endDate);

            var stats = new FunnelStatsDto
            {
                TotalSessions = totalSessions,
                Conversions = conversions,
                ConversionRate = conversionRate
            };

            foreach (var (step, (entered, completed, abandoned)) in stepStats)
            {
                var stepName = SalesFunnelMetric.GetStepName(step);
                var completionRate = entered > 0 ? (double)completed / entered * 100 : 0;
                var abandonmentRate = entered > 0 ? (double)abandoned / entered * 100 : 0;

                stats.StepStats[step] = new StepStatsDto
                {
                    StepNumber = step,
                    StepName = stepName,
                    Entered = entered,
                    Completed = completed,
                    Abandoned = abandoned,
                    CompletionRate = completionRate,
                    AbandonmentRate = abandonmentRate
                };
            }

            return stats;
        }

        public async Task<IEnumerable<IncompleteSessionDto>> GetIncompleteSessions(int hoursOld = 24, int limit = 100)
        {
            var olderThan = DateTime.UtcNow.AddHours(-hoursOld);
            var metrics = await _metricRepository.GetIncompleteSessions(olderThan, limit);

            return metrics.Select(m => new IncompleteSessionDto
            {
                SessionId = m.SessionId,
                LastStep = m.Step,
                LastStepName = m.StepName,
                CapturedData = m.CapturedData,
                PlanId = m.PlanId,
                LastActivity = m.CreatedAt,
                HoursSinceLastActivity = (int)(DateTime.UtcNow - m.CreatedAt).TotalHours
            }).ToList();
        }

        public async Task<IEnumerable<SalesFunnelMetricDto>> GetSessionMetricsAsync(string sessionId)
        {
            var metrics = await _metricRepository.GetBySessionIdAsync(sessionId);

            return metrics.Select(m => new SalesFunnelMetricDto
            {
                Id = m.Id,
                SessionId = m.SessionId,
                Step = m.Step,
                StepName = m.StepName,
                Action = m.Action,
                CapturedData = m.CapturedData,
                PlanId = m.PlanId,
                IpAddress = m.IpAddress,
                UserAgent = m.UserAgent,
                Referrer = m.Referrer,
                ClinicId = m.ClinicId,
                OwnerId = m.OwnerId,
                IsConverted = m.IsConverted,
                DurationMs = m.DurationMs,
                Metadata = m.Metadata,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        public async Task<IEnumerable<SalesFunnelMetricDto>> GetRecentSessions(int limit = 100)
        {
            var metrics = await _metricRepository.GetLatestMetricPerSessionAsync(limit: limit);

            return metrics.Select(m => new SalesFunnelMetricDto
            {
                Id = m.Id,
                SessionId = m.SessionId,
                Step = m.Step,
                StepName = m.StepName,
                Action = m.Action,
                CapturedData = m.CapturedData,
                PlanId = m.PlanId,
                IpAddress = m.IpAddress,
                UserAgent = m.UserAgent,
                Referrer = m.Referrer,
                ClinicId = m.ClinicId,
                OwnerId = m.OwnerId,
                IsConverted = m.IsConverted,
                DurationMs = m.DurationMs,
                Metadata = m.Metadata,
                CreatedAt = m.CreatedAt
            }).ToList();
        }
    }
}
