using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Domain.Enums;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Services.CRM
{
    public class ChurnPredictionService : IChurnPredictionService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<ChurnPredictionService> _logger;

        public ChurnPredictionService(
            MedicSoftDbContext context,
            ILogger<ChurnPredictionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ChurnPredictionResultDto> PredictChurnAsync(Guid patientId, string tenantId)
        {
            _logger.LogInformation("Predicting churn for patient {PatientId} in tenant {TenantId}", 
                patientId, tenantId);

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId && p.TenantId == tenantId);

            if (patient == null)
            {
                throw new ArgumentException($"Patient {patientId} not found", nameof(patientId));
            }

            // Coletar features do paciente
            var features = await CollectPatientFeaturesAsync(patientId, tenantId);

            // Calcular probabilidade de churn usando heurística
            var churnProbability = CalculateChurnProbability(features);
            var riskLevel = DetermineRiskLevel(churnProbability);

            // Identificar fatores de risco
            var riskFactors = IdentifyRiskFactors(features);

            // Gerar ações recomendadas
            var recommendedActions = GenerateRecommendedActions(riskLevel, riskFactors);

            // Salvar predição no banco
            var prediction = new ChurnPrediction(patientId, tenantId);
            prediction.SetFeatures(
                features.DaysSinceLastVisit,
                features.TotalVisits,
                features.LifetimeValue,
                features.AverageSatisfactionScore,
                features.ComplaintsCount,
                features.NoShowCount,
                features.CancelledAppointmentsCount);

            prediction.SetPrediction(churnProbability, riskLevel);

            foreach (var factor in riskFactors)
            {
                prediction.AddRiskFactor(factor.Description);
            }

            foreach (var action in recommendedActions)
            {
                prediction.AddRecommendedAction(action);
            }

            _context.ChurnPredictions.Add(prediction);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Churn prediction completed for patient {PatientId}: {RiskLevel} ({Probability}%)", 
                patientId, riskLevel, (int)(churnProbability * 100));

            return new ChurnPredictionResultDto
            {
                PatientId = patientId,
                ChurnProbability = churnProbability,
                ChurnPercentage = (int)(churnProbability * 100),
                RiskLevel = riskLevel,
                RiskLevelName = riskLevel.ToString(),
                Factors = riskFactors,
                RecommendedActions = recommendedActions,
                RequiresImmediateAction = riskLevel >= ChurnRiskLevel.High
            };
        }

        public async Task<IEnumerable<PatientChurnRiskDto>> GetHighRiskPatientsAsync(string tenantId)
        {
            _logger.LogInformation("Getting high-risk patients for tenant {TenantId}", tenantId);

            var predictions = await _context.ChurnPredictions
                .Include(p => p.Patient)
                .Where(p => p.TenantId == tenantId 
                    && (p.RiskLevel == ChurnRiskLevel.High || p.RiskLevel == ChurnRiskLevel.Critical)
                    && p.PredictedAt >= DateTime.UtcNow.AddDays(-30)) // Últimas predições (30 dias)
                .OrderByDescending(p => p.ChurnProbability)
                .ToListAsync();

            return predictions.Select(p => new PatientChurnRiskDto
            {
                PatientId = p.PatientId,
                PatientName = p.Patient?.Name ?? "N/A",
                PatientEmail = p.Patient?.Email?.Value ?? "N/A",
                PatientPhone = p.Patient?.Phone?.ToString() ?? "N/A",
                RiskLevel = p.RiskLevel,
                RiskLevelName = p.RiskLevel.ToString(),
                ChurnProbability = p.ChurnProbability,
                ChurnPercentage = (int)(p.ChurnProbability * 100),
                LastVisitDate = DateTime.UtcNow.AddDays(-p.DaysSinceLastVisit),
                DaysSinceLastVisit = p.DaysSinceLastVisit,
                TopRiskFactors = p.RiskFactors.Take(5).ToList(),
                RecommendedActions = p.RecommendedActions.Take(3).ToList()
            });
        }

        public async Task<IEnumerable<ChurnFactorDto>> GetChurnFactorsAsync(Guid patientId, string tenantId)
        {
            _logger.LogInformation("Getting churn factors for patient {PatientId}", patientId);

            var prediction = await _context.ChurnPredictions
                .Where(p => p.PatientId == patientId && p.TenantId == tenantId)
                .OrderByDescending(p => p.PredictedAt)
                .FirstOrDefaultAsync();

            if (prediction == null)
            {
                return Enumerable.Empty<ChurnFactorDto>();
            }

            var features = new PatientFeatures
            {
                DaysSinceLastVisit = prediction.DaysSinceLastVisit,
                TotalVisits = prediction.TotalVisits,
                LifetimeValue = prediction.LifetimeValue,
                AverageSatisfactionScore = prediction.AverageSatisfactionScore,
                ComplaintsCount = prediction.ComplaintsCount,
                NoShowCount = prediction.NoShowCount,
                CancelledAppointmentsCount = prediction.CancelledAppointmentsCount
            };

            return IdentifyRiskFactors(features);
        }

        public async Task RecalculateAllPredictionsAsync(string tenantId)
        {
            _logger.LogInformation("Recalculating all churn predictions for tenant {TenantId}", tenantId);

            var patients = await _context.Patients
                .Where(p => p.TenantId == tenantId)
                .Select(p => p.Id)
                .ToListAsync();

            int count = 0;
            foreach (var patientId in patients)
            {
                try
                {
                    await PredictChurnAsync(patientId, tenantId);
                    count++;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to predict churn for patient {PatientId}", patientId);
                }
            }

            _logger.LogInformation("Recalculated {Count} churn predictions", count);
        }

        // Métodos auxiliares privados

        private async Task<PatientFeatures> CollectPatientFeaturesAsync(Guid patientId, string tenantId)
        {
            var now = DateTime.UtcNow;

            // Buscar última consulta
            var lastAppointment = await _context.Appointments
                .Where(a => a.PatientId == patientId && a.TenantId == tenantId && a.Status == AppointmentStatus.Completed)
                .OrderByDescending(a => a.ScheduledDate)
                .FirstOrDefaultAsync();

            var daysSinceLastVisit = lastAppointment != null 
                ? (int)(now - lastAppointment.ScheduledDate).TotalDays 
                : 365; // Padrão: 1 ano

            // Total de consultas
            var totalVisits = await _context.Appointments
                .CountAsync(a => a.PatientId == patientId && a.TenantId == tenantId && a.Status == AppointmentStatus.Completed);

            // Satisfação média (NPS/CSAT)
            var surveyResponses = await _context.SurveyResponses
                .Where(sr => sr.PatientId == patientId && sr.TenantId == tenantId && sr.IsCompleted)
                .ToListAsync();

            var avgSatisfaction = surveyResponses.Any()
                ? surveyResponses.Average(sr => sr.NpsScore ?? sr.CsatScore ?? 5)
                : 5.0; // Padrão: neutro

            // Reclamações
            var complaintsCount = await _context.Complaints
                .CountAsync(c => c.PatientId == patientId && c.TenantId == tenantId);

            // No-shows
            var noShowCount = await _context.Appointments
                .CountAsync(a => a.PatientId == patientId && a.TenantId == tenantId && a.Status == AppointmentStatus.NoShow);

            // Cancelamentos
            var cancelledCount = await _context.Appointments
                .CountAsync(a => a.PatientId == patientId && a.TenantId == tenantId && a.Status == AppointmentStatus.Cancelled);

            // Valor do tempo de vida (simplificado)
            var lifetimeValue = totalVisits * 150m; // Assumindo R$ 150 por consulta

            return new PatientFeatures
            {
                DaysSinceLastVisit = daysSinceLastVisit,
                TotalVisits = totalVisits,
                LifetimeValue = lifetimeValue,
                AverageSatisfactionScore = avgSatisfaction,
                ComplaintsCount = complaintsCount,
                NoShowCount = noShowCount,
                CancelledAppointmentsCount = cancelledCount
            };
        }

        private double CalculateChurnProbability(PatientFeatures features)
        {
            double score = 0.0;

            // Peso dos fatores
            const double DAYS_WEIGHT = 0.25;
            const double SATISFACTION_WEIGHT = 0.20;
            const double COMPLAINTS_WEIGHT = 0.15;
            const double NOSHOW_WEIGHT = 0.15;
            const double CANCELLED_WEIGHT = 0.10;
            const double VISITS_WEIGHT = 0.15;

            // Dias desde última visita (normalizado para 0-1)
            var daysScore = Math.Min(1.0, features.DaysSinceLastVisit / 180.0); // 180 dias = 100%
            score += daysScore * DAYS_WEIGHT;

            // Satisfação (invertido: baixa satisfação = alto risco)
            var satisfactionScore = 1.0 - (features.AverageSatisfactionScore / 10.0);
            score += satisfactionScore * SATISFACTION_WEIGHT;

            // Reclamações (normalizado)
            var complaintsScore = Math.Min(1.0, features.ComplaintsCount / 5.0); // 5+ reclamações = 100%
            score += complaintsScore * COMPLAINTS_WEIGHT;

            // No-shows (normalizado)
            var noShowScore = Math.Min(1.0, features.NoShowCount / 3.0); // 3+ no-shows = 100%
            score += noShowScore * NOSHOW_WEIGHT;

            // Cancelamentos (normalizado)
            var cancelledScore = Math.Min(1.0, features.CancelledAppointmentsCount / 5.0); // 5+ cancelamentos = 100%
            score += cancelledScore * CANCELLED_WEIGHT;

            // Visitas (invertido: poucas visitas = alto risco)
            var visitsScore = features.TotalVisits == 0 ? 1.0 : Math.Max(0.0, 1.0 - (features.TotalVisits / 10.0));
            score += visitsScore * VISITS_WEIGHT;

            return Math.Clamp(score, 0.0, 1.0);
        }

        private ChurnRiskLevel DetermineRiskLevel(double probability)
        {
            if (probability >= 0.75) return ChurnRiskLevel.Critical;
            if (probability >= 0.50) return ChurnRiskLevel.High;
            if (probability >= 0.25) return ChurnRiskLevel.Medium;
            return ChurnRiskLevel.Low;
        }

        private List<ChurnFactorDto> IdentifyRiskFactors(PatientFeatures features)
        {
            var factors = new List<ChurnFactorDto>();

            if (features.DaysSinceLastVisit > 90)
            {
                factors.Add(new ChurnFactorDto
                {
                    Name = "Inatividade",
                    Description = $"Paciente sem consultas há {features.DaysSinceLastVisit} dias",
                    ImpactScore = Math.Min(1.0, features.DaysSinceLastVisit / 180.0),
                    Severity = features.DaysSinceLastVisit > 180 ? "Alta" : "Média"
                });
            }

            if (features.AverageSatisfactionScore < 7)
            {
                factors.Add(new ChurnFactorDto
                {
                    Name = "Baixa Satisfação",
                    Description = $"Satisfação média de {features.AverageSatisfactionScore:F1}/10",
                    ImpactScore = 1.0 - (features.AverageSatisfactionScore / 10.0),
                    Severity = features.AverageSatisfactionScore < 5 ? "Alta" : "Média"
                });
            }

            if (features.ComplaintsCount > 0)
            {
                factors.Add(new ChurnFactorDto
                {
                    Name = "Reclamações",
                    Description = $"{features.ComplaintsCount} reclamação(ões) registrada(s)",
                    ImpactScore = Math.Min(1.0, features.ComplaintsCount / 5.0),
                    Severity = features.ComplaintsCount >= 3 ? "Alta" : "Média"
                });
            }

            if (features.NoShowCount > 1)
            {
                factors.Add(new ChurnFactorDto
                {
                    Name = "No-Shows",
                    Description = $"{features.NoShowCount} falta(s) sem aviso",
                    ImpactScore = Math.Min(1.0, features.NoShowCount / 3.0),
                    Severity = features.NoShowCount >= 3 ? "Alta" : "Média"
                });
            }

            if (features.CancelledAppointmentsCount > 2)
            {
                factors.Add(new ChurnFactorDto
                {
                    Name = "Cancelamentos",
                    Description = $"{features.CancelledAppointmentsCount} consulta(s) cancelada(s)",
                    ImpactScore = Math.Min(1.0, features.CancelledAppointmentsCount / 5.0),
                    Severity = features.CancelledAppointmentsCount >= 5 ? "Alta" : "Média"
                });
            }

            if (features.TotalVisits < 3)
            {
                factors.Add(new ChurnFactorDto
                {
                    Name = "Baixo Engajamento",
                    Description = $"Apenas {features.TotalVisits} consulta(s) realizada(s)",
                    ImpactScore = 1.0 - (features.TotalVisits / 10.0),
                    Severity = features.TotalVisits == 0 ? "Alta" : "Baixa"
                });
            }

            return factors.OrderByDescending(f => f.ImpactScore).ToList();
        }

        private List<string> GenerateRecommendedActions(ChurnRiskLevel riskLevel, List<ChurnFactorDto> factors)
        {
            var actions = new List<string>();

            if (riskLevel >= ChurnRiskLevel.High)
            {
                actions.Add("Contato imediato do relacionamento para entender necessidades");
                actions.Add("Oferecer consulta de check-up com desconto especial");
            }

            if (factors.Any(f => f.Name == "Inatividade"))
            {
                actions.Add("Enviar campanha de reengajamento personalizada");
                actions.Add("Oferecer lembrete de consultas preventivas");
            }

            if (factors.Any(f => f.Name == "Baixa Satisfação"))
            {
                actions.Add("Agendar ligação para feedback e melhoria contínua");
                actions.Add("Oferecer consulta com especialista sênior");
            }

            if (factors.Any(f => f.Name == "Reclamações"))
            {
                actions.Add("Revisar e resolver reclamações pendentes");
                actions.Add("Designar gestor de relacionamento dedicado");
            }

            if (factors.Any(f => f.Name == "No-Shows"))
            {
                actions.Add("Implementar lembretes automáticos por WhatsApp");
                actions.Add("Facilitar reagendamento online");
            }

            if (actions.Count == 0)
            {
                actions.Add("Manter engajamento com conteúdo de saúde relevante");
                actions.Add("Enviar pesquisa de satisfação");
            }

            return actions.Take(5).ToList();
        }

        // Classe auxiliar para features do paciente
        private class PatientFeatures
        {
            public int DaysSinceLastVisit { get; set; }
            public int TotalVisits { get; set; }
            public decimal LifetimeValue { get; set; }
            public double AverageSatisfactionScore { get; set; }
            public int ComplaintsCount { get; set; }
            public int NoShowCount { get; set; }
            public int CancelledAppointmentsCount { get; set; }
        }
    }
}
