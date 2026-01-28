using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class WorkflowExecutionConfiguration : IEntityTypeConfiguration<WorkflowExecution>
    {
        public void Configure(EntityTypeBuilder<WorkflowExecution> builder)
        {
            builder.ToTable("WorkflowExecutions");
            builder.HasKey(e => e.Id);
            
            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(e => e.TriggerData)
                .IsRequired()
                .HasColumnType("text");
            
            builder.Property(e => e.Error)
                .HasColumnType("text");
            
            // Indexes
            builder.HasIndex(e => e.WorkflowId);
            builder.HasIndex(e => e.StartedAt);
            builder.HasIndex(e => e.Status);
            
            builder.HasOne(e => e.Workflow)
                .WithMany(w => w.Executions)
                .HasForeignKey(e => e.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(e => e.ActionExecutions)
                .WithOne(ae => ae.WorkflowExecution)
                .HasForeignKey(ae => ae.WorkflowExecutionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
