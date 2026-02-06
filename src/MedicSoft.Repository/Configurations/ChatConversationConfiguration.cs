using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
    {
        public void Configure(EntityTypeBuilder<ChatConversation> builder)
        {
            builder.ToTable("ChatConversations");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Type)
                .IsRequired();

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Configure the LastMessage relationship
            // This is a one-way navigation property that doesn't need to be tracked by the inverse side
            builder.HasOne(c => c.LastMessage)
                .WithMany() // No inverse navigation property
                .HasForeignKey(c => c.LastMessageId)
                .OnDelete(DeleteBehavior.SetNull); // Set to null if message is deleted

            // Relationship with CreatedBy (User)
            builder.HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relationship with Participants
            builder.HasMany(c => c.Participants)
                .WithOne(p => p.Conversation)
                .HasForeignKey(p => p.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with Messages
            builder.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("IX_ChatConversations_TenantId");

            builder.HasIndex(c => c.CreatedByUserId)
                .HasDatabaseName("IX_ChatConversations_CreatedByUserId");

            builder.HasIndex(c => c.LastMessageId)
                .HasDatabaseName("IX_ChatConversations_LastMessageId");

            builder.HasIndex(c => c.LastMessageAt)
                .HasDatabaseName("IX_ChatConversations_LastMessageAt");
        }
    }
}
