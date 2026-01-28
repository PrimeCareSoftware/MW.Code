using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.Workflows
{
    public class WorkflowDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public string TriggerType { get; set; }
        public string TriggerConfig { get; set; }
        public bool StopOnError { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public List<WorkflowActionDto> Actions { get; set; }
    }
    
    public class WorkflowActionDto
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string ActionType { get; set; }
        public string Config { get; set; }
        public string Condition { get; set; }
        public int? DelaySeconds { get; set; }
    }
    
    public class CreateWorkflowDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string TriggerType { get; set; }
        public string TriggerConfig { get; set; }
        public bool StopOnError { get; set; } = false;
        public List<CreateWorkflowActionDto> Actions { get; set; }
    }
    
    public class CreateWorkflowActionDto
    {
        public int Order { get; set; }
        public string ActionType { get; set; }
        public string Config { get; set; }
        public string Condition { get; set; }
        public int? DelaySeconds { get; set; }
    }

    public class UpdateWorkflowDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public string TriggerType { get; set; }
        public string TriggerConfig { get; set; }
        public bool StopOnError { get; set; }
        public List<CreateWorkflowActionDto> Actions { get; set; }
    }
}
