using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class NotificationRoutineConfiguration : IEntityTypeConfiguration<NotificationRoutine>
    {
        public void Configure(EntityTypeBuilder<NotificationRoutine> builder)
        {
            builder.ToTable("NotificationRoutines");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .HasMaxLength(1000);

            builder.Property(r => r.Channel)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.MessageTemplate)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(r => r.ScheduleType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.ScheduleConfiguration)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.Scope)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(r => r.IsActive)
                .IsRequired();

            builder.Property(r => r.MaxRetries)
                .IsRequired();

            builder.Property(r => r.RecipientFilter)
                .HasMaxLength(2000);

            builder.Property(r => r.LastExecutedAt);

            builder.Property(r => r.NextExecutionAt);

            builder.Property(r => r.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(r => new { r.TenantId, r.IsActive })
                .HasDatabaseName("IX_NotificationRoutines_TenantId_IsActive");

            builder.HasIndex(r => new { r.Scope, r.IsActive })
                .HasDatabaseName("IX_NotificationRoutines_Scope_IsActive");

            builder.HasIndex(r => r.NextExecutionAt)
                .HasDatabaseName("IX_NotificationRoutines_NextExecutionAt");

            builder.HasIndex(r => new { r.Channel, r.TenantId })
                .HasDatabaseName("IX_NotificationRoutines_Channel_TenantId");

            builder.HasIndex(r => new { r.Type, r.TenantId })
                .HasDatabaseName("IX_NotificationRoutines_Type_TenantId");
        }
    }
}
