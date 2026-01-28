using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            builder.ToTable("Workflows");
            builder.HasKey(w => w.Id);
            
            builder.Property(w => w.Name)
                .IsRequired()
                .HasMaxLength(200);
            
            builder.Property(w => w.Description)
                .HasMaxLength(1000);
            
            builder.Property(w => w.TriggerType)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(w => w.TriggerConfig)
                .HasColumnType("jsonb");
            
            builder.Property(w => w.CreatedBy)
                .HasMaxLength(100);
            
            builder.Property(w => w.UpdatedBy)
                .HasMaxLength(100);
            
            builder.HasMany(w => w.Actions)
                .WithOne(a => a.Workflow)
                .HasForeignKey(a => a.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(w => w.Executions)
                .WithOne(e => e.Workflow)
                .HasForeignKey(e => e.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
