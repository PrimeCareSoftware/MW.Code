using System;
using System.Collections.Generic;

namespace MedicSoft.Domain.Entities
{
    public class Workflow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public string TriggerType { get; set; } // time, event
        public string TriggerConfig { get; set; } // JSON
        public bool StopOnError { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        
        public virtual ICollection<WorkflowAction> Actions { get; set; }
        public virtual ICollection<WorkflowExecution> Executions { get; set; }
    }
}
