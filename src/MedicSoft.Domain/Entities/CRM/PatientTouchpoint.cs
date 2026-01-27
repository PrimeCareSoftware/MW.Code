using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Ponto de contato (touchpoint) do paciente com a clínica
    /// Representa cada interação registrada na jornada do paciente
    /// </summary>
    public class PatientTouchpoint : BaseEntity
    {
        public Guid JourneyStageId { get; private set; }
        public JourneyStage JourneyStage { get; private set; } = null!;
        
        public DateTime Timestamp { get; private set; }
        
        public TouchpointType Type { get; private set; }
        public string Channel { get; private set; } // Email, SMS, WhatsApp, Phone, InPerson
        public string Description { get; private set; }
        public TouchpointDirection Direction { get; private set; }
        
        // Análise de sentimento associada (opcional)
        public Guid? SentimentAnalysisId { get; private set; }
        
        private PatientTouchpoint() 
        {
            Channel = string.Empty;
            Description = string.Empty;
        }
        
        public PatientTouchpoint(
            Guid journeyStageId,
            TouchpointType type,
            string channel,
            string description,
            TouchpointDirection direction,
            string tenantId) : base(tenantId)
        {
            JourneyStageId = journeyStageId;
            Timestamp = DateTime.UtcNow;
            Type = type;
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Direction = direction;
        }
        
        public void AssociateSentimentAnalysis(Guid sentimentAnalysisId)
        {
            SentimentAnalysisId = sentimentAnalysisId;
            UpdateTimestamp();
        }
    }
}
