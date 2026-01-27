using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.DTOs.CRM
{
    public class ChurnPredictionDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        public double ChurnProbability { get; set; }
        public int ChurnPercentage { get; set; }
        public ChurnRiskLevel RiskLevel { get; set; }
        public string RiskLevelName { get; set; } = string.Empty;
        public DateTime PredictedAt { get; set; }
        
        public List<string> RiskFactors { get; set; } = new();
        public List<string> RecommendedActions { get; set; } = new();
        
        // Features
        public int DaysSinceLastVisit { get; set; }
        public int TotalVisits { get; set; }
        public decimal LifetimeValue { get; set; }
        public double AverageSatisfactionScore { get; set; }
        public int ComplaintsCount { get; set; }
        public int NoShowCount { get; set; }
        public int CancelledAppointmentsCount { get; set; }
    }

    public class PatientChurnRiskDto
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        
        public ChurnRiskLevel RiskLevel { get; set; }
        public string RiskLevelName { get; set; } = string.Empty;
        public double ChurnProbability { get; set; }
        public int ChurnPercentage { get; set; }
        
        public DateTime? LastVisitDate { get; set; }
        public int DaysSinceLastVisit { get; set; }
        
        public List<string> TopRiskFactors { get; set; } = new();
        public List<string> RecommendedActions { get; set; } = new();
    }

    public class ChurnPredictionResultDto
    {
        public Guid PatientId { get; set; }
        public double ChurnProbability { get; set; }
        public int ChurnPercentage { get; set; }
        public ChurnRiskLevel RiskLevel { get; set; }
        public string RiskLevelName { get; set; } = string.Empty;
        
        public List<ChurnFactorDto> Factors { get; set; } = new();
        public List<string> RecommendedActions { get; set; } = new();
        
        public bool RequiresImmediateAction { get; set; }
    }

    public class ChurnFactorDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double ImpactScore { get; set; }
        public string Severity { get; set; } = string.Empty;
    }
}
