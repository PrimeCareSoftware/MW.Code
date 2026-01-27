using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Resposta completa de um paciente a uma pesquisa
    /// </summary>
    public class SurveyResponse : BaseEntity
    {
        public Guid SurveyId { get; private set; }
        public Survey Survey { get; private set; } = null!;
        
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = null!;
        
        private readonly List<SurveyQuestionResponse> _questionResponses = new();
        public IReadOnlyCollection<SurveyQuestionResponse> QuestionResponses => _questionResponses.AsReadOnly();
        
        public DateTime StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public bool IsCompleted { get; private set; }
        
        // Métricas calculadas
        public int? NpsScore { get; private set; } // 0-10 para NPS
        public int? CsatScore { get; private set; } // 1-5 para CSAT
        
        private SurveyResponse() { }
        
        public SurveyResponse(
            Guid surveyId,
            Guid patientId,
            string tenantId) : base(tenantId)
        {
            SurveyId = surveyId;
            PatientId = patientId;
            StartedAt = DateTime.UtcNow;
            IsCompleted = false;
        }
        
        public void AddQuestionResponse(SurveyQuestionResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
                
            if (IsCompleted)
                throw new InvalidOperationException("Cannot add responses to a completed survey");
                
            _questionResponses.Add(response);
            UpdateTimestamp();
        }
        
        public void Complete(int? npsScore = null, int? csatScore = null)
        {
            // Validate score ranges
            if (npsScore.HasValue && (npsScore.Value < 0 || npsScore.Value > 10))
                throw new ArgumentOutOfRangeException(nameof(npsScore), "NPS score must be between 0 and 10");
                
            if (csatScore.HasValue && (csatScore.Value < 1 || csatScore.Value > 5))
                throw new ArgumentOutOfRangeException(nameof(csatScore), "CSAT score must be between 1 and 5");
            
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
            NpsScore = npsScore;
            CsatScore = csatScore;
            UpdateTimestamp();
        }
        
        public void CalculateNpsScore()
        {
            // NPS é baseado na pergunta principal (primeira questão NumericScale 0-10)
            var npsQuestion = _questionResponses
                .FirstOrDefault(r => r.NumericAnswer.HasValue);
                
            if (npsQuestion?.NumericAnswer != null)
            {
                NpsScore = npsQuestion.NumericAnswer.Value;
            }
            
            UpdateTimestamp();
        }
    }
}
