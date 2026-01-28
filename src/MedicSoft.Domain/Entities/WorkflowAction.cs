using System;

namespace MedicSoft.Domain.Entities
{
    public class WorkflowAction
    {
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public virtual Workflow Workflow { get; set; }
        
        public int Order { get; set; }
        public string ActionType { get; set; } // email, sms, webhook, tag, ticket, notification
        public string Config { get; set; } // JSON
        public string Condition { get; set; } // JSON (if/else logic)
        public int? DelaySeconds { get; set; } // opcional
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public virtual ICollection<WorkflowActionExecution> Executions { get; set; }
    }
}
