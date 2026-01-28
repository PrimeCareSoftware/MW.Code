using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class WorkflowActionExecutionConfiguration : IEntityTypeConfiguration<WorkflowActionExecution>
    {
        public void Configure(EntityTypeBuilder<WorkflowActionExecution> builder)
        {
            builder.ToTable("WorkflowActionExecutions");
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(e => e.Error)
                .HasColumnType("text");
            
            builder.Property(e => e.Result)
                .HasColumnType("text");
            
            // Indexes
            builder.HasIndex(e => e.WorkflowExecutionId);
            builder.HasIndex(e => e.WorkflowActionId);
            builder.HasIndex(e => e.Status);
            
            builder.HasOne(e => e.WorkflowExecution)
                .WithMany(we => we.ActionExecutions)
                .HasForeignKey(e => e.WorkflowExecutionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(e => e.WorkflowAction)
                .WithMany(a => a.Executions)
                .HasForeignKey(e => e.WorkflowActionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
