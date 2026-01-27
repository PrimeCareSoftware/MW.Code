using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
    {
        public void Configure(EntityTypeBuilder<Complaint> builder)
        {
            builder.ToTable("Complaints", schema: "crm");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.ProtocolNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.PatientId)
                .IsRequired();

            builder.Property(c => c.Subject)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.Category)
                .IsRequired();

            builder.Property(c => c.Priority)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(c => c.AssignedToUserId);

            builder.Property(c => c.AssignedToUserName)
                .HasMaxLength(200);

            builder.Property(c => c.ReceivedAt)
                .IsRequired();

            builder.Property(c => c.FirstResponseAt);

            builder.Property(c => c.ResolvedAt);

            builder.Property(c => c.ClosedAt);

            builder.Property(c => c.ResponseTimeMinutes);

            builder.Property(c => c.ResolutionTimeMinutes);

            builder.Property(c => c.SatisfactionRating);

            builder.Property(c => c.SatisfactionFeedback)
                .HasMaxLength(1000);

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired();

            builder.HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<ComplaintInteraction>()
                .WithOne()
                .HasForeignKey("ComplaintId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.ProtocolNumber).IsUnique();
            builder.HasIndex(c => c.PatientId);
            builder.HasIndex(c => c.Status);
            builder.HasIndex(c => c.Priority);
            builder.HasIndex(c => c.AssignedToUserId);
            builder.HasIndex(c => c.TenantId);
        }
    }
}
