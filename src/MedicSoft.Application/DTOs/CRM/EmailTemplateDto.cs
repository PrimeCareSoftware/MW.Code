namespace MedicSoft.Application.DTOs.CRM
{
    public class EmailTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public string? PlainTextBody { get; set; }
        public List<string> AvailableVariables { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateEmailTemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public string? PlainTextBody { get; set; }
    }

    public class UpdateEmailTemplateDto
    {
        public string? Name { get; set; }
        public string? Subject { get; set; }
        public string? HtmlBody { get; set; }
        public string? PlainTextBody { get; set; }
    }
}
