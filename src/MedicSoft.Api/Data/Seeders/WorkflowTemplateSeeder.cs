using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Data.Seeders
{
    public class WorkflowTemplateSeeder
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<WorkflowTemplateSeeder> _logger;

        public WorkflowTemplateSeeder(MedicSoftDbContext context, ILogger<WorkflowTemplateSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            _logger.LogInformation("Starting workflow template seeding");

            if (await _context.Workflows.AnyAsync())
            {
                _logger.LogInformation("Workflows already exist, skipping seed");
                return;
            }

            var workflows = new List<Workflow>
            {
                CreateOnboardingWorkflow(),
                CreateChurnPreventionWorkflow(),
                CreateTrialExpiringWorkflow()
            };

            await _context.Workflows.AddRangeAsync(workflows);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {workflows.Count} workflow templates");
        }

        private Workflow CreateOnboardingWorkflow()
        {
            return new Workflow
            {
                Name = "Onboarding Automático",
                Description = "Sequência de boas-vindas para novas clínicas",
                IsEnabled = true,
                TriggerType = "ClinicCreatedEvent",
                TriggerConfig = "{}",
                StopOnError = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                Actions = new List<WorkflowAction>
                {
                    new WorkflowAction
                    {
                        Order = 1,
                        ActionType = "send_email",
                        Config = JsonSerializer.Serialize(new
                        {
                            to = "{{Email}}",
                            subject = "Bem-vindo ao PrimeCare!",
                            body = "Olá! Bem-vindo ao PrimeCare!"
                        }),
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };
        }

        private Workflow CreateChurnPreventionWorkflow()
        {
            return new Workflow
            {
                Name = "Prevenção de Churn",
                Description = "Ações proativas para clientes inativos",
                IsEnabled = true,
                TriggerType = "InactivityDetectedEvent",
                TriggerConfig = "{}",
                StopOnError = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                Actions = new List<WorkflowAction>()
            };
        }

        private Workflow CreateTrialExpiringWorkflow()
        {
            return new Workflow
            {
                Name = "Trial Expirando",
                Description = "Conversão de trial para pago",
                IsEnabled = true,
                TriggerType = "TrialExpiringEvent",
                TriggerConfig = "{}",
                StopOnError = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                Actions = new List<WorkflowAction>()
            };
        }
    }
}
