using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Automação de marketing para engajamento automático com pacientes
    /// </summary>
    public class MarketingAutomation : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; }
        
        // Triggers
        public AutomationTriggerType TriggerType { get; private set; }
        public JourneyStageEnum? TriggerStage { get; private set; }
        public string? TriggerEvent { get; private set; } // "appointment_scheduled", "no_show", etc.
        public int? DelayMinutes { get; private set; } // Delay após trigger
        
        // Segmentação
        public string? SegmentFilter { get; private set; } // JSON com filtros
        private readonly List<string> _tags = new();
        public IReadOnlyCollection<string> Tags => _tags.AsReadOnly();
        
        // Ações
        private readonly List<AutomationAction> _actions = new();
        public IReadOnlyCollection<AutomationAction> Actions => _actions.AsReadOnly();
        
        // Métricas
        public int TimesExecuted { get; private set; }
        public DateTime? LastExecutedAt { get; private set; }
        public double SuccessRate { get; private set; }
        
        private MarketingAutomation()
        {
            Name = string.Empty;
            Description = string.Empty;
        }
        
        public MarketingAutomation(
            string name,
            string description,
            AutomationTriggerType triggerType,
            string tenantId) : base(tenantId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            TriggerType = triggerType;
            IsActive = false; // Start inactive, must be explicitly activated
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
            int? delayMinutes)
        {
            TriggerStage = triggerStage;
            TriggerEvent = triggerEvent;
            DelayMinutes = delayMinutes;
            UpdateTimestamp();
        }
        
        public void SetSegmentFilter(string segmentFilter)
        {
            SegmentFilter = segmentFilter;
            UpdateTimestamp();
        }
        
        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentException("Tag cannot be null or empty", nameof(tag));
                
            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                UpdateTimestamp();
            }
        }
        
        public void RemoveTag(string tag)
        {
            if (_tags.Contains(tag))
            {
                _tags.Remove(tag);
                UpdateTimestamp();
            }
        }
        
        public void AddAction(AutomationAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
                
            _actions.Add(action);
            UpdateTimestamp();
        }
        
        public void RemoveAction(Guid actionId)
        {
            var action = _actions.FirstOrDefault(a => a.Id == actionId);
            if (action != null)
            {
                _actions.Remove(action);
                UpdateTimestamp();
            }
        }
        
        public void RecordExecution(bool success)
        {
            TimesExecuted++;
            LastExecutedAt = DateTime.UtcNow;
            
            // Update success rate (exponential moving average for trend tracking)
            // Note: This EMA gives more weight to recent executions (alpha=0.2)
            // to track current performance trends rather than overall historical average
            if (TimesExecuted == 1)
            {
                SuccessRate = success ? 1.0 : 0.0;
            }
            else
            {
                const double alpha = 0.2; // Smoothing factor (20% weight on current, 80% on history)
                SuccessRate = alpha * (success ? 1.0 : 0.0) + (1 - alpha) * SuccessRate;
            }
            
            UpdateTimestamp();
        }
    }
}
