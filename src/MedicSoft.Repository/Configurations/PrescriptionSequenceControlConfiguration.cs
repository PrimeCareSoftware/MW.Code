using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class PrescriptionSequenceControlConfiguration : IEntityTypeConfiguration<PrescriptionSequenceControl>
    {
        public void Configure(EntityTypeBuilder<PrescriptionSequenceControl> builder)
        {
            builder.ToTable("PrescriptionSequenceControls");

            builder.HasKey(psc => psc.Id);

            builder.Property(psc => psc.Id)
                .ValueGeneratedNever();

            builder.Property(psc => psc.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(psc => psc.CurrentSequence)
                .IsRequired();

            builder.Property(psc => psc.Year)
                .IsRequired();

            builder.Property(psc => psc.Prefix)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(psc => psc.LastGeneratedAt)
                .IsRequired();

            builder.Property(psc => psc.TotalGenerated)
                .IsRequired();

            builder.Property(psc => psc.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(psc => psc.CreatedAt)
                .IsRequired();

            builder.Property(psc => psc.UpdatedAt);

            // Indexes
            builder.HasIndex(psc => new { psc.TenantId, psc.Type, psc.Year })
                .IsUnique()
                .HasDatabaseName("IX_PrescriptionSequenceControls_Unique");

            builder.HasIndex(psc => new { psc.TenantId, psc.Year })
                .HasDatabaseName("IX_PrescriptionSequenceControls_TenantId_Year");
        }
    }
}
