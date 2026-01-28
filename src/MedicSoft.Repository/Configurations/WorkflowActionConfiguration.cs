using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class WorkflowActionConfiguration : IEntityTypeConfiguration<WorkflowAction>
    {
        public void Configure(EntityTypeBuilder<WorkflowAction> builder)
        {
            builder.ToTable("WorkflowActions");
            builder.HasKey(a => a.Id);
            
            builder.Property(a => a.ActionType)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.Property(a => a.Config)
                .IsRequired()
                .HasColumnType("jsonb");
            
            builder.Property(a => a.Condition)
                .HasColumnType("jsonb");
            
            builder.HasOne(a => a.Workflow)
                .WithMany(w => w.Actions)
                .HasForeignKey(a => a.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(a => a.Executions)
                .WithOne(e => e.WorkflowAction)
                .HasForeignKey(e => e.WorkflowActionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
