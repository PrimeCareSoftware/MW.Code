using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Representa um estágio na jornada do paciente
    /// </summary>
    public class JourneyStage : BaseEntity
    {
        public Guid PatientJourneyId { get; private set; }
        public PatientJourney PatientJourney { get; private set; } = null!;
        
        public JourneyStageEnum Stage { get; private set; }
        public DateTime EnteredAt { get; private set; }
        public DateTime? ExitedAt { get; private set; }
        public int DurationDays { get; private set; }
        
        // Touchpoints no estágio
        private readonly List<PatientTouchpoint> _touchpoints = new();
        public IReadOnlyCollection<PatientTouchpoint> Touchpoints => _touchpoints.AsReadOnly();
        
        // Eventos que movem para próximo estágio
        public string? ExitTrigger { get; private set; }
        
        private JourneyStage() { }
        
        public JourneyStage(
            Guid patientJourneyId,
            JourneyStageEnum stage,
            string tenantId) : base(tenantId)
        {
            PatientJourneyId = patientJourneyId;
            Stage = stage;
            EnteredAt = DateTime.UtcNow;
        }
        
        public void ExitStage(string trigger)
        {
            ExitedAt = DateTime.UtcNow;
            DurationDays = (ExitedAt.Value - EnteredAt).Days;
            ExitTrigger = trigger;
            UpdateTimestamp();
        }
        
        public void AddTouchpoint(PatientTouchpoint touchpoint)
        {
            if (touchpoint == null)
                throw new ArgumentNullException(nameof(touchpoint));
                
            _touchpoints.Add(touchpoint);
            UpdateTimestamp();
        }
    }
}
