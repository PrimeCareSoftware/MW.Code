using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class RecurrenceExceptionConfiguration : IEntityTypeConfiguration<RecurrenceException>
    {
        public void Configure(EntityTypeBuilder<RecurrenceException> builder)
        {
            builder.ToTable("RecurrenceExceptions");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.RecurringPatternId)
                .IsRequired();

            builder.Property(e => e.RecurringSeriesId)
                .IsRequired();

            builder.Property(e => e.OriginalDate)
                .IsRequired();

            builder.Property(e => e.ExceptionType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.NewDate)
                .IsRequired(false);

            builder.Property(e => e.NewStartTime)
                .IsRequired(false);

            builder.Property(e => e.NewEndTime)
                .IsRequired(false);

            builder.Property(e => e.Reason)
                .HasMaxLength(500);

            builder.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(e => e.RecurringPattern)
                .WithMany()
                .HasForeignKey(e => e.RecurringPatternId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for efficient querying
            builder.HasIndex(e => new { e.TenantId, e.RecurringPatternId, e.OriginalDate })
                .HasDatabaseName("IX_RecurrenceExceptions_TenantId_PatternId_OriginalDate");

            builder.HasIndex(e => new { e.TenantId, e.RecurringSeriesId, e.OriginalDate })
                .HasDatabaseName("IX_RecurrenceExceptions_TenantId_SeriesId_OriginalDate");

            builder.HasIndex(e => new { e.TenantId, e.RecurringSeriesId })
                .HasDatabaseName("IX_RecurrenceExceptions_TenantId_SeriesId");

            builder.HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_RecurrenceExceptions_TenantId");
        }
    }
}
