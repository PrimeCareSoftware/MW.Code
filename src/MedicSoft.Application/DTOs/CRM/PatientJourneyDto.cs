using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.DTOs.CRM
{
    public class PatientJourneyDto
    {
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public string PacienteNome { get; set; } = string.Empty;
        
        public JourneyStageEnum CurrentStage { get; set; }
        public string CurrentStageName { get; set; } = string.Empty;
        
        // Métricas
        public int TotalTouchpoints { get; set; }
        public decimal LifetimeValue { get; set; }
        public int NpsScore { get; set; }
        public double SatisfactionScore { get; set; }
        public ChurnRiskLevel ChurnRisk { get; set; }
        public string ChurnRiskName { get; set; } = string.Empty;
        
        // Segmentação
        public List<string> Tags { get; set; } = new();
        public int EngagementScore { get; set; }
        
        // Estágios
        public List<JourneyStageDto> Stages { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class JourneyStageDto
    {
        public Guid Id { get; set; }
        public JourneyStageEnum Stage { get; set; }
        public string StageName { get; set; } = string.Empty;
        public DateTime EnteredAt { get; set; }
        public DateTime? ExitedAt { get; set; }
        public int DurationDays { get; set; }
        public string? ExitTrigger { get; set; }
        
        public List<PatientTouchpointDto> Touchpoints { get; set; } = new();
    }

    public class PatientTouchpointDto
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public TouchpointType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TouchpointDirection Direction { get; set; }
        public string DirectionName { get; set; } = string.Empty;
        public Guid? SentimentAnalysisId { get; set; }
    }

    public class CreatePatientTouchpointDto
    {
        public TouchpointType Type { get; set; }
        public string Channel { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TouchpointDirection Direction { get; set; }
    }

    public class UpdatePatientJourneyMetricsDto
    {
        public decimal? LifetimeValue { get; set; }
        public int? NpsScore { get; set; }
        public double? SatisfactionScore { get; set; }
        public ChurnRiskLevel? ChurnRisk { get; set; }
    }

    public class AdvanceJourneyStageDto
    {
        public JourneyStageEnum NewStage { get; set; }
        public string Trigger { get; set; } = string.Empty;
    }

    public class PatientJourneyMetricsDto
    {
        public Guid PacienteId { get; set; }
        public string PacienteNome { get; set; } = string.Empty;
        
        // Métricas atuais
        public decimal LifetimeValue { get; set; }
        public int NpsScore { get; set; }
        public double SatisfactionScore { get; set; }
        public ChurnRiskLevel ChurnRisk { get; set; }
        public int EngagementScore { get; set; }
        
        // Estatísticas de jornada
        public JourneyStageEnum CurrentStage { get; set; }
        public int TotalTouchpoints { get; set; }
        public int DaysInCurrentStage { get; set; }
        public int TotalDaysInJourney { get; set; }
        
        // Breakdown por canal
        public Dictionary<string, int> TouchpointsByChannel { get; set; } = new();
        public Dictionary<string, int> TouchpointsByType { get; set; } = new();
    }
}
