using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
    {
        public void Configure(EntityTypeBuilder<Procedure> builder)
        {
            builder.ToTable("Procedures");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => new { p.Code, p.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Procedures_Code_TenantId");

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.Property(p => p.Category)
                .IsRequired();

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.DurationMinutes)
                .IsRequired();

            builder.Property(p => p.RequiresMaterials)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.Property(p => p.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt);

            // Navigation properties
            builder.HasMany(p => p.Materials)
                .WithOne()
                .HasForeignKey("ProcedureId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
