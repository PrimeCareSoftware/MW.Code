using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Template de email para uso em automações
    /// </summary>
    public class EmailTemplate : BaseEntity
    {
        public string Name { get; private set; }
        public string Subject { get; private set; }
        public string HtmlBody { get; private set; }
        public string PlainTextBody { get; private set; }
        
        // Variáveis disponíveis no template (ex: {{nome_paciente}}, {{data_consulta}})
        private readonly List<string> _availableVariables = new();
        public IReadOnlyCollection<string> AvailableVariables => _availableVariables.AsReadOnly();
        
        private EmailTemplate()
        {
            Name = string.Empty;
            Subject = string.Empty;
            HtmlBody = string.Empty;
            PlainTextBody = string.Empty;
        }
        
        public EmailTemplate(
            string name,
            string subject,
            string htmlBody,
            string plainTextBody,
            List<string> availableVariables,
            string tenantId) : base(tenantId)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            HtmlBody = htmlBody ?? throw new ArgumentNullException(nameof(htmlBody));
            PlainTextBody = plainTextBody ?? throw new ArgumentNullException(nameof(plainTextBody));
            
            if (availableVariables != null)
            {
                _availableVariables.AddRange(availableVariables);
            }
        }
        
        public void Update(
            string name,
            string subject,
            string htmlBody,
            string plainTextBody)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            HtmlBody = htmlBody ?? throw new ArgumentNullException(nameof(htmlBody));
            PlainTextBody = plainTextBody ?? throw new ArgumentNullException(nameof(plainTextBody));
            UpdateTimestamp();
        }
        
        public void AddVariable(string variable)
        {
            if (string.IsNullOrWhiteSpace(variable))
                throw new ArgumentException("Variable cannot be null or empty", nameof(variable));
                
            if (!_availableVariables.Contains(variable))
            {
                _availableVariables.Add(variable);
                UpdateTimestamp();
            }
        }
    }
}
