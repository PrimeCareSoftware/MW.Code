using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Services.CRM
{
    public class PatientJourneyService : IPatientJourneyService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<PatientJourneyService> _logger;
        private readonly IAutomationEngine _automationEngine;

        public PatientJourneyService(
            MedicSoftDbContext context,
            ILogger<PatientJourneyService> logger,
            IAutomationEngine automationEngine)
        {
            _context = context;
            _logger = logger;
            _automationEngine = automationEngine;
        }

        public async Task<PatientJourneyDto> GetOrCreateJourneyAsync(Guid pacienteId, string tenantId)
        {
            var journey = await _context.PatientJourneys
                .Include(j => j.Stages)
                    .ThenInclude(s => s.Touchpoints)
                .FirstOrDefaultAsync(j => j.PacienteId == pacienteId && j.TenantId == tenantId);

            if (journey == null)
            {
                journey = new PatientJourney(pacienteId, tenantId);
                _context.PatientJourneys.Add(journey);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created new journey {JourneyId} for patient {PatientId}", 
                    journey.Id, pacienteId);
            }

            return await MapToDtoAsync(journey);
        }

        public async Task<PatientJourneyDto?> GetJourneyByIdAsync(Guid journeyId, string tenantId)
        {
            var journey = await _context.PatientJourneys
                .Include(j => j.Stages)
                    .ThenInclude(s => s.Touchpoints)
                .FirstOrDefaultAsync(j => j.Id == journeyId && j.TenantId == tenantId);

            return journey == null ? null : await MapToDtoAsync(journey);
        }

        public async Task<PatientJourneyDto?> GetJourneyByPatientIdAsync(Guid pacienteId, string tenantId)
        {
            var journey = await _context.PatientJourneys
                .Include(j => j.Stages)
                    .ThenInclude(s => s.Touchpoints)
                .FirstOrDefaultAsync(j => j.PacienteId == pacienteId && j.TenantId == tenantId);

            return journey == null ? null : await MapToDtoAsync(journey);
        }

        public async Task<PatientJourneyDto> AdvanceStageAsync(
            Guid pacienteId, 
            AdvanceJourneyStageDto dto, 
            string tenantId)
        {
            var journey = await GetOrCreateJourneyEntityAsync(pacienteId, tenantId);

            journey.AdvanceToStage(dto.NewStage, dto.Trigger, tenantId);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Advanced patient {PatientId} to stage {Stage} with trigger {Trigger}",
                pacienteId, dto.NewStage, dto.Trigger);

            // Trigger any automations for this stage
            await _automationEngine.TriggerStageAutomationsAsync(pacienteId, dto.NewStage, tenantId);

            return await MapToDtoAsync(journey);
        }

        public async Task<PatientJourneyDto> AddTouchpointAsync(
            Guid pacienteId, 
            CreatePatientTouchpointDto dto, 
            string tenantId)
        {
            var journey = await GetOrCreateJourneyEntityAsync(pacienteId, tenantId);
            var currentStage = journey.GetCurrentStage();

            if (currentStage == null)
            {
                throw new InvalidOperationException("No active stage found for patient journey");
            }

            var touchpoint = new PatientTouchpoint(
                currentStage.Id,
                dto.Type,
                dto.Channel,
                dto.Description,
                dto.Direction,
                tenantId);

            journey.AddTouchpoint(touchpoint);
            _context.PatientTouchpoints.Add(touchpoint);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Added touchpoint {TouchpointId} for patient {PatientId} in stage {Stage}",
                touchpoint.Id, pacienteId, currentStage.Stage);

            return await MapToDtoAsync(journey);
        }

        public async Task<PatientJourneyDto> UpdateMetricsAsync(
            Guid pacienteId, 
            UpdatePatientJourneyMetricsDto dto, 
            string tenantId)
        {
            var journey = await GetOrCreateJourneyEntityAsync(pacienteId, tenantId);

            // Get current values
            var ltv = dto.LifetimeValue ?? journey.LifetimeValue;
            var nps = dto.NpsScore ?? journey.NpsScore;
            var satisfaction = dto.SatisfactionScore ?? journey.SatisfactionScore;
            var churnRisk = dto.ChurnRisk ?? journey.ChurnRisk;

            journey.UpdateMetrics(ltv, nps, satisfaction, churnRisk);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Updated metrics for patient {PatientId}: LTV={LTV}, NPS={NPS}, Satisfaction={Satisfaction}, ChurnRisk={ChurnRisk}",
                pacienteId, ltv, nps, satisfaction, churnRisk);

            return await MapToDtoAsync(journey);
        }

        public async Task<PatientJourneyMetricsDto?> GetMetricsAsync(Guid pacienteId, string tenantId)
        {
            var journey = await _context.PatientJourneys
                .Include(j => j.Stages)
                    .ThenInclude(s => s.Touchpoints)
                .FirstOrDefaultAsync(j => j.PacienteId == pacienteId && j.TenantId == tenantId);

            if (journey == null)
                return null;

            var patient = await _context.Patients.FindAsync(pacienteId);
            var currentStage = journey.GetCurrentStage();

            // Calculate touchpoints by channel and type
            var allTouchpoints = journey.Stages.SelectMany(s => s.Touchpoints).ToList();
            var touchpointsByChannel = allTouchpoints
                .GroupBy(t => t.Channel)
                .ToDictionary(g => g.Key, g => g.Count());
            var touchpointsByType = allTouchpoints
                .GroupBy(t => t.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            return new PatientJourneyMetricsDto
            {
                PacienteId = pacienteId,
                PacienteNome = patient?.Name ?? "Unknown",
                LifetimeValue = journey.LifetimeValue,
                NpsScore = journey.NpsScore,
                SatisfactionScore = journey.SatisfactionScore,
                ChurnRisk = journey.ChurnRisk,
                EngagementScore = journey.EngagementScore,
                CurrentStage = journey.CurrentStage,
                TotalTouchpoints = journey.TotalTouchpoints,
                DaysInCurrentStage = currentStage != null 
                    ? (DateTime.UtcNow - currentStage.EnteredAt).Days 
                    : 0,
                TotalDaysInJourney = (DateTime.UtcNow - journey.CreatedAt).Days,
                TouchpointsByChannel = touchpointsByChannel,
                TouchpointsByType = touchpointsByType
            };
        }

        public async Task RecalculateMetricsAsync(Guid pacienteId, string tenantId)
        {
            var journey = await GetOrCreateJourneyEntityAsync(pacienteId, tenantId);

            // Calculate LTV (sum of all appointments/consultations)
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == pacienteId && a.TenantId == tenantId)
                .ToListAsync();
            var ltv = appointments.Sum(a => a.PaymentAmount ?? 0);

            // Get NPS from latest survey response
            var latestNps = await _context.SurveyResponses
                .Where(sr => sr.PatientId == pacienteId && 
                            sr.TenantId == tenantId &&
                            sr.Survey.Type == SurveyType.NPS)
                .OrderByDescending(sr => sr.CompletedAt)
                .Select(sr => sr.NpsScore)
                .FirstOrDefaultAsync();

            // Get CSAT average
            var csatScores = await _context.SurveyResponses
                .Where(sr => sr.PatientId == pacienteId && 
                            sr.TenantId == tenantId &&
                            sr.Survey.Type == SurveyType.CSAT)
                .Select(sr => sr.CsatScore)
                .ToListAsync();
            var avgSatisfaction = csatScores.Any() ? csatScores.Where(s => s.HasValue).Average(s => s!.Value) : 0;

            // Simple churn risk calculation (can be enhanced with ML model)
            var daysSinceLastAppointment = appointments.Any()
                ? (DateTime.UtcNow - appointments.Max(a => a.ScheduledDate)).Days
                : 0;
            var churnRisk = CalculateChurnRisk(daysSinceLastAppointment, latestNps ?? 0, avgSatisfaction);

            journey.UpdateMetrics(ltv, latestNps ?? 0, avgSatisfaction, churnRisk);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Recalculated metrics for patient {PatientId}", pacienteId);
        }

        private async Task<PatientJourney> GetOrCreateJourneyEntityAsync(Guid pacienteId, string tenantId)
        {
            var journey = await _context.PatientJourneys
                .Include(j => j.Stages)
                    .ThenInclude(s => s.Touchpoints)
                .FirstOrDefaultAsync(j => j.PacienteId == pacienteId && j.TenantId == tenantId);

            if (journey == null)
            {
                journey = new PatientJourney(pacienteId, tenantId);
                _context.PatientJourneys.Add(journey);
                await _context.SaveChangesAsync();
            }

            return journey;
        }

        private async Task<PatientJourneyDto> MapToDtoAsync(PatientJourney journey)
        {
            // Load patient if not loaded
            var patient = await _context.Patients.FindAsync(journey.PacienteId);

            return new PatientJourneyDto
            {
                Id = journey.Id,
                PacienteId = journey.PacienteId,
                PacienteNome = patient?.Name ?? "Unknown",
                CurrentStage = journey.CurrentStage,
                CurrentStageName = journey.CurrentStage.ToString(),
                TotalTouchpoints = journey.TotalTouchpoints,
                LifetimeValue = journey.LifetimeValue,
                NpsScore = journey.NpsScore,
                SatisfactionScore = journey.SatisfactionScore,
                ChurnRisk = journey.ChurnRisk,
                ChurnRiskName = journey.ChurnRisk.ToString(),
                Tags = journey.Tags,
                EngagementScore = journey.EngagementScore,
                Stages = journey.Stages.Select(MapStageToDto).ToList(),
                CreatedAt = journey.CreatedAt,
                UpdatedAt = journey.UpdatedAt
            };
        }

        private JourneyStageDto MapStageToDto(JourneyStage stage)
        {
            return new JourneyStageDto
            {
                Id = stage.Id,
                Stage = stage.Stage,
                StageName = stage.Stage.ToString(),
                EnteredAt = stage.EnteredAt,
                ExitedAt = stage.ExitedAt,
                DurationDays = stage.DurationDays,
                ExitTrigger = stage.ExitTrigger,
                Touchpoints = stage.Touchpoints.Select(MapTouchpointToDto).ToList()
            };
        }

        private PatientTouchpointDto MapTouchpointToDto(PatientTouchpoint touchpoint)
        {
            return new PatientTouchpointDto
            {
                Id = touchpoint.Id,
                Timestamp = touchpoint.Timestamp,
                Type = touchpoint.Type,
                TypeName = touchpoint.Type.ToString(),
                Channel = touchpoint.Channel,
                Description = touchpoint.Description,
                Direction = touchpoint.Direction,
                DirectionName = touchpoint.Direction.ToString(),
                SentimentAnalysisId = touchpoint.SentimentAnalysisId
            };
        }

        private ChurnRiskLevel CalculateChurnRisk(int daysSinceLastAppointment, int npsScore, double satisfactionScore)
        {
            // Simple heuristic-based calculation
            // Can be replaced with ML model prediction later
            
            if (daysSinceLastAppointment > 180 || npsScore < 3 || satisfactionScore < 2.0)
                return ChurnRiskLevel.Critical;
            
            if (daysSinceLastAppointment > 90 || npsScore < 6 || satisfactionScore < 3.0)
                return ChurnRiskLevel.High;
            
            if (daysSinceLastAppointment > 60 || npsScore < 8 || satisfactionScore < 4.0)
                return ChurnRiskLevel.Medium;
            
            return ChurnRiskLevel.Low;
        }
    }
}
