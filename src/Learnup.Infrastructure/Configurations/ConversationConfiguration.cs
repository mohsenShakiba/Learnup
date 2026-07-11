using Learnup.Domain.AggregateRoots.Conversations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Learnup.Infrastructure.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversation");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.VoiceId)
            .HasMaxLength(300);

        builder.HasMany(s => s.Items)
            .WithOne(si => si.Conversation)
            .HasForeignKey(si => si.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
