using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;
using System.Text.RegularExpressions;

namespace MedicSoft.Api.Services.CRM
{
    public class AutomationEngine : IAutomationEngine
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<AutomationEngine> _logger;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IWhatsAppService _whatsAppService;

        public AutomationEngine(
            MedicSoftDbContext context,
            ILogger<AutomationEngine> logger,
            IEmailService emailService,
            ISmsService smsService,
            IWhatsAppService whatsAppService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _smsService = smsService;
            _whatsAppService = whatsAppService;
        }

        public async Task ExecuteAutomationAsync(MarketingAutomation automation, Guid patientId, string tenantId)
        {
            try
            {
                _logger.LogInformation("Executing automation {AutomationId} for patient {PatientId}", 
                    automation.Id, patientId);

                // Get patient
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Id == patientId && p.TenantId == tenantId);

                if (patient == null)
                {
                    _logger.LogWarning("Patient {PatientId} not found", patientId);
                    return;
                }

                // Check segmentation
                if (!await IsPatientInSegmentAsync(patient, automation.SegmentFilter))
                {
                    _logger.LogInformation("Patient {PatientId} not in segment for automation {AutomationId}", 
                        patientId, automation.Id);
                    return;
                }

                // Get journey for context
                var journey = await _context.PatientJourneys
                    .FirstOrDefaultAsync(j => j.PacienteId == patientId && j.TenantId == tenantId);

                // Execute actions in order
                foreach (var action in automation.Actions.OrderBy(a => a.Order))
                {
                    await ExecuteActionAsync(action, patient, journey, tenantId);
                }

                // Update execution metrics
                automation.TimesExecuted++;
                automation.LastExecutedAt = DateTime.UtcNow;
                
                // Update success rate using Exponential Moving Average
                var alpha = 0.1; // Weight for new observation
                automation.SuccessRate = (1 - alpha) * automation.SuccessRate + alpha * 1.0;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully executed automation {AutomationId} for patient {PatientId}", 
                    automation.Id, patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing automation {AutomationId} for patient {PatientId}", 
                    automation.Id, patientId);

                // Update success rate
                var alpha = 0.1;
                automation.SuccessRate = (1 - alpha) * automation.SuccessRate + alpha * 0.0;
                await _context.SaveChangesAsync();

