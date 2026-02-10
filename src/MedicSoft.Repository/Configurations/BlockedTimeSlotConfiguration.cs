using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class BlockedTimeSlotConfiguration : IEntityTypeConfiguration<BlockedTimeSlot>
    {
        public void Configure(EntityTypeBuilder<BlockedTimeSlot> builder)
        {
            builder.ToTable("BlockedTimeSlots");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedNever();

            builder.Property(b => b.Date)
                .IsRequired();

            builder.Property(b => b.StartTime)
                .IsRequired();

            builder.Property(b => b.EndTime)
                .IsRequired();

            builder.Property(b => b.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(b => b.Reason)
                .HasMaxLength(500);

            builder.Property(b => b.IsRecurring)
                .IsRequired();

            builder.Property(b => b.RecurringPatternId)
                .IsRequired(false);

            builder.Property(b => b.RecurringSeriesId)
                .IsRequired(false);

            builder.Property(b => b.OriginalOccurrenceDate)
                .IsRequired(false);

            builder.Property(b => b.IsException)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(b => b.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(b => b.Clinic)
                .WithMany()
                .HasForeignKey(b => b.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Professional)
                .WithMany()
                .HasForeignKey(b => b.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(b => b.RecurringPattern)
                .WithMany()
                .HasForeignKey(b => b.RecurringPatternId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(b => new { b.TenantId, b.Date, b.ClinicId })
                .HasDatabaseName("IX_BlockedTimeSlots_TenantId_Date_Clinic");

            builder.HasIndex(b => new { b.TenantId, b.ClinicId, b.ProfessionalId })
                .HasDatabaseName("IX_BlockedTimeSlots_TenantId_Clinic_Professional");

            builder.HasIndex(b => new { b.TenantId, b.RecurringPatternId })
                .HasDatabaseName("IX_BlockedTimeSlots_TenantId_RecurringPatternId");

            builder.HasIndex(b => new { b.TenantId, b.RecurringSeriesId })
                .HasDatabaseName("IX_BlockedTimeSlots_TenantId_RecurringSeriesId");

            builder.HasIndex(b => b.TenantId)
                .HasDatabaseName("IX_BlockedTimeSlots_TenantId");
        }
    }
}
