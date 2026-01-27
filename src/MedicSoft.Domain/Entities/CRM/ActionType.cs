namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Tipos de ações em automações de marketing
    /// </summary>
    public enum ActionType
    {
        SendEmail,
        SendSMS,
        SendWhatsApp,
        AddTag,
        RemoveTag,
        ChangeScore,
        CreateTask,
        SendNotification,
        WebhookCall
    }
}