                throw;
            }
        }

        public async Task CheckAndTriggerAutomationsAsync(JourneyStageEnum? stage, string? eventName, string tenantId)
        {
            try
            {
                var query = _context.MarketingAutomations
                    .Include(a => a.Actions)
                    .ThenInclude(a => a.EmailTemplate)
                    .Where(a => a.TenantId == tenantId && a.IsActive && !a.IsDeleted);

                // Filter by stage if provided
                if (stage.HasValue)
                {
                    query = query.Where(a => a.TriggerType == AutomationTriggerType.StageChange 
                        && a.TriggerStage == stage.Value);
                }

                // Filter by event if provided
                if (!string.IsNullOrEmpty(eventName))
                {
                    query = query.Where(a => a.TriggerType == AutomationTriggerType.Event 
                        && a.TriggerEvent == eventName);
                }

                var automations = await query.ToListAsync();

                _logger.LogInformation("Found {Count} automations to trigger for stage {Stage} event {Event}", 
                    automations.Count, stage, eventName);

                // Note: In a real implementation, you would get the list of patients
                // that match the criteria and trigger the automation for each
                // This is a simplified version
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking automations for stage {Stage} event {Event}", 
                    stage, eventName);
            }
        }

        private async Task ExecuteActionAsync(
            AutomationAction action, 
            Patient patient, 
            PatientJourney? journey,
            string tenantId)
        {
            try
            {
                switch (action.Type)
                {
                    case ActionType.SendEmail:
                        await ExecuteSendEmailAsync(action, patient);
                        break;

                    case ActionType.SendSMS:
                        await ExecuteSendSmsAsync(action, patient);
                        break;

                    case ActionType.SendWhatsApp:
                        await ExecuteSendWhatsAppAsync(action, patient);
                        break;

                    case ActionType.AddTag:
                        await ExecuteAddTagAsync(action, patient, journey);
                        break;

                    case ActionType.RemoveTag:
                        await ExecuteRemoveTagAsync(action, patient, journey);
                        break;

                    case ActionType.ChangeScore:
                        await ExecuteChangeScoreAsync(action, patient, journey);
                        break;

                    case ActionType.CreateTask:
                        _logger.LogInformation("CreateTask action not yet implemented");
                        break;

                    case ActionType.SendNotification:
                        _logger.LogInformation("SendNotification action not yet implemented");
                        break;

                    case ActionType.WebhookCall:
                        _logger.LogInformation("WebhookCall action not yet implemented");
                        break;

                    default:
                        _logger.LogWarning("Unknown action type: {ActionType}", action.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing action {ActionId} of type {ActionType}", 
                    action.Id, action.Type);
                throw;
            }
        }

        private async Task ExecuteSendEmailAsync(AutomationAction action, Patient patient)
        {
            if (action.EmailTemplateId == null)
            {
                _logger.LogWarning("Email action has no template");
                return;
            }

            var template = await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Id == action.EmailTemplateId.Value);

            if (template == null)
            {
                _logger.LogWarning("Email template {TemplateId} not found", action.EmailTemplateId);
                return;
            }

            var subject = RenderTemplate(template.Subject, patient);
            var body = RenderTemplate(template.HtmlBody, patient);

            await _emailService.SendEmailAsync(patient.Email?.Value ?? "", subject, body);

            _logger.LogInformation("Sent email to patient {PatientId}", patient.Id);
        }

        private async Task ExecuteSendSmsAsync(AutomationAction action, Patient patient)
        {
            if (string.IsNullOrEmpty(action.MessageTemplate))
            {
                _logger.LogWarning("SMS action has no message template");
                return;
            }

            var message = RenderTemplate(action.MessageTemplate, patient);
            await _smsService.SendSmsAsync(patient.Phone?.Value ?? "", message);

            _logger.LogInformation("Sent SMS to patient {PatientId}", patient.Id);
        }

        private async Task ExecuteSendWhatsAppAsync(AutomationAction action, Patient patient)
        {
            if (string.IsNullOrEmpty(action.MessageTemplate))
            {
                _logger.LogWarning("WhatsApp action has no message template");
                return;
            }

            var message = RenderTemplate(action.MessageTemplate, patient);
            await _whatsAppService.SendWhatsAppAsync(patient.Phone?.Value ?? "", message);

            _logger.LogInformation("Sent WhatsApp to patient {PatientId}", patient.Id);
        }

        private async Task ExecuteAddTagAsync(AutomationAction action, Patient patient, PatientJourney? journey)
        {
            if (string.IsNullOrEmpty(action.TagToAdd))
            {
                _logger.LogWarning("AddTag action has no tag specified");
                return;
            }

            if (journey == null)
            {
                _logger.LogWarning("Patient {PatientId} has no journey", patient.Id);
                return;
            }

            if (journey.Tags == null)
                journey.Tags = new List<string>();

            if (!journey.Tags.Contains(action.TagToAdd))
            {
                journey.Tags.Add(action.TagToAdd);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Added tag {Tag} to patient {PatientId}", action.TagToAdd, patient.Id);
            }
        }

        private async Task ExecuteRemoveTagAsync(AutomationAction action, Patient patient, PatientJourney? journey)
        {
            if (string.IsNullOrEmpty(action.TagToRemove))
            {
                _logger.LogWarning("RemoveTag action has no tag specified");
                return;
            }

            if (journey == null)
            {
                _logger.LogWarning("Patient {PatientId} has no journey", patient.Id);
                return;
            }

            if (journey.Tags != null && journey.Tags.Contains(action.TagToRemove))
            {
                journey.Tags.Remove(action.TagToRemove);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Removed tag {Tag} from patient {PatientId}", action.TagToRemove, patient.Id);
            }
        }

        private async Task ExecuteChangeScoreAsync(AutomationAction action, Patient patient, PatientJourney? journey)
        {
            if (!action.ScoreChange.HasValue)
            {
                _logger.LogWarning("ChangeScore action has no score change specified");
                return;
            }

            if (journey == null)
            {
                _logger.LogWarning("Patient {PatientId} has no journey", patient.Id);
                return;
            }

            journey.EngagementScore += action.ScoreChange.Value;
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Changed score by {Change} for patient {PatientId}", 
                action.ScoreChange.Value, patient.Id);
        }

        private async Task<bool> IsPatientInSegmentAsync(Patient patient, string? segmentFilter)
        {
            // If no filter, include all patients
            if (string.IsNullOrEmpty(segmentFilter))
                return true;

            // Simplified segmentation logic
            // In a real implementation, this would parse the JSON filter and apply complex rules
            // For now, just return true
            return true;
        }

        private string RenderTemplate(string template, Patient patient)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            var rendered = template;

            // Replace common variables (Patient entity uses English properties)
            rendered = rendered.Replace("{{nome_paciente}}", patient.Name ?? "");
            rendered = rendered.Replace("{{primeiro_nome}}", patient.Name?.Split(' ').FirstOrDefault() ?? "");
            rendered = rendered.Replace("{{email}}", patient.Email?.Value ?? "");
            rendered = rendered.Replace("{{telefone}}", patient.Phone?.Value ?? "");
            rendered = rendered.Replace("{{celular}}", patient.Phone?.Value ?? "");
            rendered = rendered.Replace("{{data_nascimento}}", patient.DateOfBirth.ToString("dd/MM/yyyy"));
            
            // Add more variables as needed
            rendered = rendered.Replace("{{data_atual}}", DateTime.Now.ToString("dd/MM/yyyy"));
            rendered = rendered.Replace("{{ano_atual}}", DateTime.Now.Year.ToString());

            return rendered;
        }
    }
}
