using Learnup.Domain.AggregateRoots.Conversations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class ConversationItemConfiguration : IEntityTypeConfiguration<ConversationItem>
{
    public void Configure(EntityTypeBuilder<ConversationItem> builder)
    {
        builder.ToTable("ConversationItem");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.Content)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(si => si.Translation)
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasOne(si => si.Conversation)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

    
    }
}
