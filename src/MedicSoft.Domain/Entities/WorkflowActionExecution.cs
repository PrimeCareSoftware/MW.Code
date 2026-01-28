using System;

namespace MedicSoft.Domain.Entities
{
    public class WorkflowActionExecution
    {
        public int Id { get; set; }
        public int WorkflowExecutionId { get; set; }
        public virtual WorkflowExecution WorkflowExecution { get; set; }
        
        public int WorkflowActionId { get; set; }
        public virtual WorkflowAction WorkflowAction { get; set; }
        
        public string Status { get; set; } // running, completed, failed
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Error { get; set; }
        public string? Result { get; set; } // JSON
    }
}
