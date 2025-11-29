using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Id)
                .ValueGeneratedNever();

            builder.Property(n => n.PatientId)
                .IsRequired();

            builder.Property(n => n.AppointmentId);

            builder.Property(n => n.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.Channel)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.Recipient)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(n => n.Message)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(n => n.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(n => n.SentAt);

            builder.Property(n => n.DeliveredAt);

            builder.Property(n => n.ReadAt);

            builder.Property(n => n.ErrorMessage)
                .HasMaxLength(2000);

            builder.Property(n => n.RetryCount)
                .IsRequired();

            builder.Property(n => n.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.Property(n => n.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(n => n.Patient)
                .WithMany()
                .HasForeignKey(n => n.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.Appointment)
                .WithMany()
                .HasForeignKey(n => n.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(n => n.PatientId)
                .HasDatabaseName("IX_Notifications_PatientId");

            builder.HasIndex(n => n.AppointmentId)
                .HasDatabaseName("IX_Notifications_AppointmentId");

            builder.HasIndex(n => new { n.TenantId, n.Status })
                .HasDatabaseName("IX_Notifications_TenantId_Status");

            builder.HasIndex(n => new { n.Status, n.RetryCount })
                .HasDatabaseName("IX_Notifications_Status_RetryCount");

            builder.HasIndex(n => n.CreatedAt)
                .HasDatabaseName("IX_Notifications_CreatedAt");
        }
    }
}
