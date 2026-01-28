using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services.Workflows;

namespace MedicSoft.Application.Services.Workflows
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventData) where T : class;
    }

    public class EventPublisher : IEventPublisher
    {
        private readonly IWorkflowEngine _workflowEngine;
        private readonly ILogger<EventPublisher> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public EventPublisher(
            IWorkflowEngine workflowEngine,
            ILogger<EventPublisher> logger,
            IBackgroundJobClient backgroundJobClient)
        {
            _workflowEngine = workflowEngine;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task PublishAsync<T>(T eventData) where T : class
        {
            var eventType = typeof(T).Name;
            _logger.LogInformation($"Publishing event: {eventType}");

            // Buscar workflows com este trigger
            var workflows = await _workflowEngine.GetWorkflowsByTriggerAsync(eventType);

            _logger.LogInformation($"Found {workflows.Count} workflows for event {eventType}");

            // Executar cada workflow em background
            foreach (var workflow in workflows)
            {
                _logger.LogInformation($"Enqueueing workflow {workflow.Id} - {workflow.Name}");
                _backgroundJobClient.Enqueue(() => 
                    _workflowEngine.ExecuteWorkflowAsync(workflow.Id, eventData));
            }
        }
    }
}
