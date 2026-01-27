using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Ação individual dentro de uma automação de marketing
    /// </summary>
    public class AutomationAction : BaseEntity
    {
        public Guid MarketingAutomationId { get; private set; }
        public MarketingAutomation MarketingAutomation { get; private set; } = null!;
        
        public int Order { get; private set; }
        public ActionType Type { get; private set; }
        
        // Email
        public Guid? EmailTemplateId { get; private set; }
        public EmailTemplate? EmailTemplate { get; private set; }
        
        // SMS/WhatsApp
        public string? MessageTemplate { get; private set; }
        public string? Channel { get; private set; } // Email, SMS, WhatsApp
        
        // Tags/Score
        public string? TagToAdd { get; private set; }
        public int? ScoreChange { get; private set; }
        
        // Condicional
        public string? Condition { get; private set; } // Expressão condicional
        
        private AutomationAction() { }
        
        public AutomationAction(
            Guid marketingAutomationId,
            int order,
            ActionType type,
            string tenantId) : base(tenantId)
        {
            MarketingAutomationId = marketingAutomationId;
            Order = order;
            Type = type;
        }
        
        public void ConfigureEmailAction(Guid emailTemplateId)
        {
            if (Type != ActionType.SendEmail)
                throw new InvalidOperationException("This action is not of type SendEmail");
                
            EmailTemplateId = emailTemplateId;
            UpdateTimestamp();
        }
        
        public void ConfigureMessageAction(string messageTemplate, string channel)
        {
            if (Type != ActionType.SendSMS && Type != ActionType.SendWhatsApp)
                throw new InvalidOperationException("This action is not a message action");
                
            MessageTemplate = messageTemplate ?? throw new ArgumentNullException(nameof(messageTemplate));
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            UpdateTimestamp();
        }
        
        public void ConfigureTagAction(string tag)
        {
            if (Type != ActionType.AddTag && Type != ActionType.RemoveTag)
                throw new InvalidOperationException("This action is not a tag action");
                
            TagToAdd = tag ?? throw new ArgumentNullException(nameof(tag));
            UpdateTimestamp();
        }
        
        public void ConfigureScoreAction(int scoreChange)
        {
            if (Type != ActionType.ChangeScore)
                throw new InvalidOperationException("This action is not of type ChangeScore");
                
            ScoreChange = scoreChange;
            UpdateTimestamp();
        }
        
        public void SetCondition(string condition)
        {
            Condition = condition;
            UpdateTimestamp();
        }
    }
}
