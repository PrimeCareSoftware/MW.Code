using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class ComplaintInteractionConfiguration : IEntityTypeConfiguration<ComplaintInteraction>
    {
        public void Configure(EntityTypeBuilder<ComplaintInteraction> builder)
        {
            builder.ToTable("ComplaintInteractions", schema: "crm");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
                .ValueGeneratedNever();

            builder.Property(ci => ci.ComplaintId)
                .IsRequired();

            builder.Property(ci => ci.UserId)
                .IsRequired();

            builder.Property(ci => ci.UserName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ci => ci.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(ci => ci.IsInternal)
                .IsRequired();

            builder.Property(ci => ci.InteractionDate)
                .IsRequired();

            builder.Property(ci => ci.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ci => ci.CreatedAt)
                .IsRequired();

            builder.Property(ci => ci.UpdatedAt)
                .IsRequired();

            builder.HasOne(ci => ci.Complaint)
                .WithMany(c => c.Interactions)
                .HasForeignKey(ci => ci.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(ci => ci.ComplaintId);
            builder.HasIndex(ci => ci.UserId);
            builder.HasIndex(ci => ci.TenantId);
        }
    }
}
