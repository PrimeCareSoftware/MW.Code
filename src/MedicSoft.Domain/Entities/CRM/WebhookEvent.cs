namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Tipos de eventos de webhook do CRM
    /// </summary>
    public enum WebhookEvent
    {
        // Patient Journey Events
        JourneyStageChanged = 1,
        TouchpointCreated = 2,
        
        // Marketing Automation Events
        AutomationExecuted = 10,
        CampaignSent = 11,
        
        // Survey Events
        SurveyCreated = 20,
        SurveyCompleted = 21,
        NpsScoreCalculated = 22,
        
        // Complaint Events
        ComplaintCreated = 30,
        ComplaintStatusChanged = 31,
        ComplaintResolved = 32,
        
        // Sentiment Analysis Events
        SentimentAnalyzed = 40,
        NegativeSentimentDetected = 41,
        
        // Churn Prediction Events
        ChurnRiskCalculated = 50,
        HighChurnRiskDetected = 51
    }
}
