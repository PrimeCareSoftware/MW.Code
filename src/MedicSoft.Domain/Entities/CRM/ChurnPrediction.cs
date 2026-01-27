using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Predição de churn (risco de perda) de um paciente usando ML
    /// </summary>
    public class ChurnPrediction : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = null!;
        
        // Resultado da predição
        public double ChurnProbability { get; private set; } // 0-1
        public ChurnRiskLevel RiskLevel { get; private set; }
        public DateTime PredictedAt { get; private set; }
        
        // Fatores que contribuem para o risco
        private readonly List<string> _riskFactors = new();
        public IReadOnlyCollection<string> RiskFactors => _riskFactors.AsReadOnly();
        
        // Ações recomendadas
        private readonly List<string> _recommendedActions = new();
        public IReadOnlyCollection<string> RecommendedActions => _recommendedActions.AsReadOnly();
        
        // Features usadas no modelo
        public int DaysSinceLastVisit { get; private set; }
        public int TotalVisits { get; private set; }
        public decimal LifetimeValue { get; private set; }
        public double AverageSatisfactionScore { get; private set; }
        public int ComplaintsCount { get; private set; }
        public int NoShowCount { get; private set; }
        public int CancelledAppointmentsCount { get; private set; }
        
        private ChurnPrediction() { }
        
        public ChurnPrediction(
            Guid patientId,
            string tenantId) : base(tenantId)
        {
            PatientId = patientId;
            PredictedAt = DateTime.UtcNow;
        }
        
        public void SetFeatures(
            int daysSinceLastVisit,
            int totalVisits,
            decimal lifetimeValue,
            double averageSatisfactionScore,
            int complaintsCount,
            int noShowCount,
            int cancelledAppointmentsCount)
        {
            DaysSinceLastVisit = daysSinceLastVisit;
            TotalVisits = totalVisits;
            LifetimeValue = lifetimeValue;
            AverageSatisfactionScore = averageSatisfactionScore;
            ComplaintsCount = complaintsCount;
            NoShowCount = noShowCount;
            CancelledAppointmentsCount = cancelledAppointmentsCount;
            UpdateTimestamp();
        }
        
        public void SetPrediction(double churnProbability, ChurnRiskLevel riskLevel)
        {
            ChurnProbability = Math.Clamp(churnProbability, 0, 1);
            RiskLevel = riskLevel;
            UpdateTimestamp();
        }
        
        public void AddRiskFactor(string factor)
        {
            if (string.IsNullOrWhiteSpace(factor))
                throw new ArgumentException("Factor cannot be null or empty", nameof(factor));
                
            if (!_riskFactors.Contains(factor))
            {
                _riskFactors.Add(factor);
                UpdateTimestamp();
            }
        }
        
        public void AddRecommendedAction(string action)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action cannot be null or empty", nameof(action));
                
            if (!_recommendedActions.Contains(action))
            {
                _recommendedActions.Add(action);
                UpdateTimestamp();
            }
        }
    }
}
