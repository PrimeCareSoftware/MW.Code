using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }
        
        public bool IsEnabled { get; set; } = true;
        
        [Required(ErrorMessage = "TriggerType is required")]
        [MaxLength(100, ErrorMessage = "TriggerType cannot exceed 100 characters")]
        public string TriggerType { get; set; }
        
        public string TriggerConfig { get; set; }
        public bool StopOnError { get; set; } = false;
        public List<CreateWorkflowActionDto> Actions { get; set; }
    }
    
    public class CreateWorkflowActionDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Order must be a positive number")]
        public int Order { get; set; }
        
        [Required(ErrorMessage = "ActionType is required")]
        [MaxLength(50, ErrorMessage = "ActionType cannot exceed 50 characters")]
        public string ActionType { get; set; }
        
        [Required(ErrorMessage = "Config is required")]
        public string Config { get; set; }
        
        public string Condition { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "DelaySeconds must be a positive number")]
        public int? DelaySeconds { get; set; }
    }

    public class UpdateWorkflowDto
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        public string Name { get; set; }
        
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }
        
        public bool IsEnabled { get; set; }
        
        [Required(ErrorMessage = "TriggerType is required")]
        [MaxLength(100, ErrorMessage = "TriggerType cannot exceed 100 characters")]
        public string TriggerType { get; set; }
        
        public string TriggerConfig { get; set; }
        public bool StopOnError { get; set; }
        public List<CreateWorkflowActionDto> Actions { get; set; }
    }
}
