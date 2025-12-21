using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(EntityTypeBuilder<TicketHistory> builder)
        {
            builder.ToTable("TicketHistory");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .ValueGeneratedNever();

            builder.Property(h => h.OldStatus)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(h => h.NewStatus)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(h => h.ChangedByName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(h => h.TicketId)
                .HasDatabaseName("IX_TicketHistory_TicketId");

            builder.HasIndex(h => h.TenantId)
                .HasDatabaseName("IX_TicketHistory_TenantId");
        }
    }
}
