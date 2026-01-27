using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Jornada completa do paciente no sistema CRM
    /// Mapeia todos os estágios e interações do paciente
    /// </summary>
    public class PatientJourney : BaseEntity
    {
        public Guid PacienteId { get; private set; }
        public Patient Paciente { get; private set; } = null!;
        
        // Estágios da jornada
        private readonly List<JourneyStage> _stages = new();
        public IReadOnlyCollection<JourneyStage> Stages => _stages.AsReadOnly();
        
        public JourneyStageEnum CurrentStage { get; private set; }
        
        // Métricas
        public int TotalTouchpoints { get; private set; }
        public decimal LifetimeValue { get; private set; }
        public int NpsScore { get; private set; }
        public double SatisfactionScore { get; private set; }
        public ChurnRiskLevel ChurnRisk { get; private set; }
        
        // Segmentação e Engagement
        public List<string> Tags { get; set; } = new();
        public int EngagementScore { get; set; }
        
        private PatientJourney() { }
        
        public PatientJourney(Guid pacienteId, string tenantId) : base(tenantId)
        {
            PacienteId = pacienteId;
            CurrentStage = JourneyStageEnum.Descoberta;
            ChurnRisk = ChurnRiskLevel.Low;
            
            // Criar primeiro estágio
            var initialStage = new JourneyStage(Id, JourneyStageEnum.Descoberta, tenantId);
            _stages.Add(initialStage);
        }
        
        public void AdvanceToStage(JourneyStageEnum newStage, string trigger, string tenantId)
        {
            // Fechar estágio atual
            var currentStage = _stages.FirstOrDefault(s => s.ExitedAt == null);
            if (currentStage != null)
            {
                currentStage.ExitStage(trigger);
            }
            
            // Criar novo estágio
            var newJourneyStage = new JourneyStage(Id, newStage, tenantId);
            _stages.Add(newJourneyStage);
            
            CurrentStage = newStage;
            UpdateTimestamp();
        }
        
        public void AddTouchpoint(PatientTouchpoint touchpoint)
        {
            var currentStage = _stages.FirstOrDefault(s => s.ExitedAt == null);
            if (currentStage != null)
            {
                currentStage.AddTouchpoint(touchpoint);
                TotalTouchpoints++;
                UpdateTimestamp();
            }
        }
        
        public void UpdateMetrics(
            decimal lifetimeValue,
            int npsScore,
            double satisfactionScore,
            ChurnRiskLevel churnRisk)
        {
            LifetimeValue = lifetimeValue;
            NpsScore = npsScore;
            SatisfactionScore = satisfactionScore;
            ChurnRisk = churnRisk;
            UpdateTimestamp();
        }
        
        public JourneyStage? GetCurrentStage()
        {
            return _stages.FirstOrDefault(s => s.ExitedAt == null);
        }
    }
}
