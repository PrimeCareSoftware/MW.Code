using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
    {
        public void Configure(EntityTypeBuilder<TicketComment> builder)
        {
            builder.ToTable("TicketComments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.Comment)
                .IsRequired();

            builder.Property(c => c.AuthorName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(c => c.TicketId)
                .HasDatabaseName("IX_TicketComments_TicketId");

            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("IX_TicketComments_TenantId");
        }
    }
}
