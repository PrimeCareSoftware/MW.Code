using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services.Workflows
{
    public interface IWorkflowEngine
    {
        Task<WorkflowExecution> ExecuteWorkflowAsync(int workflowId, object triggerData);
        Task RegisterTriggerAsync(string triggerType, Func<object, Task> callback);
        Task<List<Workflow>> GetWorkflowsByTriggerAsync(string triggerType);
    }
}
