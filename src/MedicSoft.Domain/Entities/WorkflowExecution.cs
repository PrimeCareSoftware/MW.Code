using System;
using System.Collections.Generic;

namespace MedicSoft.Domain.Entities
{
    public class WorkflowExecution
    {
        public int Id { get; set; }
        public int WorkflowId { get; set; }
        public virtual Workflow Workflow { get; set; }
        
        public string Status { get; set; } // pending, running, completed, failed
        public string TriggerData { get; set; } // JSON - dados que dispararam
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Error { get; set; }
        
        public virtual ICollection<WorkflowActionExecution> ActionExecutions { get; set; }
    }
}
