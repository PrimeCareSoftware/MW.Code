using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("Tickets");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .IsRequired();

            builder.Property(t => t.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.UserId)
                .IsRequired();

            builder.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.UserEmail)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(t => t.ClinicName)
                .HasMaxLength(200);

            builder.Property(t => t.AssignedToName)
                .HasMaxLength(200);

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(t => new { t.TenantId, t.UserId })
                .HasDatabaseName("IX_Tickets_TenantId_UserId");

            builder.HasIndex(t => new { t.TenantId, t.ClinicId })
                .HasDatabaseName("IX_Tickets_TenantId_ClinicId");

            builder.HasIndex(t => new { t.Status, t.TenantId })
                .HasDatabaseName("IX_Tickets_Status_TenantId");

            builder.HasIndex(t => t.TenantId)
                .HasDatabaseName("IX_Tickets_TenantId");

            // Relationships
            builder.HasMany(t => t.Comments)
                .WithOne(c => c.Ticket)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.Attachments)
                .WithOne(a => a.Ticket)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.History)
                .WithOne(h => h.Ticket)
                .HasForeignKey(h => h.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
