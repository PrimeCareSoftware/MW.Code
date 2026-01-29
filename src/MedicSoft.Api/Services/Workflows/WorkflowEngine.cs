using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.Workflows;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Application.Services.Workflows;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Services.Workflows
{
    public class WorkflowEngine : IWorkflowEngine
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<WorkflowEngine> _logger;
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public WorkflowEngine(
            MedicSoftDbContext context,
            ILogger<WorkflowEngine> logger,
            IEmailService emailService,
            IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<WorkflowExecution> ExecuteWorkflowAsync(int workflowId, object triggerData)
        {
            var workflow = await _context.Workflows
                .Include(w => w.Actions.OrderBy(a => a.Order))
                .FirstOrDefaultAsync(w => w.Id == workflowId);

            if (workflow == null || !workflow.IsEnabled)
            {
                _logger.LogWarning($"Workflow {workflowId} not found or not enabled");
                return null;
            }

            var execution = new WorkflowExecution
            {
                WorkflowId = workflowId,
                Status = "running",
                StartedAt = DateTime.UtcNow,
                TriggerData = JsonSerializer.Serialize(triggerData),
                ActionExecutions = new List<WorkflowActionExecution>()
            };

            _context.WorkflowExecutions.Add(execution);
            await _context.SaveChangesAsync();

            try
            {
                foreach (var action in workflow.Actions)
                {
                    // Verificar condições (if/else)
                    if (!await EvaluateConditionAsync(action.Condition, triggerData, execution))
                    {
                        _logger.LogInformation($"Action {action.Id} condition not met, skipping");
                        continue;
                    }

                    // Delay se configurado
                    if (action.DelaySeconds.HasValue && action.DelaySeconds > 0)
                    {
                        _logger.LogInformation($"Action {action.Id} has delay of {action.DelaySeconds} seconds");
                        // Schedule for later execution
                        _backgroundJobClient.Schedule(
                            () => ExecuteActionAsync(action.Id, execution.Id, triggerData),
                            TimeSpan.FromSeconds(action.DelaySeconds.Value));
                        continue;
                    }

                    await ExecuteActionWithTrackingAsync(action, execution, triggerData);
                }

                execution.Status = "completed";
                execution.CompletedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Workflow {workflowId} execution failed");
                execution.Status = "failed";
                execution.Error = ex.Message;
                execution.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return execution;
        }

        public async Task ExecuteActionAsync(int actionId, int executionId, object triggerData)
        {
            var action = await _context.WorkflowActions.FindAsync(actionId);
            var execution = await _context.WorkflowExecutions
                .Include(e => e.ActionExecutions)
                .FirstOrDefaultAsync(e => e.Id == executionId);

            if (action == null || execution == null)
            {
                _logger.LogWarning($"Action {actionId} or Execution {executionId} not found");
                return;
            }

            await ExecuteActionWithTrackingAsync(action, execution, triggerData);
        }

        private async Task ExecuteActionWithTrackingAsync(WorkflowAction action, WorkflowExecution execution, object triggerData)
        {
            var actionExecution = new WorkflowActionExecution
            {
                WorkflowExecutionId = execution.Id,
                WorkflowActionId = action.Id,
                StartedAt = DateTime.UtcNow,
                Status = "running"
            };

            execution.ActionExecutions.Add(actionExecution);
            await _context.SaveChangesAsync();

            try
            {
                await ExecuteActionInternalAsync(action, triggerData);

                actionExecution.Status = "completed";
                actionExecution.CompletedAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Action {action.Id} execution failed");
                actionExecution.Status = "failed";
                actionExecution.Error = ex.Message;
                actionExecution.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        private async Task ExecuteActionInternalAsync(WorkflowAction action, object triggerData)
        {
            var config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(action.Config);

            switch (action.ActionType.ToLower())
            {
                case "send_email":
                    await ExecuteSendEmailActionAsync(config, triggerData);
                    break;
                case "create_notification":
                    await ExecuteCreateNotificationActionAsync(config, triggerData);
                    break;
                case "add_tag":
                    await ExecuteAddTagActionAsync(config, triggerData);
                    break;
                case "create_ticket":
                    await ExecuteCreateTicketActionAsync(config, triggerData);
                    break;
                case "webhook":
                    await ExecuteWebhookActionAsync(config, triggerData);
                    break;
                default:
                    throw new NotSupportedException($"Action type '{action.ActionType}' not supported");
            }
        }

        private async Task ExecuteSendEmailActionAsync(Dictionary<string, JsonElement> config, object triggerData)
        {
            var to = ReplaceVariables(config["to"].GetString(), triggerData);
            var subject = ReplaceVariables(config["subject"].GetString(), triggerData);
            var body = ReplaceVariables(config["body"].GetString(), triggerData);

            await _emailService.SendEmailAsync(to, subject, body);
            _logger.LogInformation($"Email sent to {to}");
        }

        private async Task ExecuteCreateNotificationActionAsync(Dictionary<string, JsonElement> config, object triggerData)
        {
            var type = config.ContainsKey("type") ? config["type"].GetString() : "info";
            var title = ReplaceVariables(config["title"].GetString(), triggerData);
            var message = ReplaceVariables(config["message"].GetString(), triggerData);

            var notification = new SystemNotification
            {
                Type = type,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.SystemNotifications.Add(notification);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"System notification created: {title}");
        }

        private async Task ExecuteAddTagActionAsync(Dictionary<string, JsonElement> config, object triggerData)
        {
            var tagName = ReplaceVariables(config["tagName"].GetString(), triggerData);
            var clinicId = GetFieldValue("clinicId", triggerData);

            if (clinicId == null)
            {
                _logger.LogWarning("ClinicId not found in trigger data for add_tag action");
                return;
            }

            // Find or create tag
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName);
            if (tag == null)
            {
                tag = new Tag
                {
                    Name = tagName,
                    Color = "#808080",
                    CreatedAt = DateTime.UtcNow
                };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }

            // Check if clinic already has this tag
            var clinicTag = await _context.ClinicTags
                .FirstOrDefaultAsync(ct => ct.ClinicId == int.Parse(clinicId.ToString()) && ct.TagId == tag.Id);

            if (clinicTag == null)
            {
                clinicTag = new ClinicTag
                {
                    ClinicId = int.Parse(clinicId.ToString()),
                    TagId = tag.Id,
                    AddedAt = DateTime.UtcNow
                };
                _context.ClinicTags.Add(clinicTag);
                await _context.SaveChangesAsync();
            }

            _logger.LogInformation($"Tag '{tagName}' added to clinic {clinicId}");
        }

        private async Task ExecuteCreateTicketActionAsync(Dictionary<string, JsonElement> config, object triggerData)
        {
            var subject = ReplaceVariables(config["subject"].GetString(), triggerData);
            var priority = config.ContainsKey("priority") ? config["priority"].GetString() : "normal";
            var category = config.ContainsKey("category") ? config["category"].GetString() : "general";
            var clinicId = GetFieldValue("clinicId", triggerData);

            var ticket = new Ticket
            {
                Subject = subject,
                Description = $"Auto-generated from workflow. Context: {JsonSerializer.Serialize(triggerData)}",
                Status = "open",
                Priority = priority,
                Category = category,
                CreatedAt = DateTime.UtcNow
            };

            if (clinicId != null)
            {
                ticket.ClinicId = int.Parse(clinicId.ToString());
            }

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Ticket created: {subject}");
        }

        private async Task ExecuteWebhookActionAsync(Dictionary<string, JsonElement> config, object triggerData)
        {
            var url = config["url"].GetString();
            var payload = JsonSerializer.Serialize(triggerData);

            using var httpClient = new System.Net.Http.HttpClient();
            var content = new System.Net.Http.StringContent(payload, System.Text.Encoding.UTF8, "application/json");
            
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            
            _logger.LogInformation($"Webhook delivered to {url}");
        }

        private async Task<bool> EvaluateConditionAsync(string conditionJson, object triggerData, WorkflowExecution execution)
        {
            if (string.IsNullOrEmpty(conditionJson))
                return true;

            try
            {
                var condition = JsonSerializer.Deserialize<ConditionDto>(conditionJson);
                var value = GetFieldValue(condition.Field, triggerData);

                return condition.Operator switch
                {
                    "==" => value?.ToString() == condition.Value?.ToString(),
                    "!=" => value?.ToString() != condition.Value?.ToString(),
                    ">" => decimal.TryParse(value?.ToString(), out var v1) && decimal.TryParse(condition.Value?.ToString(), out var cv1) && v1 > cv1,
                    ">=" => decimal.TryParse(value?.ToString(), out var v2) && decimal.TryParse(condition.Value?.ToString(), out var cv2) && v2 >= cv2,
                    "<" => decimal.TryParse(value?.ToString(), out var v3) && decimal.TryParse(condition.Value?.ToString(), out var cv3) && v3 < cv3,
                    "<=" => decimal.TryParse(value?.ToString(), out var v4) && decimal.TryParse(condition.Value?.ToString(), out var cv4) && v4 <= cv4,
                    "contains" => value?.ToString()?.Contains(condition.Value?.ToString() ?? "", StringComparison.OrdinalIgnoreCase) ?? false,
                    _ => true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating condition");
                return true; // Continue on error
            }
        }

        private string ReplaceVariables(string template, object data)
        {
            if (string.IsNullOrEmpty(template))
                return template;

            var json = JsonSerializer.Serialize(data);
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            foreach (var kvp in dict)
            {
                var value = kvp.Value.ValueKind == JsonValueKind.String 
                    ? kvp.Value.GetString() 
                    : kvp.Value.ToString();
                template = template.Replace($"{{{{{kvp.Key}}}}}", value ?? "");
            }

            return template;
        }

        private object GetFieldValue(string field, object data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data);
                var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                if (dict.ContainsKey(field))
                {
                    var element = dict[field];
                    return element.ValueKind switch
                    {
                        JsonValueKind.String => element.GetString(),
                        JsonValueKind.Number => element.GetDecimal(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => element.ToString()
                    };
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task RegisterTriggerAsync(string triggerType, Func<object, Task> callback)
        {
            // This is for future dynamic trigger registration
            _logger.LogInformation($"Trigger registered: {triggerType}");
            await Task.CompletedTask;
        }

        public async Task<List<Workflow>> GetWorkflowsByTriggerAsync(string triggerType)
        {
            return await _context.Workflows
                .Where(w => w.IsEnabled && w.TriggerType == triggerType)
                .Include(w => w.Actions.OrderBy(a => a.Order))
                .ToListAsync();
        }
    }
}
