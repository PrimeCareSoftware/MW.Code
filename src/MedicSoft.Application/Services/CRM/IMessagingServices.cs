namespace MedicSoft.Application.Services.CRM
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailWithTemplateAsync(string to, Guid templateId, Dictionary<string, string> variables, string? tenantId = null);
    }

    public interface ISmsService
    {
        Task SendSmsAsync(string to, string message);
    }

    public interface IWhatsAppService
    {
        Task SendWhatsAppAsync(string to, string message);
    }
}
