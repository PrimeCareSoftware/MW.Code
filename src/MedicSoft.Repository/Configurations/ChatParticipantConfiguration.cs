using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            builder.ToTable("ChatParticipants");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Role)
                .IsRequired();

            // Relationship with Conversation
            builder.HasOne(p => p.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(p => p.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with User
            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(p => p.TenantId)
                .HasDatabaseName("IX_ChatParticipants_TenantId");

            builder.HasIndex(p => p.ConversationId)
                .HasDatabaseName("IX_ChatParticipants_ConversationId");

            builder.HasIndex(p => p.UserId)
                .HasDatabaseName("IX_ChatParticipants_UserId");

            builder.HasIndex(p => new { p.ConversationId, p.UserId })
                .IsUnique()
                .HasDatabaseName("IX_ChatParticipants_ConversationId_UserId");

            builder.HasIndex(p => new { p.UserId, p.IsActive })
                .HasDatabaseName("IX_ChatParticipants_UserId_IsActive");
        }
    }
}
