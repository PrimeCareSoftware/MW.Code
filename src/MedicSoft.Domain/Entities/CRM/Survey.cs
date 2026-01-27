using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Pesquisa de satisfação (NPS, CSAT, etc)
    /// </summary>
    public class Survey : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public SurveyType Type { get; private set; }
        public bool IsActive { get; private set; }
        
        // Questões da pesquisa
        private readonly List<SurveyQuestion> _questions = new();
        public IReadOnlyCollection<SurveyQuestion> Questions => _questions.AsReadOnly();
        
        // Respostas recebidas
        private readonly List<SurveyResponse> _responses = new();
        public IReadOnlyCollection<SurveyResponse> Responses => _responses.AsReadOnly();
        
        // Configuração de envio
        public JourneyStageEnum? TriggerStage { get; private set; }
        public string? TriggerEvent { get; private set; } // Evento específico que dispara
        public int? DelayHours { get; private set; } // Delay após evento
        
        // Métricas agregadas
        public double AverageScore { get; private set; }
        public int TotalResponses { get; private set; }
        public int ResponseRate { get; private set; } // Percentual
        
        private Survey()
        {
            Name = string.Empty;
            Description = string.Empty;
        }
        
        public Survey(
            string name,
            string description,
            SurveyType type,
            string tenantId) : base(tenantId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Type = type;
            IsActive = false;
        }
        
        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }
        
        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
        
        public void ConfigureTrigger(
            JourneyStageEnum? triggerStage,
            string? triggerEvent,
            int? delayHours)
        {
            TriggerStage = triggerStage;
            TriggerEvent = triggerEvent;
            DelayHours = delayHours;
            UpdateTimestamp();
        }
        
        public void AddQuestion(SurveyQuestion question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));
                
            _questions.Add(question);
            UpdateTimestamp();
        }
        
        public void RecordResponse(SurveyResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
                
            _responses.Add(response);
            TotalResponses++;
            UpdateTimestamp();
        }
        
        public void RecalculateMetrics()
        {
            if (TotalResponses == 0)
            {
                AverageScore = 0;
                return;
            }
            
            double totalScore = 0;
            int scoredResponses = 0;
            
            foreach (var response in _responses)
            {
                if (Type == SurveyType.NPS && response.NpsScore.HasValue)
                {
                    totalScore += response.NpsScore.Value;
                    scoredResponses++;
                }
                else if (Type == SurveyType.CSAT && response.CsatScore.HasValue)
                {
                    totalScore += response.CsatScore.Value;
                    scoredResponses++;
                }
            }
            
            if (scoredResponses > 0)
            {
                AverageScore = totalScore / scoredResponses;
            }
            
            UpdateTimestamp();
        }
    }
}
